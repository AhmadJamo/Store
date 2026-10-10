using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReplenishmentPurchaseDemand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "PurchaseRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "PurchaseRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_TenantId_SourceType_SourceReference",
                table: "PurchaseRequests",
                columns: new[] { "TenantId", "SourceType", "SourceReference" },
                unique: true,
                filter: "[SourceType] IS NOT NULL AND [SourceReference] IS NOT NULL AND [Status] <> 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_TenantId_SourceType_SourceReference",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "PurchaseRequests");
        }
    }
}
