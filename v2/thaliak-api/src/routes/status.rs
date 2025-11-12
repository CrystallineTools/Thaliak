use crate::{db::AppState, error::ApiResult, models::StatusResponse};
use axum::{Json, extract::State};

/// GET /status - Thaliak API status
#[utoipa::path(
    get,
    path = "/status",
    responses(
        (status = 200, description = "Service status", body = StatusResponse),
        (status = 500, description = "Service unhealthy")
    ),
    tag = "metadata"
)]
pub async fn status(State(state): State<AppState>) -> ApiResult<Json<StatusResponse>> {
    // Check database connectivity
    sqlx::query!("SELECT 1 as health_check")
        .fetch_one(&state.pool)
        .await
        .map_err(|e| {
            log::error!("Health check failed: database error: {}", e);
            e
        })?;

    Ok(Json(StatusResponse {
        status: "ok".to_string(),
        database: "connected".to_string(),
    }))
}
