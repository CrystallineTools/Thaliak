using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RenameTableServiceRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_ServiceRegions_ServiceRegionId",
                table: "Repositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceRegions",
                table: "ServiceRegions");

            migrationBuilder.RenameTable(
                name: "ServiceRegions",
                newName: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceRegionId",
                table: "Repositories",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Repositories_ServiceRegionId",
                table: "Repositories",
                newName: "IX_Repositories_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_Services_ServiceId",
                table: "Repositories",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_Services_ServiceId",
                table: "Repositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "ServiceRegions");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Repositories",
                newName: "ServiceRegionId");

            migrationBuilder.RenameIndex(
                name: "IX_Repositories_ServiceId",
                table: "Repositories",
                newName: "IX_Repositories_ServiceRegionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceRegions",
                table: "ServiceRegions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_ServiceRegions_ServiceRegionId",
                table: "Repositories",
                column: "ServiceRegionId",
                principalTable: "ServiceRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
