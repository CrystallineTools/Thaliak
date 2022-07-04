using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class FixRepoName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "FFXIV Global/JP - Retail - Boot - Win32");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "FFXIV Global/JP - Boot - Win32");
        }
    }
}
