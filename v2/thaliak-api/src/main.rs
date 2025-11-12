mod db;
mod error;
mod models;
mod routes;

use axum::{Router, routing::get};
use log::info;
use std::net::SocketAddr;
use tokio::net::TcpListener;
use tower_http::cors::{Any, CorsLayer};
use utoipa::openapi::Server;
use utoipa::{Modify, OpenApi};
use utoipa_swagger_ui::SwaggerUi;

use crate::db::AppState;

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

#[tokio::main]
async fn main() -> eyre::Result<()> {
    let _ = dotenvy::dotenv();
    let pool = thaliak_common::init_db().await?;
    thaliak_common::logging::setup(None);

    // needed so Swagger UI works with reverse proxies
    let base_path = std::env::var("API_BASE_PATH").unwrap_or_default();
    let swagger = {
        let config = utoipa_swagger_ui::Config::from(format!("{}/openapi.json", base_path));
        SwaggerUi::new("/")
            .url("/openapi.json", ApiDoc::openapi())
            .config(config)
    };

    let state = AppState::new(pool);
    let cors = CorsLayer::new()
        .allow_origin(Any)
        .allow_methods(Any)
        .allow_headers(Any);

    let app = Router::new()
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
        .with_state(state)
        .layer(cors);

    let addr = SocketAddr::from(([0, 0, 0, 0], 8080));
    info!("starting Thaliak API server on http://{}", addr);

    let listener = TcpListener::bind(addr).await?;
    axum::serve(listener, app).await?;

    Ok(())
}
