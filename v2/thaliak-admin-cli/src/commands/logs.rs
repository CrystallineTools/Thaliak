use eyre::Result;
use std::process::{Command, Stdio};

use super::VALID_SERVICES;

pub fn execute(service: String) -> Result<()> {
    if !VALID_SERVICES.contains(&service.as_str()) {
        return Err(eyre::Report::msg(format!(
            "Invalid service '{}'. Valid services are: {}",
            service,
            VALID_SERVICES.join(", ")
        )));
    }

    let container_name = format!("thaliak-{}-v2", service);

    let status = Command::new("docker")
        .arg("logs")
        .arg("-f")
        .arg(&container_name)
        .stdout(Stdio::inherit())
        .stderr(Stdio::inherit())
        .status()?;

    if !status.success() {
        return Err(eyre::Report::msg(format!(
            "Failed to tail logs for container '{}'",
            container_name
        )));
    }

    Ok(())
}
