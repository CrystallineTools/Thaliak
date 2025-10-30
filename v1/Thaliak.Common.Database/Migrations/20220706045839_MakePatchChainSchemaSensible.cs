using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class MakePatchChainSchemaSensible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains");

            migrationBuilder.DropIndex(
                name: "IX_PatchChains_PatchId",
                table: "PatchChains");

            migrationBuilder.DropColumn(
                name: "HasPrerequisitePatch",
                table: "PatchChains");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousPatchId",
                table: "PatchChains",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PatchChains",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatchChains_PatchId",
                table: "PatchChains",
                column: "PatchId",
                unique: true,
                filter: "\"PreviousPatchId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PatchChains_PatchId_PreviousPatchId",
                table: "PatchChains",
                columns: new[] { "PatchId", "PreviousPatchId" },
                unique: true,
                filter: "\"PreviousPatchId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PatchChains_RepositoryId",
                table: "PatchChains",
                column: "RepositoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains",
                column: "PreviousPatchId",
                principalTable: "Patches",
                principalColumn: "Id");
            
            migrationBuilder.Sql(
                @"UPDATE ""PatchChains"" SET ""PreviousPatchId"" = NULL WHERE ""PreviousPatchId"" = ""PatchId"";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE ""PatchChains"" SET ""PreviousPatchId"" = ""PatchId"" WHERE ""PreviousPatchId"" IS NULL;");
            
            migrationBuilder.DropForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains");

            migrationBuilder.DropIndex(
                name: "IX_PatchChains_PatchId",
                table: "PatchChains");

            migrationBuilder.DropIndex(
                name: "IX_PatchChains_PatchId_PreviousPatchId",
                table: "PatchChains");

            migrationBuilder.DropIndex(
                name: "IX_PatchChains_RepositoryId",
                table: "PatchChains");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PatchChains");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousPatchId",
                table: "PatchChains",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPrerequisitePatch",
                table: "PatchChains",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatchChains",
                table: "PatchChains",
                columns: new[] { "RepositoryId", "PatchId", "PreviousPatchId" });

            migrationBuilder.CreateIndex(
                name: "IX_PatchChains_PatchId",
                table: "PatchChains",
                column: "PatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatchChains_Patches_PreviousPatchId",
                table: "PatchChains",
                column: "PreviousPatchId",
                principalTable: "Patches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
