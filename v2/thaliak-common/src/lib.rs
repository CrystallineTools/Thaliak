use eyre::Result;
use log::info;
use sqlx::migrate::MigrateDatabase;
use sqlx::sqlite::{SqliteConnectOptions, SqlitePoolOptions};
use sqlx::{ConnectOptions, Sqlite, SqlitePool};
use std::env;
use std::str::FromStr;
use std::time::Duration;

pub mod logging;

pub async fn init_db() -> Result<SqlitePool> {
    let db_url = env::var("DATABASE_URL")?;
    if !Sqlite::database_exists(&db_url).await? {
        info!("creating sqlite database at {}", db_url);
        Sqlite::create_database(&db_url).await?;
    }

    info!("initializing database connection");

    // Configure connection options with increased slow query threshold for pollers
    let connect_options = SqliteConnectOptions::from_str(&db_url)?
        .log_slow_statements(log::LevelFilter::Warn, Duration::from_secs(5))
        .busy_timeout(Duration::from_secs(30)); // Wait up to 30s for locks

    let db = SqlitePoolOptions::new()
        .connect_with(connect_options)
        .await?;

    sqlx::migrate!("../migrations").run(&db).await?;

    Ok(db)
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
