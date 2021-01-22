using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrinterLayerThickness_LayerThicknesses_LayerThicknessID",
                table: "PrinterLayerThickness");

            migrationBuilder.DropForeignKey(
                name: "FK_PrinterLayerThickness_Printers_PrinterID",
                table: "PrinterLayerThickness");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrinterLayerThickness",
                table: "PrinterLayerThickness");

            migrationBuilder.RenameTable(
                name: "PrinterLayerThickness",
                newName: "PrinterLayerThicknesses");

            migrationBuilder.RenameIndex(
                name: "IX_PrinterLayerThickness_LayerThicknessID",
                table: "PrinterLayerThicknesses",
                newName: "IX_PrinterLayerThicknesses_LayerThicknessID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrinterLayerThicknesses",
                table: "PrinterLayerThicknesses",
                columns: new[] { "PrinterID", "LayerThicknessID" });

            migrationBuilder.AddForeignKey(
                name: "FK_PrinterLayerThicknesses_LayerThicknesses_LayerThicknessID",
                table: "PrinterLayerThicknesses",
                column: "LayerThicknessID",
                principalTable: "LayerThicknesses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrinterLayerThicknesses_Printers_PrinterID",
                table: "PrinterLayerThicknesses",
                column: "PrinterID",
                principalTable: "Printers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrinterLayerThicknesses_LayerThicknesses_LayerThicknessID",
                table: "PrinterLayerThicknesses");

            migrationBuilder.DropForeignKey(
                name: "FK_PrinterLayerThicknesses_Printers_PrinterID",
                table: "PrinterLayerThicknesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrinterLayerThicknesses",
                table: "PrinterLayerThicknesses");

            migrationBuilder.RenameTable(
                name: "PrinterLayerThicknesses",
                newName: "PrinterLayerThickness");

            migrationBuilder.RenameIndex(
                name: "IX_PrinterLayerThicknesses_LayerThicknessID",
                table: "PrinterLayerThickness",
                newName: "IX_PrinterLayerThickness_LayerThicknessID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrinterLayerThickness",
                table: "PrinterLayerThickness",
                columns: new[] { "PrinterID", "LayerThicknessID" });

            migrationBuilder.AddForeignKey(
                name: "FK_PrinterLayerThickness_LayerThicknesses_LayerThicknessID",
                table: "PrinterLayerThickness",
                column: "LayerThicknessID",
                principalTable: "LayerThicknesses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrinterLayerThickness_Printers_PrinterID",
                table: "PrinterLayerThickness",
                column: "PrinterID",
                principalTable: "Printers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
