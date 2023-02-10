using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class FooterContactWays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactWaysToFooters");

            migrationBuilder.AddColumn<int>(
                name: "IsInFooter",
                table: "ContactWays",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactWays_Footers_IsInFooter",
                table: "ContactWays");

            migrationBuilder.DropIndex(
                name: "IX_ContactWays_IsInFooter",
                table: "ContactWays");

            migrationBuilder.DropColumn(
                name: "IsInFooter",
                table: "ContactWays");

            migrationBuilder.CreateTable(
                name: "ContactWaysToFooters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactWaysId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactWaysToFooters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactWaysToFooters_ContactWays_ContactWaysId",
                        column: x => x.ContactWaysId,
                        principalTable: "ContactWays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactWaysToFooters_ContactWaysId",
                table: "ContactWaysToFooters",
                column: "ContactWaysId");
        }
    }
}
