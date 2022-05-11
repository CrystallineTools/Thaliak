using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class AddKrCnRepos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "Id", "Description", "Name", "Slug" },
                values: new object[,]
                {
                    { 7, "FFXIV Korea - Retail - Base Game - Win32", "actoz/win32/release_ko/game", "de199059" },
                    { 8, "FFXIV Korea - Retail - ex1 (Heavensward) - Win32", "actoz/win32/release_ko/ex1", "573d8c07" },
                    { 9, "FFXIV Korea - Retail - ex2 (Stormblood) - Win32", "actoz/win32/release_ko/ex2", "ce34ddbd" },
                    { 10, "FFXIV Korea - Retail - ex3 (Shadowbringers) - Win32", "actoz/win32/release_ko/ex3", "b933ed2b" },
                    { 11, "FFXIV Korea - Retail - ex4 (Endwalker) - Win32", "actoz/win32/release_ko/ex4", "27577888" },
                    { 12, "FFXIV China - Retail - Base Game - Win32", "shanda/win32/release_chs/game", "c38effbc" },
                    { 13, "FFXIV China - Retail - ex1 (Heavensward) - Win32", "shanda/win32/release_chs/ex1", "77420d17" },
                    { 14, "FFXIV China - Retail - ex2 (Stormblood) - Win32", "shanda/win32/release_chs/ex2", "ee4b5cad" },
                    { 15, "FFXIV China - Retail - ex3 (Shadowbringers) - Win32", "shanda/win32/release_chs/ex3", "994c6c3b" },
                    { 16, "FFXIV China - Retail - ex4 (Endwalker) - Win32", "shanda/win32/release_chs/ex4", "0728f998" }
                });

            migrationBuilder.InsertData(
                table: "ExpansionRepositoryMappings",
                columns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                values: new object[,]
                {
                    { 0, 7, 7 },
                    { 1, 8, 7 },
                    { 2, 9, 7 },
                    { 3, 10, 7 },
                    { 4, 11, 7 },
                    { 0, 12, 12 },
                    { 1, 13, 12 },
                    { 2, 14, 12 },
                    { 3, 15, 12 },
                    { 4, 16, 12 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 0, 7, 7 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 1, 8, 7 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 2, 9, 7 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 3, 10, 7 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 4, 11, 7 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 0, 12, 12 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 1, 13, 12 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 2, 14, 12 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 3, 15, 12 });

            migrationBuilder.DeleteData(
                table: "ExpansionRepositoryMappings",
                keyColumns: new[] { "ExpansionId", "ExpansionRepositoryId", "GameRepositoryId" },
                keyValues: new object[] { 4, 16, 12 });

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
