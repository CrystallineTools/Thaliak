using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class RepositoryChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoteOrigin",
                table: "Repositories");

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Slug" },
                values: new object[] { "FFXIV Global/JP - Boot - Win32", "ffxivneo/win32/release/boot", "2b5cbc63" });

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Slug" },
                values: new object[] { "FFXIV Global/JP - Retail - Base Game - Win32", "ffxivneo/win32/release/game", "4e9a232b" });

            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "Id", "Description", "Name", "Slug" },
                values: new object[,]
                {
                    { 3, "FFXIV Global/JP - Retail - ex1 (Heavensward) - Win32", "ffxivneo/win32/release/ex1", "6b936f08" },
                    { 4, "FFXIV Global/JP - Retail - ex2 (Stormblood) - Win32", "ffxivneo/win32/release/ex2", "f29a3eb2" },
                    { 5, "FFXIV Global/JP - Retail - ex3 (Shadowbringers) - Win32", "ffxivneo/win32/release/ex3", "859d0e24" },
                    { 6, "FFXIV Global/JP - Retail - ex4 (Endwalker) - Win32", "ffxivneo/win32/release/ex4", "1bf99b87" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.AddColumn<string>(
                name: "RemoteOrigin",
                table: "Repositories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "RemoteOrigin", "Slug" },
                values: new object[] { null, "FFXIV Global/JPN - Boot - Win32", "http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{version}/?time={time}", "jp-win-boot" });

            migrationBuilder.UpdateData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "RemoteOrigin", "Slug" },
                values: new object[] { null, "FFXIV Global/JPN - Game - Win32", "https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{version}/{session}", "jp-win-game" });
        }
    }
}
