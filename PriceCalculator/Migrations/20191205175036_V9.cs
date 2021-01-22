using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "D",
                table: "Printers",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "H",
                table: "Printers",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "W",
                table: "Printers",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "D",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "H",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "W",
                table: "Printers");
        }
    }
}
