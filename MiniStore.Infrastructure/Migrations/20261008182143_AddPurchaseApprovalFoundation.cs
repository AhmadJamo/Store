using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseApprovalFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseApprovalRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    MinimumPriority = table.Column<int>(type: "int", nullable: false),
                    MaximumPriority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseApprovalRules", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseApprovalRules_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalRules_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalRules_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseApprovalInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    PurchaseApprovalRuleId = table.Column<int>(type: "int", nullable: false),
                    RuleNameSnapshot = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseApprovalInstances", x => x.Id);
                    table.UniqueConstraint("AK_PurchaseApprovalInstances_Id_TenantId", x => new { x.Id, x.TenantId });
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalInstances_PurchaseApprovalRules_PurchaseApprovalRuleId_TenantId",
                        columns: x => new { x.PurchaseApprovalRuleId, x.TenantId },
                        principalTable: "PurchaseApprovalRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalInstances_PurchaseRequests_PurchaseRequestId_TenantId",
                        columns: x => new { x.PurchaseRequestId, x.TenantId },
                        principalTable: "PurchaseRequests",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalInstances_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseApprovalRuleSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseApprovalRuleId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ApproverRoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseApprovalRuleSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalRuleSteps_PurchaseApprovalRules_PurchaseApprovalRuleId_TenantId",
                        columns: x => new { x.PurchaseApprovalRuleId, x.TenantId },
                        principalTable: "PurchaseApprovalRules",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalRuleSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseApprovalInstanceId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PurchaseApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalSteps_PurchaseApprovalInstances_PurchaseApprovalInstanceId_TenantId",
                        columns: x => new { x.PurchaseApprovalInstanceId, x.TenantId },
                        principalTable: "PurchaseApprovalInstances",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseApprovalSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalInstances_PurchaseApprovalRuleId_TenantId",
                table: "PurchaseApprovalInstances",
                columns: new[] { "PurchaseApprovalRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalInstances_PurchaseRequestId_TenantId",
                table: "PurchaseApprovalInstances",
                columns: new[] { "PurchaseRequestId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalInstances_TenantId",
                table: "PurchaseApprovalInstances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalInstances_TenantId_PurchaseRequestId",
                table: "PurchaseApprovalInstances",
                columns: new[] { "TenantId", "PurchaseRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalRules_TenantId",
                table: "PurchaseApprovalRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalRules_WarehouseId_MinimumPriority_MaximumPriority_IsActive",
                table: "PurchaseApprovalRules",
                columns: new[] { "WarehouseId", "MinimumPriority", "MaximumPriority", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalRules_WarehouseId_TenantId",
                table: "PurchaseApprovalRules",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalRuleSteps_PurchaseApprovalRuleId_TenantId",
                table: "PurchaseApprovalRuleSteps",
                columns: new[] { "PurchaseApprovalRuleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalRuleSteps_TenantId",
                table: "PurchaseApprovalRuleSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalRuleSteps_TenantId_PurchaseApprovalRuleId_Sequence",
                table: "PurchaseApprovalRuleSteps",
                columns: new[] { "TenantId", "PurchaseApprovalRuleId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalSteps_PurchaseApprovalInstanceId_TenantId",
                table: "PurchaseApprovalSteps",
                columns: new[] { "PurchaseApprovalInstanceId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalSteps_TenantId",
                table: "PurchaseApprovalSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseApprovalSteps_TenantId_PurchaseApprovalInstanceId_Sequence",
                table: "PurchaseApprovalSteps",
                columns: new[] { "TenantId", "PurchaseApprovalInstanceId", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseApprovalRuleSteps");

            migrationBuilder.DropTable(
                name: "PurchaseApprovalSteps");

            migrationBuilder.DropTable(
                name: "PurchaseApprovalInstances");

            migrationBuilder.DropTable(
                name: "PurchaseApprovalRules");
        }
    }
}
