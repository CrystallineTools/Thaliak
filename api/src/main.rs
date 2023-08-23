use crate::routes::create_router;
use axum::Server;
use log::info;
use std::time::Duration;
use tower_http::cors::{self, CorsLayer};

mod routes;

#[tokio::main]
async fn main() {
    thaliak_logging::setup(None);
    info!("starting Thaliak API on http://*:6900");

    let cors = CorsLayer::new()
        .allow_origin(cors::Any)
        .allow_methods(cors::Any)
        .allow_headers(cors::Any)
        .max_age(Duration::from_secs(86400));

    let app = create_router().layer(cors);

    // set up axum server
    Server::bind(&"0.0.0.0:6900".parse().unwrap())
        .serve(app.into_make_service())
        .await
        .unwrap();
}
