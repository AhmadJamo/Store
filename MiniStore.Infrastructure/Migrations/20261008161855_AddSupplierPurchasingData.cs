using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierPurchasingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierProductPurchasingInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PurchaseMeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    SupplierProductCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupplierDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MinimumOrderQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    OrderMultiple = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    CurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProductPurchasingInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProductPurchasingInfos_MeasurementUnits_PurchaseMeasurementUnitId_TenantId",
                        columns: x => new { x.PurchaseMeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPurchasingInfos_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPurchasingInfos_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProductPurchasingInfos_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPurchasingInfos_ProductId_TenantId",
                table: "SupplierProductPurchasingInfos",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPurchasingInfos_PurchaseMeasurementUnitId_TenantId",
                table: "SupplierProductPurchasingInfos",
                columns: new[] { "PurchaseMeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPurchasingInfos_SupplierId_TenantId",
                table: "SupplierProductPurchasingInfos",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPurchasingInfos_TenantId",
                table: "SupplierProductPurchasingInfos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPurchasingInfos_TenantId_ProductId_IsActive_Priority",
                table: "SupplierProductPurchasingInfos",
                columns: new[] { "TenantId", "ProductId", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProductPurchasingInfos_TenantId_SupplierId_ProductId_PurchaseMeasurementUnitId",
                table: "SupplierProductPurchasingInfos",
                columns: new[] { "TenantId", "SupplierId", "ProductId", "PurchaseMeasurementUnitId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierProductPurchasingInfos");
        }
    }
}
