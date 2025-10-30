using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class AddServiceRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceRegionId",
                table: "Repositories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRegions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ServiceRegions",
                columns: new[] { "Id", "Icon", "Name" },
                values: new object[,]
                {
                    { 1, "🇺🇳", "FFXIV Global" },
                    { 2, "🇰🇷", "FFXIV Korea" },
                    { 3, "🇨🇳", "FFXIV China" }
                });

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceRegionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 2,
                column: "ServiceRegionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 3,
                column: "ServiceRegionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 4,
                column: "ServiceRegionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 5,
                column: "ServiceRegionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 6,
                column: "ServiceRegionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 7,
                column: "ServiceRegionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 8,
                column: "ServiceRegionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 9,
                column: "ServiceRegionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 10,
                column: "ServiceRegionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 11,
                column: "ServiceRegionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 12,
                column: "ServiceRegionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 13,
                column: "ServiceRegionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 14,
                column: "ServiceRegionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 15,
                column: "ServiceRegionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 16,
                column: "ServiceRegionId",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_ServiceRegionId",
                table: "Repositories",
                column: "ServiceRegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_ServiceRegions_ServiceRegionId",
                table: "Repositories",
                column: "ServiceRegionId",
                principalTable: "ServiceRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_ServiceRegions_ServiceRegionId",
                table: "Repositories");

            migrationBuilder.DropTable(
                name: "ServiceRegions");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_ServiceRegionId",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "ServiceRegionId",
                table: "Repositories");
        }
    }
}
