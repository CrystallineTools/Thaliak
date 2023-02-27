using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class TidyRelationshipsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_patches_repositories_repository_id",
                table: "patches");

            migrationBuilder.DropIndex(
                name: "ix_patches_repository_id",
                table: "patches");

            migrationBuilder.DropColumn(
                name: "repository_id",
                table: "patches");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "repository_id",
                table: "patches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_patches_repository_id",
                table: "patches",
                column: "repository_id");

            migrationBuilder.AddForeignKey(
                name: "fk_patches_repositories_repository_id",
                table: "patches",
                column: "repository_id",
                principalTable: "repositories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
