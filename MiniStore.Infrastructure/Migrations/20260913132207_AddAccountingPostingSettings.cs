using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingPostingSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountingSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    PurchaseDiscountAccountId = table.Column<int>(type: "int", nullable: true),
                    SalesDiscountAccountId = table.Column<int>(type: "int", nullable: true),
                    SalesRevenueAccountId = table.Column<int>(type: "int", nullable: true),
                    CostOfSalesAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingSettings_Accounts_CostOfSalesAccountId",
                        column: x => x.CostOfSalesAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountingSettings_Accounts_PurchaseDiscountAccountId",
                        column: x => x.PurchaseDiscountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountingSettings_Accounts_SalesDiscountAccountId",
                        column: x => x.SalesDiscountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountingSettings_Accounts_SalesRevenueAccountId",
                        column: x => x.SalesRevenueAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_CostOfSalesAccountId",
                table: "AccountingSettings",
                column: "CostOfSalesAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_PurchaseDiscountAccountId",
                table: "AccountingSettings",
                column: "PurchaseDiscountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SalesDiscountAccountId",
                table: "AccountingSettings",
                column: "SalesDiscountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SalesRevenueAccountId",
                table: "AccountingSettings",
                column: "SalesRevenueAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SingletonKey",
                table: "AccountingSettings",
                column: "SingletonKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingSettings");
        }
    }
}
