use crate::patch::PatchReconciliationService;
use crate::poller::{Poller, actoz::ActozPoller, shanda::ShandaPoller, sqex::SqexPoller};
use eyre::Result;
use log::{info, warn};
use sqlx::{Sqlite, SqliteConnection, SqlitePool, migrate::MigrateDatabase};
use std::env;

mod patch;
mod poller;

pub type DbConnection = SqliteConnection;
async fn init_db() -> Result<SqlitePool> {
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

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let db = init_db().await?;
    thaliak_common::logging::setup(None);

    info!("poller started");

    let reconciliation = PatchReconciliationService::new(&db);

    // match SqexPoller::new()?.poll(&reconciliation).await {
    //     Err(e) => {
    //         warn!("Polling JP failed: {:?}", e);
    //     }
    //     _ => {}
    // }

    match ActozPoller::new().poll(&reconciliation).await {
        Err(e) => {
            warn!("Polling KR failed: {:?}", e);
        }
        _ => {}
    }

    match ShandaPoller::new().poll(&reconciliation).await {
        Err(e) => {
            warn!("Polling CN failed: {:?}", e);
        }
        _ => {}
    }

    Ok(())
}
