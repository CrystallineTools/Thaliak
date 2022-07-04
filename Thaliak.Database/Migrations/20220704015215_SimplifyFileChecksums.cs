using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class SimplifyFileChecksums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_Files_FilesId",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles");

            migrationBuilder.DropIndex(
                name: "IX_VersionFiles_VersionsId",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_Name",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_SHA256",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FilesId",
                table: "VersionFiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "MD5",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SHA256",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "FilesName",
                table: "VersionFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilesSHA1",
                table: "VersionFiles",
                type: "character(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SHA1",
                table: "Files",
                type: "character(40)",
                fixedLength: true,
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles",
                columns: new[] { "VersionsId", "FilesName", "FilesSHA1" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                columns: new[] { "Name", "SHA1" });

            migrationBuilder.CreateIndex(
                name: "IX_VersionFiles_FilesName_FilesSHA1",
                table: "VersionFiles",
                columns: new[] { "FilesName", "FilesSHA1" });

            migrationBuilder.AddForeignKey(
                name: "FK_VersionFiles_Files_FilesName_FilesSHA1",
                table: "VersionFiles",
                columns: new[] { "FilesName", "FilesSHA1" },
                principalTable: "Files",
                principalColumns: new[] { "Name", "SHA1" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VersionFiles_Files_FilesName_FilesSHA1",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles");

            migrationBuilder.DropIndex(
                name: "IX_VersionFiles_FilesName_FilesSHA1",
                table: "VersionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FilesName",
                table: "VersionFiles");

            migrationBuilder.DropColumn(
                name: "FilesSHA1",
                table: "VersionFiles");

            migrationBuilder.AddColumn<int>(
                name: "FilesId",
                table: "VersionFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SHA1",
                table: "Files",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(40)",
                oldFixedLength: true,
                oldMaxLength: 40);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Files",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "MD5",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SHA256",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VersionFiles",
                table: "VersionFiles",
                columns: new[] { "FilesId", "VersionsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VersionFiles_VersionsId",
                table: "VersionFiles",
                column: "VersionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Name",
                table: "Files",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Files_SHA256",
                table: "Files",
                column: "SHA256");

            migrationBuilder.AddForeignKey(
                name: "FK_VersionFiles_Files_FilesId",
                table: "VersionFiles",
                column: "FilesId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
