use eyre::Result;
use log::info;
use sqlx::migrate::MigrateDatabase;
use sqlx::sqlite::{SqliteConnectOptions, SqlitePoolOptions};
use sqlx::{ConnectOptions, Sqlite, SqlitePool};
use std::env;
use std::str::FromStr;
use std::time::Duration;

pub mod logging;
pub mod patch;
pub mod version;

#[cfg(feature = "webhooks")]
pub mod webhook;

#[derive(Clone)]
pub struct DatabasePools {
    pub public: SqlitePool,
    pub private: SqlitePool,
}

pub async fn init_public_db() -> Result<SqlitePool> {
    let db_url = env::var("PUBLIC_DATABASE_URL")?;

    if !Sqlite::database_exists(&db_url).await? {
        info!("creating sqlite database at {}", db_url);
        Sqlite::create_database(&db_url).await?;
    }

    info!("initializing public database connection");

    let connect_options = SqliteConnectOptions::from_str(&db_url)?
        .log_slow_statements(log::LevelFilter::Warn, Duration::from_secs(5))
        .busy_timeout(Duration::from_secs(30));

    let db = SqlitePoolOptions::new()
        .connect_with(connect_options)
        .await?;

    sqlx::migrate!("../migrations/public").run(&db).await?;

    Ok(db)
}

pub async fn init_private_db() -> Result<SqlitePool> {
    let db_url = env::var("PRIVATE_DATABASE_URL")?;

    if !Sqlite::database_exists(&db_url).await? {
        info!("creating sqlite database at {}", db_url);
        Sqlite::create_database(&db_url).await?;
    }

    info!("initializing private database connection");

    let connect_options = SqliteConnectOptions::from_str(&db_url)?
        .log_slow_statements(log::LevelFilter::Warn, Duration::from_secs(5))
        .busy_timeout(Duration::from_secs(30));

    let db = SqlitePoolOptions::new()
        .connect_with(connect_options)
        .await?;

    sqlx::migrate!("../migrations/private").run(&db).await?;

    Ok(db)
}

pub async fn init_dbs() -> Result<DatabasePools> {
    let public = init_public_db().await?;
    let private = init_private_db().await?;

    Ok(DatabasePools { public, private })
}

/// Checkpoint the WAL (Write-Ahead Log) to ensure the .db file is up to date.
/// This truncates the WAL after checkpointing.
pub async fn checkpoint_wal(db: &SqlitePool) -> Result<()> {
    info!("checkpointing WAL");
    sqlx::query("PRAGMA wal_checkpoint(TRUNCATE)")
        .execute(db)
        .await?;
    Ok(())
}
