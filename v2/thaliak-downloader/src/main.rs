use axum::{Json, Router, extract::State, http::StatusCode, routing::post};
use eyre::Result;
use log::{error, info, warn};
use std::path::{Path, PathBuf};
use std::sync::Arc;
use thaliak_common::{
    DatabasePools,
    patch::get_local_storage_path,
    webhook::{AnalysisWebhookPayload, WebhookPayload},
};
use thaliak_types::{Patch, Repository};
use tokio::sync::mpsc;

#[derive(Debug, Clone)]
struct DownloadJob {
    patch: Patch,
    repository: Repository,
}

#[derive(Clone)]
struct AppState {
    download_tx: mpsc::UnboundedSender<DownloadJob>,
}

async fn webhook_handler(
    State(state): State<Arc<AppState>>,
    Json(payload): Json<WebhookPayload>,
) -> StatusCode {
    info!(
        "Received webhook with {} repository patch groups",
        payload.new_patches.len()
    );

    for repo_patches in payload.new_patches {
        for patch in repo_patches.patches {
            let job = DownloadJob {
                patch,
                repository: repo_patches.repository.clone(),
            };

            if let Err(e) = state.download_tx.send(job) {
                error!("Failed to queue download job: {:?}", e);
            }
        }
    }

    StatusCode::OK
}

async fn download_worker(
    mut rx: mpsc::UnboundedReceiver<DownloadJob>,
    db: DatabasePools,
    download_path: PathBuf,
    analysis_webhook_url: String,
) {
    while let Some(job) = rx.recv().await {
        if let Err(e) = process_download(&job, &db, &download_path, &analysis_webhook_url).await {
            error!(
                "Failed to download patch {}: {:?}",
                job.patch.version_string, e
            );
        }
    }
}

async fn process_download(
    job: &DownloadJob,
    db: &DatabasePools,
    download_path: &Path,
    analysis_webhook_url: &str,
) -> Result<()> {
    let local_storage_path = get_local_storage_path(&job.patch.remote_url)?;
    let dest_path = download_path.join(&local_storage_path);

    if let Some(parent) = dest_path.parent() {
        tokio::fs::create_dir_all(parent).await?;
    }

    if dest_path.exists() {
        info!(
            "Skipping download of {} as it already exists locally at {}",
            job.patch.version_string,
            dest_path.display()
        );
    } else {
        info!(
            "Starting download of {} from {} to {}",
            job.patch.version_string,
            job.patch.remote_url,
            dest_path.display()
        );

        let client = reqwest::Client::builder()
            .timeout(std::time::Duration::from_secs(3600))
            .build()?;

        let response = client.get(&job.patch.remote_url).send().await?;

        if !response.status().is_success() {
            warn!(
                "Download of {} returned non-success status: {}",
                job.patch.version_string,
                response.status()
            );
            return Ok(());
        }

        let bytes = response.bytes().await?;
        tokio::fs::write(&dest_path, bytes).await?;

        info!("Download complete for {}", job.patch.version_string);
    }

    sqlx::query!(
        "UPDATE patch SET local_path = ? WHERE id = ?",
        &local_storage_path,
        job.patch.id
    )
    .execute(&db.public)
    .await?;

    info!(
        "Updated local_path for patch {} to {}",
        job.patch.version_string, local_storage_path
    );

    // send a message to the analysis service letting it know we're done here
    let payload = AnalysisWebhookPayload {
        patch: job.patch.clone(),
        repository: job.repository.clone(),
        local_path: local_storage_path.clone(),
    };

    let analysis_webhook_url = analysis_webhook_url.to_owned();
    tokio::spawn({
        async move {
            if let Err(e) = send_analysis_webhook(&analysis_webhook_url, &payload).await {
                error!("Failed to send analysis webhook: {:?}", e);
            }
        }
    });

    Ok(())
}

async fn send_analysis_webhook(url: &str, payload: &AnalysisWebhookPayload) -> Result<()> {
    info!("Sending analysis webhook to {}", url);

    let client = reqwest::Client::builder()
        .timeout(std::time::Duration::from_secs(10))
        .build()?;

    let response = client.post(url).json(payload).send().await?;

    if !response.status().is_success() {
        warn!(
            "Analysis webhook {} returned non-success status: {}",
            url,
            response.status()
        );
    }

    Ok(())
}

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let pools = thaliak_common::init_dbs().await?;
    thaliak_common::logging::setup(None);

    let commit_hash = env!("GIT_COMMIT_HASH");
    info!("downloader service started (commit: {})", commit_hash);

    thaliak_common::version::update_component_version(&pools.private, "downloader", commit_hash)
        .await?;

    let download_path: PathBuf = std::env::var("DOWNLOAD_PATH")?.into();
    let analysis_webhook_url = std::env::var("ANALYSIS_WEBHOOK_URL")?;

    let (download_tx, download_rx) = mpsc::unbounded_channel();

    let state = Arc::new(AppState { download_tx });

    tokio::spawn(download_worker(
        download_rx,
        pools.clone(),
        download_path,
        analysis_webhook_url,
    ));

    let app = Router::new()
        .route("/", post(webhook_handler))
        .with_state(state);

    let listener = tokio::net::TcpListener::bind("0.0.0.0:8081").await?;
    info!("downloader service listening on {}", listener.local_addr()?);

    axum::serve(listener, app).await?;

    Ok(())
}
