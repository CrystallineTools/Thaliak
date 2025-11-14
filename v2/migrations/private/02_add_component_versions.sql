-- component_versions table to track versions of all microservices
CREATE TABLE component_versions
(
    component    TEXT PRIMARY KEY,
    commit_hash  TEXT     NOT NULL,
    updated_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
