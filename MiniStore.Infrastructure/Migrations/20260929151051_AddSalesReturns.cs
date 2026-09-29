using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesReturns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_SaleItems_Id_TenantId",
                table: "SaleItems",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "SalesReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RevenueAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RestockedCostAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturns", x => x.Id);
                    table.UniqueConstraint("AK_SalesReturns_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SalesReturns_PaymentMethods_PaymentMethodId_TenantId",
                        columns: x => new { x.PaymentMethodId, x.TenantId },
                        principalTable: "PaymentMethods",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturns_Sales_SaleId_TenantId",
                        columns: x => new { x.SaleId, x.TenantId },
                        principalTable: "Sales",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturns_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesReturnId = table.Column<int>(type: "int", nullable: false),
                    SaleItemId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    RevenueAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Restocked = table.Column<bool>(type: "bit", nullable: false),
                    RestockedCostAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesReturnItems_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturnItems_SaleItems_SaleItemId_TenantId",
                        columns: x => new { x.SaleItemId, x.TenantId },
                        principalTable: "SaleItems",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesReturnItems_SalesReturns_SalesReturnId_TenantId",
                        columns: x => new { x.SalesReturnId, x.TenantId },
                        principalTable: "SalesReturns",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesReturnItems_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItems_ProductId_TenantId",
                table: "SalesReturnItems",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItems_SaleItemId_TenantId",
                table: "SalesReturnItems",
                columns: new[] { "SaleItemId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItems_SalesReturnId_TenantId",
                table: "SalesReturnItems",
                columns: new[] { "SalesReturnId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItems_TenantId",
                table: "SalesReturnItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItems_TenantId_SalesReturnId_SaleItemId",
                table: "SalesReturnItems",
                columns: new[] { "TenantId", "SalesReturnId", "SaleItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_PaymentMethodId_TenantId",
                table: "SalesReturns",
                columns: new[] { "PaymentMethodId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_SaleId_TenantId",
                table: "SalesReturns",
                columns: new[] { "SaleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_TenantId",
                table: "SalesReturns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_TenantId_ReturnNumber",
                table: "SalesReturns",
                columns: new[] { "TenantId", "ReturnNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturns_WarehouseId_TenantId",
                table: "SalesReturns",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesReturnItems");

            migrationBuilder.DropTable(
                name: "SalesReturns");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SaleItems_Id_TenantId",
                table: "SaleItems");
        }
    }
}
