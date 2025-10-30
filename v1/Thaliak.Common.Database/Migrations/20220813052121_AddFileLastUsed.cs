using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    public partial class AddFileLastUsed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsed",
                table: "Files",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Files_LastUsed",
                table: "Files",
                column: "LastUsed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_LastUsed",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LastUsed",
                table: "Files");
        }
    }
}
