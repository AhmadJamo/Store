using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPayments", x => x.Id);
                    table.UniqueConstraint("AK_SupplierPayments_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SupplierPayments_PaymentMethods_PaymentMethodId_TenantId",
                        columns: x => new { x.PaymentMethodId, x.TenantId },
                        principalTable: "PaymentMethods",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPayments_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPayments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPaymentLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierPaymentId = table.Column<int>(type: "int", nullable: false),
                    VendorBillId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VendorBillNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierInvoiceNumberSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPaymentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPaymentLines_SupplierPayments_SupplierPaymentId_TenantId",
                        columns: x => new { x.SupplierPaymentId, x.TenantId },
                        principalTable: "SupplierPayments",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierPaymentLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPaymentLines_VendorBills_VendorBillId_TenantId",
                        columns: x => new { x.VendorBillId, x.TenantId },
                        principalTable: "VendorBills",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPaymentLines_SupplierPaymentId_TenantId",
                table: "SupplierPaymentLines",
                columns: new[] { "SupplierPaymentId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPaymentLines_TenantId",
                table: "SupplierPaymentLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPaymentLines_TenantId_SupplierPaymentId_VendorBillId",
                table: "SupplierPaymentLines",
                columns: new[] { "TenantId", "SupplierPaymentId", "VendorBillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPaymentLines_VendorBillId_TenantId",
                table: "SupplierPaymentLines",
                columns: new[] { "VendorBillId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_PaymentMethodId_TenantId",
                table: "SupplierPayments",
                columns: new[] { "PaymentMethodId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_SupplierId_TenantId",
                table: "SupplierPayments",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_TenantId",
                table: "SupplierPayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_TenantId_PaymentNumber",
                table: "SupplierPayments",
                columns: new[] { "TenantId", "PaymentNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierPaymentLines");

            migrationBuilder.DropTable(
                name: "SupplierPayments");
        }
    }
}
