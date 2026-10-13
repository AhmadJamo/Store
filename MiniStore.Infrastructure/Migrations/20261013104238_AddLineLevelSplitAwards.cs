using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLineLevelSplitAwards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_PurchaseQuotationAwards_Id_TenantId",
                table: "PurchaseQuotationAwards",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "PurchaseQuotationAwardAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseQuotationAwardId = table.Column<int>(type: "int", nullable: false),
                    PurchaseSourcingLineId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationLineId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    AwardedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountPercentSnapshot = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    TaxPercentSnapshot = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    CurrencyCodeSnapshot = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    LeadTimeDaysSnapshot = table.Column<int>(type: "int", nullable: false),
                    PaymentTermsSnapshot = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseQuotationAwardAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwardAllocations_PurchaseQuotationAwards_PurchaseQuotationAwardId_TenantId",
                        columns: x => new { x.PurchaseQuotationAwardId, x.TenantId },
                        principalTable: "PurchaseQuotationAwards",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwardAllocations_PurchaseSourcingLines_PurchaseSourcingLineId_TenantId",
                        columns: x => new { x.PurchaseSourcingLineId, x.TenantId },
                        principalTable: "PurchaseSourcingLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwardAllocations_SupplierQuotationLines_SupplierQuotationLineId_TenantId",
                        columns: x => new { x.SupplierQuotationLineId, x.TenantId },
                        principalTable: "SupplierQuotationLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwardAllocations_SupplierQuotations_SupplierQuotationId_TenantId",
                        columns: x => new { x.SupplierQuotationId, x.TenantId },
                        principalTable: "SupplierQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwardAllocations_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwardAllocations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO PurchaseQuotationAwardAllocations
                    (PurchaseQuotationAwardId, PurchaseSourcingLineId, SupplierQuotationId,
                     SupplierQuotationLineId, SupplierId, AwardedQuantity, UnitPriceSnapshot,
                     DiscountPercentSnapshot, TaxPercentSnapshot, CurrencyCodeSnapshot,
                     LeadTimeDaysSnapshot, PaymentTermsSnapshot, Reason, ProductCodeSnapshot,
                     ProductNameSnapshot, UnitNameSnapshot, TenantId)
                SELECT award.Id, line.PurchaseSourcingLineId, quotation.Id, line.Id,
                       quotation.SupplierId, line.QuotedQuantity, line.UnitPrice,
                       line.DiscountPercent, line.TaxPercent, quotation.CurrencyCode,
                       quotation.LeadTimeDays, quotation.PaymentTermsSnapshot, award.Reason,
                       line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot,
                       award.TenantId
                FROM PurchaseQuotationAwards award
                INNER JOIN SupplierQuotations quotation
                    ON quotation.Id = award.SupplierQuotationId
                   AND quotation.TenantId = award.TenantId
                INNER JOIN SupplierQuotationLines line
                    ON line.SupplierQuotationId = quotation.Id
                   AND line.TenantId = quotation.TenantId;

                UPDATE sourcing
                SET Status = 3
                FROM PurchaseSourcingEvents sourcing
                INNER JOIN PurchaseQuotationAwards award
                    ON award.PurchaseSourcingEventId = sourcing.Id
                   AND award.TenantId = sourcing.TenantId
                WHERE sourcing.Status = 2;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_PurchaseQuotationAwardId_TenantId",
                table: "PurchaseQuotationAwardAllocations",
                columns: new[] { "PurchaseQuotationAwardId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_PurchaseSourcingLineId_TenantId",
                table: "PurchaseQuotationAwardAllocations",
                columns: new[] { "PurchaseSourcingLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_SupplierId_TenantId",
                table: "PurchaseQuotationAwardAllocations",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_SupplierQuotationId_TenantId",
                table: "PurchaseQuotationAwardAllocations",
                columns: new[] { "SupplierQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_SupplierQuotationLineId_TenantId",
                table: "PurchaseQuotationAwardAllocations",
                columns: new[] { "SupplierQuotationLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_TenantId",
                table: "PurchaseQuotationAwardAllocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwardAllocations_TenantId_PurchaseQuotationAwardId_SupplierQuotationLineId",
                table: "PurchaseQuotationAwardAllocations",
                columns: new[] { "TenantId", "PurchaseQuotationAwardId", "SupplierQuotationLineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE sourcing
                SET Status = 2
                FROM PurchaseSourcingEvents sourcing
                INNER JOIN PurchaseQuotationAwards award
                    ON award.PurchaseSourcingEventId = sourcing.Id
                   AND award.TenantId = sourcing.TenantId
                WHERE sourcing.Status = 3;
                """);

            migrationBuilder.DropTable(
                name: "PurchaseQuotationAwardAllocations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PurchaseQuotationAwards_Id_TenantId",
                table: "PurchaseQuotationAwards");
        }
    }
}
