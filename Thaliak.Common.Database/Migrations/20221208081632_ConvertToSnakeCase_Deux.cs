using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class ConvertToSnakeCase_Deux : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_patch_chains_patch_id",
                table: "patch_chains");

            migrationBuilder.DropIndex(
                name: "ix_patch_chains_patch_id_previous_patch_id",
                table: "patch_chains");

            migrationBuilder.RenameTable(
                name: "VersionFiles",
                newName: "version_files");

            migrationBuilder.RenameTable(
                name: "AccountRepositories",
                newName: "account_repositories");

            migrationBuilder.CreateIndex(
                name: "ix_patch_chains_patch_id",
                table: "patch_chains",
                column: "patch_id",
                unique: true,
                filter: "\"previous_patch_id\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_patch_chains_patch_id_previous_patch_id",
                table: "patch_chains",
                columns: new[] { "patch_id", "previous_patch_id" },
                unique: true,
                filter: "\"previous_patch_id\" IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_patch_chains_patch_id",
                table: "patch_chains");

            migrationBuilder.DropIndex(
                name: "ix_patch_chains_patch_id_previous_patch_id",
                table: "patch_chains");

            migrationBuilder.RenameTable(
                name: "version_files",
                newName: "VersionFiles");

            migrationBuilder.RenameTable(
                name: "account_repositories",
                newName: "AccountRepositories");

            migrationBuilder.CreateIndex(
                name: "ix_patch_chains_patch_id",
                table: "patch_chains",
                column: "patch_id",
                unique: true,
                filter: "\"PreviousPatchId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_patch_chains_patch_id_previous_patch_id",
                table: "patch_chains",
                columns: new[] { "patch_id", "previous_patch_id" },
                unique: true,
                filter: "\"PreviousPatchId\" IS NOT NULL");
        }
    }
}
