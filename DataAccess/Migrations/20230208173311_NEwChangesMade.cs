using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class NEwChangesMade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrusSymbol2",
                table: "Footers",
                newName: "TrustSymbol2");

            migrationBuilder.RenameColumn(
                name: "TrusSymbol",
                table: "Footers",
                newName: "TrustSymbol");

            migrationBuilder.AlterColumn<double>(
                name: "TransactionAmount",
                table: "WalletHistories",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "NewWalletAmount",
                table: "WalletHistories",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "WalletAmount",
                table: "AspNetUsers",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrustSymbol2",
                table: "Footers",
                newName: "TrusSymbol2");

            migrationBuilder.RenameColumn(
                name: "TrustSymbol",
                table: "Footers",
                newName: "TrusSymbol");

            migrationBuilder.AlterColumn<decimal>(
                name: "TransactionAmount",
                table: "WalletHistories",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "NewWalletAmount",
                table: "WalletHistories",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "WalletAmount",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
