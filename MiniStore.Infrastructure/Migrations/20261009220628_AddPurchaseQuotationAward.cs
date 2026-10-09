using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseQuotationAward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseQuotationAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AwardedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AwardedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseQuotationAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwards_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwards_SupplierQuotations_SupplierQuotationId_TenantId",
                        columns: x => new { x.SupplierQuotationId, x.TenantId },
                        principalTable: "SupplierQuotations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseQuotationAwards_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwards_PurchaseSourcingEventId_TenantId",
                table: "PurchaseQuotationAwards",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwards_SupplierQuotationId_TenantId",
                table: "PurchaseQuotationAwards",
                columns: new[] { "SupplierQuotationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwards_TenantId",
                table: "PurchaseQuotationAwards",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwards_TenantId_PurchaseSourcingEventId",
                table: "PurchaseQuotationAwards",
                columns: new[] { "TenantId", "PurchaseSourcingEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseQuotationAwards_TenantId_SupplierQuotationId",
                table: "PurchaseQuotationAwards",
                columns: new[] { "TenantId", "SupplierQuotationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseQuotationAwards");
        }
    }
}
