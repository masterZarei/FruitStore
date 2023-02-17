using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class changesToFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Send_Date",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "WillDeliver_Date",
                table: "Factors");

            migrationBuilder.AddColumn<string>(
                name: "Deliver_Date",
                table: "Factors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deliver_Time",
                table: "Factors",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deliver_Date",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "Deliver_Time",
                table: "Factors");

            migrationBuilder.AddColumn<DateTime>(
                name: "Send_Date",
                table: "Factors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WillDeliver_Date",
                table: "Factors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
