use crate::{db::AppState, error::ApiResult, models::RepositoriesResponse};
use axum::{
    Json,
    extract::{Path, State},
};
use thaliak_types::Repository;

#[utoipa::path(
    get,
    path = "/repositories",
    responses(
        (status = 200, description = "List of all repositories", body = RepositoriesResponse),
        (status = 500, description = "Internal server error")
    ),
    tag = "repositories"
)]
pub async fn get_repositories(
    State(state): State<AppState>,
) -> ApiResult<Json<RepositoriesResponse>> {
    let repositories = crate::db::get_repositories(&state.pool).await?;
    let total = repositories.len();

    Ok(Json(RepositoriesResponse {
        repositories,
        total,
    }))
}

#[utoipa::path(
    get,
    path = "/repositories/{slug}",
    params(
        ("slug" = String, Path, description = "Repository slug")
    ),
    responses(
        (status = 200, description = "Repository details", body = Repository),
        (status = 404, description = "Repository not found"),
        (status = 500, description = "Internal server error")
    ),
    tag = "repositories"
)]
pub async fn get_repository(
    State(state): State<AppState>,
    Path(slug): Path<String>,
) -> ApiResult<Json<Repository>> {
    let repository = crate::db::get_repository_by_slug(&state.pool, &slug).await?;
    Ok(Json(repository))
}
