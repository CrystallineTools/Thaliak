use crate::{
    db::AppState,
    error::ApiResult,
    models::{PatchQueryParams, PatchesResponse},
};
use axum::{
    Json,
    extract::{Path, Query, State},
};
use thaliak_types::Patch;

#[utoipa::path(
    get,
    path = "/repositories/{slug}/patches",
    params(
        ("slug" = String, Path, description = "Repository slug"),
        PatchQueryParams
    ),
    responses(
        (status = 200, description = "List of patches", body = PatchesResponse),
        (status = 404, description = "Repository not found"),
        (status = 400, description = "Invalid parameters"),
        (status = 500, description = "Internal server error")
    ),
    tag = "patches"
)]
pub async fn get_repository_patches(
    State(state): State<AppState>,
    Path(slug): Path<String>,
    Query(params): Query<PatchQueryParams>,
) -> ApiResult<Json<PatchesResponse>> {
    let repository = crate::db::get_repository_by_slug(&state.pool, &slug).await?;

    let patches = if params.all.unwrap_or(false) {
        let active_only = params.active.unwrap_or(true);
        crate::db::get_all_patches(&state.pool, repository.id, active_only).await?
    } else {
        crate::db::get_patch_chain(
            &state.pool,
            repository.id,
            params.from.as_deref(),
            params.to.as_deref(),
        )
        .await?
    };

    let total = patches.len();
    let total_size = patches.iter().map(|p| p.size).sum();

    Ok(Json(PatchesResponse {
        patches,
        total,
        total_size,
    }))
}

#[utoipa::path(
    get,
    path = "/repositories/{slug}/patches/{version}",
    params(
        ("slug" = String, Path, description = "Repository slug"),
        ("version" = String, Path, description = "Patch version string")
    ),
    responses(
        (status = 200, description = "Patch details", body = Patch),
        (status = 404, description = "Repository or patch not found"),
        (status = 500, description = "Internal server error")
    ),
    tag = "patches"
)]
pub async fn get_repository_patch(
    State(state): State<AppState>,
    Path((slug, version)): Path<(String, String)>,
) -> ApiResult<Json<Patch>> {
    let repository = crate::db::get_repository_by_slug(&state.pool, &slug).await?;
    let patch = crate::db::get_patch_by_version(&state.pool, repository.id, &version).await?;
    Ok(Json(patch))
}
