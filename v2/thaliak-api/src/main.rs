mod auth;
mod db;
mod error;
mod metrics;
mod models;
mod routes;
mod user_models;

use axum::{
    Router, middleware,
    routing::{delete, get, patch, post},
};
use log::info;
use std::net::SocketAddr;
use tokio::net::TcpListener;
use tower_http::cors::{Any, CorsLayer};
use utoipa::openapi::Server;
use utoipa::{Modify, OpenApi};
use utoipa_swagger_ui::SwaggerUi;

use crate::auth::{DiscordOAuthClient, JwtManager};
use crate::db::{AppState, AuthData};

#[derive(OpenApi)]
#[openapi(
    modifiers(&PathPrefixAddon),
    info(
        title = "Thaliak API",
        version = "2.0 (BETA)",
        description = "Final Fantasy XIV patch tracking API.\n\
        \n\
        If you would like to perform data analysis beyond what the API provides,\
        the SQLite database used by Thaliak is downloadable at https://thaliak.xiv.dev/data/thaliak.db",
    ),
    paths(
        routes::status::status,
        routes::services::get_services,
        routes::repositories::get_repositories,
        routes::repositories::get_repository,
        routes::patches::get_repository_patches,
        routes::patches::get_repository_patch,
    ),
    components(
        schemas(
            models::StatusResponse,
            models::ServicesResponse,
            models::RepositoriesResponse,
            models::PatchesResponse,
            models::PatchQueryParams,
            thaliak_types::Service,
            thaliak_types::Repository,
            thaliak_types::Patch,
            thaliak_types::HashType,
        )
    ),
    tags(
        (name = "metadata", description = "API metadata and health endpoints"),
        (name = "services", description = "Game service information"),
        (name = "repositories", description = "Patch repository information"),
        (name = "patches", description = "Patch file information and chain resolution"),
    )
)]
struct ApiDoc;

struct PathPrefixAddon;
impl Modify for PathPrefixAddon {
    fn modify(&self, openapi: &mut utoipa::openapi::OpenApi) {
        match std::env::var("API_BASE_PATH") {
            Ok(prefix) => openapi.servers = Some(vec![Server::new(prefix)]),
            Err(_) => (),
        }
    }
}

fn check_discord_config() -> Option<(DiscordOAuthClient, JwtManager, String)> {
    match (
        std::env::var("DISCORD_CLIENT_ID").ok(),
        std::env::var("DISCORD_CLIENT_SECRET").ok(),
        std::env::var("DISCORD_REDIRECT_URL").ok(),
        std::env::var("JWT_SECRET").ok(),
        std::env::var("FRONTEND_URL").ok(),
    ) {
        (
            Some(client_id),
            Some(client_secret),
            Some(redirect_uri),
            Some(jwt_secret),
            Some(frontend_url),
        ) => {
            info!("Discord OAuth is configured, authentication endpoints will be available");

            let bot_token = std::env::var("DISCORD_BOT_TOKEN").ok();
            let guild_id = std::env::var("DISCORD_GUILD_ID").ok();

            if bot_token.is_some() && guild_id.is_some() {
                info!(
                    "Discord auto-join is configured, users will be added to the server on sign-in"
                );
            } else {
                info!("Discord auto-join is not configured (optional)");
            }

            Some((
                DiscordOAuthClient::new(
                    client_id,
                    client_secret,
                    redirect_uri,
                    bot_token,
                    guild_id,
                ),
                JwtManager::new(&jwt_secret),
                frontend_url,
            ))
        }
        _ => {
            info!("Discord OAuth is not configured, authentication endpoints will be disabled");
            info!(
                "To enable authentication, set: DISCORD_CLIENT_ID, DISCORD_CLIENT_SECRET, DISCORD_REDIRECT_URL, JWT_SECRET, FRONTEND_URL"
            );
            None
        }
    }
}

fn build_public_routes() -> Router<AppState> {
    let base_path = std::env::var("API_BASE_PATH").unwrap_or_default();
    let swagger = {
        let config = utoipa_swagger_ui::Config::from(format!("{}/openapi.json", base_path));
        SwaggerUi::new("/")
            .url("/openapi.json", ApiDoc::openapi())
            .config(config)
    };

    let public_cors = CorsLayer::new()
        .allow_origin(Any)
        .allow_methods(Any)
        .allow_headers(Any);

    Router::new()
        .route("/status", get(routes::status::status))
        .route("/services", get(routes::services::get_services))
        .route("/repositories", get(routes::repositories::get_repositories))
        .route(
            "/repositories/{slug}",
            get(routes::repositories::get_repository),
        )
        .route(
            "/repositories/{slug}/patches",
            get(routes::patches::get_repository_patches),
        )
        .route(
            "/repositories/{slug}/patches/{version}",
            get(routes::patches::get_repository_patch),
        )
        .merge(swagger)
        .layer(public_cors)
}

fn build_auth_routes(state: &AppState) -> Router<AppState> {
    use axum::http::{self, Method, header};
    let auth_data = state.auth.as_ref().expect("auth should be configured");
    let auth_cors = CorsLayer::new()
        .allow_origin(auth_data.frontend_url.parse::<http::HeaderValue>().unwrap())
        .allow_methods([Method::GET, Method::POST, Method::OPTIONS])
        .allow_headers([header::CONTENT_TYPE, header::AUTHORIZATION])
        .allow_credentials(true);

    Router::new()
        .route("/auth/discord/init", get(routes::auth::init_oauth))
        .route("/auth/discord/callback", get(routes::auth::oauth_callback))
        .route("/auth/logout", post(routes::auth::logout))
        .route(
            "/auth/me",
            get(routes::auth::get_current_user).layer(middleware::from_fn_with_state(
                state.clone(),
                auth::auth_middleware,
            )),
        )
        .layer(auth_cors)
}

fn build_user_routes(state: &AppState) -> Router<AppState> {
    use axum::http::{self, Method, header};
    let auth_data = state.auth.as_ref().expect("auth should be configured");
    let user_cors = CorsLayer::new()
        .allow_origin(auth_data.frontend_url.parse::<http::HeaderValue>().unwrap())
        .allow_methods([
            Method::GET,
            Method::POST,
            Method::PATCH,
            Method::DELETE,
            Method::OPTIONS,
        ])
        .allow_headers([header::CONTENT_TYPE, header::AUTHORIZATION])
        .allow_credentials(true);

    Router::new()
        .route("/user/webhooks", get(routes::user::list_webhooks))
        .route("/user/webhooks", post(routes::user::create_webhook))
        .route("/user/webhooks/{id}", get(routes::user::get_webhook))
        .route("/user/webhooks/{id}", patch(routes::user::update_webhook))
        .route("/user/webhooks/{id}", delete(routes::user::delete_webhook))
        .route("/user/webhooks/{id}/test", post(routes::user::test_webhook))
        .layer(middleware::from_fn_with_state(
            state.clone(),
            auth::auth_middleware,
        ))
        .layer(user_cors)
}

#[tokio::main]
async fn main() -> eyre::Result<()> {
    let _ = dotenvy::dotenv();
    let pools = thaliak_common::init_dbs().await?;
    thaliak_common::logging::setup(None);
    metrics::init_metrics_exporter().await?;

    let auth = check_discord_config()
        .map(|(client, manager, frontend_url)| AuthData::new(client, manager, frontend_url));

    // Update API version in the database
    thaliak_common::version::update_component_version(
        &pools.private,
        "api",
        env!("GIT_COMMIT_HASH"),
    )
    .await?;

    let state = AppState::new(pools, auth);

    let mut app = Router::new().merge(build_public_routes());

    if state.auth.is_some() {
        app = app
            .merge(build_auth_routes(&state))
            .merge(build_user_routes(&state));
    }

    let app = app
        .with_state(state)
        .layer(middleware::from_fn(metrics::track_metrics));

    let addr = SocketAddr::from(([0, 0, 0, 0], 8080));
    info!(
        "starting Thaliak API server on http://{} (commit: {})",
        addr,
        env!("GIT_COMMIT_HASH")
    );
    info!(
        "Prometheus metrics available at http://{}:{}",
        addr.ip(),
        9090
    );

    let listener = TcpListener::bind(addr).await?;
    axum::serve(listener, app).await?;

    Ok(())
}
