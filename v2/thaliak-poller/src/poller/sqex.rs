use super::{BASE_GAME_VERSION, PatchListEntry, Poller, VersionCheckError, parse_patch_list};
use crate::patch::GameRepository;
use chrono::Utc;
use eyre::Result;
use log::info;
use regex::Regex;
use reqwest::{StatusCode, header::HeaderMap};
use sha1::{Digest, Sha1};
use std::collections::HashMap;
use std::env;
use std::fs::File;
use std::io::{Read, Write};
use std::path::{Path, PathBuf};
use thiserror::Error;
use zipatch::{ZiPatchConfig, ZiPatchFile};

// Constants
const OAUTH_TOP_URL: &str = "https://ffxiv-login.square-enix.com/oauth/ffxivarr/login/top?lng=en&rgn=3&isft=0&cssmode=1&isnew=1&launchver=3";
const USER_AGENT_TEMPLATE: &str = "SQEXAuthor/2.0.0(Windows 6.2; ja-jp; {0})";
const PATCHER_USER_AGENT: &str = "FFXIV PATCH CLIENT";

// Error types
#[derive(Error, Debug)]
pub enum SqexPollerError {
    #[error("version check failed: {0}")]
    VersionCheckError(#[from] VersionCheckError),
    #[error("HTTP request failed: {0}")]
    HttpError(#[from] reqwest::Error),
    #[error("OAuth login failed: {0}")]
    OauthError(String),
    #[error("invalid response from server: {0}")]
    InvalidResponse(String),
    #[error("no service subscription on account")]
    NoService,
    #[error("terms of service not accepted")]
    NoTerms,
    #[error("IO error: {0}")]
    IoError(#[from] std::io::Error),
    #[error("patch installation failed: {0}")]
    PatchInstallError(#[from] zipatch::ZiPatchError),
}

// Data structures
#[derive(Debug, Clone)]
pub struct OauthLoginResult {
    pub session_id: String,
    pub region: i32,
    pub terms_accepted: bool,
    pub playable: bool,
    pub max_expansion: i32,
}

#[derive(Debug, Clone, PartialEq)]
pub enum LoginState {
    Ok,
    NeedsPatchBoot,
    NeedsPatchGame,
    NoService,
    NoTerms,
}

#[derive(Debug, Clone)]
pub struct LoginResult {
    pub pending_patches: Vec<PatchListEntry>,
    pub oauth_login: OauthLoginResult,
    pub state: LoginState,
    pub unique_id: Option<String>,
}

// Helper utilities
fn make_computer_id() -> String {
    let hash_string = format!(
        "{}{}{}{}",
        env::var("COMPUTERNAME").unwrap_or_else(|_| "unknown".to_string()),
        env::var("USERNAME").unwrap_or_else(|_| "unknown".to_string()),
        env::consts::OS,
        num_cpus::get()
    );

    let mut hasher = Sha1::new();
    hasher.update(hash_string.as_bytes());
    let hash = hasher.finalize();

    let mut bytes = [0u8; 5];
    bytes[1..5].copy_from_slice(&hash[0..4]);

    // Calculate checksum
    let checksum = (-(bytes[1] as i16 + bytes[2] as i16 + bytes[3] as i16 + bytes[4] as i16)) as u8;
    bytes[0] = checksum;

    hex::encode(bytes)
}

fn generate_user_agent() -> String {
    USER_AGENT_TEMPLATE.replace("{0}", &make_computer_id())
}

fn get_launcher_formatted_time_long() -> String {
    Utc::now().format("%Y-%m-%d-%H-%M").to_string()
}

fn get_launcher_formatted_time_long_rounded() -> String {
    let mut formatted = Utc::now().format("%Y-%m-%d-%H-%M").to_string();
    // Round the last minute digit to 0
    if let Some(_last_char) = formatted.chars().last() {
        formatted = formatted[..formatted.len() - 1].to_string() + "0";
    }
    formatted
}

fn generate_frontier_referer() -> String {
    format!(
        "https://launcher.finalfantasyxiv.com/v610/index.html?rc_lang=ja&time={}",
        get_launcher_formatted_time_long()
    )
}

fn get_file_hash<P: AsRef<Path>>(path: P) -> Result<String, std::io::Error> {
    let mut file = File::open(&path)?;
    let mut buffer = Vec::new();
    file.read_to_end(&mut buffer)?;

    let mut hasher = Sha1::new();
    hasher.update(&buffer);
    let hash = hasher.finalize();
    let hash_string = hex::encode(hash);

    let metadata = std::fs::metadata(&path)?;
    let length = metadata.len();

    Ok(format!("{}/{}", length, hash_string))
}

fn get_boot_version<P: AsRef<Path>>(game_path: P) -> String {
    GameRepository::Boot.get_ver(game_path)
}

fn get_boot_version_hash<P: AsRef<Path>>(game_path: P) -> Result<String, std::io::Error> {
    let game_path = game_path.as_ref();
    let version = GameRepository::Boot.get_ver(game_path);
    let mut result = format!("{}=", version);
    let boot_path = game_path.join("boot");

    let files_to_hash = [
        "ffxivboot.exe",
        "ffxivboot64.exe",
        "ffxivlauncher.exe",
        "ffxivlauncher64.exe",
        "ffxivupdater.exe",
        "ffxivupdater64.exe",
    ];

    for file in &files_to_hash {
        let file_path = boot_path.join(file);
        if file_path.exists() {
            if let Ok(hash) = get_file_hash(&file_path) {
                result.push_str(&format!("{}/{},", file, hash));
            }
        }
    }

    Ok(result.trim_end_matches(',').to_string())
}

fn get_version_report<P: AsRef<Path>>(
    game_path: P,
    ex_level: i32,
    force_base_version: bool,
) -> Result<String, std::io::Error> {
    let game_path = game_path.as_ref();
    let mut ver_report = get_boot_version_hash(game_path)?;

    let read_ver = |repo: GameRepository| -> String {
        if force_base_version {
            BASE_GAME_VERSION.to_string()
        } else {
            repo.get_ver(game_path)
        }
    };

    if ex_level >= 1 {
        ver_report.push_str(&format!("\nex1\t{}", read_ver(GameRepository::Ex1)));
    }
    if ex_level >= 2 {
        ver_report.push_str(&format!("\nex2\t{}", read_ver(GameRepository::Ex2)));
    }
    if ex_level >= 3 {
        ver_report.push_str(&format!("\nex3\t{}", read_ver(GameRepository::Ex3)));
    }
    if ex_level >= 4 {
        ver_report.push_str(&format!("\nex4\t{}", read_ver(GameRepository::Ex4)));
    }
    if ex_level >= 5 {
        ver_report.push_str(&format!("\nex5\t{}", read_ver(GameRepository::Ex5)));
    }

    Ok(ver_report)
}

/// Filters patch list to only include patches from the specified version onwards.
/// Used to skip patches that have already been applied.
fn filter_patches_from_version(
    patches: &[PatchListEntry],
    from_version: &str,
) -> Vec<PatchListEntry> {
    let mut found = false;
    let mut result = Vec::new();

    for patch in patches {
        if !found && patch.version_id == from_version {
            found = true;
            continue; // Skip the current version, we want patches AFTER it
        }

        if found {
            result.push(patch.clone());
        }
    }

    // If we never found the current version, SE is giving us all the patches we need
    if !found {
        return patches.to_vec();
    }

    result
}

// Main poller struct
pub struct SqexPoller {
    client: reqwest::Client,
    user_agent: String,
    username: String,
    password: String,
    game_directory: PathBuf,
}

impl SqexPoller {
    pub fn new() -> Result<Self> {
        let user_agent = generate_user_agent();
        let client = reqwest::Client::builder()
            .build()
            .expect("failed to build HTTP client");

        let username = env::var("SQEX_USERNAME")?;
        let password = env::var("SQEX_PASSWORD")?;
        let game_directory = PathBuf::from(env::var("SQEX_INSTALL_DIRECTORY")?);

        Ok(Self {
            client,
            user_agent,
            username,
            password,
            game_directory,
        })
    }

    async fn get_oauth_top(&self) -> Result<String, SqexPollerError> {
        let mut headers = HeaderMap::new();
        headers.insert("Accept", "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*".parse().unwrap());
        headers.insert("Referer", generate_frontier_referer().parse().unwrap());
        headers.insert("Accept-Encoding", "gzip, deflate".parse().unwrap());
        headers.insert("Accept-Language", "ja".parse().unwrap());
        headers.insert("User-Agent", self.user_agent.parse().unwrap());
        headers.insert("Connection", "Keep-Alive".parse().unwrap());
        headers.insert("Cookie", "_rsid=\"\"".parse().unwrap());

        let response = self
            .client
            .get(OAUTH_TOP_URL)
            .headers(headers)
            .send()
            .await?;

        let text = response.text().await?;

        if text.contains("window.external.user(\\\"restartup\\\");") {
            return Err(SqexPollerError::InvalidResponse(
                "restartup, but not isSteam?".to_string(),
            ));
        }

        let stored_regex = Regex::new(r#"\t<\s*input .* name="_STORED_" value="(?<stored>.*)""#)
            .expect("failed to compile regex");

        let captures = stored_regex
            .captures(&text)
            .ok_or_else(|| SqexPollerError::InvalidResponse("Could not get STORED.".to_string()))?;

        Ok(captures["stored"].to_string())
    }

    async fn oauth_login(
        &self,
        username: &str,
        password: &str,
    ) -> Result<OauthLoginResult, SqexPollerError> {
        let top_result = self.get_oauth_top().await?;

        let mut headers = HeaderMap::new();
        headers.insert("Accept", "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*".parse().unwrap());
        headers.insert("Referer", OAUTH_TOP_URL.parse().unwrap());
        headers.insert("Accept-Language", "ja".parse().unwrap());
        headers.insert("User-Agent", self.user_agent.parse().unwrap());
        headers.insert("Accept-Encoding", "gzip, deflate".parse().unwrap());
        headers.insert("Host", "ffxiv-login.square-enix.com".parse().unwrap());
        headers.insert("Connection", "Keep-Alive".parse().unwrap());
        headers.insert("Cache-Control", "no-cache".parse().unwrap());
        headers.insert("Cookie", "_rsid=\"\"".parse().unwrap());

        let mut form = HashMap::new();
        form.insert("_STORED_", top_result);
        form.insert("sqexid", username.to_string());
        form.insert("password", password.to_string());
        form.insert("otppw", String::new());

        let response = self
            .client
            .post("https://ffxiv-login.square-enix.com/oauth/ffxivarr/login/login.send")
            .headers(headers)
            .form(&form)
            .send()
            .await?;

        let reply = response.text().await?;

        let regex = Regex::new(r#"window.external.user\("login=auth,ok,(?<launchParams>.*)\);"#)
            .expect("failed to compile regex");

        let captures = regex.captures(&reply).ok_or_else(|| {
            SqexPollerError::OauthError(format!("Login failed. Response: {}", reply))
        })?;

        let launch_params: Vec<&str> = captures["launchParams"].split(',').collect();

        Ok(OauthLoginResult {
            session_id: launch_params[1].to_string(),
            region: launch_params[5].parse().unwrap_or(0),
            terms_accepted: launch_params[3] != "0",
            playable: launch_params[9] != "0",
            max_expansion: launch_params[13].parse().unwrap_or(0),
        })
    }

    async fn check_boot_version(
        &self,
        base_version: Option<&str>,
    ) -> Result<Vec<PatchListEntry>, SqexPollerError> {
        let version = base_version.unwrap_or(BASE_GAME_VERSION);
        let url = format!(
            "http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{}/?time={}",
            version,
            get_launcher_formatted_time_long_rounded()
        );

        let mut headers = HeaderMap::new();
        headers.insert("User-Agent", PATCHER_USER_AGENT.parse().unwrap());
        headers.insert("Host", "patch-bootver.ffxiv.com".parse().unwrap());

        let response = self.client.get(&url).headers(headers).send().await?;

        let text = response.text().await?;

        if text.is_empty() {
            return Ok(Vec::new());
        }

        info!("Boot patching is needed... Parsing patch list");
        parse_patch_list(text).map_err(|e| e.into())
    }

    async fn check_game_version(
        &self,
        oauth_result: &OauthLoginResult,
        game_path: &Path,
        force_base_version: bool,
    ) -> Result<(Vec<PatchListEntry>, String), SqexPollerError> {
        let version = if force_base_version {
            BASE_GAME_VERSION.to_string()
        } else {
            GameRepository::Ffxiv.get_ver(game_path)
        };

        let version_report =
            get_version_report(game_path, oauth_result.max_expansion, force_base_version)?;

        let url = format!(
            "https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{}/{}",
            version, oauth_result.session_id
        );

        let mut headers = HeaderMap::new();
        headers.insert("X-Hash-Check", "enabled".parse().unwrap());
        headers.insert("User-Agent", PATCHER_USER_AGENT.parse().unwrap());

        let response = self
            .client
            .post(&url)
            .headers(headers)
            .body(version_report)
            .send()
            .await?;

        if response.status() == StatusCode::CONFLICT {
            return Err(SqexPollerError::VersionCheckError(
                VersionCheckError::NeedsBootPatch,
            ));
        }

        let unique_id = response
            .headers()
            .get("X-Patch-Unique-Id")
            .ok_or_else(|| {
                SqexPollerError::InvalidResponse("Could not get X-Patch-Unique-Id.".to_string())
            })?
            .to_str()
            .map_err(|_| {
                SqexPollerError::InvalidResponse("Invalid X-Patch-Unique-Id header".to_string())
            })?
            .to_string();

        let text = response.text().await?;

        if text.is_empty() {
            return Ok((Vec::new(), unique_id));
        }

        info!("Game patching is needed... Parsing patch list");
        let patches = parse_patch_list(text)?;

        Ok((patches, unique_id))
    }

    async fn login(
        &self,
        username: &str,
        password: &str,
        game_path: &Path,
    ) -> Result<LoginResult, SqexPollerError> {
        info!("Starting login flow...");

        let oauth_login_result = self.oauth_login(username, password).await?;

        info!(
            "OAuth login successful - playable:{} terms:{} region:{} expack:{}",
            oauth_login_result.playable,
            oauth_login_result.terms_accepted,
            oauth_login_result.region,
            oauth_login_result.max_expansion
        );

        if !oauth_login_result.playable {
            return Ok(LoginResult {
                pending_patches: Vec::new(),
                oauth_login: oauth_login_result,
                state: LoginState::NoService,
                unique_id: None,
            });
        }

        if !oauth_login_result.terms_accepted {
            return Ok(LoginResult {
                pending_patches: Vec::new(),
                oauth_login: oauth_login_result,
                state: LoginState::NoTerms,
                unique_id: None,
            });
        }

        match self
            .check_game_version(&oauth_login_result, game_path, true)
            .await
        {
            Ok((patches, unique_id)) => {
                let state = if !patches.is_empty() {
                    LoginState::NeedsPatchGame
                } else {
                    LoginState::Ok
                };

                Ok(LoginResult {
                    pending_patches: patches,
                    oauth_login: oauth_login_result,
                    state,
                    unique_id: Some(unique_id),
                })
            }
            Err(SqexPollerError::VersionCheckError(VersionCheckError::NeedsBootPatch)) => {
                Ok(LoginResult {
                    pending_patches: Vec::new(),
                    oauth_login: oauth_login_result,
                    state: LoginState::NeedsPatchBoot,
                    unique_id: None,
                })
            }
            Err(e) => Err(e),
        }
    }

    async fn download_patch(
        &self,
        patch: &PatchListEntry,
        dest_path: &Path,
    ) -> Result<(), SqexPollerError> {
        info!("Downloading patch: {} -> {:?}", patch.url, dest_path);

        let mut headers = HeaderMap::new();
        headers.insert("User-Agent", PATCHER_USER_AGENT.parse().unwrap());
        headers.insert("Accept", "*/*".parse().unwrap());

        let response = self.client.get(&patch.url).headers(headers).send().await?;

        if !response.status().is_success() {
            return Err(SqexPollerError::InvalidResponse(format!(
                "Failed to download patch: HTTP {}",
                response.status()
            )));
        }

        let bytes = response.bytes().await?;

        // Create parent directory if needed
        if let Some(parent) = dest_path.parent() {
            std::fs::create_dir_all(parent)?;
        }

        let mut file = File::create(dest_path)?;
        file.write_all(&bytes)?;

        info!("Downloaded {} bytes to {:?}", bytes.len(), dest_path);
        Ok(())
    }

    fn install_patch(
        &self,
        patch_path: &Path,
        game_path: &Path,
        version_id: &str,
    ) -> Result<(), SqexPollerError> {
        // Create game/boot subdirectories if needed
        let boot_dir = game_path.join("boot");
        info!(
            "Installing patch {:?} to {:?}, target version: {}",
            patch_path, boot_dir, version_id
        );
        std::fs::create_dir_all(&boot_dir)?;

        // Open and apply the patch file
        let mut patch_file = ZiPatchFile::from_path(patch_path)?;
        let mut config = ZiPatchConfig::builder(&boot_dir).build();

        for chunk_result in patch_file.chunks() {
            let mut chunk = chunk_result?;
            chunk.apply(&mut config)?;
        }

        // Update version file after successful installation
        GameRepository::Boot.set_ver(game_path, version_id)?;

        info!(
            "Patch installation complete, updated to version {}",
            version_id
        );
        Ok(())
    }

    async fn patch_boot(
        &self,
        game_path: &Path,
        patches: &[PatchListEntry],
    ) -> Result<(), SqexPollerError> {
        info!("Starting boot patch process for {} patches", patches.len());

        // Create temp directory for patch downloads
        let temp_dir = tempfile::tempdir().map_err(|e| SqexPollerError::IoError(e))?;

        for (i, patch) in patches.iter().enumerate() {
            info!(
                "Processing patch {}/{}: {}",
                i + 1,
                patches.len(),
                patch.version_id
            );

            // Download patch to temp directory
            let patch_filename = patch.url.split('/').last().unwrap();
            let patch_path = temp_dir.path().join(patch_filename);

            self.download_patch(patch, &patch_path).await?;

            // Install patch
            self.install_patch(&patch_path, game_path, &patch.version_id)?;
        }

        info!("Boot patch complete");
        Ok(())
    }
}

impl Poller for SqexPoller {
    type Error = SqexPollerError;

    async fn poll(&self) -> Result<(), Self::Error> {
        info!("Polling for JP patches...");

        // Check remote boot version
        let remote_boot_patches = self.check_boot_version(None).await?;

        if !remote_boot_patches.is_empty() {
            info!("Discovered JP boot patches: {:#?}", remote_boot_patches);

            // Get the latest remote boot version
            let latest_remote_boot = &remote_boot_patches.last().unwrap().version_id;

            // If we have a local boot directory, check if it's up to date
            let local_boot_version = get_boot_version(&self.game_directory);
            info!("Local boot version: {}", local_boot_version);
            info!("Remote boot version: {}", latest_remote_boot);

            if &local_boot_version != latest_remote_boot {
                info!(
                    "Boot needs patching (current: {}, latest: {})",
                    local_boot_version, latest_remote_boot
                );

                // Filter to only patches we need (current -> latest)
                let needed_patches =
                    filter_patches_from_version(&remote_boot_patches, &local_boot_version);

                info!("Need to apply {} patches", needed_patches.len());

                // Download and install patches
                self.patch_boot(&self.game_directory, &needed_patches)
                    .await?;

                info!("Boot patching complete!");
            } else {
                info!("Local boot is up to date!");
            }
        } else {
            info!("No boot patches available - boot is at base version or up to date");
        }

        // Perform login and check game version
        let login_result = self
            .login(&self.username, &self.password, &self.game_directory)
            .await?;

        match login_result.state {
            LoginState::Ok => {
                info!("Login successful, no patches needed");
            }
            LoginState::NeedsPatchGame => {
                info!(
                    "Discovered JP game patches ({} patches):",
                    login_result.pending_patches.len()
                );
                for patch in &login_result.pending_patches {
                    info!(
                        "  - {} (v{}, {} bytes)",
                        patch.url, patch.version_id, patch.length
                    );
                }
            }
            LoginState::NeedsPatchBoot => {
                return Err(SqexPollerError::InvalidResponse(
                    "Game server still reports boot needs patching after we patched it. This should not happen.".to_string(),
                ));
            }
            LoginState::NoService => {
                return Err(SqexPollerError::NoService);
            }
            LoginState::NoTerms => {
                return Err(SqexPollerError::NoTerms);
            }
        }

        Ok(())
    }
}
