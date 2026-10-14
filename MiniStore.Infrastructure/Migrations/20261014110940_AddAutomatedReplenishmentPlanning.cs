using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomatedReplenishmentPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoverageDays",
                table: "ReplenishmentRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanningMethod",
                table: "ReplenishmentRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ReplenishmentRules",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "UseSeasonality",
                table: "ReplenishmentRules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ReplenishmentRules_Id_TenantId",
                table: "ReplenishmentRules",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateTable(
                name: "ReplenishmentAutomationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    FrequencyHours = table.Column<int>(type: "int", nullable: false),
                    ForecastLookbackDays = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    SupplierResponseDays = table.Column<int>(type: "int", nullable: false),
                    LastRunAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRunAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenishmentAutomationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplenishmentAutomationSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReplenishmentRuns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduledForUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TriggeredByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenishmentRuns", x => x.Id);
                    table.UniqueConstraint("AK_ReplenishmentRuns_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_ReplenishmentRuns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReplenishmentSuggestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReplenishmentRunId = table.Column<long>(type: "bigint", nullable: false),
                    ReplenishmentRuleId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    ProductCodeSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IncomingQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    HistoricalDemandQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    SeasonalFactor = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TargetQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    SuggestedStockQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    SuggestedPurchaseQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PurchaseUnitSymbolSnapshot = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NeededByDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: true),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: true),
                    ReasonCode = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenishmentSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_PurchaseRequests_PurchaseRequestId_TenantId",
                        columns: x => new { x.PurchaseRequestId, x.TenantId },
                        principalTable: "PurchaseRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_ReplenishmentRules_ReplenishmentRuleId_TenantId",
                        columns: x => new { x.ReplenishmentRuleId, x.TenantId },
                        principalTable: "ReplenishmentRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_ReplenishmentRuns_ReplenishmentRunId_TenantId",
                        columns: x => new { x.ReplenishmentRunId, x.TenantId },
                        principalTable: "ReplenishmentRuns",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_Suppliers_SupplierId_TenantId",
                        columns: x => new { x.SupplierId, x.TenantId },
                        principalTable: "Suppliers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplenishmentSuggestions_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentAutomationSettings_TenantId",
                table: "ReplenishmentAutomationSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentAutomationSettings_TenantId_SingletonKey",
                table: "ReplenishmentAutomationSettings",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentRuns_TenantId",
                table: "ReplenishmentRuns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentRuns_TenantId_ScheduledForUtc",
                table: "ReplenishmentRuns",
                columns: new[] { "TenantId", "ScheduledForUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_ProductId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_PurchaseRequestId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "PurchaseRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_PurchaseSourcingEventId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_ReplenishmentRuleId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "ReplenishmentRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_ReplenishmentRunId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "ReplenishmentRunId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_SupplierId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_TenantId",
                table: "ReplenishmentSuggestions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentSuggestions_WarehouseId_TenantId",
                table: "ReplenishmentSuggestions",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplenishmentAutomationSettings");

            migrationBuilder.DropTable(
                name: "ReplenishmentSuggestions");

            migrationBuilder.DropTable(
                name: "ReplenishmentRuns");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ReplenishmentRules_Id_TenantId",
                table: "ReplenishmentRules");

            migrationBuilder.DropColumn(
                name: "CoverageDays",
                table: "ReplenishmentRules");

            migrationBuilder.DropColumn(
                name: "PlanningMethod",
                table: "ReplenishmentRules");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ReplenishmentRules");

            migrationBuilder.DropColumn(
                name: "UseSeasonality",
                table: "ReplenishmentRules");
        }
    }
}
