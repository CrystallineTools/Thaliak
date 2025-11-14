use axum::{
    Json,
    extract::{Query, State},
    http::{StatusCode, header},
    response::{IntoResponse, Redirect, Response},
};
use axum_extra::extract::cookie::{Cookie, SameSite};
use log::{error, info};
use serde::Deserialize;
use time::Duration;

use crate::auth::AuthenticatedUser;
use crate::db::AppState;
use crate::user_models::UserInfo;

#[derive(Deserialize)]
pub struct OAuthCallbackQuery {
    code: String,
    #[allow(dead_code)]
    state: String,
}

pub async fn init_oauth(State(state): State<AppState>) -> Redirect {
    let auth = state.auth.as_ref().expect("auth should be configured");

    let state_token = uuid::Uuid::new_v4().to_string();
    let auth_url = auth.discord_client.get_authorization_url(&state_token);

    Redirect::to(&auth_url)
}

pub async fn oauth_callback(
    State(state): State<AppState>,
    Query(query): Query<OAuthCallbackQuery>,
) -> Result<impl IntoResponse, AuthRouteError> {
    let auth = state.auth.as_ref().expect("auth should be configured");

    let token_response = auth
        .discord_client
        .exchange_code(&query.code)
        .await
        .map_err(|e| {
            error!("Failed to exchange Discord code: {:?}", e);
            AuthRouteError::DiscordError
        })?;

    let discord_user = auth
        .discord_client
        .get_user_info(&token_response.access_token)
        .await
        .map_err(|e| {
            error!("Failed to get Discord user info: {:?}", e);
            AuthRouteError::DiscordError
        })?;

    // Try to add user to Discord server (if configured)
    if let Err(e) = auth
        .discord_client
        .add_user_to_guild(&discord_user.id, &token_response.access_token)
        .await
    {
        error!("Failed to add user to Discord guild: {:?}", e);
    }

    let user_id: i64 = sqlx::query_scalar(
        r#"
        INSERT INTO user (discord_user_id, discord_username, discord_avatar)
        VALUES (?, ?, ?)
        ON CONFLICT(discord_user_id) DO UPDATE SET
            discord_username = excluded.discord_username,
            discord_avatar = excluded.discord_avatar
        RETURNING id
        "#,
    )
    .bind(&discord_user.id)
    .bind(&discord_user.username)
    .bind(&discord_user.avatar)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to upsert user: {:?}", e);
        AuthRouteError::DatabaseError
    })?;

    let user = sqlx::query_as::<_, crate::user_models::User>(
        r#"SELECT id, discord_user_id, discord_username, discord_avatar, registered_at FROM user WHERE id = ?"#,
    )
    .bind(user_id)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch user: {:?}", e);
        AuthRouteError::DatabaseError
    })?;

    let jwt_token = auth
        .jwt_manager
        .generate_token(user.id, &user.discord_user_id)
        .map_err(|e| {
            error!("Failed to generate JWT: {:?}", e);
            AuthRouteError::InternalError
        })?;

    info!(
        "User {} ({}) authenticated successfully",
        discord_user.username, user.discord_user_id
    );

    let cookie = Cookie::build(("thaliak_token", jwt_token))
        .path("/")
        .http_only(true)
        .same_site(SameSite::Lax)
        .max_age(Duration::days(30))
        .build();

    let redirect_url = format!("{}/webhooks", auth.frontend_url);

    Ok((
        [(header::SET_COOKIE, cookie.to_string())],
        Redirect::to(&redirect_url),
    ))
}

pub async fn get_current_user(
    State(state): State<AppState>,
    AuthenticatedUser(claims): AuthenticatedUser,
) -> Result<Json<UserInfo>, AuthRouteError> {
    let user = sqlx::query_as::<_, crate::user_models::User>(
        r#"SELECT id, discord_user_id, discord_username, discord_avatar, registered_at FROM user WHERE id = ?"#,
    )
    .bind(claims.user_id)
    .fetch_one(&state.db.private)
    .await
    .map_err(|e| {
        error!("Failed to fetch user: {:?}", e);
        AuthRouteError::DatabaseError
    })?;

    Ok(Json(UserInfo {
        id: user.id,
        discord_user_id: user.discord_user_id.clone(),
        discord_username: user
            .discord_username
            .unwrap_or_else(|| format!("<@{}>", user.discord_user_id)),
        discord_avatar: user.discord_avatar,
        registered_at: user.registered_at,
    }))
}

pub async fn logout() -> impl IntoResponse {
    let cookie = Cookie::build(("thaliak_token", ""))
        .path("/")
        .http_only(true)
        .same_site(SameSite::Lax)
        .max_age(Duration::ZERO)
        .build();

    (
        [(header::SET_COOKIE, cookie.to_string())],
        StatusCode::NO_CONTENT,
    )
}

#[derive(Debug)]
pub enum AuthRouteError {
    DiscordError,
    DatabaseError,
    InternalError,
}

impl IntoResponse for AuthRouteError {
    fn into_response(self) -> Response {
        let (status, message) = match self {
            AuthRouteError::DiscordError => (
                StatusCode::BAD_GATEWAY,
                "Failed to communicate with Discord",
            ),
            AuthRouteError::DatabaseError => (StatusCode::INTERNAL_SERVER_ERROR, "Database error"),
            AuthRouteError::InternalError => {
                (StatusCode::INTERNAL_SERVER_ERROR, "Internal server error")
            }
        };

        (status, message).into_response()
    }
}
