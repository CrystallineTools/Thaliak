use crate::{
    db::AppState,
    error::ApiResult,
    models::{ComponentStatus, StatusResponse},
};
use axum::{Json, extract::State};
use chrono::{SecondsFormat, Utc};
use sqlx::Row;

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
        .fetch_one(&state.db.public)
        .await
        .map_err(|e| {
            log::error!("Health check failed: public database error: {}", e);
            e
        })?;

    sqlx::query!("SELECT 1 as health_check")
        .fetch_one(&state.db.private)
        .await
        .map_err(|e| {
            log::error!("Health check failed: private database error: {}", e);
            e
        })?;

    // Fetch all component versions from the database
    let rows = sqlx::query("SELECT component, commit_hash, updated_at FROM component_versions")
        .fetch_all(&state.db.private)
        .await
        .map_err(|e| {
            log::error!("Failed to fetch component versions: {}", e);
            e
        })?;

    let mut components = Vec::new();
    for row in rows {
        if let (Ok(component), Ok(commit), Ok(started_at_str)) = (
            row.try_get::<String, _>("component"),
            row.try_get::<String, _>("commit_hash"),
            row.try_get::<String, _>("updated_at"),
        ) {
            // Parse SQLite datetime format: "YYYY-MM-DD HH:MM:SS"
            if let Ok(started_at) =
                chrono::NaiveDateTime::parse_from_str(&started_at_str, "%Y-%m-%d %H:%M:%S")
            {
                let started_at = started_at.and_utc();
                let uptime_seconds = (Utc::now() - started_at).num_seconds();

                components.push(ComponentStatus {
                    component,
                    commit,
                    started_at: started_at.to_rfc3339_opts(SecondsFormat::Secs, true),
                    uptime_seconds,
                });
            }
        }
    }

    Ok(Json(StatusResponse {
        status: "ok".to_string(),
        database: "connected".to_string(),
        components,
    }))
}
