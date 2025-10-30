use crate::poller::{Poller, actoz::ActozPoller, shanda::ShandaPoller, sqex::SqexPoller};
use eyre::Result;
use log::{info, warn};

mod poller;

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();

    thaliak_common::logging::setup(None);

    info!("poller started");

    match SqexPoller::new().poll().await {
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
