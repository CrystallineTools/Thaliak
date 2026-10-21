use super::{GenericPollError, PatchDiscoveryType, Poller, VersionCheckService};
use crate::patch::PatchReconciliationService;
use log::info;

const GAME_REPO_ID: i64 = 20;

pub struct FfxivtcPoller;
impl Poller for FfxivtcPoller {
    type Error = GenericPollError;

    async fn poll(
        &self,
        reconciliation: &PatchReconciliationService,
    ) -> Result<(), GenericPollError> {
        info!("Polling for TW patches...");

        let vcs = self.version_check_service();
        let patch_list = vcs
            .fetch_patch_list(None)
            .await
            .map_err(|e| GenericPollError::VersionCheckError(e))?;
        info!("TW patch list: {:#?}", patch_list);

        reconciliation
            .reconcile(GAME_REPO_ID, &patch_list, PatchDiscoveryType::Offered)
            .await
            .map_err(|e| GenericPollError::ReconciliationError(e))?;

        Ok(())
    }
}

impl FfxivtcPoller {
    const PATCHER_USER_AGENT: &'static str = "FFXIV_Patch";

    pub fn new() -> Self {
        Self {}
    }

    fn version_check_service(&self) -> VersionCheckService {
        VersionCheckService::new(
            Self::PATCHER_USER_AGENT,
            move |base_version| {
                format!(
                    "http://patch-gamever.ffxiv.com.tw/http/win32/ffxivtc_release_tc_game/{}/",
                    base_version
                )
            },
            false,
        )
    }
}
