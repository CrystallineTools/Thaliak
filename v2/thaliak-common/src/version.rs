use sqlx::SqlitePool;

/// Update the component version in the database
///
/// This function upserts the component version information into the component_versions table.
/// The updated_at timestamp is automatically set to the current time on each update.
///
/// # Arguments
/// * `pool` - The SQLite connection pool for the private database
/// * `component` - The component name (e.g., "api", "poller")
/// * `commit_hash` - The git commit hash for this component
pub async fn update_component_version(
    pool: &SqlitePool,
    component: &str,
    commit_hash: &str,
) -> Result<(), sqlx::Error> {
    sqlx::query(
        "INSERT INTO component_versions (component, commit_hash, updated_at) VALUES (?, ?, CURRENT_TIMESTAMP)
         ON CONFLICT(component) DO UPDATE SET commit_hash = excluded.commit_hash, updated_at = CURRENT_TIMESTAMP"
    )
    .bind(component)
    .bind(commit_hash)
    .execute(pool)
    .await?;

    Ok(())
}
