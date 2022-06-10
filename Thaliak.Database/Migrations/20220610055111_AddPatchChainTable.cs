using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class AddPatchChainTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatchChains",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(type: "integer", nullable: false),
                    PatchId = table.Column<int>(type: "integer", nullable: false),
                    PreviousPatchId = table.Column<int>(type: "integer", nullable: false),
                    HasPrerequisitePatch = table.Column<bool>(type: "boolean", nullable: false),
                    FirstOffered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastOffered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatchChains", x => new { x.RepositoryId, x.PatchId, x.PreviousPatchId });
                    table.ForeignKey(
                        name: "FK_PatchChains_Patches_PatchId",
                        column: x => x.PatchId,
                        principalTable: "Patches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatchChains_Patches_PreviousPatchId",
                        column: x => x.PreviousPatchId,
                        principalTable: "Patches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatchChains_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatchChains_PatchId",
                table: "PatchChains",
                column: "PatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PatchChains_PreviousPatchId",
                table: "PatchChains",
                column: "PreviousPatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatchChains");
        }
    }
}
