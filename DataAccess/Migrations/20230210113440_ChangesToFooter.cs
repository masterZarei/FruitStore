using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ChangesToFooter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactWays_Footers_IsInFooter",
                table: "ContactWays");

            migrationBuilder.DropIndex(
                name: "IX_ContactWays_IsInFooter",
                table: "ContactWays");

            migrationBuilder.AlterColumn<bool>(
                name: "IsInFooter",
                table: "ContactWays",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsInFooter",
                table: "ContactWays",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_ContactWays_IsInFooter",
                table: "ContactWays",
                column: "IsInFooter");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactWays_Footers_IsInFooter",
                table: "ContactWays",
                column: "IsInFooter",
                principalTable: "Footers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
