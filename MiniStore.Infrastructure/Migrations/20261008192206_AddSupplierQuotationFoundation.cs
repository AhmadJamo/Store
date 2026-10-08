using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierQuotationFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_PurchaseSourcingLines_Id_TenantId",
                table: "PurchaseSourcingLines",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "SupplierQuotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SupplierReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuotationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidUntilDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PaymentTermsSnapshot = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuotations", x => x.Id);
                    table.UniqueConstraint("AK_SupplierQuotations_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SupplierQuotations_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotations_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierQuotationLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierQuotationId = table.Column<int>(type: "int", nullable: false),
                    PurchaseSourcingLineId = table.Column<int>(type: "int", nullable: false),
                    QuotedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
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
                    table.PrimaryKey("PK_SupplierQuotationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationLines_PurchaseSourcingLines_PurchaseSourcingLineId_TenantId",
                        columns: x => new { x.PurchaseSourcingLineId, x.TenantId },
                        principalTable: "PurchaseSourcingLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationLines_SupplierQuotations_SupplierQuotationId_TenantId",
                        columns: x => new { x.SupplierQuotationId, x.TenantId },
                        principalTable: "SupplierQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationLines_PurchaseSourcingLineId_TenantId",
                table: "SupplierQuotationLines",
                columns: new[] { "PurchaseSourcingLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationLines_SupplierQuotationId_TenantId",
                table: "SupplierQuotationLines",
                columns: new[] { "SupplierQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationLines_TenantId",
                table: "SupplierQuotationLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationLines_TenantId_SupplierQuotationId_PurchaseSourcingLineId",
                table: "SupplierQuotationLines",
                columns: new[] { "TenantId", "SupplierQuotationId", "PurchaseSourcingLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_PurchaseSourcingEventId_TenantId",
                table: "SupplierQuotations",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_SupplierId_TenantId",
                table: "SupplierQuotations",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_TenantId",
                table: "SupplierQuotations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_TenantId_PurchaseSourcingEventId_SupplierId",
                table: "SupplierQuotations",
                columns: new[] { "TenantId", "PurchaseSourcingEventId", "SupplierId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotations_TenantId_QuotationNumber",
                table: "SupplierQuotations",
                columns: new[] { "TenantId", "QuotationNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierQuotationLines");

            migrationBuilder.DropTable(
                name: "SupplierQuotations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PurchaseSourcingLines_Id_TenantId",
                table: "PurchaseSourcingLines");
        }
    }
}
