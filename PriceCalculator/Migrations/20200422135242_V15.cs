using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndustryId",
                table: "PrintModels",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrintModels_IndustryId",
                table: "PrintModels",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrintModels_Industries_IndustryId",
                table: "PrintModels",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrintModels_Industries_IndustryId",
                table: "PrintModels");

            migrationBuilder.DropIndex(
                name: "IX_PrintModels_IndustryId",
                table: "PrintModels");

            migrationBuilder.DropColumn(
                name: "IndustryId",
                table: "PrintModels");
        }
    }
}
