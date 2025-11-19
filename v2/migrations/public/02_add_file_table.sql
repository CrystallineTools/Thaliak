-- file table
CREATE TABLE file
(
    patch_id INTEGER NOT NULL,
    name     TEXT    NOT NULL,
    sha1     TEXT    NOT NULL,
    sha256   TEXT    NOT NULL,
    md5      TEXT    NOT NULL,
    PRIMARY KEY (patch_id, name),
    FOREIGN KEY (patch_id) REFERENCES patch (id)
);
CREATE INDEX idx_file_sha1 ON file (sha1);
CREATE INDEX idx_file_sha256 ON file (sha256);
CREATE INDEX idx_file_md5 ON file (md5);
