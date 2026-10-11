using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseAgreementsAndTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_TenantId_SupplierQuotationId",
                table: "PurchaseOrders");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseAgreementId",
                table: "PurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseAgreementLineId",
                table: "PurchaseOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchaseAgreements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgreementNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PaymentTermsSnapshot = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ActivatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseAgreements", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseAgreements_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseAgreements_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreements_SupplierQuotations_SupplierQuotationId_TenantId",
                        columns: x => new { x.SupplierQuotationId, x.TenantId },
                        principalTable: "SupplierQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreements_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreements_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SourcePurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    NeededInDays = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    GenerationCount = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseTemplates", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseTemplates_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseTemplates_PurchaseOrders_SourcePurchaseOrderId_TenantId",
                        columns: x => new { x.SourcePurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplates_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplates_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseAgreementLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseAgreementId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    MaximumQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseAgreementLines", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseAgreementLines_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseAgreementLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreementLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreementLines_PurchaseAgreements_PurchaseAgreementId_TenantId",
                        columns: x => new { x.PurchaseAgreementId, x.TenantId },
                        principalTable: "PurchaseAgreements",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreementLines_SupplierQuotationLines_SupplierQuotationLineId_TenantId",
                        columns: x => new { x.SupplierQuotationLineId, x.TenantId },
                        principalTable: "SupplierQuotationLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseAgreementLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseTemplateLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseTemplateId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitFactorToBase = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    SuggestedSupplierId = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseTemplateLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplateLines_MeasurementUnits_MeasurementUnitId_TenantId",
                        columns: x => new { x.MeasurementUnitId, x.TenantId },
                        principalTable: "MeasurementUnits",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplateLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplateLines_PurchaseTemplates_PurchaseTemplateId_TenantId",
                        columns: x => new { x.PurchaseTemplateId, x.TenantId },
                        principalTable: "PurchaseTemplates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplateLines_Suppliers_SuggestedSupplierId_TenantId",
                        columns: x => new { x.SuggestedSupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTemplateLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PurchaseAgreementId_TenantId",
                table: "PurchaseOrders",
                columns: new[] { "PurchaseAgreementId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_TenantId_SupplierQuotationId",
                table: "PurchaseOrders",
                columns: new[] { "TenantId", "SupplierQuotationId" },
                unique: true,
                filter: "[PurchaseAgreementId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PurchaseAgreementLineId_TenantId",
                table: "PurchaseOrderLines",
                columns: new[] { "PurchaseAgreementLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreementLines_MeasurementUnitId_TenantId",
                table: "PurchaseAgreementLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreementLines_ProductId_TenantId",
                table: "PurchaseAgreementLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreementLines_PurchaseAgreementId_TenantId",
                table: "PurchaseAgreementLines",
                columns: new[] { "PurchaseAgreementId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreementLines_SupplierQuotationLineId_TenantId",
                table: "PurchaseAgreementLines",
                columns: new[] { "SupplierQuotationLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreementLines_TenantId",
                table: "PurchaseAgreementLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreementLines_TenantId_PurchaseAgreementId_SupplierQuotationLineId",
                table: "PurchaseAgreementLines",
                columns: new[] { "TenantId", "PurchaseAgreementId", "SupplierQuotationLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_PurchaseSourcingEventId_TenantId",
                table: "PurchaseAgreements",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_SupplierId_TenantId",
                table: "PurchaseAgreements",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_SupplierQuotationId_TenantId",
                table: "PurchaseAgreements",
                columns: new[] { "SupplierQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_TenantId",
                table: "PurchaseAgreements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_TenantId_AgreementNumber",
                table: "PurchaseAgreements",
                columns: new[] { "TenantId", "AgreementNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_TenantId_SupplierQuotationId",
                table: "PurchaseAgreements",
                columns: new[] { "TenantId", "SupplierQuotationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseAgreements_WarehouseId_TenantId",
                table: "PurchaseAgreements",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplateLines_MeasurementUnitId_TenantId",
                table: "PurchaseTemplateLines",
                columns: new[] { "MeasurementUnitId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplateLines_ProductId_TenantId",
                table: "PurchaseTemplateLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplateLines_PurchaseTemplateId_TenantId",
                table: "PurchaseTemplateLines",
                columns: new[] { "PurchaseTemplateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplateLines_SuggestedSupplierId_TenantId",
                table: "PurchaseTemplateLines",
                columns: new[] { "SuggestedSupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplateLines_TenantId",
                table: "PurchaseTemplateLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplateLines_TenantId_PurchaseTemplateId_ProductId_MeasurementUnitId",
                table: "PurchaseTemplateLines",
                columns: new[] { "TenantId", "PurchaseTemplateId", "ProductId", "MeasurementUnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplates_SourcePurchaseOrderId_TenantId",
                table: "PurchaseTemplates",
                columns: new[] { "SourcePurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplates_SupplierId_TenantId",
                table: "PurchaseTemplates",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplates_TenantId",
                table: "PurchaseTemplates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplates_TenantId_TemplateNumber",
                table: "PurchaseTemplates",
                columns: new[] { "TenantId", "TemplateNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTemplates_WarehouseId_TenantId",
                table: "PurchaseTemplates",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_PurchaseAgreementLines_PurchaseAgreementLineId_TenantId",
                table: "PurchaseOrderLines",
                columns: new[] { "PurchaseAgreementLineId", "TenantId" },
                principalTable: "PurchaseAgreementLines",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_PurchaseAgreements_PurchaseAgreementId_TenantId",
                table: "PurchaseOrders",
                columns: new[] { "PurchaseAgreementId", "TenantId" },
                principalTable: "PurchaseAgreements",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderLines_PurchaseAgreementLines_PurchaseAgreementLineId_TenantId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_PurchaseAgreements_PurchaseAgreementId_TenantId",
                table: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "PurchaseAgreementLines");

            migrationBuilder.DropTable(
                name: "PurchaseTemplateLines");

            migrationBuilder.DropTable(
                name: "PurchaseAgreements");

            migrationBuilder.DropTable(
                name: "PurchaseTemplates");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_PurchaseAgreementId_TenantId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_TenantId_SupplierQuotationId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderLines_PurchaseAgreementLineId_TenantId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "PurchaseAgreementId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PurchaseAgreementLineId",
                table: "PurchaseOrderLines");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_TenantId_SupplierQuotationId",
                table: "PurchaseOrders",
                columns: new[] { "TenantId", "SupplierQuotationId" },
                unique: true);
        }
    }
}
