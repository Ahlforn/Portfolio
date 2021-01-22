using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCVR",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerCity",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerCountry",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerZIPCode",
                table: "Quotations");

            migrationBuilder.CreateTable(
                name: "Printers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Printers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PrinterLayerThickness",
                columns: table => new
                {
                    PrinterID = table.Column<int>(nullable: false),
                    LayerThicknessID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterLayerThickness", x => new { x.PrinterID, x.LayerThicknessID });
                    table.ForeignKey(
                        name: "FK_PrinterLayerThickness_LayerThicknesses_LayerThicknessID",
                        column: x => x.LayerThicknessID,
                        principalTable: "LayerThicknesses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrinterLayerThickness_Printers_PrinterID",
                        column: x => x.PrinterID,
                        principalTable: "Printers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrinterLayerThickness_LayerThicknessID",
                table: "PrinterLayerThickness",
                column: "LayerThicknessID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrinterLayerThickness");

            migrationBuilder.DropTable(
                name: "Printers");

            migrationBuilder.AddColumn<string>(
                name: "CustomerCVR",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCity",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCountry",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerZIPCode",
                table: "Quotations",
                nullable: true);
        }
    }
}
