use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use sqlx::FromRow;

#[derive(Debug, Clone, FromRow, Serialize)]
pub struct User {
    pub id: i64,
    pub discord_user_id: String,
    pub discord_username: Option<String>,
    pub discord_avatar: Option<String>,
    pub registered_at: DateTime<Utc>,
}

#[derive(Debug, Clone, FromRow, Serialize)]
pub struct Webhook {
    pub id: i64,
    pub owner_user_id: Option<i64>,
    pub url: String,
    pub created_at: DateTime<Utc>,
    pub subscribe_jp: bool,
    pub subscribe_kr: bool,
    pub subscribe_cn: bool,
}

#[derive(Debug, Deserialize)]
pub struct CreateWebhookRequest {
    pub url: String,
    pub subscribe_jp: bool,
    pub subscribe_kr: bool,
    pub subscribe_cn: bool,
}

#[derive(Debug, Deserialize)]
pub struct UpdateWebhookRequest {
    pub url: Option<String>,
    pub subscribe_jp: Option<bool>,
    pub subscribe_kr: Option<bool>,
    pub subscribe_cn: Option<bool>,
}

#[derive(Debug, Serialize)]
pub struct UserInfo {
    pub id: i64,
    pub discord_user_id: String,
    pub discord_username: String,
    pub discord_avatar: Option<String>,
    pub registered_at: DateTime<Utc>,
}

#[derive(Debug, Serialize)]
pub struct WebhooksResponse {
    pub webhooks: Vec<Webhook>,
    pub total: usize,
}
