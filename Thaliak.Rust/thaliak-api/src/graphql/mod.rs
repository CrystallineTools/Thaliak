use std::borrow::BorrowMut;
use std::collections::HashMap;
use std::future::Future;
use std::ops::DerefMut;
use std::pin::Pin;
use std::sync::{Arc};
use hyper::{Body, Request, Response};
use juniper::Context;
use once_cell::sync::Lazy;
use thaliak_db::{DbConnection, DbConnectionPool};

pub mod ver_2022_08_14;

type ApiVersionExecutor = fn(ctx: Arc<GqlContext>, body: Request<Body>) -> Pin<Box<dyn Future<Output=Response<Body>> + Send>>;

#[derive(Copy, Clone)]
pub struct ApiVersion {
    pub version: &'static str,
    executor: ApiVersionExecutor,
}

impl ApiVersion {
    pub async fn execute_request(&self, ctx: Arc<GqlContext>, body: Request<Body>) -> Response<Body> {
        (self.executor)(ctx, body).await
    }
}

static API_VERSIONS: Lazy<HashMap<&'static str, &'static ApiVersion>> = Lazy::new(|| {
    inventory::collect!(ApiVersion);
    
    let mut map = HashMap::new();
    for version in inventory::iter::<ApiVersion> {
        map.insert(version.version, version);
    }
    
    map
});

pub fn get_api_version(version: &str) -> Option<&'static ApiVersion> {
    API_VERSIONS.get(version).copied()
}

#[derive(Clone)]
pub struct GqlContext {
    pub(crate) pool: DbConnectionPool,
}
impl Context for GqlContext {}
impl GqlContext {
    pub fn with_db<T>(&self, f: impl FnOnce(&mut DbConnection) -> T) -> T {
        f(&mut self.pool.get().unwrap())
    }
}

macro_rules! add_api_version {
    ($($version:literal => $handler:path),* $(,)?) => {
        $(
            inventory::submit! {
                ApiVersion {
                    version: $version,
                    executor: $handler
                }
            }
        )*
    };
}
pub(crate) use add_api_version;
