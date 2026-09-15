using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations;

public partial class AddTenantOwnedRoleDefinitions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TenantRoles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                NormalizedName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                IsSystem = table.Column<bool>(type: "bit", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantRoles", x => x.Id);
                table.ForeignKey("FK_TenantRoles_Tenants_TenantId", x => x.TenantId, "Tenants", "Id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex("IX_TenantRoles_TenantId_NormalizedName", "TenantRoles", new[] { "TenantId", "NormalizedName" }, unique: true);

        migrationBuilder.CreateTable(
            name: "TenantRolePermissions",
            columns: table => new
            {
                TenantRoleId = table.Column<int>(type: "int", nullable: false),
                PermissionId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantRolePermissions", x => new { x.TenantRoleId, x.PermissionId });
                table.ForeignKey("FK_TenantRolePermissions_Permissions_PermissionId", x => x.PermissionId, "Permissions", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_TenantRolePermissions_TenantRoles_TenantRoleId", x => x.TenantRoleId, "TenantRoles", "Id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex("IX_TenantRolePermissions_PermissionId", "TenantRolePermissions", "PermissionId");

        migrationBuilder.Sql(
            """
            INSERT INTO TenantRoles (TenantId, Name, NormalizedName, Description, IsSystem)
            SELECT tenants.Id,
                   LEFT(COALESCE(identityRoles.Name, N'Role'), 80),
                   LEFT(COALESCE(identityRoles.NormalizedName, UPPER(identityRoles.Name), N'ROLE'), 80),
                   CASE WHEN identityRoles.NormalizedName = N'ADMIN'
                        THEN N'Protected company owner and administration role.'
                        ELSE N'Migrated company role.' END,
                   CASE WHEN identityRoles.NormalizedName = N'ADMIN' THEN 1 ELSE 0 END
            FROM Tenants AS tenants
            CROSS JOIN AspNetRoles AS identityRoles;

            INSERT INTO TenantRolePermissions (TenantRoleId, PermissionId)
            SELECT tenantRoles.Id, legacyPermissions.PermissionId
            FROM TenantRoles AS tenantRoles
            INNER JOIN AspNetRoles AS identityRoles
                ON identityRoles.NormalizedName = tenantRoles.NormalizedName
            INNER JOIN RolePermissions AS legacyPermissions
                ON legacyPermissions.RoleId = identityRoles.Id;
            """);

        migrationBuilder.AddColumn<int>(name: "TenantRoleId", table: "TenantUserRoles", type: "int", nullable: true);
        migrationBuilder.Sql(
            """
            UPDATE assignments
            SET TenantRoleId = tenantRoles.Id
            FROM TenantUserRoles AS assignments
            INNER JOIN AspNetRoles AS identityRoles ON identityRoles.Id = assignments.RoleId
            INNER JOIN TenantRoles AS tenantRoles
                ON tenantRoles.TenantId = assignments.TenantId
               AND tenantRoles.NormalizedName = identityRoles.NormalizedName;

            IF EXISTS (SELECT 1 FROM TenantUserRoles WHERE TenantRoleId IS NULL)
                THROW 51000, 'A tenant role assignment could not be migrated.', 1;
            """);

        migrationBuilder.DropForeignKey("FK_TenantUserRoles_AspNetRoles_RoleId", "TenantUserRoles");
        migrationBuilder.DropPrimaryKey("PK_TenantUserRoles", "TenantUserRoles");
        migrationBuilder.DropIndex("IX_TenantUserRoles_RoleId", "TenantUserRoles");
        migrationBuilder.DropColumn("RoleId", "TenantUserRoles");
        migrationBuilder.AlterColumn<int>(name: "TenantRoleId", table: "TenantUserRoles", type: "int", nullable: false, oldClrType: typeof(int), oldType: "int", oldNullable: true);
        migrationBuilder.AddPrimaryKey("PK_TenantUserRoles", "TenantUserRoles", new[] { "TenantId", "UserId", "TenantRoleId" });
        migrationBuilder.CreateIndex("IX_TenantUserRoles_TenantRoleId", "TenantUserRoles", "TenantRoleId");
        migrationBuilder.AddForeignKey("FK_TenantUserRoles_TenantRoles_TenantRoleId", "TenantUserRoles", "TenantRoleId", "TenantRoles", principalColumn: "Id", onDelete: ReferentialAction.Restrict);

        migrationBuilder.DropTable("RolePermissions");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RolePermissions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                PermissionId = table.Column<int>(type: "int", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => x.Id);
                table.ForeignKey("FK_RolePermissions_Permissions_PermissionId", x => x.PermissionId, "Permissions", "Id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex("IX_RolePermissions_PermissionId", "RolePermissions", "PermissionId");
        migrationBuilder.CreateIndex("IX_RolePermissions_RoleId_PermissionId", "RolePermissions", new[] { "RoleId", "PermissionId" }, unique: true);

        migrationBuilder.Sql(
            """
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            SELECT CONVERT(nvarchar(450), NEWID()), roles.Name, roles.NormalizedName, CONVERT(nvarchar(450), NEWID())
            FROM (SELECT NormalizedName, MIN(Name) AS Name FROM TenantRoles GROUP BY NormalizedName) AS roles
            WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles existing WHERE existing.NormalizedName = roles.NormalizedName);

            INSERT INTO RolePermissions (RoleId, PermissionId)
            SELECT identityRoles.Id, tenantPermissions.PermissionId
            FROM AspNetRoles AS identityRoles
            INNER JOIN (SELECT DISTINCT roles.NormalizedName, permissions.PermissionId
                        FROM TenantRoles roles
                        INNER JOIN TenantRolePermissions permissions ON permissions.TenantRoleId = roles.Id) AS tenantPermissions
                ON tenantPermissions.NormalizedName = identityRoles.NormalizedName;
            """);

        migrationBuilder.AddColumn<string>(name: "RoleId", table: "TenantUserRoles", type: "nvarchar(450)", maxLength: 450, nullable: true);
        migrationBuilder.Sql(
            """
            UPDATE assignments
            SET RoleId = identityRoles.Id
            FROM TenantUserRoles assignments
            INNER JOIN TenantRoles tenantRoles ON tenantRoles.Id = assignments.TenantRoleId
            INNER JOIN AspNetRoles identityRoles ON identityRoles.NormalizedName = tenantRoles.NormalizedName;
            """);
        migrationBuilder.DropForeignKey("FK_TenantUserRoles_TenantRoles_TenantRoleId", "TenantUserRoles");
        migrationBuilder.DropPrimaryKey("PK_TenantUserRoles", "TenantUserRoles");
        migrationBuilder.DropIndex("IX_TenantUserRoles_TenantRoleId", "TenantUserRoles");
        migrationBuilder.DropColumn("TenantRoleId", "TenantUserRoles");
        migrationBuilder.AlterColumn<string>(name: "RoleId", table: "TenantUserRoles", type: "nvarchar(450)", maxLength: 450, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(450)", oldMaxLength: 450, oldNullable: true);
        migrationBuilder.AddPrimaryKey("PK_TenantUserRoles", "TenantUserRoles", new[] { "TenantId", "UserId", "RoleId" });
        migrationBuilder.CreateIndex("IX_TenantUserRoles_RoleId", "TenantUserRoles", "RoleId");
        migrationBuilder.AddForeignKey("FK_TenantUserRoles_AspNetRoles_RoleId", "TenantUserRoles", "RoleId", "AspNetRoles", principalColumn: "Id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.DropTable("TenantRolePermissions");
        migrationBuilder.DropTable("TenantRoles");
    }
}
