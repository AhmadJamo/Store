using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaasControlPlane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEnglish = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameArabic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionEnglish = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DescriptionArabic = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AnnualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformOperators",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformOperators", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PlatformOperators_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanFeatures",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanFeatures", x => new { x.PlanId, x.Key });
                    table.ForeignKey(
                        name: "FK_PlanFeatures_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanLimits",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanLimits", x => new { x.PlanId, x.Key });
                    table.ForeignKey(
                        name: "FK_PlanLimits_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaximumRedemptions = table.Column<int>(type: "int", nullable: true),
                    RedemptionCount = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionCodes_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BillingCycle = table.Column<int>(type: "int", nullable: false),
                    CurrentPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GracePeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelAtPeriodEnd = table.Column<bool>(type: "bit", nullable: false),
                    ExternalCustomerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalSubscriptionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSubscriptions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantSubscriptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRedemptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionCodeId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRedemptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionRedemptions_PromotionCodes_PromotionCodeId",
                        column: x => x.PromotionCodeId,
                        principalTable: "PromotionCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRedemptions_TenantSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "TenantSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRedemptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                SET IDENTITY_INSERT [Plans] ON;
                INSERT INTO [Plans] ([Id],[Code],[NameEnglish],[NameArabic],[DescriptionEnglish],[DescriptionArabic],[MonthlyPrice],[AnnualPrice],[Currency],[IsActive],[IsPublic],[SortOrder]) VALUES
                (1,N'starter',N'Starter',N'البداية',N'Essential tools for a small business.',N'الأدوات الأساسية للمنشآت الصغيرة.',19,190,N'USD',1,1,10),
                (2,N'professional',N'Professional',N'الاحترافية',N'Advanced inventory, accounting and multi-branch operations.',N'مخزون ومحاسبة متقدمة وإدارة عدة فروع.',59,590,N'USD',1,1,20),
                (3,N'enterprise',N'Enterprise',N'المؤسسات',N'Custom scale, controls and support.',N'توسع مخصص وضوابط ودعم للمؤسسات.',149,1490,N'USD',1,1,30);
                SET IDENTITY_INSERT [Plans] OFF;

                INSERT INTO [PlanLimits] ([PlanId],[Key],[Value]) VALUES
                (1,N'Users.Max',3),(1,N'Warehouses.Max',2),(1,N'Branches.Max',1),(1,N'PosTerminals.Max',1),(1,N'Products.Max',1000),
                (2,N'Users.Max',15),(2,N'Warehouses.Max',10),(2,N'Branches.Max',5),(2,N'PosTerminals.Max',10),(2,N'Products.Max',50000),
                (3,N'Users.Max',NULL),(3,N'Warehouses.Max',NULL),(3,N'Branches.Max',NULL),(3,N'PosTerminals.Max',NULL),(3,N'Products.Max',NULL);

                INSERT INTO [PlanFeatures] ([PlanId],[Key],[IsEnabled]) VALUES
                (1,N'Accounting.Advanced',0),(1,N'Inventory.Locations',0),(1,N'Pos.RestaurantMode',0),
                (2,N'Accounting.Advanced',1),(2,N'Inventory.Locations',1),(2,N'Pos.RestaurantMode',1),
                (3,N'Accounting.Advanced',1),(3,N'Inventory.Locations',1),(3,N'Pos.RestaurantMode',1);

                INSERT INTO [TenantSubscriptions] ([TenantId],[PlanId],[Status],[BillingCycle],[CurrentPeriodStart],[CurrentPeriodEnd],[GracePeriodEnd],[CancelAtPeriodEnd])
                SELECT [Id],2,1,1,SYSUTCDATETIME(),DATEADD(day,30,SYSUTCDATETIME()),NULL,0 FROM [Tenants];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_Code",
                table: "Plans",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCodes_Code",
                table: "PromotionCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCodes_PlanId",
                table: "PromotionCodes",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRedemptions_PromotionCodeId_TenantId",
                table: "PromotionRedemptions",
                columns: new[] { "PromotionCodeId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRedemptions_SubscriptionId",
                table: "PromotionRedemptions",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRedemptions_TenantId",
                table: "PromotionRedemptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscriptions_PlanId",
                table: "TenantSubscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscriptions_TenantId",
                table: "TenantSubscriptions",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanFeatures");

            migrationBuilder.DropTable(
                name: "PlanLimits");

            migrationBuilder.DropTable(
                name: "PlatformOperators");

            migrationBuilder.DropTable(
                name: "PromotionRedemptions");

            migrationBuilder.DropTable(
                name: "PromotionCodes");

            migrationBuilder.DropTable(
                name: "TenantSubscriptions");

            migrationBuilder.DropTable(
                name: "Plans");
        }
    }
}
