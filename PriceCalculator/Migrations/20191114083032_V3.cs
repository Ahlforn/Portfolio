using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PrintDirection",
                table: "PrintModels",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PrintDirection",
                table: "PrintModels",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
