using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductShelfLifeControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultShelfLifeDays",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpirationWarningDays",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<bool>(
                name: "RequireExpirationDate",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_DefaultShelfLifeDays",
                table: "Products",
                sql: "[DefaultShelfLifeDays] IS NULL OR ([DefaultShelfLifeDays] BETWEEN 1 AND 36500)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_ExpirationWarningDays",
                table: "Products",
                sql: "[ExpirationWarningDays] BETWEEN 0 AND 3650");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_DefaultShelfLifeDays",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_ExpirationWarningDays",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DefaultShelfLifeDays",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ExpirationWarningDays",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RequireExpirationDate",
                table: "Products");
        }
    }
}
