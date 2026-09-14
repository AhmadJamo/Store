using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BootstrapPlatformOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM [PlatformOperators])
                BEGIN
                    INSERT INTO [PlatformOperators] ([UserId], [Role], [IsActive], [CreatedAt])
                    SELECT TOP (1) users.[Id], 1, 1, SYSUTCDATETIME()
                    FROM [AspNetUsers] users
                    INNER JOIN [TenantUserRoles] assignments ON assignments.[UserId] = users.[Id]
                    INNER JOIN [AspNetRoles] roles ON roles.[Id] = assignments.[RoleId]
                    WHERE roles.[Name] = N'Admin'
                    ORDER BY users.[UserName];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
