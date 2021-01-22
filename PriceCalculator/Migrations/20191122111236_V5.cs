using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceCalculator.Migrations
{
    public partial class V5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Finalized",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "PrintModels");

            migrationBuilder.DropColumn(
                name: "PhoneCountryCode",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhoneNumber",
                table: "Companies",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ContactPhoneNumber",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Quotations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Completed",
                table: "Quotations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Finalized",
                table: "Quotations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "PrintModels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PhoneCountryCode",
                table: "Companies",
                nullable: false,
                defaultValue: 0);
        }
    }
}
