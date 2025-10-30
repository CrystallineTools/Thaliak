-- Enable foreign key support (required for ON DELETE CASCADE)
PRAGMA foreign_keys = ON;

-- service table
CREATE TABLE service
(
    id   INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    icon TEXT NOT NULL
);

-- repository table
CREATE TABLE repository
(
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    service_id  INTEGER NOT NULL,
    slug        TEXT    NOT NULL,
    name        TEXT    NOT NULL,
    description TEXT,
    FOREIGN KEY (service_id) REFERENCES service (id) ON DELETE CASCADE
);
CREATE INDEX ix_repository_service_id ON repository (service_id);
CREATE INDEX ix_repository_slug ON repository (slug);

-- patch table
CREATE TABLE patch
(
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    repository_id   INTEGER NOT NULL,
    patch_base_id   INTEGER NOT NULL,
    version_string  TEXT    NOT NULL,
    remote_url      TEXT    NOT NULL,
    local_path      TEXT    NOT NULL DEFAULT '',
    first_seen      TEXT,
    last_seen       TEXT,
    size            INTEGER NOT NULL,
    hash_type       TEXT,
    hash_block_size INTEGER,
    hashes          TEXT,
    first_offered   TEXT,
    last_offered    TEXT,
    is_active       BOOLEAN NOT NULL DEFAULT false,
    FOREIGN KEY (repository_id) REFERENCES repository (id) ON DELETE CASCADE,
    FOREIGN KEY (patch_base_id) REFERENCES patch_base (id) ON DELETE CASCADE
);
CREATE INDEX ix_patch_repository_id ON patch (repository_id);
CREATE INDEX ix_patch_version_string ON patch (version_string);

-- patch_chain table
CREATE TABLE patch_chain
(
    id                INTEGER PRIMARY KEY AUTOINCREMENT,
    repository_id     INTEGER NOT NULL,
    patch_id          INTEGER NOT NULL,
    previous_patch_id INTEGER,
    first_offered     TEXT,
    last_offered      TEXT,
    is_active         BOOLEAN NOT NULL DEFAULT false,
    FOREIGN KEY (repository_id) REFERENCES repository (id) ON DELETE CASCADE,
    FOREIGN KEY (patch_id) REFERENCES patch (id) ON DELETE CASCADE,
    FOREIGN KEY (previous_patch_id) REFERENCES patch (id)
);
CREATE INDEX ix_patch_chain_previous_patch_id ON patch_chain (previous_patch_id);
CREATE INDEX ix_patch_chain_repository_id ON patch_chain (repository_id);
CREATE UNIQUE INDEX ix_patch_chain_patch_id
    ON patch_chain (patch_id)
    WHERE previous_patch_id IS NULL;
CREATE UNIQUE INDEX ix_patch_chain_patch_id_previous_patch_id
    ON patch_chain (patch_id, previous_patch_id)
    WHERE previous_patch_id IS NOT NULL;

-- patch_base table
CREATE TABLE patch_base
(
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    repository_id    INTEGER NOT NULL,
    base_version     TEXT    NOT NULL,
    first_patch_id   INTEGER NOT NULL,
    previous_base_id INTEGER,
    delta_patch_id   INTEGER,
    is_active        BOOLEAN NOT NULL DEFAULT false,
    FOREIGN KEY (repository_id) REFERENCES repository (id) ON DELETE CASCADE,
    FOREIGN KEY (first_patch_id) REFERENCES patch (id) ON DELETE CASCADE,
    FOREIGN KEY (previous_base_id) REFERENCES patch_base (id) ON DELETE CASCADE,
    FOREIGN KEY (delta_patch_id) REFERENCES patch (id) ON DELETE CASCADE
);

-- game_version table
CREATE TABLE game_version
(
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    service_id     INTEGER NOT NULL,
    expansion_id   INTEGER NOT NULL,
    version_name   TEXT    NOT NULL,
    hotfix_level   INTEGER NOT NULL DEFAULT 0,
    marketing_name TEXT,
    patch_info_url TEXT,
    FOREIGN KEY (service_id) REFERENCES service (id) ON DELETE CASCADE,
    FOREIGN KEY (expansion_id) REFERENCES expansion (id) ON DELETE CASCADE
);
CREATE INDEX ix_game_version_hotfix_level ON game_version (hotfix_level);
CREATE INDEX ix_game_version_service_id ON game_version (service_id);
CREATE INDEX ix_game_version_expansion_id ON game_version (expansion_id);
CREATE INDEX ix_game_version_version_name ON game_version (version_name);

-- game_version_patch table
CREATE TABLE game_version_patch
(
    game_version_id INTEGER NOT NULL,
    patch_id        INTEGER NOT NULL,
    PRIMARY KEY (game_version_id, patch_id),
    FOREIGN KEY (game_version_id) REFERENCES game_version (id) ON DELETE CASCADE,
    FOREIGN KEY (patch_id) REFERENCES patch (id) ON DELETE CASCADE
);
CREATE INDEX ix_game_version_patch_game_version_id ON game_version_patch (game_version_id);
CREATE INDEX ix_game_version_patch_patch_id ON game_version_patch (patch_id);

-- expansion table
CREATE TABLE expansion
(
    id      INTEGER PRIMARY KEY,
    name_en TEXT NOT NULL,
    name_ja TEXT NOT NULL DEFAULT '',
    name_de TEXT NOT NULL DEFAULT '',
    name_fr TEXT NOT NULL DEFAULT '',
    name_ko TEXT NOT NULL DEFAULT '',
    name_cn TEXT NOT NULL DEFAULT '',
    name_tw TEXT NOT NULL DEFAULT ''
);

-- expansion_repository_mapping table
CREATE TABLE expansion_repository_mapping
(
    game_repository_id      INTEGER NOT NULL,
    expansion_id            INTEGER NOT NULL,
    expansion_repository_id INTEGER NOT NULL,
    PRIMARY KEY (game_repository_id, expansion_id, expansion_repository_id),
    FOREIGN KEY (game_repository_id) REFERENCES repository (id) ON DELETE CASCADE,
    FOREIGN KEY (expansion_id) REFERENCES expansion (id) ON DELETE CASCADE,
    FOREIGN KEY (expansion_repository_id) REFERENCES repository (id) ON DELETE CASCADE
);
CREATE INDEX ix_expansion_repository_mapping_expansion_repository_id
    ON expansion_repository_mapping (expansion_repository_id);
