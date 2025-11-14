use eyre::Result;
use log::info;
use std::path::PathBuf;
use thaliak_common::patch::get_local_storage_path;

#[derive(Debug)]
struct PatchStatus {
    patch_id: i64,
    version_string: String,
    remote_url: String,
    current_local_path: Option<String>,
    expected_local_path: String,
    file_exists: bool,
}

pub async fn execute() -> Result<()> {
    let pools = thaliak_common::init_dbs().await?;

    let download_path: PathBuf = std::env::var("DOWNLOAD_PATH")
        .unwrap_or_else(|_| "./patches".to_string())
        .into();

    info!("Using download path: {}", download_path.display());

    let patch_rows = sqlx::query!(
        r#"
        SELECT id, repository_id, version_string, remote_url, local_path,
               first_seen, last_seen, size, hash_type, hash_block_size, hashes,
               first_offered, last_offered, is_active
        FROM patch
        ORDER BY id
        "#
    )
    .fetch_all(&pools.public)
    .await?;

    info!("Found {} patches to check", patch_rows.len());

    let mut patches_already_correct = 0;
    let mut patches_changed = 0;
    let mut patches_missing = 0;

    let mut statuses: Vec<PatchStatus> = Vec::new();

    for row in patch_rows {
        let expected_local_path = match get_local_storage_path(&row.remote_url) {
            Ok(path) => path,
            Err(e) => {
                info!(
                    "Warning: Could not parse URL for patch {}: {}",
                    row.version_string, e
                );
                continue;
            }
        };

        let full_path = download_path.join(&expected_local_path);
        let file_exists = full_path.exists();

        let status = PatchStatus {
            patch_id: row.id,
            version_string: row.version_string,
            remote_url: row.remote_url,
            current_local_path: if row.local_path.is_empty() {
                None
            } else {
                Some(row.local_path.clone())
            },
            expected_local_path: expected_local_path.clone(),
            file_exists,
        };

        if file_exists {
            if !row.local_path.is_empty() && row.local_path == expected_local_path {
                patches_already_correct += 1;
            } else {
                sqlx::query!(
                    "UPDATE patch SET local_path = ? WHERE id = ?",
                    &expected_local_path,
                    row.id
                )
                .execute(&pools.public)
                .await?;
                patches_changed += 1;
            }
        } else {
            patches_missing += 1;
        }

        statuses.push(status);
    }

    println!("\n=== Update Local Paths Summary ===");
    println!("Total patches checked: {}", statuses.len());
    println!("Already correct: {}", patches_already_correct);
    println!("Changed: {}", patches_changed);
    println!("Missing: {}", patches_missing);

    println!("\n=== Detailed Report ===");

    if patches_already_correct > 0 {
        println!("\n[Already Correct] ({}):", patches_already_correct);
        for status in &statuses {
            if status.file_exists
                && status.current_local_path.as_ref() == Some(&status.expected_local_path)
            {
                println!(
                    "  ✓ {} → {}",
                    status.version_string, status.expected_local_path
                );
            }
        }
    }

    if patches_changed > 0 {
        println!("\n[Changed] ({}):", patches_changed);
        for status in &statuses {
            if status.file_exists
                && status.current_local_path.as_ref() != Some(&status.expected_local_path)
            {
                let old = status
                    .current_local_path
                    .as_ref()
                    .map(|s| s.as_str())
                    .unwrap_or("(none)");
                println!(
                    "  ✎ {} → {} (was: {})",
                    status.version_string, status.expected_local_path, old
                );
            }
        }
    }

    if patches_missing > 0 {
        println!("\n[Missing] ({}):", patches_missing);
        for status in &statuses {
            if !status.file_exists {
                let current = status
                    .current_local_path
                    .as_ref()
                    .map(|s| s.as_str())
                    .unwrap_or("(none)");
                println!(
                    "  ✗ {} → {} (local_path: {})",
                    status.version_string, status.expected_local_path, current
                );
            }
        }
    }

    Ok(())
}
