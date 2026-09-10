using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    public partial class AddSaleDiscounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceDiscountAmount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceDiscountType",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceDiscountValue",
                table: "Sales",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "SaleItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "SaleItems",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                table: "SaleItems",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossTotal",
                table: "SaleItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Preserve existing sales data.
            migrationBuilder.Sql("""
                UPDATE Sales
                SET Subtotal = TotalAmount,
                    InvoiceDiscountType = 1,
                    InvoiceDiscountValue = 0,
                    InvoiceDiscountAmount = 0;
                """);

            // Preserve existing sale item data.
            migrationBuilder.Sql("""
                UPDATE SaleItems
                SET GrossTotal = Total,
                    DiscountType = 1,
                    DiscountValue = 0,
                    DiscountAmount = 0;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDiscountAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "InvoiceDiscountType",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "InvoiceDiscountValue",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "GrossTotal",
                table: "SaleItems");
        }
    }
}