using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddErp008PosCashOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashPaymentMethodId",
                table: "PosCashPolicies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "PosCashMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "PosCashMovements",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosCashPolicies_CashPaymentMethodId_TenantId",
                table: "PosCashPolicies",
                columns: new[] { "CashPaymentMethodId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashMovements_TenantId_SourceType_SourceReference",
                table: "PosCashMovements",
                columns: new[] { "TenantId", "SourceType", "SourceReference" },
                unique: true,
                filter: "[SourceType] IS NOT NULL AND [SourceReference] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PosCashPolicies_PaymentMethods_CashPaymentMethodId_TenantId",
                table: "PosCashPolicies",
                columns: new[] { "CashPaymentMethodId", "TenantId" },
                principalTable: "PaymentMethods",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosCashPolicies_PaymentMethods_CashPaymentMethodId_TenantId",
                table: "PosCashPolicies");

            migrationBuilder.DropIndex(
                name: "IX_PosCashPolicies_CashPaymentMethodId_TenantId",
                table: "PosCashPolicies");

            migrationBuilder.DropIndex(
                name: "IX_PosCashMovements_TenantId_SourceType_SourceReference",
                table: "PosCashMovements");

            migrationBuilder.DropColumn(
                name: "CashPaymentMethodId",
                table: "PosCashPolicies");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "PosCashMovements");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "PosCashMovements");
        }
    }
}
