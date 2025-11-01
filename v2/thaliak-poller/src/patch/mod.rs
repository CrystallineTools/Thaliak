use sqlx::Row;
use sqlx::sqlite::SqliteRow;

mod reconciliation;
mod repository;

pub use reconciliation::*;
pub use repository::*;
use crate::DbConnection;
use crate::poller::PatchDiscoveryType;

pub type DateTime = chrono::DateTime<chrono::offset::Utc>;
#[derive(Clone, Debug)]
pub struct Patch {
    id: u32,
    repository_id: u32,
    patch_base_id: u32,
    version_string: String,
    remote_url: String,
    local_path: Option<String>,
    first_seen: Option<DateTime>,
    last_seen: Option<DateTime>,
    size: u64,
    hash: HashType,
    first_offered: Option<DateTime>,
    last_offered: Option<DateTime>,
    is_active: bool,
}

impl Patch {
    pub fn from_db(row: SqliteRow) -> Patch {
        Patch {
            id: row.get("id"),
            repository_id: row.get("repository_id"),
            patch_base_id: row.get("patch_base_id"),
            version_string: row.get("version_string"),
            remote_url: row.get("remote_url"),
            local_path: row.try_get("local_path").unwrap_or(None),
            first_seen: row.try_get("first_seen").unwrap_or(None),
            last_seen: row.try_get("last_seen").unwrap_or(None),
            size: row.get("size"),
            is_active: row.try_get("is_active").unwrap_or(false),
            first_offered: row.try_get("first_offered").unwrap_or(None),
            last_offered: row.try_get("last_offered").unwrap_or(None),
            hash: HashType::from_db(row),
        }
    }

    pub fn update_offered(self, conn: &mut DbConnection) {}
    pub fn update(self, conn: &mut DbConnection, last_seen: DateTime, patch_discovery_type: PatchDiscoveryType) -> Patch {
        let new_patch = Patch {
            id: self.id,
            repository_id: self.repository_id,
            patch_base_id: self.patch_base_id,
            version_string: self.version_string,
            remote_url: self.remote_url,
            local_path: self.local_path,
            first_seen: self.first_seen,
            last_seen: Some(last_seen),
            size: self.size,
            hash: self.hash,
            first_offered: self.first_offered,
            last_offered: self.last_offered,
            is_active: if patch_discovery_type == PatchDiscoveryType::Offered { true } else { false },
        };

        sqlx::query(r#"UPDATE patch SET last_seen = $1, is_active = $2, "#)
        new_patch
    }
}

#[derive(Clone, Debug)]
pub enum HashType {
    None,
    Sha1 {
        block_size: u64,
        hashes: Vec<String>,
    },
}

impl HashType {
    fn from_db(row: SqliteRow) -> HashType {
        let ht = row.get("hash_type");
        match ht {
            "sha1" => HashType::Sha1 {
                block_size: row.get("block_size"),
                hashes: row
                    .get::<String, _>("hashes")
                    .split(",")
                    .into_iter()
                    .map(|s| s.to_string())
                    .collect(),
            },
            _ => HashType::None,
        }
    }
}
