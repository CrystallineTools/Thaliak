using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RenameTableVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patches_Versions_VersionId",
                table: "Patches");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_Versions_VersionsId",
                table: "VersionFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Repositories_RepositoryId",
                table: "Versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Versions",
                table: "Versions");

            migrationBuilder.RenameTable(
                name: "Versions",
                newName: "RepoVersions");

            migrationBuilder.RenameColumn(
                name: "VersionId",
                table: "Patches",
                newName: "RepoVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Patches_VersionId",
                table: "Patches",
                newName: "IX_Patches_RepoVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_VersionString",
                table: "RepoVersions",
                newName: "IX_RepoVersions_VersionString");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_VersionId",
                table: "RepoVersions",
                newName: "IX_RepoVersions_VersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_RepositoryId",
                table: "RepoVersions",
                newName: "IX_RepoVersions_RepositoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepoVersions",
                table: "RepoVersions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patches_RepoVersions_RepoVersionId",
                table: "Patches",
                column: "RepoVersionId",
                principalTable: "RepoVersions",
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
                name: "FK_VersionFiles_RepoVersions_VersionsId",
                table: "VersionFiles",
                column: "VersionsId",
                principalTable: "RepoVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patches_RepoVersions_RepoVersionId",
                table: "Patches");

            migrationBuilder.DropForeignKey(
                name: "FK_RepoVersions_Repositories_RepositoryId",
                table: "RepoVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_RepoVersions_VersionsId",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepoVersions",
                table: "RepoVersions");

            migrationBuilder.RenameTable(
                name: "RepoVersions",
                newName: "Versions");

            migrationBuilder.RenameColumn(
                name: "RepoVersionId",
                table: "Patches",
                newName: "VersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Patches_RepoVersionId",
                table: "Patches",
                newName: "IX_Patches_VersionId");

            migrationBuilder.RenameIndex(
                name: "IX_RepoVersions_VersionString",
                table: "Versions",
                newName: "IX_Versions_VersionString");

            migrationBuilder.RenameIndex(
                name: "IX_RepoVersions_VersionId",
                table: "Versions",
                newName: "IX_Versions_VersionId");

            migrationBuilder.RenameIndex(
                name: "IX_RepoVersions_RepositoryId",
                table: "Versions",
                newName: "IX_Versions_RepositoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Versions",
                table: "Versions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patches_Versions_VersionId",
                table: "Patches",
                column: "VersionId",
                principalTable: "Versions",
                principalColumn: "Id",
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
