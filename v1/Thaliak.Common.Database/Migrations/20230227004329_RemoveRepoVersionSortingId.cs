using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RemoveRepoVersionSortingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RepoVersions_VersionId",
                table: "RepoVersions");

            migrationBuilder.DropColumn(
                name: "VersionId",
                table: "RepoVersions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VersionId",
                table: "RepoVersions",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RepoVersions_VersionId",
                table: "RepoVersions",
                column: "VersionId");
        }
    }
}
