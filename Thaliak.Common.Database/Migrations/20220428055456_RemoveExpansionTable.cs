using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RemoveExpansionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Expansions_ExpansionId",
                table: "Versions");

            migrationBuilder.DropTable(
                name: "Expansions");

            migrationBuilder.DropIndex(
                name: "IX_Versions_ExpansionId",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "ExpansionId",
                table: "Versions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpansionId",
                table: "Versions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Expansions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Abbreviation = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expansions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Expansions",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { 0, "ARR", "A Realm Reborn" },
                    { 1, "HW", "Heavensward" },
                    { 2, "SB", "Stormblood" },
                    { 3, "ShB", "Shadowbringers" },
                    { 4, "EW", "Endwalker" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Versions_ExpansionId",
                table: "Versions",
                column: "ExpansionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Expansions_ExpansionId",
                table: "Versions",
                column: "ExpansionId",
                principalTable: "Expansions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
