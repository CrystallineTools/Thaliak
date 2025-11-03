use serde::{Deserialize, Serialize};

pub type DateTime = chrono::DateTime<chrono::Utc>;

#[derive(Clone, Debug, Serialize, Deserialize, PartialEq)]
pub enum HashType {
    None,
    Sha1 {
        block_size: i64,
        hashes: Vec<String>,
    },
}

impl HashType {
    /// Construct HashType from database columns
    pub fn from_columns(
        hash_type: Option<String>,
        hash_block_size: Option<i64>,
        hashes: Option<String>,
    ) -> Self {
        match hash_type.as_deref() {
            Some("sha1") | Some("SHA1") => {
                let block_size = hash_block_size.unwrap_or(0);
                let hashes = hashes
                    .unwrap_or_default()
                    .split(',')
                    .filter(|s| !s.is_empty())
                    .map(|s| s.to_string())
                    .collect();
                HashType::Sha1 { block_size, hashes }
            }
            _ => HashType::None,
        }
    }

    /// Convert HashType back to database columns for inserts/updates
    pub fn to_columns(&self) -> (String, Option<i64>, Option<String>) {
        match self {
            HashType::None => ("none".to_string(), None, None),
            HashType::Sha1 { block_size, hashes } => (
                "sha1".to_string(),
                Some(*block_size),
                Some(hashes.join(",")),
            ),
        }
    }
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct Service {
    pub id: i64,
    pub name: String,
    pub icon: String,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct Repository {
    pub id: i64,
    pub service_id: i64,
    pub slug: String,
    pub name: String,
    pub description: Option<String>,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct Patch {
    pub id: i64,
    pub repository_id: i64,
    pub version_string: String,
    pub remote_url: String,
    pub local_path: String,
    pub first_seen: Option<DateTime>,
    pub last_seen: Option<DateTime>,
    pub size: i64,
    pub hash: HashType,
    pub first_offered: Option<DateTime>,
    pub last_offered: Option<DateTime>,
    pub is_active: bool,
}

// Custom FromRow implementation for Patch to construct HashType from multiple columns
#[cfg(feature = "db")]
impl sqlx::FromRow<'_, sqlx::sqlite::SqliteRow> for Patch {
    fn from_row(row: &sqlx::sqlite::SqliteRow) -> Result<Self, sqlx::Error> {
        use sqlx::Row;

        let id: i64 = row.try_get("id")?;
        let repository_id: i64 = row.try_get("repository_id")?;
        let size: i64 = row.try_get("size")?;

        let hash_type: Option<String> = row.try_get("hash_type").ok();
        let hash_block_size: Option<i64> = row.try_get("hash_block_size").ok();
        let hashes: Option<String> = row.try_get("hashes").ok();

        Ok(Patch {
            id,
            repository_id,
            version_string: row.try_get("version_string")?,
            remote_url: row.try_get("remote_url")?,
            local_path: row.try_get("local_path")?,
            first_seen: row.try_get("first_seen").ok(),
            last_seen: row.try_get("last_seen").ok(),
            size,
            hash: HashType::from_columns(hash_type, hash_block_size, hashes),
            first_offered: row.try_get("first_offered").ok(),
            last_offered: row.try_get("last_offered").ok(),
            is_active: row.try_get("is_active")?,
        })
    }
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct PatchParent {
    pub current_patch_id: Option<i64>,
    pub next_patch_id: i64,
    pub repository_id: i64,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct GameVersion {
    pub id: i64,
    pub service_id: i64,
    pub expansion_id: i64,
    pub version_name: String,
    pub hotfix_level: i64,
    pub marketing_name: Option<String>,
    pub patch_info_url: Option<String>,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct GameVersionPatch {
    pub game_version_id: i64,
    pub patch_id: i64,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct Expansion {
    pub id: i64,
    pub name_en: String,
    pub name_ja: String,
    pub name_de: String,
    pub name_fr: String,
    pub name_ko: String,
    pub name_cn: String,
    pub name_tw: String,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
pub struct ExpansionRepositoryMapping {
    pub game_repository_id: i64,
    pub expansion_id: i64,
    pub expansion_repository_id: i64,
}
