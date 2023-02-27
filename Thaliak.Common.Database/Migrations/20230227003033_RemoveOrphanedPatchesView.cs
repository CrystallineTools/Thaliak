using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class RemoveOrphanedPatchesView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW ""OrphanedPatches""");
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW ""OrphanedPatches"" AS
SELECT *
FROM ""Patches""
WHERE ""Id"" NOT IN (SELECT id
                   FROM (SELECT ""PreviousPatchId"" AS id FROM ""PatchChains"") prev
                   UNION
                   (SELECT ""PatchId"" AS id FROM ""PatchChains""));
");
        }
    }
}
