using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantScopedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantUserRoles",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserRoles", x => new { x.TenantId, x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO [TenantUserRoles] ([TenantId], [UserId], [RoleId])
                SELECT memberships.[TenantId], userRoles.[UserId], userRoles.[RoleId]
                FROM [TenantMemberships] memberships
                INNER JOIN [AspNetUserRoles] userRoles ON userRoles.[UserId] = memberships.[UserId];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_RoleId",
                table: "TenantUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantId_UserId",
                table: "TenantUserRoles",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_UserId",
                table: "TenantUserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantUserRoles");
        }
    }
}
