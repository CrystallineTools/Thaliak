// Partially adapted from https://github.com/eorzeatools/microlaunch/blob/master/microlaunch/src/auth/mod.rs
// Partially adapted from https://github.com/avafloww/FFXIVQuickLauncher/commit/cf4aefcca65705d7db6c23c007d51034eba44b0e
use log::trace;
use thiserror::Error;

pub mod actoz;
pub mod shanda;
pub mod sqex;

pub const BASE_GAME_VERSION: &str = "2012.01.01.0000.0000";

// HTTP status codes that the launcher uses
const HTTP_OK: u16 = 200;
const HTTP_CONFLICT: u16 = 409; // need boot version patch

#[derive(Error, Debug)]
pub enum VersionCheckError {
    #[error("failed to issue fetch request to endpoint: {0}")]
    FetchError(#[from] worker::Error),
    #[error("a boot version patch is required before game version check can be performed")]
    NeedsBootPatch,
    #[error("version check failed with HTTP status code {0}")]
    HttpError(u16),
    #[error("patch list is empty")]
    PatchListEmpty,
    #[error("failed to parse patch list: {0}")]
    PatchListParseError(String),
}

pub struct VersionCheckService {
    user_agent: &'static str,
    /// (base version string) -> full url of version check endpoint
    url_builder: Box<dyn Fn(&str) -> String>,
    is_boot: bool,
}

impl VersionCheckService {
    pub fn new(
        user_agent: &'static str,
        url_builder: impl Fn(&str) -> String + 'static,
        is_boot: bool,
    ) -> Self {
        Self {
            user_agent,
            url_builder: Box::new(url_builder),
            is_boot,
        }
    }

    pub async fn fetch_patch_list(
        &self,
        base_version: Option<String>,
    ) -> Result<Vec<PatchListEntry>, VersionCheckError> {
        let endpoint = (self.url_builder)(&base_version.unwrap_or(BASE_GAME_VERSION.to_string()));
        trace!("fetching patch list from {}", endpoint);

        let mut headers = worker::Headers::new();
        headers
            .set("User-Agent", self.user_agent)
            .map_err(|e| VersionCheckError::FetchError(e))?;

        // this sucks a little bit, but avoids a ton of extra abstraction for what is a single
        // edge case (global boot version check)
        if self.is_boot {
            headers
                .set("Host", "patch-bootver.ffxiv.com")
                .map_err(|e| VersionCheckError::FetchError(e))?;
        } else {
            headers
                .set("X-Hash-Check", "enabled")
                .map_err(|e| VersionCheckError::FetchError(e))?;
        }

        let request = worker::Request::new_with_init(
            &endpoint,
            &worker::RequestInit {
                body: None, //todo!("should be VersionReport"),
                headers,
                cf: worker::CfProperties {
                    // expire immediately, don't cache this result
                    cache_ttl: Some(0),
                    ..Default::default()
                },
                method: worker::Method::Post,
                redirect: worker::RequestRedirect::Follow,
            },
        )
        .map_err(|e| VersionCheckError::FetchError(e))?;

        let mut response = worker::Fetch::Request(request)
            .send()
            .await
            .map_err(|e| VersionCheckError::FetchError(e))?;

        match response.status_code() {
            HTTP_OK => {
                let patch_list = response
                    .text()
                    .await
                    .map_err(|e| VersionCheckError::FetchError(e))?;

                parse_patch_list(patch_list)
            }
            HTTP_CONFLICT => Err(VersionCheckError::NeedsBootPatch),
            code => Err(VersionCheckError::HttpError(code)),
        }
    }
}

#[derive(Clone, Debug)]
pub enum PatchHashType {
    None,
    SHA1 {
        block_size: u64,
        hashes: Vec<String>,
    },
}

#[derive(Clone, Debug)]
pub struct PatchListEntry {
    pub version_id: String,
    pub hash_type: PatchHashType,
    pub url: String,
    pub length: u64,
}

pub fn parse_patch_list(patch_list: String) -> Result<Vec<PatchListEntry>, VersionCheckError> {
    // split into newlines
    let lines: Vec<String> = patch_list.split("\r\n").map(String::from).collect();
    let len = lines.len();
    let mut output = Vec::new();

    for line in lines.into_iter().skip(5).take(len - 5 - 2) {
        let fields: Vec<String> = line.split('\t').map(String::from).collect();
        let length = fields[0]
            .clone()
            .parse::<u64>()
            .map_err(|e| VersionCheckError::PatchListParseError(e.to_string()))?;
        let version_id = fields[4].to_string();

        let hash_type = if fields.len() == 9 {
            match fields[5] {
                ref s if s.to_ascii_lowercase() == "sha1" => {
                    let block_size = fields[6]
                        .parse::<u64>()
                        .map_err(|e| VersionCheckError::PatchListParseError(e.to_string()))?;
                    let hashes: Vec<String> = fields[7].split(',').map(String::from).collect();

                    PatchHashType::SHA1 { block_size, hashes }
                }
                _ => PatchHashType::None,
            }
        } else {
            PatchHashType::None
        };

        let url = if fields.len() == 9 {
            fields[8].to_string()
        } else {
            fields[5].to_string()
        };

        output.push(PatchListEntry {
            length,
            version_id,
            hash_type,
            url,
        });
    }

    if output.is_empty() {
        Err(VersionCheckError::PatchListEmpty)
    } else {
        Ok(output)
    }
}

#[async_trait::async_trait(?Send)]
pub trait Poller {
    type Error;

    async fn poll(&self) -> Result<(), Self::Error>;
}

#[derive(Error, Debug)]
pub enum GenericPollError {
    #[error("version check failed: {0}")]
    VersionCheckError(#[from] VersionCheckError),
}
