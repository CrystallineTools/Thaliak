use crate::{db::AppState, error::ApiResult, models::ServicesResponse};
use axum::{Json, extract::State};

/// GET /services - List all services
#[utoipa::path(
    get,
    path = "/services",
    responses(
        (status = 200, description = "List of all services", body = ServicesResponse),
        (status = 500, description = "Internal server error")
    ),
    tag = "services"
)]
pub async fn get_services(State(state): State<AppState>) -> ApiResult<Json<ServicesResponse>> {
    let services = crate::db::get_services(&state.pool).await?;
    let total = services.len();

    Ok(Json(ServicesResponse { services, total }))
}
