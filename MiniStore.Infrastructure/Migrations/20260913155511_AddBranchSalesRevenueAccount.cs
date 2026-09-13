using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchSalesRevenueAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalesRevenueAccountId",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_SalesRevenueAccountId",
                table: "Branches",
                column: "SalesRevenueAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Accounts_SalesRevenueAccountId",
                table: "Branches",
                column: "SalesRevenueAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Accounts_SalesRevenueAccountId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_SalesRevenueAccountId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "SalesRevenueAccountId",
                table: "Branches");
        }
    }
}
