# Build stage
FROM rust:1.91.1-slim as builder

RUN apt-get update && \
    apt-get install -y \
    pkg-config \
    libssl-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

COPY . .

RUN cargo build --release --package thaliak-poller

# Runtime stage
FROM debian:trixie-slim

RUN apt-get update && \
    apt-get install -y \
    ca-certificates \
    libssl3 \
    sqlite3 \
    && rm -rf /var/lib/apt/lists/*

# 999:988 match the uid/gid on the Thaliak host system
RUN groupadd -g 988 thaliak && \
    useradd -m -u 999 -g thaliak thaliak && \
    mkdir -p /data && \
    chown -R thaliak:thaliak /data

WORKDIR /app
USER thaliak

COPY --from=builder /build/target/release/thaliak-poller /app/thaliak-poller

ENV DATABASE_URL=sqlite:///data/thaliak.db
ENV RUST_LOG=info

VOLUME ["/data"]

CMD ["/app/thaliak-poller"]