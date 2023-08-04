use log::info;
use thiserror::Error;

use super::{GenericPollError, Poller, VersionCheckService};

#[derive(Error, Debug)]
pub enum SqexPollError {
    #[error("{0}")]
    Generic(#[from] GenericPollError),
}

pub struct SqexPoller {
    account_username: String,
    account_password: String,
}

#[async_trait::async_trait(?Send)]
impl Poller for SqexPoller {
    type Error = SqexPollError;

    async fn poll(&self) -> Result<(), SqexPollError> {
        info!("Polling for JP patches...");

        // check boot @ base version, to get the full upgrade path chain
        self.check_boot(None).await?;

        // reconcile boot

        // check boot @ last version, to see if we need to patch boot

        // patch boot if necessary

        // check game @ base version

        Ok(())
    }
}

impl SqexPoller {
    const PATCHER_USER_AGENT: &str = "FFXIV PATCH CLIENT";

    pub fn new(env: worker::Env) -> Self {
        Self {
            account_username: worker_env!(env, "SQEX_ACCOUNT_USERNAME"),
            account_password: worker_env!(env, "SQEX_ACCOUNT_PASSWORD"),
        }
    }

    async fn check_boot(&self, version: Option<String>) -> Result<(), SqexPollError> {
        let vcs = self.boot_version_check_service();
        let boot_patches = vcs
            .fetch_patch_list(version)
            .await
            .map_err(|e| SqexPollError::Generic(GenericPollError::VersionCheckError(e)))?;

        Ok(())
    }

    fn boot_version_check_service(&self) -> VersionCheckService {
        VersionCheckService::new(
            Self::PATCHER_USER_AGENT,
            |base_version| {
                format!(
                    "https://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{}/?time={}",
                    base_version,
                    get_rounded_launcher_time()
                )
            },
            true,
        )
    }

    fn game_version_check_service(&self, session_id: String) -> VersionCheckService {
        VersionCheckService::new(
            Self::PATCHER_USER_AGENT,
            move |base_version| {
                format!(
                    "https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{}/{}",
                    base_version, session_id
                )
            },
            false,
        )
    }
}

fn get_rounded_launcher_time() -> String {
    format!(
        "{}0",
        chrono::Utc::now()
            .format("%Y-%m-%d-%H-%M")
            .to_string()
            .chars()
            .take(14)
            .collect::<String>()
    )
}
