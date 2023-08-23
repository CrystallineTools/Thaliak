use serde::{Deserialize, Serialize};
use thaliak_data::Identifiable;

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct Agent {
    pub id: String,
}

impl Identifiable for Agent {
    fn get_id(&self) -> String {
        self.id.clone()
    }
}

impl_key!(AgentKey, Agent, "agent:{}");
