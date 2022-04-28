using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class AddExpansionRepositoryMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpansionRepositoryMappings",
                columns: table => new
                {
                    GameRepositoryId = table.Column<int>(type: "integer", nullable: false),
                    ExpansionId = table.Column<int>(type: "integer", nullable: false),
                    ExpansionRepositoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpansionRepositoryMappings", x => new { x.GameRepositoryId, x.ExpansionId, x.ExpansionRepositoryId });
                    table.ForeignKey(
                        name: "FK_ExpansionRepositoryMappings_Repositories_ExpansionRepositor~",
                        column: x => x.ExpansionRepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpansionRepositoryMappings_Repositories_GameRepositoryId",
                        column: x => x.GameRepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExpansionRepositoryMappings",
                columns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                values: new object[,]
                {
                    { 0, 2, 2 },
                    { 1, 3, 2 },
                    { 2, 4, 2 },
                    { 3, 5, 2 },
                    { 4, 6, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpansionRepositoryMappings_ExpansionRepositoryId",
                table: "ExpansionRepositoryMappings",
                column: "ExpansionRepositoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpansionRepositoryMappings");
        }
    }
}
