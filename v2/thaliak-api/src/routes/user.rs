use axum::{
    Json,
    extract::{Path, State},
    http::StatusCode,
    response::{IntoResponse, Response},
};
use log::{error, info};

use crate::auth::AuthenticatedUser;
use crate::db::AppState;
use crate::user_models::{CreateWebhookRequest, UpdateWebhookRequest, Webhook, WebhooksResponse};

pub async fn list_webhooks(
    State(state): State<AppState>,
    AuthenticatedUser(claims): AuthenticatedUser,
) -> Result<Json<WebhooksResponse>, UserRouteError> {
    let webhooks = sqlx::query_as::<_, Webhook>(
        r#"
        SELECT id, owner_user_id, url, created_at, subscribe_jp, subscribe_kr, subscribe_cn
        FROM webhook
        WHERE owner_user_id = ?
        ORDER BY created_at DESC
        "#,
    )
    .bind(claims.user_id)
    .fetch_all(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch webhooks: {:?}", e);
        UserRouteError::DatabaseError
    })?;

    let total = webhooks.len();

    Ok(Json(WebhooksResponse { webhooks, total }))
}

pub async fn create_webhook(
    State(state): State<AppState>,
    AuthenticatedUser(claims): AuthenticatedUser,
    Json(payload): Json<CreateWebhookRequest>,
) -> Result<(StatusCode, Json<Webhook>), UserRouteError> {
    if !payload.url.starts_with("http://") && !payload.url.starts_with("https://") {
        return Err(UserRouteError::InvalidWebhookUrl);
    }

    let webhook_id: i64 = sqlx::query_scalar(
        r#"
        INSERT INTO webhook (owner_user_id, url, subscribe_jp, subscribe_kr, subscribe_cn)
        VALUES (?, ?, ?, ?, ?)
        RETURNING id
        "#,
    )
    .bind(claims.user_id)
    .bind(&payload.url)
    .bind(payload.subscribe_jp)
    .bind(payload.subscribe_kr)
    .bind(payload.subscribe_cn)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to create webhook: {:?}", e);
        UserRouteError::DatabaseError
    })?;

    let webhook = sqlx::query_as::<_, Webhook>(
        r#"
        SELECT id, owner_user_id, url, created_at, subscribe_jp, subscribe_kr, subscribe_cn
        FROM webhook
        WHERE id = ?
        "#,
    )
    .bind(webhook_id)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch created webhook: {:?}", e);
        UserRouteError::DatabaseError
    })?;

    info!(
        "User {} created webhook {} with URL: {}",
        claims.user_id, webhook_id, payload.url
    );

    Ok((StatusCode::CREATED, Json(webhook)))
}

pub async fn get_webhook(
    State(state): State<AppState>,
    AuthenticatedUser(claims): AuthenticatedUser,
    Path(webhook_id): Path<i64>,
) -> Result<Json<Webhook>, UserRouteError> {
    let webhook = sqlx::query_as::<_, Webhook>(
        r#"
        SELECT id, owner_user_id, url, created_at, subscribe_jp, subscribe_kr, subscribe_cn
        FROM webhook
        WHERE id = ? AND owner_user_id = ?
        "#,
    )
    .bind(webhook_id)
    .bind(claims.user_id)
    .fetch_optional(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch webhook: {:?}", e);
        UserRouteError::DatabaseError
    })?
    .ok_or(UserRouteError::WebhookNotFound)?;

    Ok(Json(webhook))
}

pub async fn update_webhook(
    State(state): State<AppState>,
    AuthenticatedUser(claims): AuthenticatedUser,
    Path(webhook_id): Path<i64>,
    Json(payload): Json<UpdateWebhookRequest>,
) -> Result<Json<Webhook>, UserRouteError> {
    let existing = sqlx::query_scalar::<_, i64>(
        r#"SELECT id FROM webhook WHERE id = ? AND owner_user_id = ?"#,
    )
    .bind(webhook_id)
    .bind(claims.user_id)
    .fetch_optional(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to check webhook ownership: {:?}", e);
        UserRouteError::DatabaseError
    })?
    .ok_or(UserRouteError::WebhookNotFound)?;

    if let Some(ref url) = payload.url {
        if !url.starts_with("http://") && !url.starts_with("https://") {
            return Err(UserRouteError::InvalidWebhookUrl);
        }
    }

    let current = sqlx::query_as::<_, Webhook>(
        r#"SELECT id, owner_user_id, url, created_at, subscribe_jp, subscribe_kr, subscribe_cn FROM webhook WHERE id = ?"#,
    )
    .bind(existing)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch current webhook: {:?}", e);
        UserRouteError::DatabaseError
    })?;

    let new_url = payload.url.unwrap_or(current.url);
    let new_jp = payload.subscribe_jp.unwrap_or(current.subscribe_jp);
    let new_kr = payload.subscribe_kr.unwrap_or(current.subscribe_kr);
    let new_cn = payload.subscribe_cn.unwrap_or(current.subscribe_cn);

    sqlx::query(
        r#"
        UPDATE webhook
        SET url = ?, subscribe_jp = ?, subscribe_kr = ?, subscribe_cn = ?
        WHERE id = ?
        "#,
    )
    .bind(&new_url)
    .bind(new_jp)
    .bind(new_kr)
    .bind(new_cn)
    .bind(webhook_id)
    .execute(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to update webhook: {:?}", e);
        UserRouteError::DatabaseError
    })?;

    let webhook = sqlx::query_as::<_, Webhook>(
        r#"
        SELECT id, owner_user_id, url, created_at, subscribe_jp, subscribe_kr, subscribe_cn
        FROM webhook
        WHERE id = ?
        "#,
    )
    .bind(webhook_id)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch updated webhook: {:?}", e);
        UserRouteError::DatabaseError
    })?;

    info!("User {} updated webhook {}", claims.user_id, webhook_id);

    Ok(Json(webhook))
}

pub async fn delete_webhook(
    State(state): State<AppState>,
    AuthenticatedUser(claims): AuthenticatedUser,
    Path(webhook_id): Path<i64>,
) -> Result<StatusCode, UserRouteError> {
    let result = sqlx::query(r#"DELETE FROM webhook WHERE id = ? AND owner_user_id = ?"#)
        .bind(webhook_id)
        .bind(claims.user_id)
        .execute(&state.db.private)
        .await
        .map_err(|e| {
            error!("Failed to delete webhook: {:?}", e);
            UserRouteError::DatabaseError
        })?;

    if result.rows_affected() == 0 {
        return Err(UserRouteError::WebhookNotFound);
    }

    info!("User {} deleted webhook {}", claims.user_id, webhook_id);

    Ok(StatusCode::NO_CONTENT)
}

#[derive(Debug)]
pub enum UserRouteError {
    DatabaseError,
    WebhookNotFound,
    InvalidWebhookUrl,
}

impl IntoResponse for UserRouteError {
    fn into_response(self) -> Response {
        let (status, message) = match self {
            UserRouteError::DatabaseError => (StatusCode::INTERNAL_SERVER_ERROR, "Database error"),
            UserRouteError::WebhookNotFound => {
                (StatusCode::NOT_FOUND, "Webhook not found or access denied")
            }
            UserRouteError::InvalidWebhookUrl => (
                StatusCode::BAD_REQUEST,
                "Invalid webhook URL - must start with http:// or https://",
            ),
        };

        (status, message).into_response()
    }
}
