use axum::{Json, Router, extract::State, http::StatusCode, routing::post};
use eyre::Result;
use log::{error, info, warn};
use sha1::Sha1;
use sha2::{Digest, Sha256};
use sqlx::SqlitePool;
use std::path::{Path, PathBuf};
use thaliak_common::patch::{BASE_GAME_VERSION, GameRepository};
use thaliak_common::webhook::AnalysisWebhookPayload;
use tokio::sync::mpsc;
use walkdir::WalkDir;

#[derive(Clone)]
struct AppState {
    analysis_cache_dir: PathBuf,
    analysis_binary_dir: PathBuf,
    download_path: PathBuf,
    queue_tx: mpsc::UnboundedSender<AnalysisWebhookPayload>,
}

async fn webhook_handler(
    State(state): State<AppState>,
    Json(payload): Json<AnalysisWebhookPayload>,
) -> StatusCode {
    info!(
        "Received analysis webhook for patch {} ({})",
        payload.patch.version_string, payload.repository.slug
    );

    if let Err(e) = state.queue_tx.send(payload) {
        error!("Failed to queue patch for analysis: {:?}", e);
        return StatusCode::INTERNAL_SERVER_ERROR;
    }

    StatusCode::OK
}

async fn process_patch_queue(
    mut queue_rx: mpsc::UnboundedReceiver<AnalysisWebhookPayload>,
    public_db: SqlitePool,
    analysis_cache_dir: PathBuf,
    analysis_binary_dir: PathBuf,
    download_path: PathBuf,
) {
    while let Some(payload) = queue_rx.recv().await {
        info!(
            "Processing patch {} (repository: {})",
            payload.patch.version_string, payload.repository.slug
        );

        if let Err(e) = analyze_patch(
            &payload,
            &public_db,
            &analysis_cache_dir,
            &analysis_binary_dir,
            &download_path,
        )
        .await
        {
            error!(
                "Failed to analyze patch {} (repository: {}): {:?}",
                payload.patch.version_string, payload.repository.slug, e
            );
        } else {
            info!(
                "Successfully analyzed patch {} (repository: {})",
                payload.patch.version_string, payload.repository.slug
            );
        }
    }
}

async fn analyze_patch(
    payload: &AnalysisWebhookPayload,
    public_db: &SqlitePool,
    analysis_cache_dir: &Path,
    analysis_binary_dir: &Path,
    download_path: &Path,
) -> Result<()> {
    let repository_id = sqlx::query_scalar!(
        r#"SELECT id FROM repository WHERE slug = ? AND service_id = ?"#,
        payload.repository.slug,
        payload.repository.service_id
    )
    .fetch_one(public_db)
    .await?;

    let patch_id = sqlx::query_scalar!(
        r#"SELECT id FROM patch WHERE version_string = ? AND repository_id = ?"#,
        payload.patch.version_string,
        repository_id
    )
    .fetch_one(public_db)
    .await?;

    let analysis_dir = analysis_cache_dir
        .join("gameroots")
        .join(&payload.repository.service_id);

    let patch_path = resolve_patch_path(&payload.local_path, download_path);

    let is_root_patch = check_if_root_patch(public_db, patch_id).await?;

    if is_root_patch {
        info!(
            "Patch {} is a root patch, cleaning analysis directory",
            payload.patch.version_string
        );
        if analysis_dir.exists() {
            tokio::fs::remove_dir_all(&analysis_dir).await?;
        }
    }

    if !analysis_dir.exists() {
        tokio::fs::create_dir_all(&analysis_dir).await?;
    }

    if let Some(game_repo) = GameRepository::from_slug(&payload.repository.slug) {
        let current_version = game_repo.get_ver(&analysis_dir);

        if !is_root_patch && current_version != BASE_GAME_VERSION {
            let is_valid = validate_patch_edge(
                public_db,
                &current_version,
                &payload.patch.version_string,
                repository_id,
            )
            .await?;

            if !is_valid {
                return Err(eyre::Report::msg(format!(
                    "Invalid patch edge: cannot apply patch {} (current version: {}) for repository {}",
                    payload.patch.version_string, current_version, payload.repository.slug
                )));
            }

            info!(
                "Validated patch edge: {} -> {}",
                current_version, payload.patch.version_string
            );
        }

        apply_patch(&patch_path, &analysis_dir).await?;

        game_repo.set_ver(&analysis_dir, &payload.patch.version_string)?;
        info!("Updated .ver file to {}", payload.patch.version_string);
    } else {
        warn!(
            "Unknown repository slug {}, skipping .ver validation",
            payload.repository.slug
        );
        apply_patch(&patch_path, &analysis_dir).await?;
    }

    let exe_dll_files = collect_exe_dll_files(&analysis_dir)?;

    let mut tasks = Vec::new();

    for file_path in exe_dll_files {
        let analysis_dir = analysis_dir.clone();
        let public_db = public_db.clone();
        let analysis_binary_dir = analysis_binary_dir.to_path_buf();
        let file_patch_id = patch_id;

        let task = tokio::spawn(async move {
            let relative_path = file_path
                .strip_prefix(&analysis_dir)?
                .to_string_lossy()
                .replace('\\', "/");

            let (sha1_hash, sha256_hash, md5_hash) = calculate_checksums(&file_path).await?;

            upsert_file_record(
                &public_db,
                file_patch_id,
                &relative_path,
                &sha1_hash,
                &sha256_hash,
                &md5_hash,
            )
            .await?;

            copy_and_link_binary(
                &file_path,
                &analysis_binary_dir,
                &sha1_hash,
                &sha256_hash,
                &md5_hash,
            )
            .await?;

            Ok::<_, eyre::Report>(())
        });

        tasks.push(task);
    }

    let num_tasks = tasks.len();

    for task in tasks {
        task.await??;
    }

    info!(
        "Processed {} exe/dll files for patch {}",
        num_tasks, payload.patch.version_string
    );

    Ok(())
}

fn resolve_patch_path(local_path: &str, download_path: &Path) -> PathBuf {
    let path = Path::new(local_path);
    if path.is_absolute() {
        path.to_path_buf()
    } else {
        download_path.join(path)
    }
}

async fn check_if_root_patch(public_db: &SqlitePool, patch_id: i64) -> Result<bool> {
    let result = sqlx::query_scalar!(
        r#"SELECT 1 as "result: i64" FROM patch_edge WHERE next_patch_id = ? AND current_patch_id IS NULL LIMIT 1"#,
        patch_id
    )
    .fetch_optional(public_db)
    .await?;

    Ok(result.is_some())
}

async fn validate_patch_edge(
    public_db: &SqlitePool,
    current_version: &str,
    next_version: &str,
    repository_id: i64,
) -> Result<bool> {
    let current_patch_id = sqlx::query_scalar!(
        r#"SELECT id FROM patch WHERE version_string = ? AND repository_id = ?"#,
        current_version,
        repository_id
    )
    .fetch_optional(public_db)
    .await?;

    let next_patch_id = sqlx::query_scalar!(
        r#"SELECT id FROM patch WHERE version_string = ? AND repository_id = ?"#,
        next_version,
        repository_id
    )
    .fetch_optional(public_db)
    .await?;

    let Some(next_id) = next_patch_id else {
        return Ok(false);
    };

    let edge_exists = sqlx::query_scalar!(
        r#"SELECT 1 as "result: i64" FROM patch_edge WHERE current_patch_id IS ? AND next_patch_id = ? LIMIT 1"#,
        current_patch_id,
        next_id
    )
    .fetch_optional(public_db)
    .await?;

    Ok(edge_exists.is_some())
}

async fn apply_patch(patch_path: &Path, target_dir: &Path) -> Result<()> {
    info!("Applying patch {} to {:?}", patch_path.display(), target_dir);

    if !patch_path.exists() {
        return Err(eyre::Report::msg(format!(
            "Patch file does not exist: {}",
            patch_path.display()
        )));
    }

    let patch_path_buf = patch_path.to_path_buf();
    let target_dir = target_dir.to_path_buf();

    tokio::task::spawn_blocking(move || -> Result<()> {
        let mut patch_file = zipatch::ZiPatchFile::from_path(&patch_path_buf)?;
        let mut config = zipatch::ZiPatchConfig::builder(&target_dir).build();

        for chunk_result in patch_file.chunks() {
            let mut chunk = chunk_result?;
            chunk.apply(&mut config)?;
        }

        Ok(())
    })
    .await??;

    Ok(())
}

fn collect_exe_dll_files(dir: &Path) -> Result<Vec<PathBuf>> {
    let mut files = Vec::new();

    for entry in WalkDir::new(dir).follow_links(false) {
        let entry = entry?;
        if !entry.file_type().is_file() {
            continue;
        }

        let path = entry.path();
        let path_lower = path.to_string_lossy().to_lowercase();

        if path_lower.ends_with(".exe") || path_lower.ends_with(".dll") {
            files.push(path.to_path_buf());
        }
    }

    Ok(files)
}

async fn calculate_checksums(file_path: &Path) -> Result<(String, String, String)> {
    let file_path = file_path.to_path_buf();

    let sha1_task = {
        let file_path = file_path.clone();
        tokio::task::spawn_blocking(move || -> Result<String> {
            let contents = std::fs::read(&file_path)?;
            let mut hasher = Sha1::new();
            hasher.update(&contents);
            Ok(hex::encode(hasher.finalize()))
        })
    };

    let sha256_task = {
        let file_path = file_path.clone();
        tokio::task::spawn_blocking(move || -> Result<String> {
            let contents = std::fs::read(&file_path)?;
            let mut hasher = Sha256::new();
            hasher.update(&contents);
            Ok(hex::encode(hasher.finalize()))
        })
    };

    let md5_task = {
        let file_path = file_path.clone();
        tokio::task::spawn_blocking(move || -> Result<String> {
            let contents = std::fs::read(&file_path)?;
            let digest = md5::compute(&contents);
            Ok(format!("{:x}", digest))
        })
    };

    let (sha1_result, sha256_result, md5_result) = tokio::join!(sha1_task, sha256_task, md5_task);

    let sha1_hash = sha1_result??;
    let sha256_hash = sha256_result??;
    let md5_hash = md5_result??;

    Ok((sha1_hash, sha256_hash, md5_hash))
}

async fn upsert_file_record(
    public_db: &SqlitePool,
    patch_id: i64,
    name: &str,
    sha1: &str,
    sha256: &str,
    md5: &str,
) -> Result<()> {
    sqlx::query!(
        r#"INSERT INTO file (patch_id, name, sha1, sha256, md5)
           VALUES (?, ?, ?, ?, ?)
           ON CONFLICT (patch_id, name)
           DO UPDATE SET sha1 = excluded.sha1, sha256 = excluded.sha256, md5 = excluded.md5"#,
        patch_id,
        name,
        sha1,
        sha256,
        md5
    )
    .execute(public_db)
    .await?;

    Ok(())
}

async fn copy_and_link_binary(
    file_path: &Path,
    analysis_binary_dir: &Path,
    sha1: &str,
    sha256: &str,
    md5: &str,
) -> Result<()> {
    let file_name = file_path
        .file_name()
        .ok_or_else(|| eyre::Report::msg("Failed to get file name"))?
        .to_string_lossy();

    let sha1_dir = analysis_binary_dir.join("sha1").join(sha1);
    let sha256_dir = analysis_binary_dir.join("sha256").join(sha256);
    let md5_dir = analysis_binary_dir.join("md5").join(md5);

    tokio::fs::create_dir_all(&sha1_dir).await?;

    let dest_path = sha1_dir.join(file_name.as_ref());

    if !dest_path.exists() {
        tokio::fs::copy(file_path, &dest_path).await?;
    }

    tokio::fs::create_dir_all(sha256_dir.parent().unwrap()).await?;

    if sha256_dir.exists() || sha256_dir.is_symlink() {
        if let Err(e) = tokio::fs::remove_dir(&sha256_dir).await {
            warn!("Failed to remove existing sha256 directory: {:?}", e);
        }
    }

    #[cfg(unix)]
    {
        use std::os::unix::fs::symlink;
        let sha1_path = PathBuf::from("../..").join("sha1").join(sha1);
        tokio::task::spawn_blocking({
            let sha256_dir = sha256_dir.clone();
            let sha1_path = sha1_path.clone();
            move || symlink(&sha1_path, &sha256_dir)
        })
        .await??;
    }

    #[cfg(windows)]
    {
        use std::os::windows::fs::symlink_dir;
        let sha1_path = PathBuf::from("..\\..").join("sha1").join(sha1);
        tokio::task::spawn_blocking({
            let sha256_dir = sha256_dir.clone();
            let sha1_path = sha1_path.clone();
            move || symlink_dir(&sha1_path, &sha256_dir)
        })
        .await??;
    }

    tokio::fs::create_dir_all(md5_dir.parent().unwrap()).await?;

    if md5_dir.exists() || md5_dir.is_symlink() {
        if let Err(e) = tokio::fs::remove_dir(&md5_dir).await {
            warn!("Failed to remove existing md5 directory: {:?}", e);
        }
    }

    #[cfg(unix)]
    {
        use std::os::unix::fs::symlink;
        let sha1_path = PathBuf::from("../..").join("sha1").join(sha1);
        tokio::task::spawn_blocking({
            let md5_dir = md5_dir.clone();
            let sha1_path = sha1_path.clone();
            move || symlink(&sha1_path, &md5_dir)
        })
        .await??;
    }

    #[cfg(windows)]
    {
        use std::os::windows::fs::symlink_dir;
        let sha1_path = PathBuf::from("..\\..").join("sha1").join(sha1);
        tokio::task::spawn_blocking({
            let md5_dir = md5_dir.clone();
            let sha1_path = sha1_path.clone();
            move || symlink_dir(&sha1_path, &md5_dir)
        })
        .await??;
    }

    Ok(())
}

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let pools = thaliak_common::init_dbs().await?;
    thaliak_common::logging::setup(None);

    let commit_hash = env!("GIT_COMMIT_HASH");
    info!("analysis service started (commit: {})", commit_hash);

    thaliak_common::version::update_component_version(&pools.private, "analysis", commit_hash)
        .await?;

    let analysis_cache_dir = std::env::var("ANALYSIS_CACHE_DIR")
        .map_err(|_| eyre::Report::msg("ANALYSIS_CACHE_DIR environment variable is required"))?;
    let analysis_binary_dir = std::env::var("ANALYSIS_BINARY_DIR")
        .map_err(|_| eyre::Report::msg("ANALYSIS_BINARY_DIR environment variable is required"))?;
    let download_path = std::env::var("DOWNLOAD_PATH")
        .map_err(|_| eyre::Report::msg("DOWNLOAD_PATH environment variable is required"))?;

    let analysis_cache_dir = PathBuf::from(analysis_cache_dir);
    let analysis_binary_dir = PathBuf::from(analysis_binary_dir);
    let download_path = PathBuf::from(download_path);

    info!("Analysis cache directory: {:?}", analysis_cache_dir);
    info!("Analysis binary directory: {:?}", analysis_binary_dir);
    info!("Download path: {:?}", download_path);

    let (queue_tx, queue_rx) = mpsc::unbounded_channel();

    let state = AppState {
        analysis_cache_dir: analysis_cache_dir.clone(),
        analysis_binary_dir: analysis_binary_dir.clone(),
        download_path: download_path.clone(),
        queue_tx,
    };

    tokio::spawn(process_patch_queue(
        queue_rx,
        pools.public,
        analysis_cache_dir,
        analysis_binary_dir,
        download_path,
    ));

    let app = Router::new()
        .route("/", post(webhook_handler))
        .with_state(state);

    let listener = tokio::net::TcpListener::bind("0.0.0.0:8082").await?;
    info!("analysis service listening on {}", listener.local_addr()?);

    axum::serve(listener, app).await?;

    Ok(())
}
