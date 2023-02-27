using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class AddGameVersionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_versions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    version_name = table.Column<string>(type: "text", nullable: false),
                    hotfix_level = table.Column<int>(type: "integer", nullable: false),
                    marketing_name = table.Column<string>(type: "text", nullable: true),
                    patch_info_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_versions", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_versions_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_version_repo_versions",
                columns: table => new
                {
                    game_versions_id = table.Column<int>(type: "integer", nullable: false),
                    repo_versions_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_version_repo_versions", x => new { x.game_versions_id, x.repo_versions_id });
                    table.ForeignKey(
                        name: "fk_game_version_repo_versions_game_versions_game_versions_id",
                        column: x => x.game_versions_id,
                        principalTable: "game_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_version_repo_versions_repo_versions_repo_versions_id",
                        column: x => x.repo_versions_id,
                        principalTable: "repo_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_game_version_repo_versions_repo_versions_id",
                table: "game_version_repo_versions",
                column: "repo_versions_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_versions_hotfix_level",
                table: "game_versions",
                column: "hotfix_level");

            migrationBuilder.CreateIndex(
                name: "ix_game_versions_service_id",
                table: "game_versions",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_versions_version_name",
                table: "game_versions",
                column: "version_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_version_repo_versions");

            migrationBuilder.DropTable(
                name: "game_versions");
        }
    }
}
