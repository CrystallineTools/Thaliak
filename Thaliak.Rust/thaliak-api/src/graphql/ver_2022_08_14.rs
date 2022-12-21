use bigdecimal::{BigDecimal, ToPrimitive};
use thaliak_db::models;
use diesel::prelude::*;
use log::info;
use thaliak_db::schema::repositories::dsl::repositories;
use thaliak_db::schema::versions;
use thaliak_db::schema::versions::dsl;
use crate::graphql::ApiVersion;
use super::GqlContext;

#[api_version(version = "2022-08-14")]
pub struct Query;

#[graphql_object(context = GqlContext)]
impl Query {
    fn hello() -> String {
        "Hello world!".to_string()
    }
    
    // repositories: [Repository!]!
    fn repositories(
        #[graphql(context)] ctx: &GqlContext,
    ) -> Vec<Repository> {
        ctx.with_db(|conn| {
            use thaliak_db::schema::repositories::dsl::*;
            repositories.load::<models::Repository>(conn).unwrap_or(vec![]) // todo: error handling
        }).into_iter().map(|model| model.into()).collect()
    }

    //   patches(repositoryId: String, repositorySlug: String, repositoryName: String, isActive: Boolean): [Patch!]!
    //   patch(repositoryId: String, repositorySlug: String, repositoryName: String, isActive: Boolean, id: ID, url: String): Patch
    //   patchChains: [PatchChain!]!
    //   patchChain(id: ID!): PatchChain
    //   files(name: String, sha1: String): [File!]!
    //   file(name: String, sha1: String, id: ID): File
    //   repository(slug: String, name: String, id: ID): Repository
    //   versions(repositoryId: String, repositorySlug: String, repositoryName: String): [Version!]!
    //   version(repositoryId: String, repositorySlug: String, repositoryName: String, id: ID, versionString: String): Version
    fn version(
        #[graphql(context)] ctx: &GqlContext,
        id: Option<i32>,
        version_string: Option<String>,
    ) -> Option<Version> {
        ctx.with_db(|conn| {
            let mut query = versions::table.into_boxed();
            if let Some(id) = id {
                query = query.filter(versions::id.eq(id));
            }
            if let Some(version_string) = version_string {
                query = query.filter(versions::version_string.eq(version_string));
            }
            query.first::<models::Version>(conn).ok()
        }).map(|model| model.into())
    }
    
}

#[derive(Clone, Debug)]
pub struct JsDateTime(String);

#[graphql_scalar(
    name = "DateTime",
    description = "The javascript `Date` as string. Type represents date and time as the ISO Date string."
)]
impl<S> GraphQLScalar for JsDateTime
    where
        S: juniper::ScalarValue,
{
    fn resolve(&self) -> juniper::Value {
        juniper::Value::scalar(self.0.to_owned())
    }

    fn from_input_value(v: &juniper::InputValue) -> Option<JsDateTime> {
        v.as_string_value().map(|s| JsDateTime(s.to_owned()))
    }

    fn from_str(value: juniper::ScalarToken) -> juniper::ParseScalarResult<S> {
        <String as juniper::ParseScalarValue<S>>::from_str(value)
    }
}

// type File {
//   name: String!
//   sha1: String!
//   size: Int!
//   lastUsed: DateTime!
//   versions: [Version!]!
//   id: ID!
//   version(id: String!): Version
// }
#[derive(GraphQLObject, Clone)]
pub struct File {
    pub name: String,
    pub sha1: String,
    pub size: i32,
    pub last_used: JsDateTime,
    // pub versions: Vec<Version>,
    pub id: String,
}

// type Version {
//   id: ID!
//   versionId: Float!
//   versionString: String!
//   repository: Repository!
//   patches: [Patch!]!
//   files: [File!]!
//   firstSeen: DateTime
//   firstOffered: DateTime
//   lastSeen: DateTime
//   lastOffered: DateTime
//   isActive: Boolean!
//   prerequisiteVersions: [Version!]!
//   dependentVersions: [Version!]!
// }
#[derive(Clone)]
pub struct Version {
    pub id: String,
    pub version_id: BigDecimal,
    pub version_string: String,
    // pub patches: Vec<Patch>,
    // pub files: Vec<File>,
    pub first_seen: Option<JsDateTime>,
    pub first_offered: Option<JsDateTime>,
    pub last_seen: Option<JsDateTime>,
    pub last_offered: Option<JsDateTime>,
    pub is_active: bool,
    // pub prerequisite_versions: Vec<Version>,
    // pub dependent_versions: Vec<Version>,
}

#[graphql_object(context = GqlContext)]
impl Version {
    fn id(&self) -> &str {
        &self.id
    }
    
    fn version_id(&self) -> f64 {
        self.version_id.to_f64().unwrap()
    }
    
    fn version_string(&self) -> &str {
        &self.version_string
    }
    
    fn first_seen(&self) -> Option<&JsDateTime> {
        self.first_seen.as_ref()
    }
    
    fn first_offered(&self) -> Option<&JsDateTime> {
        self.first_offered.as_ref()
    }
    
    fn last_seen(&self) -> Option<&JsDateTime> {
        self.last_seen.as_ref()
    }
    
    fn last_offered(&self) -> Option<&JsDateTime> {
        self.last_offered.as_ref()
    }
    
    fn is_active(&self) -> bool {
        self.is_active
    }
    
    fn repository(&self, #[graphql(context)] ctx: &GqlContext) -> Repository {
        // repositories
            // .filter(repositories::id.eq(&self.id))
            // .first::<models::Repository>(ctx.get_db()).unwrap().into()
        Repository {
            id: "test".to_string(),
            slug: "test".to_string(),
            name: "test".to_string(),
            description: None,
        }
    }
}

impl From<models::Version> for Version {
    fn from(model: models::Version) -> Self {
        Version {
            id: model.id.to_string(),
            version_id: model.version_id,
            version_string: model.version_string,
            first_seen: None,
            first_offered: None,
            last_seen: None,
            last_offered: None,
            is_active: false,
        }
    }
}

// type Repository {
//   id: ID!
//   name: String!
//   description: String
//   slug: String!
//   versions: [Version!]!
//   latestVersion: Version!
//   version(id: String!): Version
// }
#[derive(GraphQLObject, Clone)]
pub struct Repository {
    pub id: String,
    pub name: String,
    pub description: Option<String>,
    pub slug: String,
    // pub versions: Vec<Version>,
    // pub latest_version: Version,
}

impl From<models::Repository> for Repository {
    fn from(model: models::Repository) -> Self {
        Self {
            id: "".to_string(), // todo
            name: model.name,
            description: model.description,
            slug: model.slug,
        }
    }
}

// type PatchChain {
//   id: ID!
//   repository: Repository!
//   patch: Patch!
//   previousPatch: Patch
//   firstOffered: DateTime
//   lastOffered: DateTime
//   isActive: Boolean!
// }
#[derive(GraphQLObject, Clone)]
pub struct PatchChain {
    pub id: String,
    // pub repository: Repository,
    // pub patch: Patch,
    // pub previous_patch: Option<Patch>,
    pub first_offered: Option<JsDateTime>,
    pub last_offered: Option<JsDateTime>,
    pub is_active: bool,
}

// type Patch {
//   id: ID!
//   version: Version!
//   repository: Repository!
//   url: String!
//   firstSeen: DateTime
//   lastSeen: DateTime
//   firstOffered: DateTime
//   lastOffered: DateTime
//   isActive: Boolean!
//   size: Int!
//   hashType: String
//   hashBlockSize: Int
//   prerequisitePatches: [PatchChain!]!
//   dependentPatches: [PatchChain!]!
//   versionString: String!
//   prerequisiteVersions: [Version!]!
//   dependentVersions: [Version!]!
// }
#[derive(GraphQLObject, Clone)]
pub struct Patch {
    pub id: String,
    // pub version: Version,
    // pub repository: Repository,
    pub url: String,
    pub first_seen: Option<JsDateTime>,
    pub last_seen: Option<JsDateTime>,
    pub first_offered: Option<JsDateTime>,
    pub last_offered: Option<JsDateTime>,
    pub is_active: bool,
    pub size: i32,
    pub hash_type: Option<String>,
    pub hash_block_size: Option<i32>,
    // pub prerequisite_patches: Vec<PatchChain>,
    // pub dependent_patches: Vec<PatchChain>,
    pub version_string: String,
    // pub prerequisite_versions: Vec<Version>,
    // pub dependent_versions: Vec<Version>,
}
