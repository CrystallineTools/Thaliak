using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class AddPatchLocalStoragePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalStoragePath",
                table: "Patches",
                type: "text",
                nullable: false,
                defaultValue: "");

            // this might be a crime in many countries, but yolo, we're doing it
            migrationBuilder.Sql(@"
WITH matched AS (
    SELECT ""Id"", ""RemoteOriginPath"", CONCAT(matches[1], '/', matches[2], '/', matches[3]) AS ""LocalStoragePath""
    FROM (SELECT ""Id"", ""RemoteOriginPath"",
                 regexp_matches(""RemoteOriginPath"",
                                '(?:https?:\/\/(.+?)\/)?(?:ff\/)?((?:game|boot)\/.+)\/(.*)') AS matches
          FROM ""Patches"") sel
)
UPDATE ""Patches"" SET ""LocalStoragePath"" = matched.""LocalStoragePath"" FROM matched WHERE matched.""Id"" = ""Patches"".""Id"";
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalStoragePath",
                table: "Patches");
        }
    }
}
