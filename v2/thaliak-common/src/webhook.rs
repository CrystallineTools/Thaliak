use eyre::Result;
use log::{error, info, warn};
use serde::Serialize;
use sqlx::{FromRow, SqlitePool};
use thaliak_types::{Patch, Repository};

use crate::DatabasePools;

#[derive(Debug, Clone, FromRow)]
struct Webhook {
    id: i64,
    url: String,
    subscribe_jp: bool,
    subscribe_kr: bool,
    subscribe_cn: bool,
}

#[derive(Debug, Clone, Serialize)]
pub struct RepositoryPatches {
    pub repository: Repository,
    pub patches: Vec<Patch>,
}

#[derive(Debug, Clone, Serialize)]
pub struct WebhookPayload {
    pub new_patches: Vec<RepositoryPatches>,
}

pub async fn dispatch_webhooks(db: &DatabasePools, new_patches: Vec<Patch>) -> Result<()> {
    if new_patches.is_empty() {
        return Ok(());
    }

    let webhooks = sqlx::query_as::<_, Webhook>(
        r#"SELECT id, url, subscribe_jp, subscribe_kr, subscribe_cn FROM webhook"#,
    )
    .fetch_all(&db.private)
    .await?;

    if webhooks.is_empty() {
        return Ok(());
    }

    for webhook in webhooks {
        let filtered_patches =
            filter_patches_for_webhook(&webhook, &new_patches, &db.public).await?;

        if filtered_patches.new_patches.is_empty() {
            continue;
        }

        tokio::spawn(async move {
            info!(
                "Dispatching {} repository patch groups to webhook {}",
                filtered_patches.new_patches.len(),
                webhook.url
            );

            if let Err(e) = send_webhook(&webhook.url, &filtered_patches).await {
                error!("Failed to send webhook to {}: {:?}", webhook.url, e);
            }
        });
    }

    Ok(())
}

pub async fn send_webhook_async(url: String, payload: WebhookPayload) {
    tokio::spawn(async move {
        if let Err(e) = send_webhook(&url, &payload).await {
            error!("Failed to send webhook to {}: {:?}", url, e);
        }
    });
}

async fn filter_patches_for_webhook(
    webhook: &Webhook,
    patches: &[Patch],
    public_db: &SqlitePool,
) -> Result<WebhookPayload> {
    use std::collections::HashMap;

    let mut patches_by_repo: HashMap<i64, Vec<Patch>> = HashMap::new();

    for patch in patches {
        let service_id = sqlx::query_scalar!(
            r#"SELECT service_id FROM repository WHERE id = ?"#,
            patch.repository_id
        )
        .fetch_one(public_db)
        .await?;

        let should_include = match service_id.as_str() {
            "jp" => webhook.subscribe_jp,
            "kr" => webhook.subscribe_kr,
            "cn" => webhook.subscribe_cn,
            _ => {
                warn!("Unknown service_id: {}", service_id);
                false
            }
        };

        if should_include {
            patches_by_repo
                .entry(patch.repository_id)
                .or_default()
                .push(patch.clone());
        }
    }

    let mut repo_patches = Vec::new();
    for (repo_id, patches) in patches_by_repo {
        let (repo_service_id, repo_slug, repo_name, repo_description): (
            String,
            String,
            String,
            Option<String>,
        ) = sqlx::query_as(
            r#"SELECT service_id, slug, name, description FROM repository WHERE id = ?"#,
        )
        .bind(repo_id)
        .fetch_one(public_db)
        .await?;

        repo_patches.push(RepositoryPatches {
            repository: Repository {
                id: repo_id,
                service_id: repo_service_id,
                slug: repo_slug,
                name: repo_name,
                description: repo_description,
                latest_patch: None,
            },
            patches,
        });
    }

    Ok(WebhookPayload {
        new_patches: repo_patches,
    })
}

async fn send_webhook(url: &str, payload: &WebhookPayload) -> Result<()> {
    info!("Sending webhook to {}", url);

    let client = reqwest::Client::builder()
        .timeout(std::time::Duration::from_secs(10))
        .build()?;

    let response = client.post(url).json(payload).send().await?;

    if !response.status().is_success() {
        warn!(
            "Webhook {} returned non-success status: {}",
            url,
            response.status()
        );
    }

    Ok(())
}
