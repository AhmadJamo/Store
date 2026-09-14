using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPosExperienceSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosTerminalSettings",
                columns: table => new
                {
                    PosTerminalId = table.Column<int>(type: "int", nullable: false),
                    Profile = table.Column<int>(type: "int", nullable: false),
                    ProductLayout = table.Column<int>(type: "int", nullable: false),
                    Theme = table.Column<int>(type: "int", nullable: false),
                    CartPosition = table.Column<int>(type: "int", nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    HeaderTitle = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ProductColumns = table.Column<int>(type: "int", nullable: false),
                    CompactProductCards = table.Column<bool>(type: "bit", nullable: false),
                    ShowBarcode = table.Column<bool>(type: "bit", nullable: false),
                    ShowPrice = table.Column<bool>(type: "bit", nullable: false),
                    ShowStock = table.Column<bool>(type: "bit", nullable: false),
                    AutoFocusSearch = table.Column<bool>(type: "bit", nullable: false),
                    TouchOptimized = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTerminalSettings", x => x.PosTerminalId);
                    table.ForeignKey(
                        name: "FK_PosTerminalSettings_PosTerminals_PosTerminalId",
                        column: x => x.PosTerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO PosTerminalSettings
                    (PosTerminalId, Profile, ProductLayout, Theme, CartPosition,
                     AccentColor, HeaderTitle, ProductColumns, CompactProductCards,
                     ShowBarcode, ShowPrice, ShowStock, AutoFocusSearch, TouchOptimized)
                SELECT Id, 1, 1, 1, 1, '#2563A8', 'Point of Sale', 4,
                       0, 1, 1, 1, 1, 1
                FROM PosTerminals;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosTerminalSettings");
        }
    }
}
