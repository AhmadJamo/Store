using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMovingWeightedAverageInventoryCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageUnitCostAfter",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageUnitCostBefore",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostVariance",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InventoryValueAfter",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InventoryValueBefore",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityAfter",
                table: "StockTransactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityBefore",
                table: "StockTransactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionValue",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "StockTransactions",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostOfGoodsSold",
                table: "SaleItems",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "SaleItems",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageUnitCost",
                table: "ProductStocks",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InventoryValue",
                table: "ProductStocks",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastReferenceUnitCost",
                table: "ProductStocks",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE stocks
                SET AverageUnitCost = products.PurchasePrice,
                    LastReferenceUnitCost = products.PurchasePrice,
                    InventoryValue = ROUND(stocks.Quantity * products.PurchasePrice, 8)
                FROM ProductStocks stocks
                INNER JOIN Products products
                    ON products.Id = stocks.ProductId
                   AND products.TenantId = stocks.TenantId;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageUnitCostAfter",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "AverageUnitCostBefore",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "CostVariance",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "InventoryValueAfter",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "InventoryValueBefore",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "QuantityAfter",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "QuantityBefore",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionValue",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "CostOfGoodsSold",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "AverageUnitCost",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "InventoryValue",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "LastReferenceUnitCost",
                table: "ProductStocks");
        }
    }
}
