use eyre::Result;
use regex::Regex;
use std::path::{Path, PathBuf};
use std::sync::LazyLock;

pub static PATCH_URL_REGEX: LazyLock<Regex> =
    LazyLock::new(|| Regex::new(r"(?:https?://(.+?)/)?(?:ff/)?((?:game|boot)/.+)/(.*)").unwrap());

pub const BASE_GAME_VERSION: &str = "2012.01.01.0000.0000";

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum GameRepository {
    Boot,
    Ffxiv,
    Ex1,
    Ex2,
    Ex3,
    Ex4,
    Ex5,
}

impl GameRepository {
    pub fn from_slug(slug: &str) -> Option<Self> {
        match slug {
            "ffxivboot" => Some(GameRepository::Boot),
            "4e9a232b" => Some(GameRepository::Ffxiv),
            "6b936f08" => Some(GameRepository::Ex1),
            "f29a3eb2" => Some(GameRepository::Ex2),
            "859d0e24" => Some(GameRepository::Ex3),
            "1bf99b87" => Some(GameRepository::Ex4),
            "2b927962" => Some(GameRepository::Ex5),
            _ => None,
        }
    }

    pub fn get_ver_file<P: AsRef<Path>>(&self, game_path: P) -> PathBuf {
        let game_path = game_path.as_ref();
        match self {
            GameRepository::Boot => game_path.join("ffxivboot.ver"),
            GameRepository::Ffxiv => game_path.join("ffxivgame.ver"),
            GameRepository::Ex1 => game_path.join("ex1.ver"),
            GameRepository::Ex2 => game_path.join("ex2.ver"),
            GameRepository::Ex3 => game_path.join("ex3.ver"),
            GameRepository::Ex4 => game_path.join("ex4.ver"),
            GameRepository::Ex5 => game_path.join("ex5.ver"),
        }
    }

    pub fn get_ver<P: AsRef<Path>>(&self, game_path: P) -> String {
        let ver_file = self.get_ver_file(game_path);

        if !ver_file.exists() {
            return BASE_GAME_VERSION.to_string();
        }

        std::fs::read_to_string(ver_file)
            .unwrap_or_else(|_| BASE_GAME_VERSION.to_string())
            .trim()
            .to_string()
    }

    pub fn set_ver<P: AsRef<Path>>(&self, game_path: P, new_ver: &str) -> std::io::Result<()> {
        let ver_file = self.get_ver_file(game_path.as_ref());

        if let Some(parent) = ver_file.parent() {
            std::fs::create_dir_all(parent)?;
        }

        std::fs::write(ver_file, new_ver)?;
        Ok(())
    }
}

pub fn get_local_storage_path(remote_url: &str) -> Result<String> {
    let caps = PATCH_URL_REGEX.captures(remote_url).ok_or_else(|| {
        eyre::Report::msg(format!(
            "Unable to match URL to PatchUrlRegex: {}",
            remote_url
        ))
    })?;

    let hostname = caps.get(1).map(|m| m.as_str()).unwrap_or("");
    let repo_path = caps.get(2).map(|m| m.as_str()).unwrap_or("");
    let filename = caps.get(3).map(|m| m.as_str()).unwrap_or("");

    Ok(format!("{}/{}/{}", hostname, repo_path, filename))
}
