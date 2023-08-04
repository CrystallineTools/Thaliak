use worker::*;

#[event(start)]
fn startup() {
    thaliak_worker::init();
}

/// The main entrypoint for the worker.
#[event(fetch)]
pub async fn main(req: Request, env: Env, _ctx: Context) -> Result<Response> {
    let router = Router::new();

    router.get_async("/ping", ping).run(req, env).await
}

async fn ping<D>(_: Request, _: RouteContext<D>) -> Result<Response> {
    Response::ok("pong")
}
