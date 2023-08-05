use fern::colors::{Color, ColoredLevelConfig};

pub fn setup(log_file: Option<&str>) {
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
        .chain(std::io::stdout());

    if let Some(log_file) = log_file {
        builder = builder.chain(fern::log_file(log_file).unwrap());
    }

    builder.apply().expect("failed to setup logging");
}
