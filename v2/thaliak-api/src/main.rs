mod db;
mod error;
mod models;
mod routes;

use axum::{Router, routing::get};
use log::info;
use std::net::SocketAddr;
use tokio::net::TcpListener;
use tower_http::cors::{Any, CorsLayer};
use utoipa::OpenApi;
use utoipa_swagger_ui::SwaggerUi;

use crate::db::AppState;

#[derive(OpenApi)]
#[openapi(
    info(
        title = "Thaliak API",
        version = "2.0",
        description = "Final Fantasy XIV patch tracking API",
    ),
    paths(
        routes::health::health_check,
        routes::services::get_services,
        routes::repositories::get_repositories,
        routes::repositories::get_repository,
        routes::patches::get_repository_patches,
        routes::patches::get_repository_patch,
        routes::database::download_database,
        routes::database::get_database_info,
    ),
    components(
        schemas(
            models::HealthResponse,
            models::ServicesResponse,
            models::RepositoriesResponse,
            models::PatchesResponse,
            models::DatabaseInfo,
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
        (name = "database", description = "Database download and information"),
    )
)]
struct ApiDoc;

#[tokio::main]
async fn main() -> eyre::Result<()> {
    let _ = dotenvy::dotenv();
    let pool = thaliak_common::init_db().await?;
    thaliak_common::logging::setup(None);

    let state = AppState::new(pool);
    let cors = CorsLayer::new()
        .allow_origin(Any)
        .allow_methods(Any)
        .allow_headers(Any);

    let app = Router::new()
        .route("/health", get(routes::health::health_check))
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
        .route(
            "/database/download",
            get(routes::database::download_database),
        )
        .route("/database/info", get(routes::database::get_database_info))
        .merge(SwaggerUi::new("/").url("/openapi.json", ApiDoc::openapi()))
        .with_state(state)
        .layer(cors);

    let addr = SocketAddr::from(([0, 0, 0, 0], 8080));
    info!("starting Thaliak API server on http://{}", addr);

    let listener = TcpListener::bind(addr).await?;
    axum::serve(listener, app).await?;

    Ok(())
}
