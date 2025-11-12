use crate::{db::AppState, error::ApiResult, models::HealthResponse};
use axum::{Json, extract::State};

/// GET /health - Health check
#[utoipa::path(
    get,
    path = "/health",
    responses(
        (status = 200, description = "Service health status", body = HealthResponse),
        (status = 500, description = "Service unhealthy")
    ),
    tag = "metadata"
)]
pub async fn health_check(State(state): State<AppState>) -> ApiResult<Json<HealthResponse>> {
    // Check database connectivity
    sqlx::query!("SELECT 1 as health_check")
        .fetch_one(&state.pool)
        .await
        .map_err(|e| {
            log::error!("Health check failed: database error: {}", e);
            e
        })?;

    Ok(Json(HealthResponse {
        status: "ok".to_string(),
        database: "connected".to_string(),
    }))
}
