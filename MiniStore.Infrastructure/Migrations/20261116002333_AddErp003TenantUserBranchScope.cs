using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations;

public partial class AddErp003TenantUserBranchScope : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TenantUserBranchGrants",
            columns: table => new
            {
                TenantId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: false),
                GrantedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                GrantedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantUserBranchGrants", x => new { x.TenantId, x.UserId, x.BranchId });
                table.ForeignKey("FK_TenantUserBranchGrants_AspNetUsers_UserId", x => x.UserId,
                    "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_TenantUserBranchGrants_Branches_BranchId_TenantId",
                    x => new { x.BranchId, x.TenantId },
                    "Branches", new[] { "Id", "TenantId" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_TenantUserBranchGrants_Tenants_TenantId", x => x.TenantId,
                    "Tenants", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_TenantUserBranchGrants_BranchId_TenantId",
            "TenantUserBranchGrants", new[] { "BranchId", "TenantId" });
        migrationBuilder.CreateIndex("IX_TenantUserBranchGrants_TenantId",
            "TenantUserBranchGrants", "TenantId");
        migrationBuilder.CreateIndex("IX_TenantUserBranchGrants_TenantId_BranchId",
            "TenantUserBranchGrants", new[] { "TenantId", "BranchId" });
        migrationBuilder.CreateIndex("IX_TenantUserBranchGrants_UserId",
            "TenantUserBranchGrants", "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable("TenantUserBranchGrants");
}
