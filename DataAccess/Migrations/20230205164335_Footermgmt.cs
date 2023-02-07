using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class Footermgmt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Footers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrusSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrusSymbol2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Footers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactWaysToFooters_ContactWaysId",
                table: "ContactWaysToFooters",
                column: "ContactWaysId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactWaysToFooters");

            migrationBuilder.DropTable(
                name: "Footers");
        }
    }
}
