using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class wallletHistoryUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastWalletAmount",
                table: "WalletHistories",
                newName: "TrackingCode");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "WalletHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "WalletHistories");

            migrationBuilder.RenameColumn(
                name: "TrackingCode",
                table: "WalletHistories",
                newName: "LastWalletAmount");
        }
    }
}
