using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrinterID",
                table: "PrintModels",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrintModels_PrinterID",
                table: "PrintModels",
                column: "PrinterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrintModels_Printers_PrinterID",
                table: "PrintModels",
                column: "PrinterID",
                principalTable: "Printers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrintModels_Printers_PrinterID",
                table: "PrintModels");

            migrationBuilder.DropIndex(
                name: "IX_PrintModels_PrinterID",
                table: "PrintModels");

            migrationBuilder.DropColumn(
                name: "PrinterID",
                table: "PrintModels");
        }
    }
}
