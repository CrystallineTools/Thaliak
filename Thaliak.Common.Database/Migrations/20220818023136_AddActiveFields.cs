using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class AddActiveFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Patches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PatchChains",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // one-time migration to set all existing patches to active
            migrationBuilder.Sql(@"UPDATE ""Patches"" SET ""IsActive"" = true;");
            migrationBuilder.Sql(@"UPDATE ""PatchChains"" SET ""IsActive"" = true;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Patches");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PatchChains");
        }
    }
}
