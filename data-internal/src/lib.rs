#[macro_use]
extern crate thaliak_data;

pub use thaliak_data::*;
pub mod internal_types;

pub async fn connect() {
    let client = redis::Client::open("redis://redis:6379");
}
