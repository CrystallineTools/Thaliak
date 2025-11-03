use crate::patch::{DateTime, Patch};
use crate::poller::{PatchDiscoveryType, PatchListEntry};
use eyre::Result;
use regex::Regex;
use sqlx::SqlitePool;
use std::sync::LazyLock;
use crate::DbConnection;

fn get_expansion_from_patch_url(url: &str) -> Option<i64> {
    static RE: LazyLock<Regex> = LazyLock::new(|| {
        Regex::new(r#"(?:https?://.*/)?(game|boot)/(?:ex(\d)|\w+)/(.*)"#).unwrap()
    });
    if let Some(caps) = RE.captures(url) {
        if let Some(id_string) = caps.get(2) {
            if let Ok(id) = id_string.as_str().parse::<i64>() {
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
        game_repo_id: i64,
        remote_patches: &[PatchListEntry],
        discovery_type: PatchDiscoveryType,
    ) -> Result<()> {
        // use a consistent timestamp through reconciliation of each repo's patch list
        let now = chrono::Utc::now();
        let mut conn = self.db.acquire().await?;

        // get the list of expansions and their repository mappings
        let expansions = sqlx::query!(
            r#"SELECT game_repository_id, expansion_id, expansion_repository_id FROM expansion_repository_mapping WHERE game_repository_id = ?"#,
            game_repo_id
        )
            .fetch_all(&mut *conn)
            .await?;

        let get_effective_repo_id = |patch_url: &str| -> i64 {
            if let Some(expansion_repo_id) = get_expansion_from_patch_url(patch_url) {
                expansion_repo_id
            } else {
                game_repo_id
            }
        };

        // retrieve list of patches we know about for this game repo and all expansion repos
        let mut repo_ids = vec![game_repo_id];
        for exp in &expansions {
            repo_ids.push(exp.expansion_repository_id);
        }

        let local_patches = {
            let placeholders = repo_ids.iter().map(|_| "?").collect::<Vec<_>>().join(", ");
            let query_str = format!("SELECT * FROM patch WHERE repository_id IN ({})", placeholders);

            let mut query = sqlx::query_as::<_, Patch>(&query_str);
            for id in &repo_ids {
                query = query.bind(id);
            }
            query.fetch_all(&mut *conn).await?
        };

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
