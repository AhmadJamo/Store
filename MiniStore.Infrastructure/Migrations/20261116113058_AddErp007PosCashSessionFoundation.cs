using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddErp007PosCashSessionFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosCashDrawers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    PosTerminalId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashDrawers", x => x.Id);
                    table.UniqueConstraint("AK_PosCashDrawers_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PosCashDrawers_Branches_BranchId_TenantId",
                        columns: x => new { x.BranchId, x.TenantId },
                        principalTable: "Branches",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashDrawers_PosTerminals_PosTerminalId_TenantId",
                        columns: x => new { x.PosTerminalId, x.TenantId },
                        principalTable: "PosTerminals",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashDrawers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosCashPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RequireBlindClose = table.Column<bool>(type: "bit", nullable: false),
                    RequireApprovalForAnyVariance = table.Column<bool>(type: "bit", nullable: false),
                    VarianceTolerance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MandatoryApprovalThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosCashPolicies_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosCashSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    PosTerminalId = table.Column<int>(type: "int", nullable: false),
                    PosCashDrawerId = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    OpenedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpeningFloat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpectedCash = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DeclaredCash = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Variance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ClosedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashSessions", x => x.Id);
                    table.UniqueConstraint("AK_PosCashSessions_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PosCashSessions_Branches_BranchId_TenantId",
                        columns: x => new { x.BranchId, x.TenantId },
                        principalTable: "Branches",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashSessions_PosCashDrawers_PosCashDrawerId_TenantId",
                        columns: x => new { x.PosCashDrawerId, x.TenantId },
                        principalTable: "PosCashDrawers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashSessions_PosTerminals_PosTerminalId_TenantId",
                        columns: x => new { x.PosTerminalId, x.TenantId },
                        principalTable: "PosTerminals",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashSessions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosCashCounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PosCashSessionId = table.Column<long>(type: "bigint", nullable: false),
                    DeclaredCash = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CountedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    WasBlind = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosCashCounts_PosCashSessions_PosCashSessionId_TenantId",
                        columns: x => new { x.PosCashSessionId, x.TenantId },
                        principalTable: "PosCashSessions",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashCounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosCashMovements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PosCashSessionId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosCashMovements_PosCashSessions_PosCashSessionId_TenantId",
                        columns: x => new { x.PosCashSessionId, x.TenantId },
                        principalTable: "PosCashSessions",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosCashMovements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashCounts_PosCashSessionId_TenantId",
                table: "PosCashCounts",
                columns: new[] { "PosCashSessionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashCounts_TenantId",
                table: "PosCashCounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashCounts_TenantId_PosCashSessionId_CountedAtUtc",
                table: "PosCashCounts",
                columns: new[] { "TenantId", "PosCashSessionId", "CountedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashDrawers_BranchId_TenantId",
                table: "PosCashDrawers",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashDrawers_PosTerminalId_TenantId",
                table: "PosCashDrawers",
                columns: new[] { "PosTerminalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashDrawers_TenantId",
                table: "PosCashDrawers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashDrawers_TenantId_BranchId_PosTerminalId_IsActive",
                table: "PosCashDrawers",
                columns: new[] { "TenantId", "BranchId", "PosTerminalId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashDrawers_TenantId_Code",
                table: "PosCashDrawers",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosCashMovements_PosCashSessionId_TenantId",
                table: "PosCashMovements",
                columns: new[] { "PosCashSessionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashMovements_TenantId",
                table: "PosCashMovements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashMovements_TenantId_PosCashSessionId_OccurredAtUtc",
                table: "PosCashMovements",
                columns: new[] { "TenantId", "PosCashSessionId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashPolicies_TenantId",
                table: "PosCashPolicies",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_BranchId_TenantId",
                table: "PosCashSessions",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_PosCashDrawerId_TenantId",
                table: "PosCashSessions",
                columns: new[] { "PosCashDrawerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_PosTerminalId_TenantId",
                table: "PosCashSessions",
                columns: new[] { "PosTerminalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_TenantId",
                table: "PosCashSessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_TenantId_BranchId_OpenedAtUtc",
                table: "PosCashSessions",
                columns: new[] { "TenantId", "BranchId", "OpenedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashSessions_TenantId_PosTerminalId_Status",
                table: "PosCashSessions",
                columns: new[] { "TenantId", "PosTerminalId", "Status" },
                unique: true,
                filter: "[Status] IN (1, 2, 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosCashCounts");

            migrationBuilder.DropTable(
                name: "PosCashMovements");

            migrationBuilder.DropTable(
                name: "PosCashPolicies");

            migrationBuilder.DropTable(
                name: "PosCashSessions");

            migrationBuilder.DropTable(
                name: "PosCashDrawers");
        }
    }
}
