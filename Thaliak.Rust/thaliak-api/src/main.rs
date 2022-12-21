#[macro_use] extern crate juniper;
#[macro_use] extern crate thaliak_macros;

mod graphql;

use std::convert::Infallible;
use std::sync::Arc;
use hyper::{Body, Method, Request, Response, Server, StatusCode};
use hyper::service::{make_service_fn, service_fn};
use log::{error, info};
use crate::graphql::{ApiVersion, get_api_version, GqlContext};

#[cfg(debug)]
const DEFAULT_LOG_LEVEL: &str = "trace";

#[cfg(not(debug))]
const DEFAULT_LOG_LEVEL: &str = "info";

#[tokio::main]
async fn main() {
    // init logging
    if std::env::var("RUST_LOG").is_err() {
        std::env::set_var("RUST_LOG", DEFAULT_LOG_LEVEL);
    }
    pretty_env_logger::init_timed();
    
    // setup the db
    thaliak_db::setup().unwrap();
    
    // setup hyper and juniper
    let ctx = Arc::new(GqlContext {
        pool: thaliak_db::get_pool(),
    });
    
    let service = make_service_fn(move |_| {
        let ctx = ctx.clone();
        async {
            Ok::<_, hyper::Error>(service_fn(move |req| {
                let ctx = ctx.clone();
                async {
                    Ok::<_, Infallible>(handle_request(ctx, req).await)
                }
            }))
        }
    });
    
    let addr = ([127, 0, 0, 1], 8008).into();
    let server = Server::bind(&addr).serve(service);
    
    info!("listening on http://{addr}");
    
    if let Err(e) = server.await {
        error!("server error: {}", e)
    }
}

async fn handle_request(ctx: Arc<GqlContext>, req: Request<Body>) -> Response<Body> {
    match req.method() {
        &Method::GET | &Method::POST => {
            let split = req.uri().path().split('/').collect::<Vec<_>>();
            let version = if split.len() > 1 && !split[1].is_empty() {
                split[1]
            } else {
                // todo: remove this fallback after 2023-01-04 (official unversioned API deprecation date)
                "2022-08-14"
            };
            
            if let Some(api_version) = get_api_version(version) {
                return api_version.execute_request(ctx, req).await;
            }
        }
        _ => {}
    }

    Response::builder()
        .status(StatusCode::NOT_FOUND)
        .body(Body::from("Invalid API version specified"))
        .unwrap()
}
