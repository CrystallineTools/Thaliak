use crate::{error::ApiResult, models::DatabaseInfo};
use axum::{
    Json,
    body::Body,
    http::{StatusCode, header},
    response::Response,
};
use tokio::fs::File;
use tokio_util::io::ReaderStream;

const DATABASE_PATH: &str = "thaliak.db";

/// GET /database/download - Downloads the Thaliak database, for external data analysis
#[utoipa::path(
    get,
    path = "/database/download",
    responses(
        (status = 200, description = "Database file stream", content_type = "application/octet-stream"),
        (status = 500, description = "Internal server error")
    ),
    tag = "database"
)]
pub async fn download_database() -> ApiResult<Response> {
    let file = File::open(DATABASE_PATH).await?;
    let stream = ReaderStream::new(file);
    let body = Body::from_stream(stream);

    Ok(Response::builder()
        .status(StatusCode::OK)
        .header(header::CONTENT_TYPE, "application/octet-stream")
        .header(
            header::CONTENT_DISPOSITION,
            "attachment; filename=\"thaliak.db\"",
        )
        .body(body)
        .unwrap())
}

/// GET /database/info - Get database file metadata
#[utoipa::path(
    get,
    path = "/database/info",
    responses(
        (status = 200, description = "Database file information", body = DatabaseInfo),
        (status = 404, description = "Database file not found"),
        (status = 500, description = "Internal server error")
    ),
    tag = "database"
)]
pub async fn get_database_info() -> ApiResult<Json<DatabaseInfo>> {
    let metadata = tokio::fs::metadata(DATABASE_PATH).await?;

    let modified = metadata
        .modified()?
        .duration_since(std::time::UNIX_EPOCH)
        .unwrap();
    let modified_str = chrono::DateTime::from_timestamp(modified.as_secs() as i64, 0)
        .unwrap()
        .to_rfc3339();

    Ok(Json(DatabaseInfo {
        filename: "thaliak.db".to_string(),
        size: metadata.len(),
        modified: modified_str,
    }))
}
