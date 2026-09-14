using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchAndPosWarehouseAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_BranchId",
                table: "PosTerminals");

            migrationBuilder.CreateTable(
                name: "BranchWarehouseAccesses",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsDefaultForPos = table.Column<bool>(type: "bit", nullable: false),
                    AllowPosSales = table.Column<bool>(type: "bit", nullable: false),
                    AllowPurchases = table.Column<bool>(type: "bit", nullable: false),
                    AllowTransferOut = table.Column<bool>(type: "bit", nullable: false),
                    AllowTransferIn = table.Column<bool>(type: "bit", nullable: false),
                    AllowReplenishment = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchWarehouseAccesses", x => new { x.BranchId, x.WarehouseId });
                    table.ForeignKey(
                        name: "FK_BranchWarehouseAccesses_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchWarehouseAccesses_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosTerminalWarehouses",
                columns: table => new
                {
                    PosTerminalId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTerminalWarehouses", x => new { x.PosTerminalId, x.WarehouseId });
                    table.ForeignKey(
                        name: "FK_PosTerminalWarehouses_PosTerminals_PosTerminalId",
                        column: x => x.PosTerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PosTerminalWarehouses_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                INSERT INTO BranchWarehouseAccesses
                    (BranchId, WarehouseId, Priority, IsDefaultForPos, AllowPosSales,
                     AllowPurchases, AllowTransferOut, AllowTransferIn, AllowReplenishment)
                SELECT w.BranchId, w.Id,
                       ROW_NUMBER() OVER (PARTITION BY w.BranchId ORDER BY w.Id),
                       CASE WHEN ROW_NUMBER() OVER (PARTITION BY w.BranchId ORDER BY w.Id) = 1
                            THEN 1 ELSE 0 END,
                       w.AllowPosSales, 1, 1, 1, 1
                FROM Warehouses w
                WHERE w.BranchId IS NOT NULL;

                INSERT INTO BranchWarehouseAccesses
                    (BranchId, WarehouseId, Priority, IsDefaultForPos, AllowPosSales,
                     AllowPurchases, AllowTransferOut, AllowTransferIn, AllowReplenishment)
                SELECT DISTINCT p.BranchId, p.DefaultWarehouseId, 1, 1, 1, 1, 1, 1, 1
                FROM PosTerminals p
                WHERE NOT EXISTS (
                    SELECT 1 FROM BranchWarehouseAccesses a
                    WHERE a.BranchId = p.BranchId
                      AND a.WarehouseId = p.DefaultWarehouseId);

                INSERT INTO PosTerminals (Name, BranchId, DefaultWarehouseId, IsActive)
                SELECT 'Main POS', a.BranchId, a.WarehouseId, 1
                FROM BranchWarehouseAccesses a
                WHERE a.IsDefaultForPos = 1
                  AND NOT EXISTS (
                      SELECT 1 FROM PosTerminals p
                      WHERE p.BranchId = a.BranchId);

                INSERT INTO PosTerminalWarehouses (PosTerminalId, WarehouseId, Priority)
                SELECT Id, DefaultWarehouseId, 1
                FROM PosTerminals;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId_Name",
                table: "PosTerminals",
                columns: new[] { "BranchId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchWarehouseAccesses_BranchId_Priority",
                table: "BranchWarehouseAccesses",
                columns: new[] { "BranchId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchWarehouseAccesses_WarehouseId",
                table: "BranchWarehouseAccesses",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalWarehouses_PosTerminalId_Priority",
                table: "PosTerminalWarehouses",
                columns: new[] { "PosTerminalId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalWarehouses_WarehouseId",
                table: "PosTerminalWarehouses",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchWarehouseAccesses");

            migrationBuilder.DropTable(
                name: "PosTerminalWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_BranchId_Name",
                table: "PosTerminals");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId",
                table: "PosTerminals",
                column: "BranchId");
        }
    }
}
