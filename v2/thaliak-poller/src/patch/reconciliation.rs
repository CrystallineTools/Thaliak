use crate::patch::{DateTime, Patch};
use crate::poller::{PatchDiscoveryType, PatchListEntry};
use eyre::Result;
use regex::Regex;
use sqlx::{Row, Sqlite, SqlitePool};
use std::sync::{LazyLock, OnceLock};
use sqlx::pool::PoolConnection;
use crate::DbConnection;

fn get_expansion_from_patch_url(url: &str) -> Option<u32> {
    static RE: LazyLock<Regex> = LazyLock::new(|| {
        Regex::new(r#"(?:https?://.*/)?(game|boot)/(?:ex(\d)|\w+)/(.*)"#).unwrap()
    });
    if let Some(caps) = RE.captures(url) {
        if let Some(id_string) = caps.get(2) {
            if let Ok(id) = id_string.as_str().parse::<u32>() {
                return Some(id);
            }
        }
    }

    None
}

pub struct PatchReconciliationService {
    db: SqlitePool,
}

impl PatchReconciliationService {
    pub fn new(db: &SqlitePool) -> Self {
        Self { db: db.clone() }
    }

    pub async fn reconcile(
        &self,
        game_repo_id: u32,
        remote_patches: &[PatchListEntry],
        discovery_type: PatchDiscoveryType,
    ) -> Result<()> {
        // use a consistent timestamp through reconciliation of each repo's patch list
        let now = chrono::Utc::now();
        let mut conn = self.db.acquire().await?;

        // get the list of expansions and their repository mappings
        let expansions = sqlx::query(
            r#"SELECT game_repository_id, expansion_id, expansion_repository_id FROM expansion_repository_mapping WHERE game_repository_id = $1"#,
        )
            .bind(game_repo_id)
            .fetch_all(&mut *conn)
            .await?;

        let get_effective_repo_id = |patch_url: &str| -> u32 {
            if let Some(expansion_repo_id) = get_expansion_from_patch_url(patch_url) {
                expansion_repo_id
            } else {
                game_repo_id
            }
        };

        // retrieve list of patches we know about for this game repo and all expansion repos
        let repo_ids = {
            let mut ids = vec![game_repo_id.to_string()];
            expansions.iter().for_each(|exp| {
                ids.push(exp.get::<String, _>("expansion_repository_id"));
            });
            ids.join(", ")
        };
        let local_patches: Vec<Patch> =
            sqlx::query(r#"SELECT * FROM patch WHERE repository_id IN ($1)"#)
                .bind(repo_ids)
                .fetch_all(&mut *conn)
                .await?
                .into_iter()
                .map(Patch::from_db)
                .collect();

        // let's go
        for remote_patch in remote_patches {
            let effective_repo_id = get_effective_repo_id(&remote_patch.url);
            if let Some(local_patch) = local_patches.iter().find(|p| {
                p.version_string == remote_patch.version_id && p.repository_id == effective_repo_id
            }) {
                // UpdateExistingPatchData
                self.update_existing_patch_data(&mut conn, now, local_patch, remote_patch, discovery_type).await;
            } else {
                // RecordNewPatchData
            }
        }

        Ok(())
    }

    async fn update_existing_patch_data(
        &self,
        conn: &mut DbConnection,
        now: DateTime,
        local_patch: &Patch,
        remote_patch: &PatchListEntry,
        discovery_type: PatchDiscoveryType,
    ) {
    }
}
