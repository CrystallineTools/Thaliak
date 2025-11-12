use serde::{Deserialize, Deserializer, Serialize, Serializer};

pub type DateTime = chrono::DateTime<chrono::Utc>;

#[derive(Clone, Debug, PartialEq)]
#[cfg_attr(feature = "openapi", derive(utoipa::ToSchema))]
pub enum HashType {
    None,
    Sha1 {
        block_size: i64,
        hashes: Vec<String>,
    },
}

impl Serialize for HashType {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: Serializer,
    {
        use serde::ser::SerializeMap;

        match self {
            HashType::None => {
                let mut map = serializer.serialize_map(Some(1))?;
                map.serialize_entry("type", "none")?;
                map.end()
            }
            HashType::Sha1 { block_size, hashes } => {
                let mut map = serializer.serialize_map(Some(3))?;
                map.serialize_entry("type", "sha1")?;
                map.serialize_entry("block_size", block_size)?;
                map.serialize_entry("hashes", hashes)?;
                map.end()
            }
        }
    }
}

impl<'de> Deserialize<'de> for HashType {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: Deserializer<'de>,
    {
        use serde::de::{self, MapAccess, Visitor};
        use std::fmt;

        struct HashTypeVisitor;

        impl<'de> Visitor<'de> for HashTypeVisitor {
            type Value = HashType;

            fn expecting(&self, formatter: &mut fmt::Formatter) -> fmt::Result {
                formatter.write_str("a hash type object with a 'type' field")
            }

            fn visit_map<A>(self, mut map: A) -> Result<Self::Value, A::Error>
            where
                A: MapAccess<'de>,
            {
                let mut hash_type: Option<String> = None;
                let mut block_size: Option<i64> = None;
                let mut hashes: Option<Vec<String>> = None;

                while let Some(key) = map.next_key::<String>()? {
                    match key.as_str() {
                        "type" => hash_type = Some(map.next_value()?),
                        "block_size" => block_size = Some(map.next_value()?),
                        "hashes" => hashes = Some(map.next_value()?),
                        _ => {
                            let _: serde::de::IgnoredAny = map.next_value()?;
                        }
                    }
                }

                match hash_type.as_deref() {
                    Some("none") => Ok(HashType::None),
                    Some("sha1") => {
                        let block_size =
                            block_size.ok_or_else(|| de::Error::missing_field("block_size"))?;
                        let hashes = hashes.ok_or_else(|| de::Error::missing_field("hashes"))?;
                        Ok(HashType::Sha1 { block_size, hashes })
                    }
                    _ => Err(de::Error::unknown_variant(
                        hash_type.as_deref().unwrap_or(""),
                        &["none", "sha1"],
                    )),
                }
            }
        }

        deserializer.deserialize_map(HashTypeVisitor)
    }
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
#[cfg_attr(feature = "openapi", derive(utoipa::ToSchema))]
pub struct Service {
    pub id: String,
    pub name: String,
    pub icon: String,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "openapi", derive(utoipa::ToSchema))]
pub struct LatestPatchInfo {
    pub version_string: String,
    pub first_offered: Option<DateTime>,
    pub last_offered: Option<DateTime>,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "db", derive(sqlx::FromRow))]
#[cfg_attr(feature = "openapi", derive(utoipa::ToSchema))]
pub struct Repository {
    #[serde(skip)]
    pub id: i64,
    pub service_id: String,
    pub slug: String,
    pub name: String,
    pub description: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    #[cfg_attr(feature = "db", sqlx(default))]
    pub latest_patch: Option<LatestPatchInfo>,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
#[cfg_attr(feature = "openapi", derive(utoipa::ToSchema))]
pub struct Patch {
    #[serde(skip)]
    pub id: i64,
    #[serde(skip)]
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
pub struct PatchEdge {
    pub repository_id: i64,
    pub current_patch_id: Option<i64>,
    pub next_patch_id: i64,
    pub first_offered: DateTime,
    pub last_offered: DateTime,
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
