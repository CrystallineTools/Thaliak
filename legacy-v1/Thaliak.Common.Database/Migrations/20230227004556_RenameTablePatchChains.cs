using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RenameTablePatchChains : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Patches_PatchId",
                table: "PatchChains");

            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains");

            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Repositories_RepositoryId",
                table: "PatchChains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains");

            migrationBuilder.RenameTable(
                name: "PatchChains",
                newName: "UpgradePaths");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_RepositoryId",
                table: "UpgradePaths",
                newName: "IX_UpgradePaths_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_PreviousPatchId",
                table: "UpgradePaths",
                newName: "IX_UpgradePaths_PreviousPatchId");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_PatchId_PreviousPatchId",
                table: "UpgradePaths",
                newName: "IX_UpgradePaths_PatchId_PreviousPatchId");

            migrationBuilder.RenameIndex(
                name: "IX_PatchChains_PatchId",
                table: "UpgradePaths",
                newName: "IX_UpgradePaths_PatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UpgradePaths",
                table: "UpgradePaths",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UpgradePaths_Patches_PatchId",
                table: "UpgradePaths",
                column: "PatchId",
                principalTable: "Patches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UpgradePaths_Patches_PreviousPatchId",
                table: "UpgradePaths",
                column: "PreviousPatchId",
                principalTable: "Patches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UpgradePaths_Repositories_RepositoryId",
                table: "UpgradePaths",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UpgradePaths_Patches_PatchId",
                table: "UpgradePaths");

            migrationBuilder.DropForeignKey(
                name: "FK_UpgradePaths_Patches_PreviousPatchId",
                table: "UpgradePaths");

            migrationBuilder.DropForeignKey(
                name: "FK_UpgradePaths_Repositories_RepositoryId",
                table: "UpgradePaths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UpgradePaths",
                table: "UpgradePaths");

            migrationBuilder.RenameTable(
                name: "UpgradePaths",
                newName: "PatchChains");

            migrationBuilder.RenameIndex(
                name: "IX_UpgradePaths_RepositoryId",
                table: "PatchChains",
                newName: "IX_PatchChains_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "IX_UpgradePaths_PreviousPatchId",
                table: "PatchChains",
                newName: "IX_PatchChains_PreviousPatchId");

            migrationBuilder.RenameIndex(
                name: "IX_UpgradePaths_PatchId_PreviousPatchId",
                table: "PatchChains",
                newName: "IX_PatchChains_PatchId_PreviousPatchId");

            migrationBuilder.RenameIndex(
                name: "IX_UpgradePaths_PatchId",
                table: "PatchChains",
                newName: "IX_PatchChains_PatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Patches_PatchId",
                table: "PatchChains",
                column: "PatchId",
                principalTable: "Patches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains",
                column: "PreviousPatchId",
                principalTable: "Patches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Repositories_RepositoryId",
                table: "PatchChains",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
