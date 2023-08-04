use log::info;

use super::{GenericPollError, Poller, VersionCheckService};

pub struct ActozPoller;
#[async_trait::async_trait(?Send)]
impl Poller for ActozPoller {
    type Error = GenericPollError;

    async fn poll(&self) -> Result<(), GenericPollError> {
        info!("Polling for KR patches...");

        let vcs = self.version_check_service();
        let patch_list = vcs
            .fetch_patch_list(None)
            .await
            .map_err(|e| GenericPollError::VersionCheckError(e))?;
        info!("KR patch list: {:#?}", patch_list);

        Ok(())
    }
}

impl ActozPoller {
    const PATCHER_USER_AGENT: &str = "FFXIV_Patch";

    pub fn new() -> Self {
        Self {}
    }

    fn version_check_service(&self) -> VersionCheckService {
        VersionCheckService::new(
            Self::PATCHER_USER_AGENT,
            move |base_version| {
                format!(
                    "http://ngamever-live.ff14.co.kr/http/win32/actoz_release_ko_game/{}/",
                    base_version
                )
            },
            false,
        )
    }
}
