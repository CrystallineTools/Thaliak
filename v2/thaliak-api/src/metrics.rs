use axum::{
    extract::{MatchedPath, Request},
    http::StatusCode,
    middleware::Next,
    response::Response,
};
use log::info;
use metrics_exporter_prometheus::PrometheusBuilder;
use std::net::SocketAddr;
use std::time::Instant;

pub async fn init_metrics_exporter() -> eyre::Result<()> {
    let addr = SocketAddr::from(([0, 0, 0, 0], 9090));
    PrometheusBuilder::new()
        .with_http_listener(addr)
        .set_buckets_for_metric(
            metrics_exporter_prometheus::Matcher::Full(
                "thaliak_http_request_duration_seconds".to_string(),
            ),
            &[
                0.001, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0, 2.5, 5.0, 10.0,
            ],
        )?
        .set_buckets_for_metric(
            metrics_exporter_prometheus::Matcher::Full(
                "thaliak_db_query_duration_seconds".to_string(),
            ),
            &[
                0.0001, 0.0005, 0.001, 0.0025, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0,
            ],
        )?
        .install()?;

    info!("starting Prometheus metrics server at http://{}", addr);

    metrics::describe_counter!(
        "thaliak_http_requests_total",
        "Total number of HTTP requests"
    );

    metrics::describe_histogram!(
        "thaliak_http_request_duration_seconds",
        "HTTP request duration in seconds"
    );

    metrics::describe_histogram!(
        "thaliak_db_query_duration_seconds",
        "Database query duration in seconds"
    );

    metrics::describe_counter!(
        "thaliak_db_queries_total",
        "Total number of database queries"
    );

    metrics::describe_counter!("thaliak_errors_total", "Total number of errors");

    metrics::describe_counter!(
        "thaliak_repository_requests_total",
        "Total number of requests by repository slug"
    );

    metrics::describe_counter!(
        "thaliak_patch_requests_total",
        "Total number of patch requests by repository and version"
    );

    metrics::describe_counter!(
        "thaliak_patch_chain_requests_total",
        "Total number of patch chain requests with parameters"
    );

    Ok(())
}

pub async fn track_metrics(req: Request, next: Next) -> Result<Response, StatusCode> {
    let start = Instant::now();
    let method = req.method().to_string();
    let path = req
        .extensions()
        .get::<MatchedPath>()
        .map(|p| p.as_str().to_string())
        .unwrap_or_else(|| req.uri().path().to_string());

    let response = next.run(req).await;

    let duration = start.elapsed().as_secs_f64();
    let status = response.status().as_u16().to_string();

    metrics::counter!("thaliak_http_requests_total",
        "method" => method.clone(),
        "endpoint" => path.clone(),
        "status" => status.clone()
    )
    .increment(1);

    metrics::histogram!("thaliak_http_request_duration_seconds",
        "method" => method,
        "endpoint" => path,
        "status" => status
    )
    .record(duration);

    Ok(response)
}

pub fn record_db_query(operation: &str, duration_secs: f64) {
    metrics::counter!("thaliak_db_queries_total",
        "operation" => operation.to_string()
    )
    .increment(1);

    metrics::histogram!("thaliak_db_query_duration_seconds",
        "operation" => operation.to_string()
    )
    .record(duration_secs);
}

pub fn record_error(error_type: &str) {
    metrics::counter!("thaliak_errors_total",
        "error_type" => error_type.to_string()
    )
    .increment(1);
}

pub fn record_repository_request(slug: &str) {
    metrics::counter!("thaliak_repository_requests_total",
        "repository" => slug.to_string()
    )
    .increment(1);
}

pub fn record_patch_request(slug: &str, version: &str) {
    metrics::counter!("thaliak_patch_requests_total",
        "repository" => slug.to_string(),
        "version" => version.to_string()
    )
    .increment(1);
}

pub fn record_patch_chain_request(slug: &str, from: Option<&str>, to: Option<&str>, all: bool) {
    metrics::counter!("thaliak_patch_chain_requests_total",
        "repository" => slug.to_string(),
        "has_from" => from.is_some().to_string(),
        "has_to" => to.is_some().to_string(),
        "all" => all.to_string()
    )
    .increment(1);
}
