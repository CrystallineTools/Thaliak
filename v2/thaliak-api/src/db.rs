use crate::error::{ApiError, ApiResult};
use sqlx::Row;
use sqlx::sqlite::SqlitePool;
use thaliak_types::{LatestPatchInfo, Patch, Repository, Service};

#[derive(Clone)]
pub struct AppState {
    pub pool: SqlitePool,
}

impl AppState {
    pub fn new(pool: SqlitePool) -> Self {
        Self { pool }
    }
}

pub async fn get_services(pool: &SqlitePool) -> ApiResult<Vec<Service>> {
    let services = sqlx::query_as!(Service, r#"SELECT id, name, icon FROM service ORDER BY id"#)
        .fetch_all(pool)
        .await?;
    Ok(services)
}

pub async fn get_repositories(pool: &SqlitePool) -> ApiResult<Vec<Repository>> {
    let rows = sqlx::query(
        r#"SELECT r.id, r.service_id, r.slug, r.name, r.description,
                  p.version_string as latest_version_string,
                  p.first_offered as latest_first_offered,
                  p.last_offered as latest_last_offered
           FROM repository r
           LEFT JOIN (
               SELECT repository_id, version_string, first_offered, last_offered,
                      ROW_NUMBER() OVER (
                          PARTITION BY repository_id
                          ORDER BY LTRIM(version_string, 'HD') DESC
                      ) as rn
               FROM patch
               WHERE is_active = true
           ) p ON p.repository_id = r.id AND p.rn = 1
           ORDER BY CASE r.service_id
                WHEN 'jp' THEN 1
                WHEN 'kr' THEN 2
                WHEN 'cn' THEN 3
                ELSE 4
            END, r.id"#,
    )
    .fetch_all(pool)
    .await?;

    let repositories = rows
        .into_iter()
        .map(|row| {
            let latest_patch = row
                .try_get::<Option<String>, _>("latest_version_string")
                .ok()
                .flatten()
                .map(|version_string| LatestPatchInfo {
                    version_string,
                    first_offered: row.try_get("latest_first_offered").ok().flatten(),
                    last_offered: row.try_get("latest_last_offered").ok().flatten(),
                });

            Repository {
                id: row.get("id"),
                service_id: row.get("service_id"),
                slug: row.get("slug"),
                name: row.get("name"),
                description: row.try_get("description").ok(),
                latest_patch,
            }
        })
        .collect();

    Ok(repositories)
}

pub async fn get_repository_by_slug(pool: &SqlitePool, slug: &str) -> ApiResult<Repository> {
    let row = match sqlx::query(
        r#"SELECT r.id, r.service_id, r.slug, r.name, r.description,
                  p.version_string as latest_version_string,
                  p.first_offered as latest_first_offered,
                  p.last_offered as latest_last_offered
           FROM repository r
           LEFT JOIN (
               SELECT repository_id, version_string, first_offered, last_offered,
                      ROW_NUMBER() OVER (
                          PARTITION BY repository_id
                          ORDER BY LTRIM(version_string, 'HD') DESC
                      ) as rn
               FROM patch
               WHERE is_active = true
           ) p ON p.repository_id = r.id AND p.rn = 1
           WHERE r.slug = ?"#,
    )
    .bind(slug)
    .fetch_one(pool)
    .await
    {
        Ok(row) => row,
        Err(sqlx::Error::RowNotFound) => {
            return Err(ApiError::NotFound(format!(
                "Repository '{}' not found",
                slug
            )));
        }
        Err(e) => return Err(ApiError::from(e)),
    };

    let latest_patch = row
        .try_get::<Option<String>, _>("latest_version_string")
        .ok()
        .flatten()
        .map(|version_string| LatestPatchInfo {
            version_string,
            first_offered: row.try_get("latest_first_offered").ok().flatten(),
            last_offered: row.try_get("latest_last_offered").ok().flatten(),
        });

    Ok(Repository {
        id: row.get("id"),
        service_id: row.get("service_id"),
        slug: row.get("slug"),
        name: row.get("name"),
        description: row.try_get("description").ok(),
        latest_patch,
    })
}

pub async fn get_patch_by_version(
    pool: &SqlitePool,
    repository_id: i64,
    version_string: &str,
) -> ApiResult<Patch> {
    match sqlx::query_as::<_, Patch>(
        r#"SELECT p.id, p.repository_id, r.slug as repository_slug, p.version_string,
                  p.remote_url, p.local_path, p.first_seen, p.last_seen, p.size,
                  p.hash_type, p.hash_block_size, p.hashes,
                  p.first_offered, p.last_offered, p.is_active
           FROM patch p
           INNER JOIN repository r ON p.repository_id = r.id
           WHERE p.repository_id = ? AND p.version_string = ?"#,
    )
    .bind(repository_id)
    .bind(version_string)
    .fetch_one(pool)
    .await
    {
        Ok(patch) => Ok(patch),
        Err(sqlx::Error::RowNotFound) => Err(ApiError::NotFound(format!(
            "Patch version '{}' not found",
            version_string
        ))),
        Err(e) => Err(ApiError::from(e)),
    }
}

async fn get_patch_by_id(pool: &SqlitePool, id: i64) -> ApiResult<Patch> {
    match sqlx::query_as::<_, Patch>(
        r#"SELECT p.id, p.repository_id, r.slug as repository_slug, p.version_string,
                  p.remote_url, p.local_path, p.first_seen, p.last_seen, p.size,
                  p.hash_type, p.hash_block_size, p.hashes,
                  p.first_offered, p.last_offered, p.is_active
           FROM patch p
           INNER JOIN repository r ON p.repository_id = r.id
           WHERE p.id = ?"#,
    )
    .bind(id)
    .fetch_one(pool)
    .await
    {
        Ok(patch) => Ok(patch),
        Err(sqlx::Error::RowNotFound) => Err(ApiError::NotFound(format!("Patch {} not found", id))),
        Err(e) => Err(ApiError::from(e)),
    }
}

pub async fn get_all_patches(
    pool: &SqlitePool,
    repository_id: i64,
    active_only: bool,
) -> ApiResult<Vec<Patch>> {
    let patches = if active_only {
        sqlx::query_as::<_, Patch>(
            r#"SELECT p.id, p.repository_id, r.slug as repository_slug, p.version_string,
                      p.remote_url, p.local_path, p.first_seen, p.last_seen, p.size,
                      p.hash_type, p.hash_block_size, p.hashes,
                      p.first_offered, p.last_offered, p.is_active
               FROM patch p
               INNER JOIN repository r ON p.repository_id = r.id
               WHERE p.repository_id = ? AND p.is_active = true
               ORDER BY p.id"#,
        )
        .bind(repository_id)
        .fetch_all(pool)
        .await?
    } else {
        sqlx::query_as::<_, Patch>(
            r#"SELECT p.id, p.repository_id, r.slug as repository_slug, p.version_string,
                      p.remote_url, p.local_path, p.first_seen, p.last_seen, p.size,
                      p.hash_type, p.hash_block_size, p.hashes,
                      p.first_offered, p.last_offered, p.is_active
               FROM patch p
               INNER JOIN repository r ON p.repository_id = r.id
               WHERE p.repository_id = ?
               ORDER BY LTRIM(p.version_string, 'HD')"#,
        )
        .bind(repository_id)
        .fetch_all(pool)
        .await?
    };

    Ok(patches)
}

pub async fn get_patch_chain(
    pool: &SqlitePool,
    repository_id: i64,
    from_version: Option<&str>,
    to_version: Option<&str>,
) -> ApiResult<Vec<Patch>> {
    let mut patches = Vec::new();

    let mut current_id: Option<i64> = if let Some(from) = from_version {
        let patch = match sqlx::query_as::<_, Patch>(
            r#"SELECT p.id, p.repository_id, r.slug as repository_slug, p.version_string,
                      p.remote_url, p.local_path, p.first_seen, p.last_seen, p.size,
                      p.hash_type, p.hash_block_size, p.hashes,
                      p.first_offered, p.last_offered, p.is_active
               FROM patch p
               INNER JOIN repository r ON p.repository_id = r.id
               WHERE p.repository_id = ? AND p.version_string = ?"#,
        )
        .bind(repository_id)
        .bind(from)
        .fetch_one(pool)
        .await
        {
            Ok(patch) => patch,
            Err(sqlx::Error::RowNotFound) => {
                return Err(ApiError::NotFound(format!(
                    "Starting version '{}' not found",
                    from
                )));
            }
            Err(e) => return Err(ApiError::from(e)),
        };

        Some(patch.id)
    } else {
        None
    };

    let target_id: Option<i64> = if let Some(to) = to_version {
        let patch = match sqlx::query_as::<_, Patch>(
            r#"SELECT p.id, p.repository_id, r.slug as repository_slug, p.version_string,
                      p.remote_url, p.local_path, p.first_seen, p.last_seen, p.size,
                      p.hash_type, p.hash_block_size, p.hashes,
                      p.first_offered, p.last_offered, p.is_active
               FROM patch p
               INNER JOIN repository r ON p.repository_id = r.id
               WHERE p.repository_id = ? AND p.version_string = ?"#,
        )
        .bind(repository_id)
        .bind(to)
        .fetch_one(pool)
        .await
        {
            Ok(patch) => patch,
            Err(sqlx::Error::RowNotFound) => {
                return Err(ApiError::NotFound(format!(
                    "Target version '{}' not found",
                    to
                )));
            }
            Err(e) => return Err(ApiError::from(e)),
        };

        Some(patch.id)
    } else {
        None
    };

    loop {
        let next_id = if let Some(current) = current_id {
            sqlx::query_scalar!(
                r#"SELECT next_patch_id FROM patch_edge
                   WHERE repository_id = ? AND current_patch_id = ? AND is_active = true
                   ORDER BY last_offered DESC
                   LIMIT 1"#,
                repository_id,
                current
            )
            .fetch_optional(pool)
            .await?
        } else {
            sqlx::query_scalar!(
                r#"SELECT next_patch_id FROM patch_edge
                   WHERE repository_id = ? AND current_patch_id IS NULL AND is_active = true
                   ORDER BY last_offered DESC
                   LIMIT 1"#,
                repository_id
            )
            .fetch_optional(pool)
            .await?
        };

        match next_id {
            Some(id) => {
                let patch = get_patch_by_id(pool, id).await?;
                patches.push(patch);

                if let Some(target) = target_id {
                    if id == target {
                        break;
                    }
                }

                current_id = Some(id);
            }
            None => break,
        }
    }

    Ok(patches)
}
