use super::{GenericPollError, PatchDiscoveryType, Poller, VersionCheckService};
use crate::patch::PatchReconciliationService;
use log::info;

const GAME_REPO_ID: i64 = 7;

pub struct ActozPoller;

impl Poller for ActozPoller {
    type Error = GenericPollError;

    async fn poll(
        &self,
        reconciliation: &PatchReconciliationService,
    ) -> Result<(), GenericPollError> {
        info!("Polling for KR patches...");

        let vcs = self.version_check_service();
        let patch_list = vcs
            .fetch_patch_list(None)
            .await
            .map_err(|e| GenericPollError::VersionCheckError(e))?;
        info!("KR patch list: {:#?}", patch_list);

        reconciliation
            .reconcile(GAME_REPO_ID, &patch_list, PatchDiscoveryType::Offered)
            .await
            .map_err(|e| GenericPollError::ReconciliationError(e))?;

        Ok(())
    }
}

impl ActozPoller {
    const PATCHER_USER_AGENT: &'static str = "FFXIV_Patch";

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
