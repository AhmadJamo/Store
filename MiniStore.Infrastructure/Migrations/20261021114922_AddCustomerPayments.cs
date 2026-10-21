using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodAccountIdSnapshot = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CompanyCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(24,12)", precision: 24, scale: 12, nullable: false),
                    ExchangeRateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExchangeRateSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PostedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReversedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReversedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReversalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReversalReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPayments", x => x.Id);
                    table.UniqueConstraint("AK_CustomerPayments_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_CustomerPayments_Accounts_PaymentMethodAccountIdSnapshot_TenantId",
                        columns: x => new { x.PaymentMethodAccountIdSnapshot, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_PaymentMethods_PaymentMethodId_TenantId",
                        columns: x => new { x.PaymentMethodId, x.TenantId },
                        principalTable: "PaymentMethods",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPaymentLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerPaymentId = table.Column<int>(type: "int", nullable: false),
                    CustomerInvoiceId = table.Column<int>(type: "int", nullable: false),
                    ReceivableAccountIdSnapshot = table.Column<int>(type: "int", nullable: false),
                    CustomerInvoiceNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompanyCurrencyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceivableCarryingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPaymentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPaymentLines_Accounts_ReceivableAccountIdSnapshot_TenantId",
                        columns: x => new { x.ReceivableAccountIdSnapshot, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPaymentLines_CustomerInvoices_CustomerInvoiceId_TenantId",
                        columns: x => new { x.CustomerInvoiceId, x.TenantId },
                        principalTable: "CustomerInvoices",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPaymentLines_CustomerPayments_CustomerPaymentId_TenantId",
                        columns: x => new { x.CustomerPaymentId, x.TenantId },
                        principalTable: "CustomerPayments",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerPaymentLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentLines_CustomerInvoiceId_TenantId",
                table: "CustomerPaymentLines",
                columns: new[] { "CustomerInvoiceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentLines_CustomerPaymentId_TenantId",
                table: "CustomerPaymentLines",
                columns: new[] { "CustomerPaymentId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentLines_ReceivableAccountIdSnapshot_TenantId",
                table: "CustomerPaymentLines",
                columns: new[] { "ReceivableAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentLines_TenantId",
                table: "CustomerPaymentLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentLines_TenantId_CustomerPaymentId_CustomerInvoiceId",
                table: "CustomerPaymentLines",
                columns: new[] { "TenantId", "CustomerPaymentId", "CustomerInvoiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_CustomerId_TenantId",
                table: "CustomerPayments",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_PaymentMethodAccountIdSnapshot_TenantId",
                table: "CustomerPayments",
                columns: new[] { "PaymentMethodAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_PaymentMethodId_TenantId",
                table: "CustomerPayments",
                columns: new[] { "PaymentMethodId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_TenantId",
                table: "CustomerPayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_TenantId_PaymentNumber",
                table: "CustomerPayments",
                columns: new[] { "TenantId", "PaymentNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPaymentLines");

            migrationBuilder.DropTable(
                name: "CustomerPayments");
        }
    }
}
