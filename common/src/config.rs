use anyhow::{bail, Result};
use serde::{Deserialize, Serialize};
use std::{
    fs::File,
    io::{BufReader, BufWriter, Read, Write},
    sync::OnceLock,
};

static CONFIG_FILE: &str = "config.toml";
static LOADED_CONFIG: OnceLock<Config> = OnceLock::new();

#[derive(Debug, Deserialize, Serialize)]
pub struct Config {
    pub database_url: String,
}

impl Config {
    pub fn get() -> &'static Config {
        LOADED_CONFIG.get().expect("config not loaded")
    }

    pub fn load() -> Result<&'static Config> {
        let file = File::open(CONFIG_FILE)?;
        let mut reader = BufReader::new(file);
        let mut contents = String::new();
        reader.read_to_string(&mut contents)?;

        let config: Config = Config::deserialize(toml::de::Deserializer::new(&contents))?;
        if LOADED_CONFIG.set(config).is_err() {
            bail!("failed to set config");
        }

        Ok(Self::get())
    }

    pub fn save(&self) -> Result<()> {
        let file = File::create(CONFIG_FILE)?;
        let mut contents = String::new();

        self.serialize(toml::ser::Serializer::new(&mut contents))?;

        let mut writer = BufWriter::new(file);
        writer.write_all(contents.as_bytes())?;

        Ok(())
    }
}
