-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- user table
CREATE TABLE user
(
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    discord_user_id TEXT     NOT NULL UNIQUE,
    registered_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_user_discord_id ON user (discord_user_id);

-- webhook table
CREATE TABLE webhook
(
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    owner_user_id INTEGER,
    url           TEXT     NOT NULL,
    created_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    subscribe_jp  BOOLEAN  NOT NULL DEFAULT 0,
    subscribe_kr  BOOLEAN  NOT NULL DEFAULT 0,
    subscribe_cn  BOOLEAN  NOT NULL DEFAULT 0,
    FOREIGN KEY (owner_user_id) REFERENCES user (id) ON DELETE CASCADE
);

CREATE INDEX idx_webhook_owner ON webhook (owner_user_id);
CREATE INDEX idx_webhook_subscriptions ON webhook (subscribe_jp, subscribe_kr, subscribe_cn);
