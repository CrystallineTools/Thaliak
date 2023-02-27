using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class CreateCompatibilityViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE VIEW ""Files"" AS
            SELECT
                name AS ""Name"",
            sha1 AS ""SHA1"",
            size AS ""Size"",
            last_used AS ""LastUsed""
            FROM files;

            CREATE VIEW ""PatchChains"" AS
                SELECT
            u.repository_id AS ""RepositoryId"",
            np.id AS ""PatchId"",
            pp.id AS ""PreviousPatchId"",
            u.first_offered AS ""FirstOffered"",
            u.last_offered AS ""LastOffered"",
            u.id AS ""Id"",
            u.is_active AS ""IsActive""
            FROM upgrade_paths u, patches np, patches pp
            WHERE u.repo_version_id = np.id AND u.repo_version_id = pp.id;

            CREATE VIEW ""Patches"" AS
                SELECT
            p.id AS ""Id"",
            rv.id AS ""VersionId"",
            rv.repository_id AS ""RepositoryId"",
            p.remote_origin_path AS ""RemoteOriginPath"",
            p.first_seen AS ""FirstSeen"",
            p.last_seen AS ""LastSeen"",
            p.size AS ""Size"",
            p.hash_type AS ""HashType"",
            p.hash_block_size AS ""HashBlockSize"",
            p.hashes AS ""Hashes"",
            p.first_offered AS ""FirstOffered"",
            p.last_offered AS ""LastOffered"",
            p.local_storage_path AS ""LocalStoragePath"",
            p.is_active AS ""IsActive""
            FROM patches p, repo_versions rv
                WHERE p.repo_version_id = rv.id;

            CREATE VIEW ""Repositories"" AS
                SELECT
            r.id AS ""Id"",
            r.name AS ""Name"",
            r.description AS ""Description"",
            r.slug AS ""Slug"",
            r.service_id AS ""ServiceRegionId""
            FROM repositories r;

            CREATE VIEW ""VersionFiles"" AS
                SELECT
            vf.versions_id AS ""VersionsId"",
            vf.files_name AS ""FilesName"",
            vf.files_sha1 AS ""FilesSHA1""
            FROM version_files vf;

            CREATE VIEW ""Versions"" AS
            SELECT rv.id             AS ""Id"",
                   rv.repository_id  AS ""RepositoryId"",
                   CASE
                       WHEN (regexp_match(rv.version_string, '[a-z]$'))[1] IS NOT NULL THEN
                           concat(
                                   regexp_replace(rv.version_string, '\D', '', 'g'),
                                   lpad((ascii((regexp_match(rv.version_string, '[a-z]$'))[1]) - 96)::text, 2, '0'),
                                   '0'
                               )::bigint
                       ELSE
                           concat(regexp_replace(rv.version_string, '\D', '', 'g'), '000')::bigint
                       END           AS ""VersionId"",
                   rv.version_string AS ""VersionString"",
                   p.first_seen      AS ""FirstSeen"",
                   p.last_seen       AS ""LastSeen"",
                   p.is_active       AS ""IsActive""
            FROM repo_versions rv,
                 patches p
            WHERE rv.id = p.repo_version_id;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP VIEW ""Files"";
DROP VIEW ""PatchChains"";
DROP VIEW ""Patches"";
DROP VIEW ""Repositories"";
DROP VIEW ""VersionFiles"";
DROP VIEW ""Versions""
");
        }
    }
}
