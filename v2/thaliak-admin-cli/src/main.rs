mod commands;

use clap::{Parser, Subcommand};
use eyre::Result;

#[derive(Parser)]
#[command(name = "thaliak-admin")]
#[command(about = "Thaliak administration CLI", long_about = None)]
struct Cli {
    #[command(subcommand)]
    command: Commands,
}

#[derive(Subcommand)]
enum Commands {
    #[command(about = "Analyze a patch chain from a given patch back to the beginning")]
    AnalyseChain {
        #[arg(help = "Repository slug (e.g., 4e9a232b)")]
        repository_slug: String,

        #[arg(help = "Patch version string (e.g., 2025.10.30.0000.0000)")]
        patch_version: String,
    },
    #[command(about = "Migrate v1 database (Postgres) to v2 (SQLite)")]
    Migrate {
        #[arg(long, env, help = "Postgres connection string for v1 database")]
        v1_db: String,
    },
}

#[tokio::main]
async fn main() -> Result<()> {
    let _ = dotenvy::dotenv();
    thaliak_common::logging::setup(None);

    let cli = Cli::parse();

    match cli.command {
        Commands::AnalyseChain {
            repository_slug,
            patch_version,
        } => commands::analyse_chain::execute(repository_slug, patch_version).await?,
        Commands::Migrate { v1_db } => commands::migrate::execute(v1_db).await?,
    }

    Ok(())
}
