use anyhow::{anyhow, Error};
use diesel::{PgConnection};
use diesel::r2d2::{ConnectionManager, Pool, PooledConnection};
use diesel_migrations::{embed_migrations, EmbeddedMigrations, MigrationHarness};
use dotenvy::dotenv;
use log::info;
use once_cell::sync::OnceCell;

pub mod models;
pub mod schema;

pub type DbConnectionType = PgConnection;
pub type DbConnection = PooledConnection<ConnectionManager<DbConnectionType>>;
pub type DbConnectionPool = Pool<ConnectionManager<DbConnectionType>>;

const MIGRATIONS: EmbeddedMigrations = embed_migrations!();

static CONNECTION: OnceCell<DbConnectionPool> = OnceCell::new();

pub fn setup() -> Result<(), Error> {
    dotenv().ok();
    
    info!("connecting to database");
    let database_url = std::env::var("DATABASE_URL").expect("DATABASE_URL must be set");
    let manager = ConnectionManager::<DbConnectionType>::new(database_url);
    let pool = Pool::builder()
        .test_on_check_out(true)
        .build(manager)?;
    CONNECTION.set(pool).map_err(|_| anyhow!("failed to set connection"))?;
    
    // run migrations
    info!("running migrations");
    let mut connection = get_connection();
    let migrations_run = connection.run_pending_migrations(MIGRATIONS).unwrap();
    info!("migrations run: {:?}", migrations_run);
    
    Ok(())
}

pub fn get_pool() -> DbConnectionPool {
    CONNECTION.get().unwrap().clone()
}

pub fn get_connection() -> DbConnection {
    CONNECTION.get().unwrap().get().unwrap()
}
