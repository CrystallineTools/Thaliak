# Build stage
FROM rust:1.91.1-slim as builder

RUN apt-get update && \
    apt-get install -y \
    pkg-config \
    libssl-dev \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

COPY .git .git
COPY v2 v2

# Set DATABASE_URL for compile-time SQLx verification
ENV DATABASE_URL=sqlite://thaliak-public.db

WORKDIR /build/v2
RUN cargo build --release --package thaliak-admin-cli

# Runtime stage
FROM debian:trixie-slim

RUN apt-get update && \
    apt-get install -y \
    ca-certificates \
    libssl3 \
    sqlite3 \
    curl \
    docker-cli \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=builder /build/v2/target/release/thaliak-admin /app/thaliak-admin

# Runtime database configuration
ENV PUBLIC_DATABASE_URL=sqlite:///db/thaliak-public.db
ENV PRIVATE_DATABASE_URL=sqlite:///db/thaliak-private.db

VOLUME ["/data", "/db"]

ENTRYPOINT ["/app/thaliak-admin"]
