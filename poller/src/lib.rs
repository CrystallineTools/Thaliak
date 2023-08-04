#[macro_use]
extern crate thaliak_worker;

use log::{info, warn};
use worker::*;

use crate::poller::{actoz::ActozPoller, shanda::ShandaPoller, Poller};

mod poller;

#[event(start)]
fn startup() {
    thaliak_worker::init();
}

/// Endpoint used in local development to indicate when the worker is ready to receive requests.
#[cfg(feature = "debug")]
#[event(fetch)]
async fn ping(_: Request, _: Env, _: Context) -> Result<Response> {
    Response::ok("pong")
}

/// The main scheduled entrypoint for the worker.
#[event(scheduled)]
async fn main(event: ScheduledEvent, _env: Env, _ctx: ScheduleContext) {
    info!("Poller starting on event: {:?}", event);

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
