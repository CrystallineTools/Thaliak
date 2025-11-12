use crate::patch::PatchReconciliationService;
use crate::poller::{Poller, actoz::ActozPoller, shanda::ShandaPoller, sqex::SqexPoller};
use eyre::Result;
use log::{info, warn};
use sqlx::{Sqlite, SqliteConnection, SqlitePool, migrate::MigrateDatabase};
use std::env;

mod patch;
mod poller;

pub type DbConnection = SqliteConnection;

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let db = thaliak_common::init_db().await?;
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
