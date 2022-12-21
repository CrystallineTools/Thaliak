#![allow(unused)]
#![allow(clippy::all)]

use crate::schema::*;
use diesel::prelude::*;

use chrono::DateTime;
use chrono::offset::Utc;

use bigdecimal::BigDecimal;

#[derive(Queryable, Debug, Identifiable)]
#[diesel(
    table_name = account_repositories,
    primary_key(applicable_accounts_id, applicable_repositories_id)
)]
pub struct AccountRepository {
    pub applicable_accounts_id: i32,
    pub applicable_repositories_id: i32,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct Account {
    pub id: i32,
    pub username: String,
    pub password: String,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct DiscordHook {
    pub id: i32,
    pub url: String,
    pub name: Option<String>,
}

#[derive(Queryable, Debug, Identifiable)]
#[diesel(primary_key(game_repository_id, expansion_id, expansion_repository_id))]
pub struct ExpansionRepositoryMapping {
    pub game_repository_id: i32,
    pub expansion_id: i32,
    pub expansion_repository_id: i32,
}

#[derive(Queryable, Debug, Identifiable)]
#[diesel(primary_key(name, sha1))]
pub struct File {
    pub name: String,
    pub sha1: String,
    pub size: u64,
    pub last_used: DateTime<Utc>,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct PatchChain {
    pub repository_id: i32,
    pub patch_id: i32,
    pub previous_patch_id: Option<i32>,
    pub first_offered: Option<DateTime<Utc>>,
    pub last_offered: Option<DateTime<Utc>>,
    pub id: i32,
    pub is_active: bool,
}

#[derive(Queryable, Debug, Identifiable, Associations)]
#[diesel(
    table_name = patches,
    belongs_to(Version, foreign_key = version_id)
)]
pub struct Patch {
    pub id: i32,
    pub version_id: i32,
    pub repository_id: i32,
    pub remote_origin_path: String,
    pub first_seen: Option<DateTime<Utc>>,
    pub last_seen: Option<DateTime<Utc>>,
    pub size: u64,
    pub hash_type: Option<String>,
    pub hash_block_size: Option<u64>,
    pub hashes: Option<String>,
    pub first_offered: Option<DateTime<Utc>>,
    pub last_offered: Option<DateTime<Utc>>,
    pub local_storage_path: String,
    pub is_active: bool,
}

#[derive(Queryable, Debug, Identifiable, Associations)]
#[diesel(
    table_name = repositories,
    belongs_to(ServiceRegion, foreign_key = service_region_id)
)]
pub struct Repository {
    pub id: i32,
    pub name: String,
    pub description: Option<String>,
    pub slug: String,
    pub service_region_id: i32,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct ServiceRegion {
    pub id: i32,
    pub name: String,
    pub icon: String,
}

#[derive(Queryable, Debug, Identifiable, Associations)]
#[diesel(
    primary_key(versions_id, files_name, files_sha1),
    belongs_to(Version, foreign_key = versions_id)
)]
pub struct VersionFile {
    pub versions_id: i32,
    pub files_name: String,
    pub files_sha1: String,
}

#[derive(Queryable, Debug, Identifiable, Associations)]
#[diesel(belongs_to(Repository, foreign_key = repository_id))]
pub struct Version {
    pub id: i32,
    pub version_id: BigDecimal,
    pub version_string: String,
    pub repository_id: i32,
}
