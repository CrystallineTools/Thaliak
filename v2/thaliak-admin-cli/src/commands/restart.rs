use eyre::Result;
use std::process::Command;

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
        .arg("restart")
        .arg(&container_name)
        .status()?;

    if !status.success() {
        return Err(eyre::Report::msg(format!(
            "Failed to restart container '{}'",
            container_name
        )));
    }

    Ok(())
}
