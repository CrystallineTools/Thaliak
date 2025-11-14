use axum::{
    Json,
    http::StatusCode,
    response::{IntoResponse, Response},
};
use serde_json::json;
use thiserror::Error;

use crate::metrics;

/// API error types
#[derive(Debug, Error)]
pub enum ApiError {
    #[error("Database error: {0}")]
    Database(#[from] sqlx::Error),

    #[error("Not found: {0}")]
    NotFound(String),

    #[error("Internal server error: {0}")]
    #[allow(dead_code)]
    Internal(String),

    #[error("IO error: {0}")]
    Io(#[from] std::io::Error),
}

impl IntoResponse for ApiError {
    fn into_response(self) -> Response {
        let (status, message, _error_type) = match self {
            ApiError::Database(ref e) => {
                log::error!("Database error: {}", e);
                metrics::record_error("database");
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "Database error occurred".to_string(),
                    "database",
                )
            }
            ApiError::NotFound(ref msg) => {
                metrics::record_error("not_found");
                (StatusCode::NOT_FOUND, msg.clone(), "not_found")
            }
            ApiError::Internal(ref msg) => {
                log::error!("Internal error: {}", msg);
                metrics::record_error("internal");
                (StatusCode::INTERNAL_SERVER_ERROR, msg.clone(), "internal")
            }
            ApiError::Io(ref e) => {
                log::error!("IO error: {}", e);
                metrics::record_error("io");
                (
                    StatusCode::INTERNAL_SERVER_ERROR,
                    "IO error occurred".to_string(),
                    "io",
                )
            }
        };

        let body = Json(json!({
            "error": message,
        }));

        (status, body).into_response()
    }
}

pub type ApiResult<T> = Result<T, ApiError>;
