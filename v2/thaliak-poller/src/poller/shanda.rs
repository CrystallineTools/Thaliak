use super::{GenericPollError, PatchDiscoveryType, Poller, VersionCheckService};
use crate::patch::PatchReconciliationService;
use log::info;

const GAME_REPO_ID: i64 = 12;

pub struct ShandaPoller;
impl Poller for ShandaPoller {
    type Error = GenericPollError;

    async fn poll(
        &self,
        reconciliation: &PatchReconciliationService,
    ) -> Result<(), GenericPollError> {
        info!("Polling for CN patches...");

        let vcs = self.version_check_service();
        let patch_list = vcs
            .fetch_patch_list(None)
            .await
            .map_err(|e| GenericPollError::VersionCheckError(e))?;
        info!("CN patch list: {:#?}", patch_list);

        reconciliation
            .reconcile(GAME_REPO_ID, &patch_list, PatchDiscoveryType::Offered)
            .await
            .map_err(|e| GenericPollError::ReconciliationError(e))?;

        Ok(())
    }
}

impl ShandaPoller {
    const PATCHER_USER_AGENT: &'static str = "FFXIV_Patch";

    pub fn new() -> Self {
        Self {}
    }

    fn version_check_service(&self) -> VersionCheckService {
        VersionCheckService::new(
            Self::PATCHER_USER_AGENT,
            move |base_version| {
                format!(
                    "http://ffxivpatch01.ff14.sdo.com/http/win32/shanda_release_chs_game/{}/",
                    base_version
                )
            },
            false,
        )
    }
}
