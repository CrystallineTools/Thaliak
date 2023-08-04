use super::*;
use crate::{impl_key, Identifiable};
use phf::phf_map;

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct RepositoryState {
    pub id: String,
    pub latest_version: String,
}

impl Identifiable for RepositoryState {
    fn get_id(&self) -> String {
        self.id.clone()
    }
}

impl_key!(RepositoryKey, RepositoryState, "repository:{}:state");

#[derive(Clone, Debug, Serialize)]
pub struct Repository {
    pub region: ServiceRegion,
    pub id: &'static str,
    pub name: &'static str,
    pub repo_type: RepositoryType,
}

impl TryFrom<RepositoryState> for &Repository {
    type Error = ();

    fn try_from(value: RepositoryState) -> Result<Self, Self::Error> {
        Repository::by_id(value.id.as_str()).ok_or(())
    }
}

#[derive(Clone, Debug, Serialize)]
pub enum RepositoryType {
    Boot,
    Game,
    Expansion(u8),
}

impl Repository {
    pub const ALL: phf::Map<&'static str, Repository> = phf_map! {
        //
        // Global (Square Enix)
        //

        // Global - Boot
        "2b5cbc63" => Repository {
            id: "2b5cbc63",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/boot",
            repo_type: RepositoryType::Boot,
        },
        // Global - Game
        "4e9a232b" => Repository {
            id: "4e9a232b",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/game",
            repo_type: RepositoryType::Game,
        },
        // Global - Heavensward
        "6b936f08" => Repository {
            id: "6b936f08",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/ex1",
            repo_type: RepositoryType::Expansion(1),
        },
        // Global - Stormblood
        "a4c8f8a1" => Repository {
            id: "a4c8f8a1",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/ex2",
            repo_type: RepositoryType::Expansion(2),
        },
        // Global - Shadowbringers
        "b4e5a1a1" => Repository {
            id: "b4e5a1a1",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/ex3",
            repo_type: RepositoryType::Expansion(3),
        },
        // Global - Endwalker
        "c4e5a1a1" => Repository {
            id: "c4e5a1a1",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/ex4",
            repo_type: RepositoryType::Expansion(4),
        },
        // Global - Dawntrail (unreleased)
        "6cfeab11" => Repository {
            id: "6cfeab11",
            region: ServiceRegion::Jp,
            name: "ffxivneo/win32/release/ex5",
            repo_type: RepositoryType::Expansion(5),
        },

        //
        // China (Shanda)
        //

        // China - Game
        "c38effbc" => Repository {
            id: "c38effbc",
            region: ServiceRegion::Cn,
            name: "shanda/win32/release_chs/game",
            repo_type: RepositoryType::Game,
        },
        // China - Heavensward
        "77420d17" => Repository {
            id: "77420d17",
            region: ServiceRegion::Cn,
            name: "shanda/win32/release_chs/ex1",
            repo_type: RepositoryType::Expansion(1),
        },
        // China - Stormblood
        "ee4b5cad" => Repository {
            id: "ee4b5cad",
            region: ServiceRegion::Cn,
            name: "shanda/win32/release_chs/ex2",
            repo_type: RepositoryType::Expansion(2),
        },
        // China - Shadowbringers
        "994c6c3b" => Repository {
            id: "994c6c3b",
            region: ServiceRegion::Cn,
            name: "shanda/win32/release_chs/ex3",
            repo_type: RepositoryType::Expansion(3),
        },
        // China - Endwalker
        "0728f998" => Repository {
            id: "0728f998",
            region: ServiceRegion::Cn,
            name: "shanda/win32/release_chs/ex4",
            repo_type: RepositoryType::Expansion(4),
        },
        // China - Dawntrail (unreleased)
        "702fc90e" => Repository {
            id: "702fc90e",
            region: ServiceRegion::Cn,
            name: "shanda/win32/release_chs/ex5",
            repo_type: RepositoryType::Expansion(5),
        },

        //
        // Korea (Actoz)
        //

        // Korea - Game
        "de199059" => Repository {
            id: "de199059",
            region: ServiceRegion::Kr,
            name: "actoz/win32/release_ko/game",
            repo_type: RepositoryType::Game,
        },
        // Korea - Heavensward
        "573d8c07" => Repository {
            id: "573d8c07",
            region: ServiceRegion::Kr,
            name: "actoz/win32/release_ko/ex1",
            repo_type: RepositoryType::Expansion(1),
        },
        // Korea - Stormblood
        "ce34ddbd" => Repository {
            id: "ce34ddbd",
            region: ServiceRegion::Kr,
            name: "actoz/win32/release_ko/ex2",
            repo_type: RepositoryType::Expansion(2),
        },
        // Korea - Shadowbringers
        "b933ed2b" => Repository {
            id: "b933ed2b",
            region: ServiceRegion::Kr,
            name: "actoz/win32/release_ko/ex3",
            repo_type: RepositoryType::Expansion(3),
        },
        // Korea - Endwalker
        "27577888" => Repository {
            id: "27577888",
            region: ServiceRegion::Kr,
            name: "actoz/win32/release_ko/ex4",
            repo_type: RepositoryType::Expansion(4),
        },
        // Korea - Dawntrail (unreleased)
        "5050481e" => Repository {
            id: "5050481e",
            region: ServiceRegion::Kr,
            name: "actoz/win32/release_ko/ex5",
            repo_type: RepositoryType::Expansion(5),
        },
    };

    pub fn by_id(id: &str) -> Option<&'static Repository> {
        Self::ALL.get(id)
    }

    pub fn for_region(region: ServiceRegion) -> impl Iterator<Item = &'static Repository> {
        Self::ALL.values().filter(move |repo| repo.region == region)
    }
}
