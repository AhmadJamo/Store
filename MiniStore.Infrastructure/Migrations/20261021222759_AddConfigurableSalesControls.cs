using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurableSalesControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesApprovalRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Metric = table.Column<int>(type: "int", nullable: false),
                    Comparison = table.Column<int>(type: "int", nullable: false),
                    Threshold = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    AllowRequesterApproval = table.Column<bool>(type: "bit", nullable: false),
                    RequireDistinctApprovers = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesApprovalRules", x => x.Id);
                    table.UniqueConstraint("AK_SalesApprovalRules_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.CheckConstraint("CK_SalesApprovalRules_ThresholdPriority", "[Threshold] >= 0 AND [Priority] BETWEEN 1 AND 9999");
                    table.ForeignKey(
                        name: "FK_SalesApprovalRules_Customers_CustomerId_TenantId",
                        columns: x => new { x.CustomerId, x.TenantId },
                        principalTable: "Customers",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesApprovalRules_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllowRequesterApproval = table.Column<bool>(type: "bit", nullable: false),
                    RequireDistinctApprovers = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderApprovals", x => x.Id);
                    table.UniqueConstraint("AK_SalesOrderApprovals_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovals_SalesOrders_SalesOrderId_TenantId",
                        columns: x => new { x.SalesOrderId, x.TenantId },
                        principalTable: "SalesOrders",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovals_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesApprovalRuleSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesApprovalRuleId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ApproverRoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesApprovalRuleSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesApprovalRuleSteps_SalesApprovalRules_SalesApprovalRuleId_TenantId",
                        columns: x => new { x.SalesApprovalRuleId, x.TenantId },
                        principalTable: "SalesApprovalRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesApprovalRuleSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderApprovalFindings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderApprovalId = table.Column<int>(type: "int", nullable: false),
                    SalesApprovalRuleId = table.Column<int>(type: "int", nullable: false),
                    RuleNameSnapshot = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Metric = table.Column<int>(type: "int", nullable: false),
                    Comparison = table.Column<int>(type: "int", nullable: false),
                    Threshold = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderApprovalFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovalFindings_SalesApprovalRules_SalesApprovalRuleId_TenantId",
                        columns: x => new { x.SalesApprovalRuleId, x.TenantId },
                        principalTable: "SalesApprovalRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovalFindings_SalesOrderApprovals_SalesOrderApprovalId_TenantId",
                        columns: x => new { x.SalesOrderApprovalId, x.TenantId },
                        principalTable: "SalesOrderApprovals",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovalFindings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderApprovalId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ApproverRoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DecidedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DecidedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovalSteps_SalesOrderApprovals_SalesOrderApprovalId_TenantId",
                        columns: x => new { x.SalesOrderApprovalId, x.TenantId },
                        principalTable: "SalesOrderApprovals",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderApprovalSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesApprovalRules_CustomerId_TenantId",
                table: "SalesApprovalRules",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesApprovalRules_IsActive_Priority_Metric",
                table: "SalesApprovalRules",
                columns: new[] { "IsActive", "Priority", "Metric" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesApprovalRules_TenantId",
                table: "SalesApprovalRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesApprovalRuleSteps_SalesApprovalRuleId_TenantId",
                table: "SalesApprovalRuleSteps",
                columns: new[] { "SalesApprovalRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesApprovalRuleSteps_TenantId",
                table: "SalesApprovalRuleSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesApprovalRuleSteps_TenantId_SalesApprovalRuleId_Sequence",
                table: "SalesApprovalRuleSteps",
                columns: new[] { "TenantId", "SalesApprovalRuleId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovalFindings_SalesApprovalRuleId_TenantId",
                table: "SalesOrderApprovalFindings",
                columns: new[] { "SalesApprovalRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovalFindings_SalesOrderApprovalId_TenantId",
                table: "SalesOrderApprovalFindings",
                columns: new[] { "SalesOrderApprovalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovalFindings_TenantId",
                table: "SalesOrderApprovalFindings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovals_SalesOrderId_RequestedAtUtc",
                table: "SalesOrderApprovals",
                columns: new[] { "SalesOrderId", "RequestedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovals_SalesOrderId_TenantId",
                table: "SalesOrderApprovals",
                columns: new[] { "SalesOrderId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovals_TenantId",
                table: "SalesOrderApprovals",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovals_TenantId_SalesOrderId",
                table: "SalesOrderApprovals",
                columns: new[] { "TenantId", "SalesOrderId" },
                unique: true,
                filter: "[Status] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovalSteps_SalesOrderApprovalId_TenantId",
                table: "SalesOrderApprovalSteps",
                columns: new[] { "SalesOrderApprovalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovalSteps_TenantId",
                table: "SalesOrderApprovalSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderApprovalSteps_TenantId_SalesOrderApprovalId_Sequence",
                table: "SalesOrderApprovalSteps",
                columns: new[] { "TenantId", "SalesOrderApprovalId", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesApprovalRuleSteps");

            migrationBuilder.DropTable(
                name: "SalesOrderApprovalFindings");

            migrationBuilder.DropTable(
                name: "SalesOrderApprovalSteps");

            migrationBuilder.DropTable(
                name: "SalesApprovalRules");

            migrationBuilder.DropTable(
                name: "SalesOrderApprovals");
        }
    }
}
