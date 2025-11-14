use eyre::Result;
use regex::Regex;
use std::sync::LazyLock;

pub static PATCH_URL_REGEX: LazyLock<Regex> =
    LazyLock::new(|| Regex::new(r"(?:https?://(.+?)/)?(?:ff/)?((?:game|boot)/.+)/(.*)").unwrap());

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
