use eyre::Result;
use log::{error, info, warn};
use serde::Serialize;
use sqlx::{FromRow, SqlitePool};
use thaliak_common::DatabasePools;
use thaliak_types::Patch;

#[derive(Debug, Clone, FromRow)]
struct Webhook {
    id: i64,
    url: String,
    subscribe_jp: bool,
    subscribe_kr: bool,
    subscribe_cn: bool,
}

#[derive(Debug, Serialize)]
struct WebhookPayload {
    patches: Vec<Patch>,
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

        if filtered_patches.is_empty() {
            continue;
        }

        tokio::spawn(async move {
            info!(
                "Dispatching {} patches to webhook {}",
                filtered_patches.len(),
                webhook.url
            );

            if let Err(e) = send_webhook(&webhook.url, &filtered_patches).await {
                error!("Failed to send webhook to {}: {:?}", webhook.url, e);
            }
        });
    }

    Ok(())
}

async fn filter_patches_for_webhook(
    webhook: &Webhook,
    patches: &[Patch],
    public_db: &SqlitePool,
) -> Result<Vec<Patch>> {
    let mut filtered = Vec::new();

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
            filtered.push(patch.clone());
        }
    }

    Ok(filtered)
}

async fn send_webhook(url: &str, patches: &[Patch]) -> Result<()> {
    let client = reqwest::Client::builder()
        .timeout(std::time::Duration::from_secs(10))
        .build()?;

    let payload = WebhookPayload {
        patches: patches.to_vec(),
    };

    let response = client.post(url).json(&payload).send().await?;

    if !response.status().is_success() {
        warn!(
            "Webhook {} returned non-success status: {}",
            url,
            response.status()
        );
    }

    Ok(())
}
