### aliases
## rust build base
FROM rust:1.71 AS rust-base
WORKDIR /app
RUN cargo install cargo-chef

## runtime base
FROM debian:12-slim AS runtime-base
WORKDIR /app

### building/caching shared deps
## plan the build
FROM rust-base AS planner
COPY . .
RUN cargo chef prepare --recipe-path recipe.json

## build shared deps
FROM rust-base AS builder
COPY --from=planner /app/recipe.json recipe.json
# cache deps
RUN cargo chef cook --release --recipe-path recipe.json
# build our code
COPY . .
RUN cargo build --release

### building the final images
## poller
FROM runtime-base AS thaliak-poller
COPY --from=builder /app/target/release/thaliak-poller /app/thaliak-poller
CMD ["/app/thaliak-poller"]

## api
FROM runtime-base AS thaliak-api
COPY --from=builder /app/target/release/thaliak-api /app/thaliak-api
CMD ["/app/thaliak-api"]
