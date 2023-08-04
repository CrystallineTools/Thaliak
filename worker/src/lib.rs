pub mod data;

pub fn init() {
    worker_logger::init_with_level(
        #[cfg(feature = "debug")]
        &log::Level::Trace,
        #[cfg(not(feature = "debug"))]
        &log::Level::Info,
    );
}

#[macro_export]
macro_rules! worker_env {
    ($var:ident, $name:literal) => {
        $var.var($name)
            .expect(concat!("worker env var ", $name, " not set"))
            .to_string()
    };
}
