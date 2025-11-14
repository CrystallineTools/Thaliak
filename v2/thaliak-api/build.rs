use std::process::Command;

fn main() {
    // Get the git commit hash
    let output = Command::new("git")
        .args(["rev-parse", "--short", "HEAD"])
        .output();

    let mut git_hash = match output {
        Ok(output) if output.status.success() => String::from_utf8(output.stdout)
            .unwrap_or_else(|_| "unknown".to_string())
            .trim()
            .to_string(),
        _ => "unknown".to_string(),
    };

    // Check if the working tree is dirty
    if git_hash != "unknown" {
        let dirty_check = Command::new("git").args(["status", "--porcelain"]).output();

        if let Ok(output) = dirty_check {
            if output.status.success() && !output.stdout.is_empty() {
                git_hash.push_str("-dirty");
            }
        }
    }

    println!("cargo:rustc-env=GIT_COMMIT_HASH={}", git_hash);
    println!("cargo:rerun-if-changed=../../.git/HEAD");
}
