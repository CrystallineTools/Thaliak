#![allow(unused)]
#![allow(clippy::all)]

use crate::schema::*;
use diesel::prelude::*;

use chrono::DateTime;
use chrono::offset::Utc;

#[derive(Queryable, Debug, Identifiable)]
#[diesel(primary_key(applicable_accounts_id, applicable_repositories_id), table_name = account_repositories)]
pub struct AccountRepository {
    pub applicable_accounts_id: u32,
    pub applicable_repositories_id: u32,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct Account {
    pub id: u32,
    pub username: String,
    pub password: String,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct DiscordHook {
    pub id: u32,
    pub url: String,
    pub name: Option<String>,
}

#[derive(Queryable, Debug, Identifiable)]
#[diesel(primary_key(game_repository_id, expansion_id, expansion_repository_id))]
pub struct ExpansionRepositoryMapping {
    pub game_repository_id: u32,
    pub expansion_id: u32,
    pub expansion_repository_id: u32,
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
    pub repository_id: u32,
    pub patch_id: u32,
    pub previous_patch_id: Option<u32>,
    pub first_offered: Option<DateTime<Utc>>,
    pub last_offered: Option<DateTime<Utc>>,
    pub id: u32,
    pub is_active: bool,
}

#[derive(Queryable, Debug, Identifiable)]
#[diesel(table_name = patches)]
pub struct Patch {
    pub id: u32,
    pub version_id: u32,
    pub repository_id: u32,
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

#[derive(Queryable, Debug, Identifiable)]
#[diesel(table_name = repositories)]
pub struct Repository {
    pub id: u32,
    pub name: String,
    pub description: Option<String>,
    pub slug: String,
    pub service_region_id: u32,
}

#[derive(Queryable, Debug, Identifiable)]
pub struct ServiceRegion {
    pub id: u32,
    pub name: String,
    pub icon: String,
}

#[derive(Queryable, Debug, Identifiable)]
#[diesel(primary_key(versions_id, files_name, files_sha1))]
pub struct VersionFile {
    pub versions_id: u32,
    pub files_name: String,
    pub files_sha1: String,
}

#[derive(Queryable, Debug)]
pub struct Version {
    pub id: u32,
    pub version_id: u64,
    pub version_string: String,
    pub repository_id: u32,
}
