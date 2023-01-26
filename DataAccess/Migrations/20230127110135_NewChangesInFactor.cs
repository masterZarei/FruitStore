using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class NewChangesInFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isReadyToDeliver",
                table: "Factors");

            migrationBuilder.AddColumn<byte>(
                name: "DeliverState",
                table: "Factors",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliverState",
                table: "Factors");

            migrationBuilder.AddColumn<bool>(
                name: "isReadyToDeliver",
                table: "Factors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
