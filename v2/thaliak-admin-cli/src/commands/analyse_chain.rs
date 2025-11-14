use eyre::Result;
use log::info;
use thaliak_common::webhook::AnalysisWebhookPayload;
use thaliak_types::{HashType, Patch, Repository};

async fn resolve_patch_chain(
    db: &sqlx::SqlitePool,
    repository_id: i64,
    starting_patch_id: i64,
) -> Result<Vec<Patch>> {
    let mut chain = Vec::new();
    let mut current_patch_id = Some(starting_patch_id);

    loop {
        let Some(patch_id) = current_patch_id else {
            break;
        };

        let row = sqlx::query!(
            r#"
            SELECT id, repository_id, version_string, remote_url, local_path,
                   first_seen, last_seen, size, hash_type, hash_block_size, hashes,
                   first_offered, last_offered, is_active
            FROM patch WHERE id = ?
            "#,
            patch_id
        )
        .fetch_one(db)
        .await?;

        let patch = Patch {
            id: row.id,
            repository_id: row.repository_id,
            version_string: row.version_string,
            remote_url: row.remote_url,
            local_path: row.local_path,
            first_seen: row.first_seen.and_then(|s| s.parse().ok()),
            last_seen: row.last_seen.and_then(|s| s.parse().ok()),
            size: row.size,
            hash: HashType::from_columns(row.hash_type, row.hash_block_size, row.hashes),
            first_offered: row.first_offered.and_then(|s| s.parse().ok()),
            last_offered: row.last_offered.and_then(|s| s.parse().ok()),
            is_active: row.is_active,
        };

        chain.push(patch);

        let previous_edge = sqlx::query_scalar!(
            r#"
            SELECT current_patch_id
            FROM patch_edge
            WHERE next_patch_id = ? AND repository_id = ?
            ORDER BY last_offered DESC
            LIMIT 1
            "#,
            patch_id,
            repository_id
        )
        .fetch_optional(db)
        .await?;

        current_patch_id = previous_edge.flatten();
    }

    chain.reverse();
    Ok(chain)
}

async fn send_analysis_webhook(url: &str, payload: &AnalysisWebhookPayload) -> Result<()> {
    let client = reqwest::Client::builder()
        .timeout(std::time::Duration::from_secs(10))
        .build()?;

    let response = client.post(url).json(payload).send().await?;

    if !response.status().is_success() {
        log::warn!(
            "Analysis webhook {} returned non-success status: {}",
            url,
            response.status()
        );
    }

    Ok(())
}

pub async fn execute(repository_slug: String, patch_version: String) -> Result<()> {
    let pools = thaliak_common::init_dbs().await?;

    let repo_row = sqlx::query!(
        r#"SELECT id, service_id, slug, name, description FROM repository WHERE slug = ?"#,
        repository_slug
    )
    .fetch_one(&pools.public)
    .await?;

    let repository = Repository {
        id: repo_row.id,
        service_id: repo_row.service_id,
        slug: repo_row.slug.clone(),
        name: repo_row.name.clone(),
        description: repo_row.description,
        latest_patch: None,
    };

    info!("Found repository: {} ({})", repo_row.name, repo_row.slug);

    let patch_row = sqlx::query!(
        r#"
        SELECT id, repository_id, version_string, remote_url, local_path,
               first_seen, last_seen, size, hash_type, hash_block_size, hashes,
               first_offered, last_offered, is_active
        FROM patch
        WHERE repository_id = ? AND version_string = ?
        "#,
        repository.id,
        patch_version
    )
    .fetch_one(&pools.public)
    .await?;

    let starting_patch = Patch {
        id: patch_row.id,
        repository_id: patch_row.repository_id,
        version_string: patch_row.version_string,
        remote_url: patch_row.remote_url,
        local_path: patch_row.local_path,
        first_seen: patch_row.first_seen.and_then(|s| s.parse().ok()),
        last_seen: patch_row.last_seen.and_then(|s| s.parse().ok()),
        size: patch_row.size,
        hash: HashType::from_columns(
            patch_row.hash_type,
            patch_row.hash_block_size,
            patch_row.hashes,
        ),
        first_offered: patch_row.first_offered.and_then(|s| s.parse().ok()),
        last_offered: patch_row.last_offered.and_then(|s| s.parse().ok()),
        is_active: patch_row.is_active,
    };

    info!(
        "Found starting patch: {} (ID: {})",
        starting_patch.version_string, starting_patch.id
    );

    let chain = resolve_patch_chain(&pools.public, repository.id, starting_patch.id).await?;

    info!("Resolved patch chain with {} patches", chain.len());

    let analysis_webhook_url = std::env::var("ANALYSIS_WEBHOOK_URL")?;

    println!(
        "Sending {} patches to analysis service at {}",
        chain.len(),
        analysis_webhook_url
    );
    println!();

    for patch in &chain {
        let payload = AnalysisWebhookPayload {
            patch: patch.clone(),
            repository: repository.clone(),
            local_path: patch.local_path.clone(),
        };

        println!("  → {}", patch.version_string);

        send_analysis_webhook(&analysis_webhook_url, &payload).await?;
    }

    println!();
    println!(
        "Successfully sent {} patches to analysis service",
        chain.len()
    );

    Ok(())
}
