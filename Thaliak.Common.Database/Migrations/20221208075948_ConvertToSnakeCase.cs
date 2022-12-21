using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class ConvertToSnakeCase : Migration
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
                name: "FK_PatchChains_Patches_PatchId",
                table: "PatchChains");

            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains");

            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Repositories_RepositoryId",
                table: "PatchChains");

            migrationBuilder.DropForeignKey(
                name: "FK_Patches_Repositories_RepositoryId",
                table: "Patches");

            migrationBuilder.DropForeignKey(
                name: "FK_Patches_Versions_VersionId",
                table: "Patches");

            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_ServiceRegions_ServiceRegionId",
                table: "Repositories");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_Files_FilesName_FilesSHA1",
                table: "VersionFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_Versions_VersionsId",
                table: "VersionFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Repositories_RepositoryId",
                table: "Versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Versions",
                table: "Versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles");

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
                name: "PK_AccountRepositories",
                table: "AccountRepositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceRegions",
                table: "ServiceRegions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpansionRepositoryMappings",
                table: "ExpansionRepositoryMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscordHooks",
                table: "DiscordHooks");

            migrationBuilder.RenameTable(
                name: "Versions",
                newName: "versions");

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
                name: "ServiceRegions",
                newName: "service_regions");

            migrationBuilder.RenameTable(
                name: "PatchChains",
                newName: "patch_chains");

            migrationBuilder.RenameTable(
                name: "ExpansionRepositoryMappings",
                newName: "expansion_repository_mappings");

            migrationBuilder.RenameTable(
                name: "DiscordHooks",
                newName: "discord_hooks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "versions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VersionString",
                table: "versions",
                newName: "version_string");

            migrationBuilder.RenameColumn(
                name: "VersionId",
                table: "versions",
                newName: "version_id");

            migrationBuilder.RenameColumn(
                name: "RepositoryId",
                table: "versions",
                newName: "repository_id");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_VersionString",
                table: "versions",
                newName: "ix_versions_version_string");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_VersionId",
                table: "versions",
                newName: "ix_versions_version_id");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_RepositoryId",
                table: "versions",
                newName: "ix_versions_repository_id");

            migrationBuilder.RenameColumn(
                name: "FilesSHA1",
                table: "VersionFiles",
                newName: "files_sha1");

            migrationBuilder.RenameColumn(
                name: "FilesName",
                table: "VersionFiles",
                newName: "files_name");

            migrationBuilder.RenameColumn(
                name: "VersionsId",
                table: "VersionFiles",
                newName: "versions_id");

            migrationBuilder.RenameIndex(
                name: "IX_VersionFiles_FilesName_FilesSHA1",
                table: "VersionFiles",
                newName: "ix_version_files_files_name_files_sha1");

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
                name: "ServiceRegionId",
                table: "repositories",
                newName: "service_region_id");

            migrationBuilder.RenameIndex(
                name: "IX_Repositories_Slug",
                table: "repositories",
                newName: "ix_repositories_slug");

            migrationBuilder.RenameIndex(
                name: "IX_Repositories_ServiceRegionId",
                table: "repositories",
                newName: "ix_repositories_service_region_id");

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
                name: "VersionId",
                table: "patches",
                newName: "version_id");

            migrationBuilder.RenameColumn(
                name: "RepositoryId",
                table: "patches",
                newName: "repository_id");

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
                name: "IX_Patches_VersionId",
                table: "patches",
                newName: "ix_patches_version_id");

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
                name: "ApplicableRepositoriesId",
                table: "AccountRepositories",
                newName: "applicable_repositories_id");

            migrationBuilder.RenameColumn(
                name: "ApplicableAccountsId",
                table: "AccountRepositories",
                newName: "applicable_accounts_id");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRepositories_ApplicableRepositoriesId",
                table: "AccountRepositories",
                newName: "ix_account_repositories_applicable_repositories_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "service_regions",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "service_regions",
                newName: "icon");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "service_regions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "patch_chains",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RepositoryId",
                table: "patch_chains",
                newName: "repository_id");

            migrationBuilder.RenameColumn(
                name: "PreviousPatchId",
                table: "patch_chains",
                newName: "previous_patch_id");

            migrationBuilder.RenameColumn(
                name: "PatchId",
                table: "patch_chains",
                newName: "patch_id");

            migrationBuilder.RenameColumn(
                name: "LastOffered",
                table: "patch_chains",
                newName: "last_offered");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "patch_chains",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FirstOffered",
                table: "patch_chains",
                newName: "first_offered");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_RepositoryId",
                table: "patch_chains",
                newName: "ix_patch_chains_repository_id");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_PreviousPatchId",
                table: "patch_chains",
                newName: "ix_patch_chains_previous_patch_id");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_PatchId_PreviousPatchId",
                table: "patch_chains",
                newName: "ix_patch_chains_patch_id_previous_patch_id");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_PatchId",
                table: "patch_chains",
                newName: "ix_patch_chains_patch_id");

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

            migrationBuilder.AddPrimaryKey(
                name: "pk_versions",
                table: "versions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_version_files",
                table: "VersionFiles",
                columns: new[] { "versions_id", "files_name", "files_sha1" });

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
                name: "pk_account_repositories",
                table: "AccountRepositories",
                columns: new[] { "applicable_accounts_id", "applicable_repositories_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_service_regions",
                table: "service_regions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_patch_chains",
                table: "patch_chains",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_expansion_repository_mappings",
                table: "expansion_repository_mappings",
                columns: new[] { "game_repository_id", "expansion_id", "expansion_repository_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_discord_hooks",
                table: "discord_hooks",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_account_repositories_accounts_applicable_accounts_id",
                table: "AccountRepositories",
                column: "applicable_accounts_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_account_repositories_repositories_applicable_repositories_id",
                table: "AccountRepositories",
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
                name: "fk_patch_chains_patches_patch_id",
                table: "patch_chains",
                column: "patch_id",
                principalTable: "patches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_patch_chains_patches_previous_patch_id",
                table: "patch_chains",
                column: "previous_patch_id",
                principalTable: "patches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_patch_chains_repositories_repository_id",
                table: "patch_chains",
                column: "repository_id",
                principalTable: "repositories",
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
                name: "fk_patches_versions_version_id",
                table: "patches",
                column: "version_id",
                principalTable: "versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_repositories_service_regions_service_region_id",
                table: "repositories",
                column: "service_region_id",
                principalTable: "service_regions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_version_files_files_files_temp_id",
                table: "VersionFiles",
                columns: new[] { "files_name", "files_sha1" },
                principalTable: "files",
                principalColumns: new[] { "name", "sha1" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_version_files_versions_versions_id",
                table: "VersionFiles",
                column: "versions_id",
                principalTable: "versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_versions_repositories_repository_id",
                table: "versions",
                column: "repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_account_repositories_accounts_applicable_accounts_id",
                table: "AccountRepositories");

            migrationBuilder.DropForeignKey(
                name: "fk_account_repositories_repositories_applicable_repositories_id",
                table: "AccountRepositories");

            migrationBuilder.DropForeignKey(
                name: "fk_expansion_repository_mappings_repositories_expansion_reposi",
                table: "expansion_repository_mappings");

            migrationBuilder.DropForeignKey(
                name: "fk_expansion_repository_mappings_repositories_game_repository_",
                table: "expansion_repository_mappings");

            migrationBuilder.DropForeignKey(
                name: "fk_patch_chains_patches_patch_id",
                table: "patch_chains");

            migrationBuilder.DropForeignKey(
                name: "fk_patch_chains_patches_previous_patch_id",
                table: "patch_chains");

            migrationBuilder.DropForeignKey(
                name: "fk_patch_chains_repositories_repository_id",
                table: "patch_chains");

            migrationBuilder.DropForeignKey(
                name: "fk_patches_repositories_repository_id",
                table: "patches");

            migrationBuilder.DropForeignKey(
                name: "fk_patches_versions_version_id",
                table: "patches");

            migrationBuilder.DropForeignKey(
                name: "fk_repositories_service_regions_service_region_id",
                table: "repositories");

            migrationBuilder.DropForeignKey(
                name: "fk_version_files_files_files_temp_id",
                table: "VersionFiles");

            migrationBuilder.DropForeignKey(
                name: "fk_version_files_versions_versions_id",
                table: "VersionFiles");

            migrationBuilder.DropForeignKey(
                name: "fk_versions_repositories_repository_id",
                table: "versions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_versions",
                table: "versions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_version_files",
                table: "VersionFiles");

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
                name: "pk_account_repositories",
                table: "AccountRepositories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_service_regions",
                table: "service_regions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_patch_chains",
                table: "patch_chains");

            migrationBuilder.DropPrimaryKey(
                name: "pk_expansion_repository_mappings",
                table: "expansion_repository_mappings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_discord_hooks",
                table: "discord_hooks");

            migrationBuilder.RenameTable(
                name: "versions",
                newName: "Versions");

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
                name: "service_regions",
                newName: "ServiceRegions");

            migrationBuilder.RenameTable(
                name: "patch_chains",
                newName: "PatchChains");

            migrationBuilder.RenameTable(
                name: "expansion_repository_mappings",
                newName: "ExpansionRepositoryMappings");

            migrationBuilder.RenameTable(
                name: "discord_hooks",
                newName: "DiscordHooks");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Versions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "version_string",
                table: "Versions",
                newName: "VersionString");

            migrationBuilder.RenameColumn(
                name: "version_id",
                table: "Versions",
                newName: "VersionId");

            migrationBuilder.RenameColumn(
                name: "repository_id",
                table: "Versions",
                newName: "RepositoryId");

            migrationBuilder.RenameIndex(
                name: "ix_versions_version_string",
                table: "Versions",
                newName: "IX_Versions_VersionString");

            migrationBuilder.RenameIndex(
                name: "ix_versions_version_id",
                table: "Versions",
                newName: "IX_Versions_VersionId");

            migrationBuilder.RenameIndex(
                name: "ix_versions_repository_id",
                table: "Versions",
                newName: "IX_Versions_RepositoryId");

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
                name: "service_region_id",
                table: "Repositories",
                newName: "ServiceRegionId");

            migrationBuilder.RenameIndex(
                name: "ix_repositories_slug",
                table: "Repositories",
                newName: "IX_Repositories_Slug");

            migrationBuilder.RenameIndex(
                name: "ix_repositories_service_region_id",
                table: "Repositories",
                newName: "IX_Repositories_ServiceRegionId");

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
                name: "version_id",
                table: "Patches",
                newName: "VersionId");

            migrationBuilder.RenameColumn(
                name: "repository_id",
                table: "Patches",
                newName: "RepositoryId");

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
                name: "ix_patches_version_id",
                table: "Patches",
                newName: "IX_Patches_VersionId");

            migrationBuilder.RenameIndex(
                name: "ix_patches_repository_id",
                table: "Patches",
                newName: "IX_Patches_RepositoryId");

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

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ServiceRegions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "icon",
                table: "ServiceRegions",
                newName: "Icon");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ServiceRegions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PatchChains",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "repository_id",
                table: "PatchChains",
                newName: "RepositoryId");

            migrationBuilder.RenameColumn(
                name: "previous_patch_id",
                table: "PatchChains",
                newName: "PreviousPatchId");

            migrationBuilder.RenameColumn(
                name: "patch_id",
                table: "PatchChains",
                newName: "PatchId");

            migrationBuilder.RenameColumn(
                name: "last_offered",
                table: "PatchChains",
                newName: "LastOffered");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "PatchChains",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "first_offered",
                table: "PatchChains",
                newName: "FirstOffered");

            migrationBuilder.RenameIndex(
                name: "ix_patch_chains_repository_id",
                table: "PatchChains",
                newName: "IX_PatchChains_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "ix_patch_chains_previous_patch_id",
                table: "PatchChains",
                newName: "IX_PatchChains_PreviousPatchId");

            migrationBuilder.RenameIndex(
                name: "ix_patch_chains_patch_id_previous_patch_id",
                table: "PatchChains",
                newName: "IX_PatchChains_PatchId_PreviousPatchId");

            migrationBuilder.RenameIndex(
                name: "ix_patch_chains_patch_id",
                table: "PatchChains",
                newName: "IX_PatchChains_PatchId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Versions",
                table: "Versions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles",
                columns: new[] { "VersionsId", "FilesName", "FilesSHA1" });

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
                name: "PK_AccountRepositories",
                table: "AccountRepositories",
                columns: new[] { "ApplicableAccountsId", "ApplicableRepositoriesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceRegions",
                table: "ServiceRegions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpansionRepositoryMappings",
                table: "ExpansionRepositoryMappings",
                columns: new[] { "GameRepositoryId", "ExpansionId", "ExpansionRepositoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscordHooks",
                table: "DiscordHooks",
                column: "Id");

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
                name: "FK_PatchChains_Patches_PatchId",
                table: "PatchChains",
                column: "PatchId",
                principalTable: "Patches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains",
                column: "PreviousPatchId",
                principalTable: "Patches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Repositories_RepositoryId",
                table: "PatchChains",
                column: "RepositoryId",
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
                name: "FK_Patches_Versions_VersionId",
                table: "Patches",
                column: "VersionId",
                principalTable: "Versions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_ServiceRegions_ServiceRegionId",
                table: "Repositories",
                column: "ServiceRegionId",
                principalTable: "ServiceRegions",
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
                name: "FK_VersionFiles_Versions_VersionsId",
                table: "VersionFiles",
                column: "VersionsId",
                principalTable: "Versions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Repositories_RepositoryId",
                table: "Versions",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
