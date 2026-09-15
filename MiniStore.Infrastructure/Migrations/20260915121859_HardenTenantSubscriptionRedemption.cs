using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HardenTenantSubscriptionRedemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionRedemptions_TenantSubscriptions_SubscriptionId",
                table: "PromotionRedemptions");

            migrationBuilder.DropIndex(
                name: "IX_PromotionRedemptions_SubscriptionId",
                table: "PromotionRedemptions");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TenantSubscriptions_Id_TenantId",
                table: "TenantSubscriptions",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRedemptions_SubscriptionId_TenantId",
                table: "PromotionRedemptions",
                columns: new[] { "SubscriptionId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionRedemptions_TenantSubscriptions_SubscriptionId_TenantId",
                table: "PromotionRedemptions",
                columns: new[] { "SubscriptionId", "TenantId" },
                principalTable: "TenantSubscriptions",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionRedemptions_TenantSubscriptions_SubscriptionId_TenantId",
                table: "PromotionRedemptions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TenantSubscriptions_Id_TenantId",
                table: "TenantSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PromotionRedemptions_SubscriptionId_TenantId",
                table: "PromotionRedemptions");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRedemptions_SubscriptionId",
                table: "PromotionRedemptions",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionRedemptions_TenantSubscriptions_SubscriptionId",
                table: "PromotionRedemptions",
                column: "SubscriptionId",
                principalTable: "TenantSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
