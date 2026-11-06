using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorBillDueDateSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DueDate",
                table: "VendorBills",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTermDaysSnapshot",
                table: "VendorBills",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "VendorBills");

            migrationBuilder.DropColumn(
                name: "PaymentTermDaysSnapshot",
                table: "VendorBills");
        }
    }
}
