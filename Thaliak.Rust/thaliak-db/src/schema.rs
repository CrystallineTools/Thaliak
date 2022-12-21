// @generated automatically by Diesel CLI.

diesel::table! {
    account_repositories (applicable_accounts_id, applicable_repositories_id) {
        applicable_accounts_id -> Int4,
        applicable_repositories_id -> Int4,
    }
}

diesel::table! {
    accounts (id) {
        id -> Int4,
        username -> Text,
        password -> Text,
    }
}

diesel::table! {
    discord_hooks (id) {
        id -> Int4,
        url -> Text,
        name -> Nullable<Text>,
    }
}

diesel::table! {
    expansion_repository_mappings (game_repository_id, expansion_id, expansion_repository_id) {
        game_repository_id -> Int4,
        expansion_id -> Int4,
        expansion_repository_id -> Int4,
    }
}

diesel::table! {
    files (name, sha1) {
        name -> Text,
        sha1 -> Bpchar,
        size -> Numeric,
        last_used -> Timestamptz,
    }
}

diesel::table! {
    patch_chains (id) {
        repository_id -> Int4,
        patch_id -> Int4,
        previous_patch_id -> Nullable<Int4>,
        first_offered -> Nullable<Timestamptz>,
        last_offered -> Nullable<Timestamptz>,
        id -> Int4,
        is_active -> Bool,
    }
}

diesel::table! {
    patches (id) {
        id -> Int4,
        version_id -> Int4,
        repository_id -> Int4,
        remote_origin_path -> Text,
        first_seen -> Nullable<Timestamptz>,
        last_seen -> Nullable<Timestamptz>,
        size -> Int8,
        hash_type -> Nullable<Text>,
        hash_block_size -> Nullable<Int8>,
        hashes -> Nullable<Text>,
        first_offered -> Nullable<Timestamptz>,
        last_offered -> Nullable<Timestamptz>,
        local_storage_path -> Text,
        is_active -> Bool,
    }
}

diesel::table! {
    repositories (id) {
        id -> Int4,
        name -> Text,
        description -> Nullable<Text>,
        slug -> Text,
        service_region_id -> Int4,
    }
}

diesel::table! {
    service_regions (id) {
        id -> Int4,
        name -> Text,
        icon -> Text,
    }
}

diesel::table! {
    version_files (versions_id, files_name, files_sha1) {
        versions_id -> Int4,
        files_name -> Text,
        files_sha1 -> Bpchar,
    }
}

diesel::table! {
    versions (id) {
        id -> Int4,
        version_id -> Numeric,
        version_string -> Text,
        repository_id -> Int4,
    }
}

diesel::joinable!(account_repositories -> accounts (applicable_accounts_id));
diesel::joinable!(account_repositories -> repositories (applicable_repositories_id));
diesel::joinable!(patch_chains -> repositories (repository_id));
diesel::joinable!(patches -> repositories (repository_id));
diesel::joinable!(patches -> versions (version_id));
diesel::joinable!(repositories -> service_regions (service_region_id));
diesel::joinable!(version_files -> versions (versions_id));
diesel::joinable!(versions -> repositories (repository_id));

diesel::allow_tables_to_appear_in_same_query!(
    account_repositories,
    accounts,
    discord_hooks,
    expansion_repository_mappings,
    files,
    patch_chains,
    patches,
    repositories,
    service_regions,
    version_files,
    versions,
);
