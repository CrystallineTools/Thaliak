using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RenameUpgradePathsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_patches_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_patches_previous_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropIndex(
                name: "ix_upgrade_paths_patch_id",
                table: "upgrade_paths");

            migrationBuilder.DropIndex(
                name: "ix_upgrade_paths_patch_id_previous_patch_id",
                table: "upgrade_paths");

            migrationBuilder.RenameColumn(
                name: "previous_patch_id",
                table: "upgrade_paths",
                newName: "previous_repo_version_id");

            migrationBuilder.RenameColumn(
                name: "patch_id",
                table: "upgrade_paths",
                newName: "repo_version_id");

            migrationBuilder.RenameIndex(
                name: "ix_upgrade_paths_previous_patch_id",
                table: "upgrade_paths",
                newName: "ix_upgrade_paths_previous_repo_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_upgrade_paths_repo_version_id",
                table: "upgrade_paths",
                column: "repo_version_id",
                unique: true,
                filter: "\"previous_repo_version_id\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_upgrade_paths_repo_version_id_previous_repo_version_id",
                table: "upgrade_paths",
                columns: new[] { "repo_version_id", "previous_repo_version_id" },
                unique: true,
                filter: "\"previous_repo_version_id\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_upgrade_paths_repo_versions_previous_repo_version_id",
                table: "upgrade_paths",
                column: "previous_repo_version_id",
                principalTable: "repo_versions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_upgrade_paths_repo_versions_repo_version_id",
                table: "upgrade_paths",
                column: "repo_version_id",
                principalTable: "repo_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_repo_versions_previous_repo_version_id",
                table: "upgrade_paths");

            migrationBuilder.DropForeignKey(
                name: "fk_upgrade_paths_repo_versions_repo_version_id",
                table: "upgrade_paths");

            migrationBuilder.DropIndex(
                name: "ix_upgrade_paths_repo_version_id",
                table: "upgrade_paths");

            migrationBuilder.DropIndex(
                name: "ix_upgrade_paths_repo_version_id_previous_repo_version_id",
                table: "upgrade_paths");

            migrationBuilder.RenameColumn(
                name: "repo_version_id",
                table: "upgrade_paths",
                newName: "patch_id");

            migrationBuilder.RenameColumn(
                name: "previous_repo_version_id",
                table: "upgrade_paths",
                newName: "previous_patch_id");

            migrationBuilder.RenameIndex(
                name: "ix_upgrade_paths_previous_repo_version_id",
                table: "upgrade_paths",
                newName: "ix_upgrade_paths_previous_patch_id");

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
        }
    }
}
