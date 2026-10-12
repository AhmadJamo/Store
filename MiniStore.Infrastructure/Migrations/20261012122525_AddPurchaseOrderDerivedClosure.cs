using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderDerivedClosure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAtUtc",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedByUserId",
                table: "PurchaseOrders",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosureReason",
                table: "PurchaseOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosureType",
                table: "PurchaseOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLifecycleHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLifecycleHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLifecycleHistories_PurchaseOrders_PurchaseOrderId_TenantId",
                        columns: x => new { x.PurchaseOrderId, x.TenantId },
                        principalTable: "PurchaseOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLifecycleHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLifecycleHistories_PurchaseOrderId_CreatedAtUtc",
                table: "PurchaseOrderLifecycleHistories",
                columns: new[] { "PurchaseOrderId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLifecycleHistories_PurchaseOrderId_TenantId",
                table: "PurchaseOrderLifecycleHistories",
                columns: new[] { "PurchaseOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLifecycleHistories_TenantId",
                table: "PurchaseOrderLifecycleHistories",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderLifecycleHistories");

            migrationBuilder.DropColumn(
                name: "ClosedAtUtc",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ClosureReason",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ClosureType",
                table: "PurchaseOrders");
        }
    }
}
