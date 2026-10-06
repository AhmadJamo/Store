using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPutawayRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PutawayRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    StorageLocationId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PutawayRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PutawayRules_ProductCategories_ProductCategoryId_TenantId",
                        columns: x => new { x.ProductCategoryId, x.TenantId },
                        principalTable: "ProductCategories",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PutawayRules_Products_ProductId_TenantId",
                        columns: x => new { x.ProductId, x.TenantId },
                        principalTable: "Products",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PutawayRules_StorageLocations_StorageLocationId_TenantId",
                        columns: x => new { x.StorageLocationId, x.TenantId },
                        principalTable: "StorageLocations",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PutawayRules_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PutawayRules_Warehouses_WarehouseId_TenantId",
                        columns: x => new { x.WarehouseId, x.TenantId },
                        principalTable: "Warehouses",
                        principalColumns: new[] { "Id", "TenantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PutawayRules_ProductCategoryId_TenantId",
                table: "PutawayRules",
                columns: new[] { "ProductCategoryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PutawayRules_ProductId_TenantId",
                table: "PutawayRules",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PutawayRules_StorageLocationId_TenantId",
                table: "PutawayRules",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PutawayRules_TenantId",
                table: "PutawayRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PutawayRules_WarehouseId_Priority",
                table: "PutawayRules",
                columns: new[] { "WarehouseId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_PutawayRules_WarehouseId_TenantId",
                table: "PutawayRules",
                columns: new[] { "WarehouseId", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PutawayRules");
        }
    }
}
