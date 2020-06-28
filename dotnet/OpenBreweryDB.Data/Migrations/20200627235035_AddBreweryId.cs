using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenBreweryDB.Data.Migrations
{
    public partial class AddBreweryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BreweryId",
                table: "breweries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreweryId",
                table: "breweries");
        }
    }
}
