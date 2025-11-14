use axum::{Router, http::StatusCode, routing::post, Json};
use eyre::Result;
use log::info;
use thaliak_common::webhook::AnalysisWebhookPayload;

async fn webhook_handler(Json(payload): Json<AnalysisWebhookPayload>) -> StatusCode {
    info!(
        "Received analysis webhook for patch {} ({})",
        payload.patch.version_string, payload.repository.slug
    );
    info!("  Local path: {}", payload.local_path);
    info!("  Patch size: {} bytes", payload.patch.size);

    StatusCode::OK
}

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let pools = thaliak_common::init_dbs().await?;
    thaliak_common::logging::setup(None);

    let commit_hash = env!("GIT_COMMIT_HASH");
    info!("analysis service started (commit: {})", commit_hash);

    thaliak_common::version::update_component_version(&pools.private, "analysis", commit_hash)
        .await?;

    let app = Router::new().route("/", post(webhook_handler));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:8082").await?;
    info!("analysis service listening on {}", listener.local_addr()?);

    axum::serve(listener, app).await?;

    Ok(())
}
