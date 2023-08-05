use log::{info, warn};

use crate::poller::{actoz::ActozPoller, shanda::ShandaPoller, Poller};

mod poller;

#[tokio::main]
async fn main() {
    thaliak_logging::setup(None);

    info!("poller started");

    // todo: poll sqex/global once everything works properly

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
}
