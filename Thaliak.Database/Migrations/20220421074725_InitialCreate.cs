using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RemoteOrigin = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountRepositories",
                columns: table => new
                {
                    ApplicableAccountsId = table.Column<int>(type: "integer", nullable: false),
                    ApplicableRepositoriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRepositories", x => new { x.ApplicableAccountsId, x.ApplicableRepositoriesId });
                    table.ForeignKey(
                        name: "FK_AccountRepositories_Accounts_ApplicableAccountsId",
                        column: x => x.ApplicableAccountsId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRepositories_Repositories_ApplicableRepositoriesId",
                        column: x => x.ApplicableRepositoriesId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VersionId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VersionString = table.Column<string>(type: "text", nullable: false),
                    RepositoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Versions_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VersionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MD5 = table.Column<string>(type: "text", nullable: false),
                    SHA1 = table.Column<string>(type: "text", nullable: false),
                    SHA256 = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Versions_VersionId",
                        column: x => x.VersionId,
                        principalTable: "Versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VersionId = table.Column<int>(type: "integer", nullable: false),
                    RepositoryId = table.Column<int>(type: "integer", nullable: false),
                    RemoteOriginPath = table.Column<string>(type: "text", nullable: false),
                    FirstSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patches_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patches_Versions_VersionId",
                        column: x => x.VersionId,
                        principalTable: "Versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "Id", "Description", "Name", "RemoteOrigin" },
                values: new object[,]
                {
                    { 1, null, "FFXIV Global - Boot - Win32", "http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{version}/?time={time}" },
                    { 2, null, "FFXIV Global - Game - Win32", "https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{version}/{session}" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRepositories_ApplicableRepositoriesId",
                table: "AccountRepositories",
                column: "ApplicableRepositoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Name",
                table: "Files",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Files_VersionId",
                table: "Files",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Patches_RepositoryId",
                table: "Patches",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Patches_VersionId",
                table: "Patches",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_RepositoryId",
                table: "Versions",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_VersionId",
                table: "Versions",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_VersionString",
                table: "Versions",
                column: "VersionString");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRepositories");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Patches");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "Repositories");
        }
    }
}
