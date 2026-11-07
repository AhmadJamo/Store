using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringJournalTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurringJournalTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRunDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastRunAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRunDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastJournalEntryId = table.Column<int>(type: "int", nullable: true),
                    LastRunKey = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringJournalTemplates", x => x.Id);
                    table.UniqueConstraint("AK_RecurringJournalTemplates_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_RecurringJournalTemplates_DateRange", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_RecurringJournalTemplates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringJournalTemplateLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecurringJournalTemplateId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringJournalTemplateLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringJournalTemplateLines_Accounts_AccountId_TenantId",
                        columns: x => new { x.AccountId, x.TenantId },
                        principalTable: "Accounts",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringJournalTemplateLines_Branches_BranchId_TenantId",
                        columns: x => new { x.BranchId, x.TenantId },
                        principalTable: "Branches",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringJournalTemplateLines_RecurringJournalTemplates_RecurringJournalTemplateId_TenantId",
                        columns: x => new { x.RecurringJournalTemplateId, x.TenantId },
                        principalTable: "RecurringJournalTemplates",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringJournalTemplateLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringJournalTemplateLines_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringJournalTemplateLines_AccountId_TenantId",
                table: "RecurringJournalTemplateLines",
                columns: new[] { "AccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringJournalTemplateLines_BranchId_TenantId",
                table: "RecurringJournalTemplateLines",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringJournalTemplateLines_RecurringJournalTemplateId_TenantId",
                table: "RecurringJournalTemplateLines",
                columns: new[] { "RecurringJournalTemplateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringJournalTemplateLines_TenantId",
                table: "RecurringJournalTemplateLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringJournalTemplateLines_WarehouseId_TenantId",
                table: "RecurringJournalTemplateLines",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringJournalTemplates_TenantId",
                table: "RecurringJournalTemplates",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringJournalTemplateLines");

            migrationBuilder.DropTable(
                name: "RecurringJournalTemplates");
        }
    }
}
