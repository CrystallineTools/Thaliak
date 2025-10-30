use crate::poller::{Poller, actoz::ActozPoller, shanda::ShandaPoller, sqex::SqexPoller};
use eyre::Result;
use log::{info, warn};
use sqlx::SqlitePool;
use std::env;

mod poller;

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let db = SqlitePool::connect(&env::var("DATABASE_URL")?).await?;
    sqlx::migrate!("../migrations").run(&db).await?;

    thaliak_common::logging::setup(None);

    info!("poller started");

    match SqexPoller::new()?.poll().await {
        Err(e) => {
            warn!("Polling JP failed: {:?}", e);
        }
        _ => {}
    }

    match ActozPoller::new().poll().await {
        Err(e) => {
            warn!("Polling KR failed: {:?}", e);
        }
        _ => {}
    }

    match ShandaPoller::new().poll().await {
        Err(e) => {
            warn!("Polling CN failed: {:?}", e);
        }
        _ => {}
    }

    Ok(())
}
