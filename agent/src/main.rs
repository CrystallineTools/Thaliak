use log::info;

#[tokio::main]
async fn main() {
    thaliak_logging::setup(None);
    info!("starting Thaliak agent");
}
