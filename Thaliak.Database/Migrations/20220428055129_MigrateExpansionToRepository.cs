using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class MigrateExpansionToRepository : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Versions\" SET \"RepositoryId\" = \"ExpansionId\" + 2 WHERE \"ExpansionId\" > 0;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Versions\" SET \"ExpansionId\" = \"RepositoryId\" - 2, \"RepositoryId\" = 2 WHERE \"RepositoryId\" > 2;");
        }
    }
}
