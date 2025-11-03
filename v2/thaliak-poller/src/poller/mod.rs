use crate::patch::{HashType, PatchReconciliationService};
use eyre::Report;
use log::trace;
use reqwest::{Method, StatusCode, header::HeaderMap};
use std::time::Duration;
use thiserror::Error;

pub mod actoz;
pub mod shanda;
pub mod sqex;

pub const BASE_GAME_VERSION: &str = "2012.01.01.0000.0000";

#[derive(Error, Debug)]
pub enum VersionCheckError {
    #[error("failed to issue fetch request to endpoint: {0}")]
    FetchError(#[from] reqwest::Error),
    #[error("a boot version patch is required before game version check can be performed")]
    NeedsBootPatch,
    #[error("version check failed with HTTP status code {0}")]
    HttpError(StatusCode),
    #[error("patch list is empty")]
    PatchListEmpty,
    #[error("failed to parse patch list: {0}")]
    PatchListParseError(String),
}

pub struct VersionCheckService {
    /// (base version string) -> full url of version check endpoint
    url_builder: Box<dyn Fn(&str) -> String>,
    client: reqwest::Client,
}

impl VersionCheckService {
    pub fn new(
        user_agent: &'static str,
        url_builder: impl Fn(&str) -> String + 'static,
        is_boot: bool,
    ) -> Self {
        let mut headers = HeaderMap::new();
        // this sucks a little bit, but avoids a ton of extra abstraction for what is a single
        // edge case (global boot version check)
        if is_boot {
            headers.insert("Host", "patch-bootver.ffxiv.com".parse().unwrap());
        } else {
            headers.insert("X-Hash-Check", "enabled".parse().unwrap());
        }

        Self {
            url_builder: Box::new(url_builder),
            client: reqwest::Client::builder()
                .default_headers(headers)
                .user_agent(user_agent)
                .connect_timeout(Duration::from_secs(10))
                .build()
                .expect("failed to build reqwest client"),
        }
    }

    pub async fn fetch_patch_list(
        &self,
        base_version: Option<String>,
    ) -> Result<Vec<PatchListEntry>, VersionCheckError> {
        let endpoint = (self.url_builder)(&base_version.unwrap_or(BASE_GAME_VERSION.to_string()));
        trace!("fetching patch list from {}", endpoint);

        let response = self
            .client
            .request(Method::POST, endpoint)
            .send()
            .await
            .map_err(|e| VersionCheckError::FetchError(e))?;

        match response.status() {
            StatusCode::OK => {
                let patch_list = response
                    .text()
                    .await
                    .map_err(|e| VersionCheckError::FetchError(e))?;

                parse_patch_list(patch_list)
            }
            StatusCode::CONFLICT => Err(VersionCheckError::NeedsBootPatch),
            code => Err(VersionCheckError::HttpError(code)),
        }
    }
}

#[derive(Clone, Debug)]
pub struct PatchListEntry {
    pub version_id: String,
    pub hash_type: HashType,
    pub url: String,
    pub length: u64,
}

#[derive(Copy, Clone, Debug, PartialEq, Eq)]
pub enum PatchDiscoveryType {
    Scraped,
    Offered,
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
                        .parse::<i64>()
                        .map_err(|e| VersionCheckError::PatchListParseError(e.to_string()))?;
                    let hashes: Vec<String> = fields[7].split(',').map(String::from).collect();

                    HashType::Sha1 { block_size, hashes }
                }
                _ => HashType::None,
            }
        } else {
            HashType::None
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

pub trait Poller {
    type Error;

    async fn poll(&self, reconciliation: &PatchReconciliationService) -> Result<(), Self::Error>;
}

#[derive(Error, Debug)]
pub enum GenericPollError {
    #[error("version check failed: {0}")]
    VersionCheckError(#[from] VersionCheckError),
    #[error("reconciliation failed: {0}")]
    ReconciliationError(#[from] Report),
}
