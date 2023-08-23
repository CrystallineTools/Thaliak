use axum::{routing::get, Router};

pub mod agent;

pub async fn index() -> &'static str {
    "Hello, world!"
}

pub fn create_router() -> Router {
    Router::new()
        .nest("/agent", agent::router())
        .route("/", get(index))
}
