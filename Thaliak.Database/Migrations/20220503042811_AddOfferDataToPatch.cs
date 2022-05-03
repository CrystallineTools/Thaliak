using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thaliak.Database.Migrations
{
    public partial class AddOfferDataToPatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstOffered",
                table: "Patches",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOffered",
                table: "Patches",
                type: "timestamp with time zone",
                nullable: true);
            
            // default to the seen values
            migrationBuilder.Sql("UPDATE \"Patches\" SET \"FirstOffered\" = \"FirstSeen\", \"LastOffered\" = \"LastSeen\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstOffered",
                table: "Patches");

            migrationBuilder.DropColumn(
                name: "LastOffered",
                table: "Patches");
        }
    }
}
