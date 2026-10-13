using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseServiceBillingPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingPolicySnapshot",
                table: "PurchaseOrderLines",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseExpenseAccountIdSnapshot",
                table: "PurchaseOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseBillingPolicy",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseExpenseAccountId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchaseServiceAcceptances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcceptanceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseServiceAcceptances", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseServiceAcceptances_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseServiceAcceptances_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseServiceAcceptances_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseServiceAcceptanceLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseServiceAcceptanceId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CompletedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseServiceAcceptanceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseServiceAcceptanceLines_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseServiceAcceptanceLines_PurchaseOrderLines_PurchaseOrderLineId_TenantId",
                        columns: x => new { x.PurchaseOrderLineId, x.TenantId },
                        principalTable: "PurchaseOrderLines",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseServiceAcceptanceLines_PurchaseServiceAcceptances_PurchaseServiceAcceptanceId_TenantId",
                        columns: x => new { x.PurchaseServiceAcceptanceId, x.TenantId },
                        principalTable: "PurchaseServiceAcceptances",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseServiceAcceptanceLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "PurchaseOrderLines",
                columns: new[] { "PurchaseExpenseAccountIdSnapshot", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_PurchaseExpenseAccountId_TenantId",
                table: "Products",
                columns: new[] { "PurchaseExpenseAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptanceLines_ProductId_TenantId",
                table: "PurchaseServiceAcceptanceLines",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptanceLines_PurchaseOrderLineId_TenantId",
                table: "PurchaseServiceAcceptanceLines",
                columns: new[] { "PurchaseOrderLineId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptanceLines_PurchaseServiceAcceptanceId_TenantId",
                table: "PurchaseServiceAcceptanceLines",
                columns: new[] { "PurchaseServiceAcceptanceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptanceLines_TenantId",
                table: "PurchaseServiceAcceptanceLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptanceLines_TenantId_PurchaseServiceAcceptanceId_PurchaseOrderLineId",
                table: "PurchaseServiceAcceptanceLines",
                columns: new[] { "TenantId", "PurchaseServiceAcceptanceId", "PurchaseOrderLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptances_PurchaseOrderId_TenantId",
                table: "PurchaseServiceAcceptances",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptances_TenantId",
                table: "PurchaseServiceAcceptances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseServiceAcceptances_TenantId_AcceptanceNumber",
                table: "PurchaseServiceAcceptances",
                columns: new[] { "TenantId", "AcceptanceNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Accounts_PurchaseExpenseAccountId_TenantId",
                table: "Products",
                columns: new[] { "PurchaseExpenseAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_Accounts_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "PurchaseOrderLines",
                columns: new[] { "PurchaseExpenseAccountIdSnapshot", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Accounts_PurchaseExpenseAccountId_TenantId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderLines_Accounts_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "PurchaseServiceAcceptanceLines");

            migrationBuilder.DropTable(
                name: "PurchaseServiceAcceptances");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderLines_PurchaseExpenseAccountIdSnapshot_TenantId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_Products_PurchaseExpenseAccountId_TenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BillingPolicySnapshot",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "PurchaseExpenseAccountIdSnapshot",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "PurchaseBillingPolicy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchaseExpenseAccountId",
                table: "Products");
        }
    }
}
