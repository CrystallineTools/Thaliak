use chrono::{DateTime, Utc};
use eyre::Result;
use eyre::eyre::WrapErr;
use log::info;
use sqlx::{PgPool, SqlitePool};
use std::collections::HashMap;

#[derive(sqlx::FromRow, Debug, Clone)]
struct V1Patch {
    id: i32,
    repo_version_id: i32,
    repository_id: i32,
    remote_origin_path: String,
    first_seen: Option<DateTime<Utc>>,
    last_seen: Option<DateTime<Utc>>,
    first_offered: Option<DateTime<Utc>>,
    last_offered: Option<DateTime<Utc>>,
    size: i64,
    hash_type: Option<String>,
    hash_block_size: Option<i64>,
    hashes: Option<String>,
    is_active: bool,
}

#[derive(sqlx::FromRow, Debug, Clone)]
struct V1UpgradePath {
    repository_id: i32,
    repo_version_id: i32,
    previous_repo_version_id: Option<i32>,
    first_offered: Option<DateTime<Utc>>,
    last_offered: Option<DateTime<Utc>>,
    is_active: bool,
}

struct MigrationContext {
    repo_version_to_patches: HashMap<i32, Vec<i64>>,
}

impl MigrationContext {
    fn new() -> Self {
        Self {
            repo_version_to_patches: HashMap::new(),
        }
    }
}

async fn migrate_patches(v1: &PgPool, v2: &SqlitePool, ctx: &mut MigrationContext) -> Result<()> {
    info!("Migrating patches...");

    let patches: Vec<V1Patch> = sqlx::query_as(
        r#"
        SELECT p.id, p.repo_version_id, rv.repository_id, p.remote_origin_path,
               p.first_seen, p.last_seen, p.first_offered, p.last_offered,
               p.size, p.hash_type, p.hash_block_size, p.hashes, p.is_active
        FROM patches p
        INNER JOIN repo_versions rv ON p.repo_version_id = rv.id
        ORDER BY p.repo_version_id, p.id
        "#,
    )
    .fetch_all(v1)
    .await?;

    let patch_count = patches.len();
    info!("Found {} patches to migrate", patch_count);

    for patch in &patches {
        let version_string = extract_version_from_path(&patch.remote_origin_path)
            .unwrap_or_else(|| patch.remote_origin_path.clone());

        sqlx::query!(
            r#"
            INSERT INTO patch (
                id, repository_id, version_string, remote_url,
                first_seen, last_seen, size, hash_type, hash_block_size, hashes,
                first_offered, last_offered, is_active
            )
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            "#,
            patch.id,
            patch.repository_id,
            version_string,
            patch.remote_origin_path,
            patch.first_seen,
            patch.last_seen,
            patch.size,
            patch.hash_type,
            patch.hash_block_size,
            patch.hashes,
            patch.first_offered,
            patch.last_offered,
            patch.is_active
        )
        .execute(v2)
        .await?;

        ctx.repo_version_to_patches
            .entry(patch.repo_version_id)
            .or_insert_with(Vec::new)
            .push(patch.id as i64);
    }

    info!("Successfully migrated {} patches", patch_count);

    if let Some(max_id) = patches.iter().map(|p| p.id).max() {
        sqlx::query!(
            "UPDATE sqlite_sequence SET seq = ? WHERE name = 'patch'",
            max_id
        )
        .execute(v2)
        .await?;
    }

    Ok(())
}

async fn migrate_upgrade_paths_to_patch_edges(
    v1: &PgPool,
    v2: &SqlitePool,
    ctx: &mut MigrationContext,
) -> Result<()> {
    info!("Migrating upgrade paths to patch edges...");

    let upgrade_paths: Vec<V1UpgradePath> = sqlx::query_as(
        r#"
        SELECT repository_id, repo_version_id, previous_repo_version_id,
               first_offered, last_offered, is_active
        FROM upgrade_paths
        "#,
    )
    .fetch_all(v1)
    .await?;

    let upgrade_path_count = upgrade_paths.len();
    info!("Found {} upgrade paths to migrate", upgrade_path_count);

    for up in &upgrade_paths {
        let next_patches = ctx
            .repo_version_to_patches
            .get(&up.repo_version_id)
            .cloned()
            .unwrap_or_default();

        let current_patches = if let Some(prev_rv_id) = up.previous_repo_version_id {
            ctx.repo_version_to_patches
                .get(&prev_rv_id)
                .cloned()
                .unwrap_or_default()
        } else {
            vec![]
        };

        if let Some(&next_patch_id) = next_patches.first() {
            let current_patch_id = current_patches.first().copied();

            let first_offered_str = up
                .first_offered
                .map(|dt| dt.to_rfc3339())
                .unwrap_or_else(|| "1970-01-01T00:00:00Z".to_string());
            let last_offered_str = up
                .last_offered
                .map(|dt| dt.to_rfc3339())
                .unwrap_or_else(|| "9999-12-31T23:59:59Z".to_string());

            sqlx::query!(
                r#"
                INSERT INTO patch_edge (
                    repository_id, current_patch_id, next_patch_id,
                    first_offered, last_offered, is_active
                )
                VALUES (?, ?, ?, ?, ?, ?)
                "#,
                up.repository_id,
                current_patch_id,
                next_patch_id,
                first_offered_str,
                last_offered_str,
                up.is_active
            )
            .execute(v2)
            .await?;
        }
    }

    info!(
        "Successfully migrated {} upgrade paths to patch edges",
        upgrade_path_count
    );
    Ok(())
}

fn extract_version_from_path(path: &str) -> Option<String> {
    path.split('/')
        .last()
        .and_then(|filename| filename.strip_suffix(".patch"))
        .map(|s| s.to_string())
}

pub async fn execute(v1_db: String) -> Result<()> {
    info!("Connecting to v1 database (Postgres)...");
    let v1_pool = PgPool::connect(&v1_db)
        .await
        .wrap_err("Failed to connect to v1 database")?;

    info!("Connecting to v2 database (SQLite)...");
    let pools = thaliak_common::init_dbs().await?;

    let mut ctx = MigrationContext::new();

    info!("Starting migration...");
    info!("Note: Services, repositories, and expansions are already seeded by v2 migrations");
    info!("WARNING: This will add data to the existing v2 database without clearing it first");

    migrate_patches(&v1_pool, &pools.public, &mut ctx).await?;
    migrate_upgrade_paths_to_patch_edges(&v1_pool, &pools.public, &mut ctx).await?;

    info!("Migration completed successfully!");

    Ok(())
}
