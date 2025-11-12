use fern::colors::{Color, ColoredLevelConfig};

pub fn setup(log_file: Option<&str>) {
    // always enable trace logging on debug builds
    #[cfg(debug_assertions)]
    let log_level = log::LevelFilter::Trace;

    // on release builds, control via an env var
    #[cfg(not(debug_assertions))]
    let log_level = if std::env::var("LOG_DEBUG")
        .map(|v| matches!(v.to_lowercase().as_str(), "1" | "true" | "yes" | "on"))
        .unwrap_or(false)
    {
        log::LevelFilter::Trace
    } else {
        log::LevelFilter::Info
    };

    // I think the lack of consistency would bother me more than abandoning the
    // Queen's English here, so I will let this one slide.
    let colors = ColoredLevelConfig::new()
        .error(Color::BrightRed)
        .warn(Color::BrightYellow)
        .info(Color::BrightWhite)
        .debug(Color::BrightCyan)
        .trace(Color::BrightMagenta);

    let mut builder = fern::Dispatch::new()
        .format(move |out, message, record| {
            out.finish(format_args!(
                "[{}] [{}] {}",
                chrono::Local::now().format("%Y-%m-%d %H:%M:%S"),
                colors.color(record.level()),
                message
            ))
        })
        .level(log_level)
        .chain(std::io::stdout());

    if let Some(log_file) = log_file {
        builder = builder.chain(fern::log_file(log_file).unwrap());
    }

    builder.apply().expect("failed to setup logging");
}
