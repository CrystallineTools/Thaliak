use crate::poller::BASE_GAME_VERSION;
use std::path::{Path, PathBuf};

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Repository {
    Boot,
    Ffxiv,
    Ex1,
    Ex2,
    Ex3,
    Ex4,
    Ex5,
}

impl Repository {
    fn get_repo_path<P: AsRef<Path>>(&self, game_path: P) -> PathBuf {
        let game_path = game_path.as_ref();
        match self {
            Repository::Boot => game_path.join("boot"),
            Repository::Ffxiv => game_path.join("game"),
            Repository::Ex1 => game_path.join("game").join("sqpack").join("ex1"),
            Repository::Ex2 => game_path.join("game").join("sqpack").join("ex2"),
            Repository::Ex3 => game_path.join("game").join("sqpack").join("ex3"),
            Repository::Ex4 => game_path.join("game").join("sqpack").join("ex4"),
            Repository::Ex5 => game_path.join("game").join("sqpack").join("ex5"),
        }
    }

    pub fn get_ver_file<P: AsRef<Path>>(&self, game_path: P) -> PathBuf {
        let repo_path = self.get_repo_path(game_path);
        match self {
            Repository::Boot => repo_path.join("ffxivboot.ver"),
            Repository::Ffxiv => repo_path.join("ffxivgame.ver"),
            Repository::Ex1 => repo_path.join("ex1.ver"),
            Repository::Ex2 => repo_path.join("ex2.ver"),
            Repository::Ex3 => repo_path.join("ex3.ver"),
            Repository::Ex4 => repo_path.join("ex4.ver"),
            Repository::Ex5 => repo_path.join("ex5.ver"),
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

    pub fn is_base_ver<P: AsRef<Path>>(&self, game_path: P) -> bool {
        self.get_ver(game_path) == BASE_GAME_VERSION
    }

    pub fn set_ver<P: AsRef<Path>>(&self, game_path: P, new_ver: &str) -> std::io::Result<()> {
        let ver_file = self.get_ver_file(game_path.as_ref());

        // Create parent directory if it doesn't exist
        if let Some(parent) = ver_file.parent() {
            std::fs::create_dir_all(parent)?;
        }

        std::fs::write(ver_file, new_ver)?;
        Ok(())
    }
}
