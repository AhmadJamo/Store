using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPosOrderWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuestCount",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PosOrderType",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceReference",
                table: "Sales",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SaleItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultOrderType",
                table: "PosTerminalSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableGuestCount",
                table: "PosTerminalSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableItemNotes",
                table: "PosTerminalSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EnabledOrderTypes",
                table: "PosTerminalSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "QuickAddOnBarcodeScan",
                table: "PosTerminalSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireServiceReference",
                table: "PosTerminalSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE PosTerminalSettings
                SET EnabledOrderTypes = CASE Profile
                        WHEN 3 THEN 7
                        WHEN 4 THEN 14
                        WHEN 5 THEN 13
                        ELSE 1
                    END,
                    DefaultOrderType = CASE Profile
                        WHEN 4 THEN 2
                        WHEN 5 THEN 3
                        ELSE 1
                    END,
                    RequireServiceReference = CASE WHEN Profile = 4 THEN 1 ELSE 0 END,
                    EnableGuestCount = CASE WHEN Profile IN (3, 4) THEN 1 ELSE 0 END,
                    EnableItemNotes = CASE WHEN Profile IN (3, 4, 5) THEN 1 ELSE 0 END,
                    QuickAddOnBarcodeScan = CASE WHEN Profile IN (1, 2, 5) THEN 1 ELSE 0 END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestCount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PosOrderType",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ServiceReference",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "DefaultOrderType",
                table: "PosTerminalSettings");

            migrationBuilder.DropColumn(
                name: "EnableGuestCount",
                table: "PosTerminalSettings");

            migrationBuilder.DropColumn(
                name: "EnableItemNotes",
                table: "PosTerminalSettings");

            migrationBuilder.DropColumn(
                name: "EnabledOrderTypes",
                table: "PosTerminalSettings");

            migrationBuilder.DropColumn(
                name: "QuickAddOnBarcodeScan",
                table: "PosTerminalSettings");

            migrationBuilder.DropColumn(
                name: "RequireServiceReference",
                table: "PosTerminalSettings");
        }
    }
}
