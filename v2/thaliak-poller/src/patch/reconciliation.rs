use crate::DbConnection;
use crate::patch::{DateTime, Patch};
use crate::poller::{PatchDiscoveryType, PatchListEntry};
use eyre::Result;
use log::{info, trace};
use regex::Regex;
use sqlx::SqlitePool;
use std::sync::LazyLock;
use thaliak_common::DatabasePools;
use thaliak_types::ExpansionRepositoryMapping;

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

#[derive(Clone)]
pub struct PatchReconciliationService {
    db: DatabasePools,
}

impl PatchReconciliationService {
    pub fn new(db: DatabasePools) -> Self {
        Self { db }
    }

    pub async fn reconcile(
        &self,
        game_repo_id: i64,
        remote_patches: &[PatchListEntry],
        discovery_type: PatchDiscoveryType,
    ) -> Result<()> {
        // use a consistent timestamp through reconciliation of each repo's patch list
        let now = chrono::Utc::now();

        // Use an explicit transaction to ensure atomicity and proper lock handling
        let mut tx = self.db.public.begin().await?;

        let mut new_patches = Vec::new();

        // get the list of expansions and their repository mappings
        let expansions = sqlx::query_as!(
            ExpansionRepositoryMapping,
            r#"SELECT game_repository_id, expansion_id, expansion_repository_id FROM expansion_repository_mapping WHERE game_repository_id = ?"#,
            game_repo_id
        )
            .fetch_all(&mut *tx)
            .await?;

        let get_effective_repo_id = |patch_url: &str| -> i64 {
            if let Some(expansion_id) = get_expansion_from_patch_url(patch_url) {
                expansions
                    .iter()
                    .find(|erp| {
                        erp.game_repository_id == game_repo_id && erp.expansion_id == expansion_id
                    })
                    .expect(
                        format!(
                            "no ExpansionRepositoryMapping for game repo {}, expansion id {}",
                            game_repo_id, expansion_id
                        )
                        .as_str(),
                    )
                    .expansion_repository_id
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
            let query_str = format!(
                "SELECT * FROM patch WHERE repository_id IN ({})",
                placeholders
            );

            let mut query = sqlx::QueryBuilder::new(&query_str);
            let mut query = query.build_query_as::<Patch>();
            for id in &repo_ids {
                query = query.bind(id);
            }
            query.fetch_all(&mut *tx).await?
        };

        // let's go
        for remote_patch in remote_patches {
            let effective_repo_id = get_effective_repo_id(&remote_patch.url);
            if let Some(local_patch) = local_patches.iter().find(|p| {
                p.version_string == remote_patch.version_id && p.repository_id == effective_repo_id
            }) {
                self.update_existing_patch_data(
                    &mut *tx,
                    now,
                    local_patch,
                    remote_patch,
                    discovery_type,
                )
                .await?;
            } else {
                let patch_id = self
                    .record_new_patch_data(
                        &mut *tx,
                        now,
                        effective_repo_id,
                        remote_patch,
                        discovery_type,
                    )
                    .await?;

                // If this is an offered patch, fetch it and add to new patches list for webhooks
                if discovery_type == PatchDiscoveryType::Offered {
                    let patch = sqlx::query_as::<_, Patch>("SELECT * FROM patch WHERE id = ?")
                        .bind(patch_id)
                        .fetch_one(&mut *tx)
                        .await?;
                    new_patches.push(patch.into());
                }
            }
        }

        // record edges
        for repo_id in &repo_ids {
            let repo_patches = remote_patches
                .iter()
                .filter(|rp| get_effective_repo_id(&rp.url) == *repo_id)
                .collect::<Vec<_>>();
            self.record_patch_edge_data(&mut *tx, now, *repo_id, &repo_patches)
                .await?;
        }

        // record active status
        for repo_id in &repo_ids {
            self.record_active_status(&mut *tx, now, *repo_id).await?;
        }

        // Commit the transaction
        tx.commit().await?;

        // Dispatch webhooks for new patches asynchronously (don't block)
        if !new_patches.is_empty() {
            let db = self.db.clone();
            let patches = new_patches.clone();
            tokio::spawn(async move {
                if let Err(e) = crate::webhook::dispatch_webhooks(&db, patches).await {
                    log::error!("Failed to dispatch webhooks: {:?}", e);
                }
            });
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
    ) -> Result<()> {
        trace!("Patch already present: {:?}", remote_patch);

        // for all types of discoveries, we update last_seen
        sqlx::query!(
            "UPDATE patch SET last_seen = ? WHERE id = ?",
            now,
            local_patch.id
        )
        .execute(&mut *conn)
        .await?;

        // if the launcher offered it in the patch list, we have a few more things to update
        if discovery_type == PatchDiscoveryType::Offered {
            if local_patch.first_offered.is_none() {
                let (hash_type, hash_block_size, hashes) = local_patch.hash.to_columns();

                sqlx::query!("UPDATE patch SET is_active = ?, last_offered = ?, first_offered = ?, size = ?, hash_type = ?, hash_block_size = ?, hashes = ? WHERE id = ?",
                    true, now, now, local_patch.size, hash_type, hash_block_size, hashes, local_patch.id)
                    .execute(&mut *conn)
                    .await?;
            } else {
                sqlx::query!(
                    "UPDATE patch SET is_active = ?, last_offered = ? WHERE id = ?",
                    true,
                    now,
                    local_patch.id
                )
                .execute(&mut *conn)
                .await?;
            }
        }

        Ok(())
    }

    async fn record_new_patch_data(
        &self,
        conn: &mut DbConnection,
        now: DateTime,
        effective_repo_id: i64,
        remote_patch: &PatchListEntry,
        discovery_type: PatchDiscoveryType,
    ) -> Result<i64> {
        info!("Discovered new patch: {:?}", remote_patch);

        let version_string = &remote_patch.version_id;
        let remote_url = &remote_patch.url;
        let (offered, is_active) = if discovery_type == PatchDiscoveryType::Offered {
            (Some(now), true)
        } else {
            (None, false)
        };
        let size = remote_patch.length as i64;
        let (hash_type, hash_block_size, hashes) = remote_patch.hash_type.to_columns();

        let result = sqlx::query!(
            "INSERT INTO patch (version_string, repository_id, remote_url, first_seen, last_seen, first_offered, last_offered, is_active, size, hash_type, hash_block_size, hashes)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
            version_string,
            effective_repo_id,
            remote_url,
            now,
            now,
            offered,
            offered,
            is_active,
            size,
            hash_type,
            hash_block_size,
            hashes
        )
            .execute(&mut *conn)
            .await?;

        // TODO: port DownloaderService.AddToQueue

        Ok(result.last_insert_rowid())
    }

    async fn record_patch_edge_data(
        &self,
        conn: &mut DbConnection,
        now: DateTime,
        effective_repo_id: i64,
        remote_patches: &[&PatchListEntry],
    ) -> Result<()> {
        let mut previous_patch: Option<&PatchListEntry> = None;

        for remote_patch in remote_patches {
            // find the corresponding local patch
            let remote_patch_id = sqlx::query_as::<_, Patch>(
                "SELECT * FROM patch WHERE version_string = $1 AND repository_id = $2",
            )
            .bind(remote_patch.version_id.clone())
            .bind(effective_repo_id)
            .fetch_one(&mut *conn)
            .await?
            .id;

            if previous_patch.is_some() {
                let prev = previous_patch.clone().unwrap();
                let prev_version_string = prev.version_id.clone();
                let previous_patch_id = sqlx::query_as::<_, Patch>(
                    "SELECT * FROM patch WHERE version_string = $1 AND repository_id = $2",
                )
                .bind(prev_version_string)
                .bind(effective_repo_id)
                .fetch_one(&mut *conn)
                .await?
                .id;

                sqlx::query!(
                    "INSERT INTO patch_edge (repository_id, current_patch_id, next_patch_id, first_offered, last_offered, is_active)
                        VALUES (?, ?, ?, ?, ?, 1) ON CONFLICT DO UPDATE SET last_offered = ?",
                    effective_repo_id,
                    previous_patch_id,
                    remote_patch_id,
                    now,
                    now,
                    now,
                )
                .execute(&mut *conn)
                .await?;
            } else {
                // first patch of a chain
                sqlx::query!(
                    "INSERT INTO patch_edge (repository_id, next_patch_id, first_offered, last_offered, is_active)
                        VALUES (?, ?, ?, ?, 1) ON CONFLICT DO UPDATE SET last_offered = ?",
                    effective_repo_id,
                    remote_patch_id,
                    now,
                    now,
                    now,
                )
                    .execute(&mut *conn)
                    .await?;
            }

            previous_patch = Some(remote_patch);
        }

        Ok(())
    }

    async fn record_active_status(
        &self,
        conn: &mut DbConnection,
        now: DateTime,
        effective_repo_id: i64,
    ) -> Result<()> {
        sqlx::query!(
            "UPDATE patch SET is_active = 0 WHERE last_offered < ? AND repository_id = ? AND is_active = 1",
            now,
            effective_repo_id
        )
        .execute(&mut *conn)
        .await?;

        sqlx::query!(
            "UPDATE patch_edge SET is_active = 0 WHERE last_offered < ? AND repository_id = ? AND is_active = 1",
            now,
            effective_repo_id
        )
            .execute(&mut *conn)
            .await?;

        Ok(())
    }
}
