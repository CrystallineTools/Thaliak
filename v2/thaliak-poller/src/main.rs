use crate::patch::PatchReconciliationService;
use crate::poller::{Poller, actoz::ActozPoller, shanda::ShandaPoller, sqex::SqexPoller};
use chrono::{Timelike, Utc};
use eyre::Result;
use log::{info, warn};
use rand::Rng;
use sqlx::{Sqlite, SqliteConnection, SqlitePool, migrate::MigrateDatabase};
use std::env;
use std::fmt::Debug;
use std::time::Duration;
use tokio::time::{Instant, sleep_until};

mod patch;
mod poller;

pub type DbConnection = SqliteConnection;

fn minutes_until_next_odd_minute() -> u64 {
    let current_minute = Utc::now().minute();
    if current_minute % 2 == 0 { 1 } else { 2 }
}

async fn execute_poll<P: Poller>(
    poller: &P,
    reconciliation: &PatchReconciliationService,
    name: &str,
) where
    P::Error: Debug,
{
    match poller.poll(reconciliation).await {
        Ok(_) => info!("{}: poll completed successfully", name),
        Err(e) => warn!("{}: polling failed: {:?}", name, e),
    }
}

async fn poll_sqex_loop(reconciliation: PatchReconciliationService) {
    let poller = match SqexPoller::new() {
        Ok(p) => p,
        Err(e) => {
            warn!("Failed to create SqexPoller: {:?}", e);
            return;
        }
    };

    info!("SqexPoller: starting initial poll");
    execute_poll(&poller, &reconciliation, "SqexPoller").await;

    loop {
        let wait_minutes = minutes_until_next_odd_minute();
        info!("SqexPoller: sleeping for {} minute(s)", wait_minutes);
        sleep_until(Instant::now() + Duration::from_secs(wait_minutes * 60)).await;

        info!("SqexPoller: starting poll");
        execute_poll(&poller, &reconciliation, "SqexPoller").await;
    }
}

async fn poll_actoz_loop(reconciliation: PatchReconciliationService) {
    let mut rng = rand::thread_rng();
    let poller = ActozPoller::new();

    info!("ActozPoller: starting initial poll");
    execute_poll(&poller, &reconciliation, "ActozPoller").await;

    loop {
        let wait_minutes = rng.gen_range(40..=59);
        let total_wait = Duration::from_secs((wait_minutes * 60) as u64);

        info!("ActozPoller: sleeping for {} minutes", wait_minutes);
        sleep_until(Instant::now() + total_wait).await;

        info!("ActozPoller: starting poll");
        execute_poll(&poller, &reconciliation, "ActozPoller").await;
    }
}

async fn poll_shanda_loop(reconciliation: PatchReconciliationService) {
    let mut rng = rand::thread_rng();
    let poller = ShandaPoller::new();

    info!("ShandaPoller: starting initial poll");
    execute_poll(&poller, &reconciliation, "ShandaPoller").await;

    loop {
        let wait_minutes = rng.gen_range(40..=59);
        let total_wait = Duration::from_secs((wait_minutes * 60) as u64);

        info!("ShandaPoller: sleeping for {} minutes", wait_minutes);
        sleep_until(Instant::now() + total_wait).await;

        info!("ShandaPoller: starting poll");
        execute_poll(&poller, &reconciliation, "ShandaPoller").await;
    }
}

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    let db = thaliak_common::init_db().await?;
    thaliak_common::logging::setup(None);

    info!("poller service started");

    let reconciliation = PatchReconciliationService::new(&db);

    tokio::select! {
        _ = poll_sqex_loop(reconciliation.clone()) => {},
        _ = poll_actoz_loop(reconciliation.clone()) => {},
        _ = poll_shanda_loop(reconciliation.clone()) => {},
    }

    Ok(())
}
