using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReversiMvcApp.Migrations
{
    public partial class SpelerMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spelers",
                columns: table => new
                {
                    Guuid = table.Column<string>(type: "TEXT", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", nullable: false),
                    AantalGewonnen = table.Column<int>(type: "INTEGER", nullable: false),
                    AantalVerloren = table.Column<int>(type: "INTEGER", nullable: false),
                    AantalGelijk = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spelers", x => x.Guuid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spelers");
        }
    }
}
