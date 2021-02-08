using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Companies_CustomerId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CustomerId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Quotations");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhoneNumber",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Quotations",
                nullable: true);

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
                name: "CustomerName",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerZIPCode",
                table: "Quotations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "ContactPhoneNumber",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Quotations");

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
                name: "CustomerName",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "CustomerZIPCode",
                table: "Quotations");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CustomerId",
                table: "Quotations",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Companies_CustomerId",
                table: "Quotations",
                column: "CustomerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
