using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class ConvertSchemaToSnakeCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRepositories_Accounts_ApplicableAccountsId",
                table: "AccountRepositories");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRepositories_Repositories_ApplicableRepositoriesId",
                table: "AccountRepositories");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpansionRepositoryMappings_Repositories_ExpansionRepositor~",
                table: "ExpansionRepositoryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpansionRepositoryMappings_Repositories_GameRepositoryId",
                table: "ExpansionRepositoryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Patches_Repositories_RepositoryId",
                table: "Patches");

            migrationBuilder.DropForeignKey(
                name: "FK_Patches_RepoVersions_RepoVersionId",
                table: "Patches");

            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_Services_ServiceId",
                table: "Repositories");

            migrationBuilder.DropForeignKey(
                name: "FK_RepoVersions_Repositories_RepositoryId",
                table: "RepoVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_UpgradePaths_Patches_PatchId",
                table: "UpgradePaths");

            migrationBuilder.DropForeignKey(
                name: "FK_UpgradePaths_Patches_PreviousPatchId",
                table: "UpgradePaths");

            migrationBuilder.DropForeignKey(
                name: "FK_UpgradePaths_Repositories_RepositoryId",
                table: "UpgradePaths");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_Files_FilesName_FilesSHA1",
                table: "VersionFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_RepoVersions_VersionsId",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Repositories",
                table: "Repositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patches",
                table: "Patches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UpgradePaths",
                table: "UpgradePaths");

            migrationBuilder.DropIndex(
                name: "IX_UpgradePaths_PatchId",
                table: "UpgradePaths");

            migrationBuilder.DropIndex(
                name: "IX_UpgradePaths_PatchId_PreviousPatchId",
                table: "UpgradePaths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepoVersions",
                table: "RepoVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpansionRepositoryMappings",
                table: "ExpansionRepositoryMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscordHooks",
                table: "DiscordHooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRepositories",
                table: "AccountRepositories");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "services");

            migrationBuilder.RenameTable(
                name: "Repositories",
                newName: "repositories");

            migrationBuilder.RenameTable(
                name: "Patches",
                newName: "patches");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "files");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "accounts");

            migrationBuilder.RenameTable(
                name: "VersionFiles",
                newName: "version_files");

            migrationBuilder.RenameTable(
                name: "UpgradePaths",
                newName: "upgrade_paths");

            migrationBuilder.RenameTable(
                name: "RepoVersions",
                newName: "repo_versions");

            migrationBuilder.RenameTable(
                name: "ExpansionRepositoryMappings",
                newName: "expansion_repository_mappings");

            migrationBuilder.RenameTable(
                name: "DiscordHooks",
                newName: "discord_hooks");

            migrationBuilder.RenameTable(
                name: "AccountRepositories",
                newName: "account_repositories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "services",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "services",
                newName: "icon");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "services",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "repositories",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "repositories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "repositories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "repositories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "repositories",
                newName: "service_id");

            migrationBuilder.RenameIndex(
                name: "IX_Repositories_Slug",
                table: "repositories",
                newName: "ix_repositories_slug");

            migrationBuilder.RenameIndex(
                name: "IX_Repositories_ServiceId",
                table: "repositories",
                newName: "ix_repositories_service_id");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "patches",
                newName: "size");

            migrationBuilder.RenameColumn(
                name: "Hashes",
                table: "patches",
                newName: "hashes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "patches",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RepositoryId",
                table: "patches",
                newName: "repository_id");

            migrationBuilder.RenameColumn(
                name: "RepoVersionId",
                table: "patches",
                newName: "repo_version_id");

            migrationBuilder.RenameColumn(
                name: "RemoteOriginPath",
                table: "patches",
                newName: "remote_origin_path");

            migrationBuilder.RenameColumn(
                name: "LocalStoragePath",
                table: "patches",
                newName: "local_storage_path");

            migrationBuilder.RenameColumn(
                name: "LastSeen",
                table: "patches",
                newName: "last_seen");

            migrationBuilder.RenameColumn(
                name: "LastOffered",
                table: "patches",
                newName: "last_offered");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "patches",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "HashType",
                table: "patches",
                newName: "hash_type");

            migrationBuilder.RenameColumn(
                name: "HashBlockSize",
                table: "patches",
                newName: "hash_block_size");

            migrationBuilder.RenameColumn(
                name: "FirstSeen",
                table: "patches",
                newName: "first_seen");

            migrationBuilder.RenameColumn(
                name: "FirstOffered",
                table: "patches",
                newName: "first_offered");

            migrationBuilder.RenameIndex(
                name: "IX_Patches_RepoVersionId",
                table: "patches",
                newName: "ix_patches_repo_version_id");

            migrationBuilder.RenameIndex(
                name: "IX_Patches_RepositoryId",
                table: "patches",
                newName: "ix_patches_repository_id");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "files",
                newName: "size");

            migrationBuilder.RenameColumn(
                name: "SHA1",
                table: "files",
                newName: "sha1");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "files",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LastUsed",
                table: "files",
                newName: "last_used");

            migrationBuilder.RenameIndex(
                name: "IX_Files_LastUsed",
                table: "files",
                newName: "ix_files_last_used");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "accounts",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "accounts",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "accounts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "FilesSHA1",
                table: "version_files",
                newName: "files_sha1");

            migrationBuilder.RenameColumn(
                name: "FilesName",
                table: "version_files",
                newName: "files_name");

            migrationBuilder.RenameColumn(
                name: "VersionsId",
                table: "version_files",
                newName: "versions_id");

            migrationBuilder.RenameIndex(
                name: "IX_VersionFiles_FilesName_FilesSHA1",
                table: "version_files",
                newName: "ix_version_files_files_name_files_sha1");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "upgrade_paths",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RepositoryId",
                table: "upgrade_paths",
                newName: "repository_id");

            migrationBuilder.RenameColumn(
                name: "PreviousPatchId",
                table: "upgrade_paths",
                newName: "previous_patch_id");

            migrationBuilder.RenameColumn(
                name: "PatchId",
                table: "upgrade_paths",
                newName: "patch_id");

            migrationBuilder.RenameColumn(
                name: "LastOffered",
                table: "upgrade_paths",
                newName: "last_offered");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "upgrade_paths",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FirstOffered",
                table: "upgrade_paths",
                newName: "first_offered");

            migrationBuilder.RenameIndex(
                name: "IX_UpgradePaths_RepositoryId",
                table: "upgrade_paths",
                newName: "ix_upgrade_paths_repository_id");

            migrationBuilder.RenameIndex(
                name: "IX_UpgradePaths_PreviousPatchId",
                table: "upgrade_paths",
                newName: "ix_upgrade_paths_previous_patch_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "repo_versions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VersionString",
                table: "repo_versions",
                newName: "version_string");

            migrationBuilder.RenameColumn(
                name: "RepositoryId",
                table: "repo_versions",
                newName: "repository_id");

            migrationBuilder.RenameIndex(
                name: "IX_RepoVersions_VersionString",
                table: "repo_versions",
                newName: "ix_repo_versions_version_string");

            migrationBuilder.RenameIndex(
                name: "IX_RepoVersions_RepositoryId",
                table: "repo_versions",
                newName: "ix_repo_versions_repository_id");

            migrationBuilder.RenameColumn(
                name: "ExpansionRepositoryId",
                table: "expansion_repository_mappings",
                newName: "expansion_repository_id");

            migrationBuilder.RenameColumn(
                name: "ExpansionId",
                table: "expansion_repository_mappings",
                newName: "expansion_id");

            migrationBuilder.RenameColumn(
                name: "GameRepositoryId",
                table: "expansion_repository_mappings",
                newName: "game_repository_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExpansionRepositoryMappings_ExpansionRepositoryId",
                table: "expansion_repository_mappings",
                newName: "ix_expansion_repository_mappings_expansion_repository_id");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "discord_hooks",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "discord_hooks",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "discord_hooks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ApplicableRepositoriesId",
                table: "account_repositories",
                newName: "applicable_repositories_id");

            migrationBuilder.RenameColumn(
                name: "ApplicableAccountsId",
                table: "account_repositories",
                newName: "applicable_accounts_id");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRepositories_ApplicableRepositoriesId",
                table: "account_repositories",
                newName: "ix_account_repositories_applicable_repositories_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_services",
                table: "services",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_repositories",
                table: "repositories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_patches",
                table: "patches",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_files",
                table: "files",
                columns: new[] { "name", "sha1" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_accounts",
                table: "accounts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_version_files",
                table: "version_files",
                columns: new[] { "versions_id", "files_name", "files_sha1" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_upgrade_paths",
                table: "upgrade_paths",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_repo_versions",
                table: "repo_versions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_expansion_repository_mappings",
                table: "expansion_repository_mappings",
                columns: new[] { "game_repository_id", "expansion_id", "expansion_repository_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_discord_hooks",
                table: "discord_hooks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_account_repositories",
                table: "account_repositories",
                columns: new[] { "applicable_accounts_id", "applicable_repositories_id" });

            migrationBuilder.CreateIndex(
                name: "ix_upgrade_paths_patch_id",
                table: "upgrade_paths",
                column: "patch_id",
                unique: true,
                filter: "\"previous_patch_id\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_upgrade_paths_patch_id_previous_patch_id",
                table: "upgrade_paths",
                columns: new[] { "patch_id", "previous_patch_id" },
                unique: true,
                filter: "\"previous_patch_id\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_account_repositories_accounts_applicable_accounts_id",
                table: "account_repositories",
                column: "applicable_accounts_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_account_repositories_repositories_applicable_repositories_id",
                table: "account_repositories",
                column: "applicable_repositories_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_expansion_repository_mappings_repositories_expansion_reposi",
                table: "expansion_repository_mappings",
                column: "expansion_repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_expansion_repository_mappings_repositories_game_repository_",
                table: "expansion_repository_mappings",
                column: "game_repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_patches_repo_versions_repo_version_id",
                table: "patches",
                column: "repo_version_id",
                principalTable: "repo_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_patches_repositories_repository_id",
                table: "patches",
                column: "repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_repo_versions_repositories_repository_id",
                table: "repo_versions",
                column: "repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_repositories_services_service_id",
                table: "repositories",
                column: "service_id",
                principalTable: "services",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_upgrade_paths_patches_patch_id",
                table: "upgrade_paths",
                column: "patch_id",
                principalTable: "patches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_upgrade_paths_patches_previous_patch_id",
                table: "upgrade_paths",
                column: "previous_patch_id",
                principalTable: "patches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_upgrade_paths_repositories_repository_id",
                table: "upgrade_paths",
                column: "repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_version_files_files_files_temp_id",
                table: "version_files",
                columns: new[] { "files_name", "files_sha1" },
                principalTable: "files",
                principalColumns: new[] { "name", "sha1" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_version_files_repo_versions_versions_id",
                table: "version_files",
                column: "versions_id",
                principalTable: "repo_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_repositories_accounts_applicable_accounts_id",
                table: "account_repositories");

            migrationBuilder.DropForeignKey(
                name: "fk_account_repositories_repositories_applicable_repositories_id",
                table: "account_repositories");

            migrationBuilder.DropForeignKey(
                name: "fk_expansion_repository_mappings_repositories_expansion_reposi",
                table: "expansion_repository_mappings");

            migrationBuilder.DropForeignKey(
                name: "fk_expansion_repository_mappings_repositories_game_repository_",
                table: "expansion_repository_mappings");

            migrationBuilder.DropForeignKey(
                name: "fk_patches_repo_versions_repo_version_id",
                table: "patches");

            migrationBuilder.DropForeignKey(
                name: "fk_patches_repositories_repository_id",
                table: "patches");

            migrationBuilder.DropForeignKey(
                name: "fk_repo_versions_repositories_repository_id",
                table: "repo_versions");

            migrationBuilder.DropForeignKey(
                name: "fk_repositories_services_service_id",
                table: "repositories");

            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_patches_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_patches_previous_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_repositories_repository_id",
                table: "upgrade_paths");

            migrationBuilder.DropForeignKey(
                name: "fk_version_files_files_files_temp_id",
                table: "version_files");

            migrationBuilder.DropForeignKey(
                name: "fk_version_files_repo_versions_versions_id",
                table: "version_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_services",
                table: "services");

            migrationBuilder.DropPrimaryKey(
                name: "pk_repositories",
                table: "repositories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_patches",
                table: "patches");

            migrationBuilder.DropPrimaryKey(
                name: "pk_files",
                table: "files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_accounts",
                table: "accounts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_version_files",
                table: "version_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_upgrade_paths",
                table: "upgrade_paths");

            migrationBuilder.DropIndex(
                name: "ix_upgrade_paths_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropIndex(
                name: "ix_upgrade_paths_patch_id_previous_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropPrimaryKey(
                name: "pk_repo_versions",
                table: "repo_versions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_expansion_repository_mappings",
                table: "expansion_repository_mappings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_discord_hooks",
                table: "discord_hooks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_account_repositories",
                table: "account_repositories");

            migrationBuilder.RenameTable(
                name: "services",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "repositories",
                newName: "Repositories");

            migrationBuilder.RenameTable(
                name: "patches",
                newName: "Patches");

            migrationBuilder.RenameTable(
                name: "files",
                newName: "Files");

            migrationBuilder.RenameTable(
                name: "accounts",
                newName: "Accounts");

            migrationBuilder.RenameTable(
                name: "version_files",
                newName: "VersionFiles");

            migrationBuilder.RenameTable(
                name: "upgrade_paths",
                newName: "UpgradePaths");

            migrationBuilder.RenameTable(
                name: "repo_versions",
                newName: "RepoVersions");

            migrationBuilder.RenameTable(
                name: "expansion_repository_mappings",
                newName: "ExpansionRepositoryMappings");

            migrationBuilder.RenameTable(
                name: "discord_hooks",
                newName: "DiscordHooks");

            migrationBuilder.RenameTable(
                name: "account_repositories",
                newName: "AccountRepositories");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Services",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "icon",
                table: "Services",
                newName: "Icon");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Repositories",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Repositories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Repositories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Repositories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "service_id",
                table: "Repositories",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "ix_repositories_slug",
                table: "Repositories",
                newName: "IX_Repositories_Slug");

            migrationBuilder.RenameIndex(
                name: "ix_repositories_service_id",
                table: "Repositories",
                newName: "IX_Repositories_ServiceId");

            migrationBuilder.RenameColumn(
                name: "size",
                table: "Patches",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "hashes",
                table: "Patches",
                newName: "Hashes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Patches",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "repository_id",
                table: "Patches",
                newName: "RepositoryId");

            migrationBuilder.RenameColumn(
                name: "repo_version_id",
                table: "Patches",
                newName: "RepoVersionId");

            migrationBuilder.RenameColumn(
                name: "remote_origin_path",
                table: "Patches",
                newName: "RemoteOriginPath");

            migrationBuilder.RenameColumn(
                name: "local_storage_path",
                table: "Patches",
                newName: "LocalStoragePath");

            migrationBuilder.RenameColumn(
                name: "last_seen",
                table: "Patches",
                newName: "LastSeen");

            migrationBuilder.RenameColumn(
                name: "last_offered",
                table: "Patches",
                newName: "LastOffered");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Patches",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "hash_type",
                table: "Patches",
                newName: "HashType");

            migrationBuilder.RenameColumn(
                name: "hash_block_size",
                table: "Patches",
                newName: "HashBlockSize");

            migrationBuilder.RenameColumn(
                name: "first_seen",
                table: "Patches",
                newName: "FirstSeen");

            migrationBuilder.RenameColumn(
                name: "first_offered",
                table: "Patches",
                newName: "FirstOffered");

            migrationBuilder.RenameIndex(
                name: "ix_patches_repository_id",
                table: "Patches",
                newName: "IX_Patches_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "ix_patches_repo_version_id",
                table: "Patches",
                newName: "IX_Patches_RepoVersionId");

            migrationBuilder.RenameColumn(
                name: "size",
                table: "Files",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "sha1",
                table: "Files",
                newName: "SHA1");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Files",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "last_used",
                table: "Files",
                newName: "LastUsed");

            migrationBuilder.RenameIndex(
                name: "ix_files_last_used",
                table: "Files",
                newName: "IX_Files_LastUsed");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Accounts",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Accounts",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Accounts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "files_sha1",
                table: "VersionFiles",
                newName: "FilesSHA1");

            migrationBuilder.RenameColumn(
                name: "files_name",
                table: "VersionFiles",
                newName: "FilesName");

            migrationBuilder.RenameColumn(
                name: "versions_id",
                table: "VersionFiles",
                newName: "VersionsId");

            migrationBuilder.RenameIndex(
                name: "ix_version_files_files_name_files_sha1",
                table: "VersionFiles",
                newName: "IX_VersionFiles_FilesName_FilesSHA1");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UpgradePaths",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "repository_id",
                table: "UpgradePaths",
                newName: "RepositoryId");

            migrationBuilder.RenameColumn(
                name: "previous_patch_id",
                table: "UpgradePaths",
                newName: "PreviousPatchId");

            migrationBuilder.RenameColumn(
                name: "patch_id",
                table: "UpgradePaths",
                newName: "PatchId");

            migrationBuilder.RenameColumn(
                name: "last_offered",
                table: "UpgradePaths",
                newName: "LastOffered");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "UpgradePaths",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "first_offered",
                table: "UpgradePaths",
                newName: "FirstOffered");

            migrationBuilder.RenameIndex(
                name: "ix_upgrade_paths_repository_id",
                table: "UpgradePaths",
                newName: "IX_UpgradePaths_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "ix_upgrade_paths_previous_patch_id",
                table: "UpgradePaths",
                newName: "IX_UpgradePaths_PreviousPatchId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RepoVersions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "version_string",
                table: "RepoVersions",
                newName: "VersionString");

            migrationBuilder.RenameColumn(
                name: "repository_id",
                table: "RepoVersions",
                newName: "RepositoryId");

            migrationBuilder.RenameIndex(
                name: "ix_repo_versions_version_string",
                table: "RepoVersions",
                newName: "IX_RepoVersions_VersionString");

            migrationBuilder.RenameIndex(
                name: "ix_repo_versions_repository_id",
                table: "RepoVersions",
                newName: "IX_RepoVersions_RepositoryId");

            migrationBuilder.RenameColumn(
                name: "expansion_repository_id",
                table: "ExpansionRepositoryMappings",
                newName: "ExpansionRepositoryId");

            migrationBuilder.RenameColumn(
                name: "expansion_id",
                table: "ExpansionRepositoryMappings",
                newName: "ExpansionId");

            migrationBuilder.RenameColumn(
                name: "game_repository_id",
                table: "ExpansionRepositoryMappings",
                newName: "GameRepositoryId");

            migrationBuilder.RenameIndex(
                name: "ix_expansion_repository_mappings_expansion_repository_id",
                table: "ExpansionRepositoryMappings",
                newName: "IX_ExpansionRepositoryMappings_ExpansionRepositoryId");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "DiscordHooks",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "DiscordHooks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DiscordHooks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "applicable_repositories_id",
                table: "AccountRepositories",
                newName: "ApplicableRepositoriesId");

            migrationBuilder.RenameColumn(
                name: "applicable_accounts_id",
                table: "AccountRepositories",
                newName: "ApplicableAccountsId");

            migrationBuilder.RenameIndex(
                name: "ix_account_repositories_applicable_repositories_id",
                table: "AccountRepositories",
                newName: "IX_AccountRepositories_ApplicableRepositoriesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repositories",
                table: "Repositories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patches",
                table: "Patches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                columns: new[] { "Name", "SHA1" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles",
                columns: new[] { "VersionsId", "FilesName", "FilesSHA1" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UpgradePaths",
                table: "UpgradePaths",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepoVersions",
                table: "RepoVersions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpansionRepositoryMappings",
                table: "ExpansionRepositoryMappings",
                columns: new[] { "GameRepositoryId", "ExpansionId", "ExpansionRepositoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscordHooks",
                table: "DiscordHooks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRepositories",
                table: "AccountRepositories",
                columns: new[] { "ApplicableAccountsId", "ApplicableRepositoriesId" });

            migrationBuilder.CreateIndex(
                name: "IX_UpgradePaths_PatchId",
                table: "UpgradePaths",
                column: "PatchId",
                unique: true,
                filter: "\"PreviousPatchId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UpgradePaths_PatchId_PreviousPatchId",
                table: "UpgradePaths",
                columns: new[] { "PatchId", "PreviousPatchId" },
                unique: true,
                filter: "\"PreviousPatchId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRepositories_Accounts_ApplicableAccountsId",
                table: "AccountRepositories",
                column: "ApplicableAccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRepositories_Repositories_ApplicableRepositoriesId",
                table: "AccountRepositories",
                column: "ApplicableRepositoriesId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpansionRepositoryMappings_Repositories_ExpansionRepositor~",
                table: "ExpansionRepositoryMappings",
                column: "ExpansionRepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpansionRepositoryMappings_Repositories_GameRepositoryId",
                table: "ExpansionRepositoryMappings",
                column: "GameRepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patches_Repositories_RepositoryId",
                table: "Patches",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patches_RepoVersions_RepoVersionId",
                table: "Patches",
                column: "RepoVersionId",
                principalTable: "RepoVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_Services_ServiceId",
                table: "Repositories",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepoVersions_Repositories_RepositoryId",
                table: "RepoVersions",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UpgradePaths_Patches_PatchId",
                table: "UpgradePaths",
                column: "PatchId",
                principalTable: "Patches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UpgradePaths_Patches_PreviousPatchId",
                table: "UpgradePaths",
                column: "PreviousPatchId",
                principalTable: "Patches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UpgradePaths_Repositories_RepositoryId",
                table: "UpgradePaths",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionFiles_Files_FilesName_FilesSHA1",
                table: "VersionFiles",
                columns: new[] { "FilesName", "FilesSHA1" },
                principalTable: "Files",
                principalColumns: new[] { "Name", "SHA1" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionFiles_RepoVersions_VersionsId",
                table: "VersionFiles",
                column: "VersionsId",
                principalTable: "RepoVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
