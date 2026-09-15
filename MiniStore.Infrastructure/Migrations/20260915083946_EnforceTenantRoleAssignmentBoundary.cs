using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnforceTenantRoleAssignmentBoundary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantUserRoles_TenantRoles_TenantRoleId",
                table: "TenantUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TenantUserRoles_TenantRoleId",
                table: "TenantUserRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TenantRoles_Id_TenantId",
                table: "TenantRoles",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantRoleId_TenantId",
                table: "TenantUserRoles",
                columns: new[] { "TenantRoleId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUserRoles_TenantRoles_TenantRoleId_TenantId",
                table: "TenantUserRoles",
                columns: new[] { "TenantRoleId", "TenantId" },
                principalTable: "TenantRoles",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantUserRoles_TenantRoles_TenantRoleId_TenantId",
                table: "TenantUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_TenantUserRoles_TenantRoleId_TenantId",
                table: "TenantUserRoles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TenantRoles_Id_TenantId",
                table: "TenantRoles");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantRoleId",
                table: "TenantUserRoles",
                column: "TenantRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUserRoles_TenantRoles_TenantRoleId",
                table: "TenantUserRoles",
                column: "TenantRoleId",
                principalTable: "TenantRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
