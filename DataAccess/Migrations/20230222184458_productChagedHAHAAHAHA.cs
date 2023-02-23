using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class productChagedHAHAAHAHA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitToProducts_ProductId",
                table: "UnitToProducts");

            migrationBuilder.CreateIndex(
                name: "IX_UnitToProducts_ProductId",
                table: "UnitToProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitToProducts_ProductId",
                table: "UnitToProducts");

            migrationBuilder.CreateIndex(
                name: "IX_UnitToProducts_ProductId",
                table: "UnitToProducts",
                column: "ProductId",
                unique: true);
        }
    }
}
