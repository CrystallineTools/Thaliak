using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class VersionFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Versions_VersionId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_VersionId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "VersionId",
                table: "Files");

            migrationBuilder.CreateTable(
                name: "VersionFiles",
                columns: table => new
                {
                    FilesId = table.Column<int>(type: "integer", nullable: false),
                    VersionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionFiles", x => new { x.FilesId, x.VersionsId });
                    table.ForeignKey(
                        name: "FK_VersionFiles_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VersionFiles_Versions_VersionsId",
                        column: x => x.VersionsId,
                        principalTable: "Versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_SHA256",
                table: "Files",
                column: "SHA256");

            migrationBuilder.CreateIndex(
                name: "IX_VersionFiles_VersionsId",
                table: "VersionFiles",
                column: "VersionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VersionFiles");

            migrationBuilder.DropIndex(
                name: "IX_Files_SHA256",
                table: "Files");

            migrationBuilder.AddColumn<int>(
                name: "VersionId",
                table: "Files",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Files_VersionId",
                table: "Files",
                column: "VersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Versions_VersionId",
                table: "Files",
                column: "VersionId",
                principalTable: "Versions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
