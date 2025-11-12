use serde::{Deserialize, Serialize};
use thaliak_types::{Patch, Repository, Service};
use utoipa::{IntoParams, ToSchema};

/// Health check response
#[derive(Debug, Serialize, ToSchema)]
pub struct HealthResponse {
    pub status: String,
    pub database: String,
}

/// List of services response
#[derive(Debug, Serialize, ToSchema)]
pub struct ServicesResponse {
    pub services: Vec<Service>,
    pub total: usize,
}

/// List of repositories response
#[derive(Debug, Serialize, ToSchema)]
pub struct RepositoriesResponse {
    pub repositories: Vec<Repository>,
    pub total: usize,
}

/// Patch list response with metadata
#[derive(Debug, Serialize, ToSchema)]
pub struct PatchesResponse {
    pub patches: Vec<Patch>,
    pub total: usize,
    pub total_size: i64,
}

/// Database file information
#[derive(Debug, Serialize, ToSchema)]
pub struct DatabaseInfo {
    pub filename: String,
    pub size: u64,
    pub modified: String,
}

/// Query parameters for patch listing
#[derive(Debug, Deserialize, IntoParams, ToSchema)]
pub struct PatchQueryParams {
    /// Starting version (default: first patch of the current patch lineage)
    pub from: Option<String>,
    /// Ending version (default: latest patch of the current patch lineage)
    pub to: Option<String>,
    /// Return all known patches (from all lineages) without chain resolution
    pub all: Option<bool>,
    /// If true, only shows patches that are actively being offered by the official launcher.
    /// Defaults to `true` and only applies when the `all` option is not true.
    /// Ignored when `all` is set to true.
    pub active: Option<bool>,
}
