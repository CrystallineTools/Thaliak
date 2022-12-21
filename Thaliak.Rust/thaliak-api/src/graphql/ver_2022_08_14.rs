use crate::graphql::ApiVersion;
use super::GqlContext;

#[api_version(version = "2022-08-14")]
pub struct Query;

#[graphql_object(context = GqlContext)]
impl Query {
    fn hello() -> String {
        "Hello world!".to_string()
    }
}
