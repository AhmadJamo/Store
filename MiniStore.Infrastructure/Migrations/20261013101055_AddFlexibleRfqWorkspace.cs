using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFlexibleRfqWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseSourcingLines_TenantId_PurchaseSourcingEventId_PurchaseRequestLineId",
                table: "PurchaseSourcingLines");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseSourcingEvents_TenantId_PurchaseRequestId",
                table: "PurchaseSourcingEvents");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseRequestLineId",
                table: "PurchaseSourcingLines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseRequestId",
                table: "PurchaseSourcingEvents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BuyerReference",
                table: "PurchaseSourcingEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DirectPolicyMaximumAmountSnapshot",
                table: "PurchaseSourcingEvents",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DirectPolicyRoleSnapshot",
                table: "PurchaseSourcingEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedAmountCompanyCurrency",
                table: "PurchaseSourcingEvents",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Path",
                table: "PurchaseSourcingEvents",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SupplierResponseDeadline",
                table: "PurchaseSourcingEvents",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql(
                """
                UPDATE PurchaseSourcingEvents
                SET Path = 1,
                    SupplierResponseDeadline = CASE
                        WHEN DATEADD(day, 7, CAST(CreatedAtUtc AS date)) <= RequestedByDate
                            THEN DATEADD(day, 7, CAST(CreatedAtUtc AS date))
                        ELSE RequestedByDate
                    END;
                """);

            migrationBuilder.CreateTable(
                name: "PurchaseSourcingAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseSourcingEventId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    UploadedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSourcingAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingAttachments_PurchaseSourcingEvents_PurchaseSourcingEventId_TenantId",
                        columns: x => new { x.PurchaseSourcingEventId, x.TenantId },
                        principalTable: "PurchaseSourcingEvents",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingAttachments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseSourcingPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    AllowDirectRfq = table.Column<bool>(type: "bit", nullable: false),
                    MaximumEstimatedAmountCompanyCurrency = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    AllowedRoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequireActiveSuppliers = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSourcingPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseSourcingPolicies_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_TenantId_PurchaseSourcingEventId_PurchaseRequestLineId",
                table: "PurchaseSourcingLines",
                columns: new[] { "TenantId", "PurchaseSourcingEventId", "PurchaseRequestLineId" },
                unique: true,
                filter: "[PurchaseRequestLineId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_TenantId_PurchaseRequestId",
                table: "PurchaseSourcingEvents",
                columns: new[] { "TenantId", "PurchaseRequestId" },
                unique: true,
                filter: "[PurchaseRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingAttachments_PurchaseSourcingEventId_TenantId",
                table: "PurchaseSourcingAttachments",
                columns: new[] { "PurchaseSourcingEventId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingAttachments_TenantId",
                table: "PurchaseSourcingAttachments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingAttachments_TenantId_PurchaseSourcingEventId_FileName",
                table: "PurchaseSourcingAttachments",
                columns: new[] { "TenantId", "PurchaseSourcingEventId", "FileName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingPolicies_TenantId",
                table: "PurchaseSourcingPolicies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingPolicies_TenantId_SingletonKey",
                table: "PurchaseSourcingPolicies",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM PurchaseSourcingEvents WHERE PurchaseRequestId IS NULL)
                    THROW 51000, 'Direct RFQ records must be migrated before rolling back AddFlexibleRfqWorkspace.', 1;
                """);
            migrationBuilder.DropTable(
                name: "PurchaseSourcingAttachments");

            migrationBuilder.DropTable(
                name: "PurchaseSourcingPolicies");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseSourcingLines_TenantId_PurchaseSourcingEventId_PurchaseRequestLineId",
                table: "PurchaseSourcingLines");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseSourcingEvents_TenantId_PurchaseRequestId",
                table: "PurchaseSourcingEvents");

            migrationBuilder.DropColumn(
                name: "BuyerReference",
                table: "PurchaseSourcingEvents");

            migrationBuilder.DropColumn(
                name: "DirectPolicyMaximumAmountSnapshot",
                table: "PurchaseSourcingEvents");

            migrationBuilder.DropColumn(
                name: "DirectPolicyRoleSnapshot",
                table: "PurchaseSourcingEvents");

            migrationBuilder.DropColumn(
                name: "EstimatedAmountCompanyCurrency",
                table: "PurchaseSourcingEvents");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "PurchaseSourcingEvents");

            migrationBuilder.DropColumn(
                name: "SupplierResponseDeadline",
                table: "PurchaseSourcingEvents");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseRequestLineId",
                table: "PurchaseSourcingLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseRequestId",
                table: "PurchaseSourcingEvents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingLines_TenantId_PurchaseSourcingEventId_PurchaseRequestLineId",
                table: "PurchaseSourcingLines",
                columns: new[] { "TenantId", "PurchaseSourcingEventId", "PurchaseRequestLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourcingEvents_TenantId_PurchaseRequestId",
                table: "PurchaseSourcingEvents",
                columns: new[] { "TenantId", "PurchaseRequestId" },
                unique: true);
        }
    }
}
