use chrono::{DateTime, Utc};
use clap::Parser;
use log::info;
use sqlx::migrate::MigrateDatabase;
use sqlx::{PgPool, Sqlite, SqlitePool};
use std::collections::HashMap;
use std::path::Path;
use eyre::eyre::WrapErr;
use eyre::Result;

/// Migration tool to convert Thaliak v1 (Postgres) to v2 (SQLite)
#[derive(Parser, Debug)]
#[command(author, version, about, long_about = None)]
struct Args {
    /// Postgres connection string for v1 database
    #[arg(long, env)]
    v1_db: String,

    /// SQLite database file path for v2 (will be recreated if exists)
    #[arg(long, env)]
    v2_db: String,
}

// V1 (Postgres) Models
#[derive(sqlx::FromRow, Debug, Clone)]
struct V1Patch {
    id: i32,
    repo_version_id: i32,
    repository_id: i32, // Joined from repo_versions table
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

// Migration context to track ID mappings
struct MigrationContext {
    // v1 repo_version_id -> list of v2 patch ids
    repo_version_to_patches: HashMap<i32, Vec<i64>>,
}

impl MigrationContext {
    fn new() -> Self {
        Self {
            repo_version_to_patches: HashMap::new(),
        }
    }
}

#[tokio::main]
async fn main() -> Result<()> {
    thaliak_common::logging::setup(None);

    let args = Args::parse();

    info!("Connecting to v1 database (Postgres)...");
    let v1_pool = PgPool::connect(&args.v1_db)
        .await
        .wrap_err("Failed to connect to v1 database")?;

    // Parse the SQLite connection string to get the file path
    let db_path = args.v2_db.strip_prefix("sqlite://").unwrap_or(&args.v2_db);

    // Remove existing database file if it exists
    if Path::new(db_path).exists() {
        info!("Removing existing v2 database at: {}", db_path);
        std::fs::remove_file(db_path).wrap_err("Failed to remove existing v2 database file")?;
    }

    info!("Creating fresh v2 database at: {}", db_path);
    Sqlite::create_database(&args.v2_db)
        .await
        .wrap_err("Failed to create v2 database")?;

    let v2_pool = SqlitePool::connect(&args.v2_db)
        .await
        .wrap_err("Failed to connect to v2 database")?;

    info!("Running v2 migrations...");
    sqlx::migrate!("../migrations")
        .run(&v2_pool)
        .await
        .wrap_err("Failed to run v2 migrations")?;

    let mut ctx = MigrationContext::new();

    info!("Starting migration...");
    info!("Note: Services, repositories, and expansions are already seeded by v2 migrations");

    migrate_patches(&v1_pool, &v2_pool, &mut ctx).await?;
    migrate_upgrade_paths_to_patch_edges(&v1_pool, &v2_pool, &mut ctx).await?;

    info!("Migration completed successfully!");

    Ok(())
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
        // Extract version string from remote_origin_path
        // e.g., "/patch/D2024.05.31.0000.0000.patch" -> "D2024.05.31.0000.0000"
        let version_string = extract_version_from_path(&patch.remote_origin_path)
            .unwrap_or_else(|| patch.remote_origin_path.clone());

        // Preserve the original patch ID from v1
        sqlx::query(
            r#"
            INSERT INTO patch (
                id, repository_id, version_string, remote_url,
                first_seen, last_seen, size, hash_type, hash_block_size, hashes,
                first_offered, last_offered, is_active
            )
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            "#,
        )
        .bind(patch.id)
        .bind(patch.repository_id)
        .bind(&version_string)
        .bind(&patch.remote_origin_path)
        .bind(patch.first_seen)
        .bind(patch.last_seen)
        .bind(patch.size)
        .bind(&patch.hash_type)
        .bind(patch.hash_block_size)
        .bind(&patch.hashes)
        .bind(patch.first_offered)
        .bind(patch.last_offered)
        .bind(patch.is_active)
        .execute(v2)
        .await?;

        // Track which patches belong to which repo version (using original v1 IDs)
        ctx.repo_version_to_patches
            .entry(patch.repo_version_id)
            .or_insert_with(Vec::new)
            .push(patch.id as i64);
    }

    info!("Successfully migrated {} patches", patch_count);

    // Reset SQLite's autoincrement sequence for the patch table
    // This ensures future inserts will use IDs after the last migrated patch
    if let Some(max_id) = patches.iter().map(|p| p.id).max() {
        sqlx::query("UPDATE sqlite_sequence SET seq = ? WHERE name = 'patch'")
            .bind(max_id)
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
        // Get patches for the current repo_version (these are the "next" patches)
        let next_patches = ctx
            .repo_version_to_patches
            .get(&up.repo_version_id)
            .cloned()
            .unwrap_or_default();

        // Get patches for the previous repo_version (these are the "current" patches)
        let current_patches = if let Some(prev_rv_id) = up.previous_repo_version_id {
            ctx.repo_version_to_patches
                .get(&prev_rv_id)
                .cloned()
                .unwrap_or_default()
        } else {
            vec![]
        };

        // In v2, patch_edge represents: current_patch_id -> next_patch_id
        // If current_patch_id is NULL, it means this is a "head" patch (no predecessor)

        // Create edges for each combination of current -> next
        // In the simple case, we create edges from first patch of prev version to first patch of current version
        if let Some(&next_patch_id) = next_patches.first() {
            let current_patch_id = current_patches.first().copied();

            // Ensure first_offered and last_offered are not None
            let first_offered_str = up
                .first_offered
                .map(|dt| dt.to_rfc3339())
                .unwrap_or_else(|| "1970-01-01T00:00:00Z".to_string());
            let last_offered_str = up
                .last_offered
                .map(|dt| dt.to_rfc3339())
                .unwrap_or_else(|| "9999-12-31T23:59:59Z".to_string());

            sqlx::query(
                r#"
                INSERT INTO patch_edge (
                    repository_id, current_patch_id, next_patch_id,
                    first_offered, last_offered, is_active
                )
                VALUES (?, ?, ?, ?, ?, ?)
                "#,
            )
            .bind(up.repository_id)
            .bind(current_patch_id)
            .bind(next_patch_id)
            .bind(&first_offered_str)
            .bind(&last_offered_str)
            .bind(up.is_active)
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

/// Extracts version string from patch path
/// e.g., "/patch/D2024.05.31.0000.0000.patch" -> "D2024.05.31.0000.0000"
fn extract_version_from_path(path: &str) -> Option<String> {
    path.split('/')
        .last()
        .and_then(|filename| filename.strip_suffix(".patch"))
        .map(|s| s.to_string())
}
