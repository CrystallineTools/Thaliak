using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class MigrateUpgradePathsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
WITH cte AS (SELECT u.id,
                    np.repo_version_id AS repo_version_id,
                    pp.repo_version_id AS previous_repo_version_id
             FROM upgrade_paths u,
                  patches np,
                  patches pp
             WHERE u.repo_version_id = np.id
               AND u.previous_repo_version_id = pp.id)
UPDATE upgrade_paths
SET repo_version_id          = cte.repo_version_id,
    previous_repo_version_id = cte.previous_repo_version_id
FROM cte
WHERE upgrade_paths.id = cte.id;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new System.NotImplementedException();
        }
    }
}
