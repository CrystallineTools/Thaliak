use eyre::Result;
use log::info;
use sqlx::migrate::MigrateDatabase;
use sqlx::{Sqlite, SqlitePool};
use std::env;

pub mod logging;

pub async fn init_db() -> Result<SqlitePool> {
    let db_url = env::var("DATABASE_URL")?;
    if !Sqlite::database_exists(&db_url).await? {
        info!("creating sqlite database at {}", db_url);
        Sqlite::create_database(&db_url).await?;
    }

    info!("initializing database connection");
    let db = SqlitePool::connect(&db_url).await?;
    sqlx::migrate!("../migrations").run(&db).await?;

    Ok(db)
}
