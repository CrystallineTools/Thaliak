use serde::{Deserialize, Serialize};

mod repository;
pub use repository::*;

#[derive(Copy, Clone, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub enum ServiceRegion {
    Jp,
    Cn,
    Kr,
}
