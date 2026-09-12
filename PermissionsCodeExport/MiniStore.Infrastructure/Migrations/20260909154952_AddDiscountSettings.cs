using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    AllowLineDiscount = table.Column<bool>(type: "bit", nullable: false),
                    AllowInvoiceDiscount = table.Column<bool>(type: "bit", nullable: false),
                    AllowPercentageDiscount = table.Column<bool>(type: "bit", nullable: false),
                    AllowFixedAmountDiscount = table.Column<bool>(type: "bit", nullable: false),
                    DefaultDiscountType = table.Column<int>(type: "int", nullable: false),
                    MaxLineDiscountPercent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaxLineDiscountAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaxInvoiceDiscountPercent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaxInvoiceDiscountAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AllowDiscountAboveLimit = table.Column<bool>(type: "bit", nullable: false),
                    DiscountOverridePermission = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountSettings", x => x.Id);
                    table.CheckConstraint("CK_DiscountSettings_SingletonKey", "[SingletonKey] = 1");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountSettings_SingletonKey",
                table: "DiscountSettings",
                column: "SingletonKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountSettings");
        }
    }
}
