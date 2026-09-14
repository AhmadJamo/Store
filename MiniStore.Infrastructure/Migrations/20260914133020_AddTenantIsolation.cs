using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_WarehouseId_Code",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_TransferNumber",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_StockTransferId_ProductId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_InvoiceNumber",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ProductId_WarehouseId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_ProductId_StorageLocationId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_BranchId_Name",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_Name",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_EntryNumber",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_SourceType_SourceReference",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_InventorySettings_SingletonKey",
                table: "InventorySettings");

            migrationBuilder.DropIndex(
                name: "IX_GeneralSettings_SingletonKey",
                table: "GeneralSettings");

            migrationBuilder.DropIndex(
                name: "IX_DiscountSettings_SingletonKey",
                table: "DiscountSettings");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Branches_Code",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Code",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_SingletonKey",
                table: "AccountingSettings");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TaxRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StorageLocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StockTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StockTransferItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StockTransferHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StockTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SaleItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ProductLocationStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PosTerminalWarehouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PosTerminalSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PosTerminals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PosTerminalProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PaymentMethods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "LocationMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "JournalEntryLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "JournalEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InvoiceSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InventorySettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "GeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "DocumentNumberSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "DiscountSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "BranchWarehouseAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AccountingSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantMemberships",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMemberships", x => new { x.TenantId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TenantMemberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantMemberships_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                SET IDENTITY_INSERT [Tenants] ON;
                INSERT INTO [Tenants] ([Id], [Name], [Slug], [IsActive], [CreatedAt])
                VALUES (1, N'Demo Company', N'demo-company', 1, SYSUTCDATETIME());
                SET IDENTITY_INSERT [Tenants] OFF;

                DECLARE @tenantTables TABLE ([Name] sysname);
                INSERT INTO @tenantTables ([Name]) VALUES
                    (N'Warehouses'), (N'TaxRates'), (N'Suppliers'), (N'StorageLocations'),
                    (N'StockTransfers'), (N'StockTransferItems'), (N'StockTransferHistories'),
                    (N'StockTransactions'), (N'Sales'), (N'SaleItems'), (N'Purchases'),
                    (N'PurchaseItems'), (N'ProductStocks'), (N'Products'),
                    (N'ProductLocationStocks'), (N'PosTerminalWarehouses'),
                    (N'PosTerminalSettings'), (N'PosTerminals'), (N'PosTerminalProducts'),
                    (N'PaymentMethods'), (N'LocationMovements'), (N'JournalEntryLines'),
                    (N'JournalEntries'), (N'InvoiceSettings'), (N'InventorySettings'),
                    (N'GeneralSettings'), (N'DocumentNumberSettings'), (N'DiscountSettings'),
                    (N'Customers'), (N'BranchWarehouseAccesses'), (N'Branches'),
                    (N'AuditLogs'), (N'Accounts'), (N'AccountingSettings');

                DECLARE @tableName sysname;
                DECLARE @updateTenantSql nvarchar(max);
                DECLARE tenant_cursor CURSOR LOCAL FAST_FORWARD FOR
                    SELECT [Name] FROM @tenantTables;
                OPEN tenant_cursor;
                FETCH NEXT FROM tenant_cursor INTO @tableName;
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @updateTenantSql =
                        N'UPDATE ' + QUOTENAME(@tableName) + N' SET [TenantId] = 1;';
                    EXEC sp_executesql @updateTenantSql;
                    FETCH NEXT FROM tenant_cursor INTO @tableName;
                END
                CLOSE tenant_cursor;
                DEALLOCATE tenant_cursor;

                INSERT INTO [TenantMemberships] ([TenantId], [UserId], [IsOwner], [IsActive], [CreatedAt])
                SELECT
                    1,
                    users.[Id],
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM [AspNetUserRoles] userRoles
                        INNER JOIN [AspNetRoles] roles ON roles.[Id] = userRoles.[RoleId]
                        WHERE userRoles.[UserId] = users.[Id] AND roles.[Name] = N'Admin'
                    ) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
                    CAST(1 AS bit),
                    SYSUTCDATETIME()
                FROM [AspNetUsers] users;

                DECLARE @dropDefaults nvarchar(max) = N'';
                SELECT @dropDefaults +=
                    N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(defaults.[parent_object_id])) +
                    N'.' + QUOTENAME(OBJECT_NAME(defaults.[parent_object_id])) +
                    N' DROP CONSTRAINT ' + QUOTENAME(defaults.[name]) + N';'
                FROM sys.default_constraints defaults
                INNER JOIN sys.columns columns
                    ON columns.[object_id] = defaults.[parent_object_id]
                    AND columns.[column_id] = defaults.[parent_column_id]
                INNER JOIN @tenantTables tenantTables
                    ON tenantTables.[Name] = OBJECT_NAME(defaults.[parent_object_id])
                WHERE columns.[name] = N'TenantId';

                EXEC sp_executesql @dropDefaults;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_TenantId",
                table: "TaxRates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantId",
                table: "Suppliers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_TenantId",
                table: "StorageLocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_TenantId_WarehouseId_Code",
                table: "StorageLocations",
                columns: new[] { "TenantId", "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_WarehouseId",
                table: "StorageLocations",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_TenantId",
                table: "StockTransfers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_TenantId_TransferNumber",
                table: "StockTransfers",
                columns: new[] { "TenantId", "TransferNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_StockTransferId",
                table: "StockTransferItems",
                column: "StockTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_TenantId",
                table: "StockTransferItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_TenantId_StockTransferId_ProductId",
                table: "StockTransferItems",
                columns: new[] { "TenantId", "StockTransferId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferHistories_TenantId",
                table: "StockTransferHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_TenantId",
                table: "StockTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId",
                table: "Sales",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_InvoiceNumber",
                table: "Sales",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_TenantId",
                table: "SaleItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId",
                table: "Purchases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_InvoiceNumber",
                table: "Purchases",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_TenantId",
                table: "PurchaseItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ProductId",
                table: "ProductStocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_TenantId",
                table: "ProductStocks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_TenantId_ProductId_WarehouseId",
                table: "ProductStocks",
                columns: new[] { "TenantId", "ProductId", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_ProductId",
                table: "ProductLocationStocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_TenantId",
                table: "ProductLocationStocks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_TenantId_ProductId_StorageLocationId",
                table: "ProductLocationStocks",
                columns: new[] { "TenantId", "ProductId", "StorageLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalWarehouses_TenantId",
                table: "PosTerminalWarehouses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalSettings_TenantId",
                table: "PosTerminalSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId",
                table: "PosTerminals",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_TenantId",
                table: "PosTerminals",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_TenantId_BranchId_Name",
                table: "PosTerminals",
                columns: new[] { "TenantId", "BranchId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalProducts_TenantId",
                table: "PosTerminalProducts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_TenantId",
                table: "PaymentMethods",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_TenantId_Name",
                table: "PaymentMethods",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_TenantId",
                table: "LocationMovements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_TenantId",
                table: "JournalEntryLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId",
                table: "JournalEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_EntryNumber",
                table: "JournalEntries",
                columns: new[] { "TenantId", "EntryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_SourceType_SourceReference",
                table: "JournalEntries",
                columns: new[] { "TenantId", "SourceType", "SourceReference" },
                unique: true,
                filter: "[SourceType] IS NOT NULL AND [SourceReference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceSettings_TenantId",
                table: "InvoiceSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySettings_TenantId",
                table: "InventorySettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySettings_TenantId_SingletonKey",
                table: "InventorySettings",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_TenantId",
                table: "GeneralSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_TenantId_SingletonKey",
                table: "GeneralSettings",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumberSettings_TenantId",
                table: "DocumentNumberSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountSettings_TenantId",
                table: "DiscountSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountSettings_TenantId_SingletonKey",
                table: "DiscountSettings",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_AccountId",
                table: "Customers",
                columns: new[] { "TenantId", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchWarehouseAccesses_TenantId",
                table: "BranchWarehouseAccesses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_TenantId",
                table: "Branches",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_TenantId_Code",
                table: "Branches",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId",
                table: "AuditLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TenantId",
                table: "Accounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TenantId_Code",
                table: "Accounts",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_TenantId",
                table: "AccountingSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_TenantId_SingletonKey",
                table: "AccountingSettings",
                columns: new[] { "TenantId", "SingletonKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_UserId_IsActive",
                table: "TenantMemberships",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Tenants_TenantId",
                table: "AccountingSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Tenants_TenantId",
                table: "Accounts",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Tenants_TenantId",
                table: "AuditLogs",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Tenants_TenantId",
                table: "Branches",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWarehouseAccesses_Tenants_TenantId",
                table: "BranchWarehouseAccesses",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Tenants_TenantId",
                table: "Customers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountSettings_Tenants_TenantId",
                table: "DiscountSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentNumberSettings_Tenants_TenantId",
                table: "DocumentNumberSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralSettings_Tenants_TenantId",
                table: "GeneralSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventorySettings_Tenants_TenantId",
                table: "InventorySettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceSettings_Tenants_TenantId",
                table: "InvoiceSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Tenants_TenantId",
                table: "JournalEntries",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Tenants_TenantId",
                table: "JournalEntryLines",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_Tenants_TenantId",
                table: "LocationMovements",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Tenants_TenantId",
                table: "PaymentMethods",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalProducts_Tenants_TenantId",
                table: "PosTerminalProducts",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Tenants_TenantId",
                table: "PosTerminals",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalSettings_Tenants_TenantId",
                table: "PosTerminalSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalWarehouses_Tenants_TenantId",
                table: "PosTerminalWarehouses",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_Tenants_TenantId",
                table: "ProductLocationStocks",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Tenants_TenantId",
                table: "ProductStocks",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Tenants_TenantId",
                table: "PurchaseItems",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Tenants_TenantId",
                table: "Purchases",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Tenants_TenantId",
                table: "SaleItems",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Tenants_TenantId",
                table: "Sales",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Tenants_TenantId",
                table: "StockTransactions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferHistories_Tenants_TenantId",
                table: "StockTransferHistories",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_Tenants_TenantId",
                table: "StockTransferItems",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Tenants_TenantId",
                table: "StockTransfers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorageLocations_Tenants_TenantId",
                table: "StorageLocations",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Tenants_TenantId",
                table: "Suppliers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRates_Tenants_TenantId",
                table: "TaxRates",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Tenants_TenantId",
                table: "Warehouses",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Tenants_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Tenants_TenantId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Tenants_TenantId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Tenants_TenantId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchWarehouseAccesses_Tenants_TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Tenants_TenantId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountSettings_Tenants_TenantId",
                table: "DiscountSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentNumberSettings_Tenants_TenantId",
                table: "DocumentNumberSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralSettings_Tenants_TenantId",
                table: "GeneralSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_InventorySettings_Tenants_TenantId",
                table: "InventorySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceSettings_Tenants_TenantId",
                table: "InvoiceSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_Tenants_TenantId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Tenants_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_Tenants_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Tenants_TenantId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalProducts_Tenants_TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Tenants_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalSettings_Tenants_TenantId",
                table: "PosTerminalSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalWarehouses_Tenants_TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_Tenants_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Tenants_TenantId",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Tenants_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Tenants_TenantId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Tenants_TenantId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Tenants_TenantId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Tenants_TenantId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferHistories_Tenants_TenantId",
                table: "StockTransferHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_Tenants_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Tenants_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageLocations_Tenants_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Tenants_TenantId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxRates_Tenants_TenantId",
                table: "TaxRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Tenants_TenantId",
                table: "Warehouses");

            migrationBuilder.DropTable(
                name: "TenantMemberships");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_TenantId",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TenantId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_TenantId_WarehouseId_Code",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_WarehouseId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_TenantId_TransferNumber",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_StockTransferId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_TenantId_StockTransferId_ProductId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferHistories_TenantId",
                table: "StockTransferHistories");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_TenantId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_InvoiceNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_TenantId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_InvoiceNumber",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ProductId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_TenantId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_TenantId_ProductId_WarehouseId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_ProductId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_TenantId_ProductId_StorageLocationId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalWarehouses_TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalSettings_TenantId",
                table: "PosTerminalSettings");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_BranchId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_TenantId_BranchId_Name",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalProducts_TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_TenantId",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_TenantId_Name",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_TenantId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_TenantId_EntryNumber",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_TenantId_SourceType_SourceReference",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceSettings_TenantId",
                table: "InvoiceSettings");

            migrationBuilder.DropIndex(
                name: "IX_InventorySettings_TenantId",
                table: "InventorySettings");

            migrationBuilder.DropIndex(
                name: "IX_InventorySettings_TenantId_SingletonKey",
                table: "InventorySettings");

            migrationBuilder.DropIndex(
                name: "IX_GeneralSettings_TenantId",
                table: "GeneralSettings");

            migrationBuilder.DropIndex(
                name: "IX_GeneralSettings_TenantId_SingletonKey",
                table: "GeneralSettings");

            migrationBuilder.DropIndex(
                name: "IX_DocumentNumberSettings_TenantId",
                table: "DocumentNumberSettings");

            migrationBuilder.DropIndex(
                name: "IX_DiscountSettings_TenantId",
                table: "DiscountSettings");

            migrationBuilder.DropIndex(
                name: "IX_DiscountSettings_TenantId_SingletonKey",
                table: "DiscountSettings");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_AccountId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_BranchWarehouseAccesses_TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropIndex(
                name: "IX_Branches_TenantId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_TenantId_Code",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_TenantId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_TenantId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_TenantId_Code",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_TenantId_SingletonKey",
                table: "AccountingSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StockTransferHistories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PosTerminalSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PosTerminals");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LocationMovements");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InvoiceSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventorySettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DocumentNumberSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DiscountSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AccountingSettings");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_WarehouseId_Code",
                table: "StorageLocations",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_TransferNumber",
                table: "StockTransfers",
                column: "TransferNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_StockTransferId_ProductId",
                table: "StockTransferItems",
                columns: new[] { "StockTransferId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_InvoiceNumber",
                table: "Purchases",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ProductId_WarehouseId",
                table: "ProductStocks",
                columns: new[] { "ProductId", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_ProductId_StorageLocationId",
                table: "ProductLocationStocks",
                columns: new[] { "ProductId", "StorageLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId_Name",
                table: "PosTerminals",
                columns: new[] { "BranchId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Name",
                table: "PaymentMethods",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryNumber",
                table: "JournalEntries",
                column: "EntryNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_SourceType_SourceReference",
                table: "JournalEntries",
                columns: new[] { "SourceType", "SourceReference" },
                unique: true,
                filter: "[SourceType] IS NOT NULL AND [SourceReference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySettings_SingletonKey",
                table: "InventorySettings",
                column: "SingletonKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_SingletonKey",
                table: "GeneralSettings",
                column: "SingletonKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountSettings_SingletonKey",
                table: "DiscountSettings",
                column: "SingletonKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Code",
                table: "Branches",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Code",
                table: "Accounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SingletonKey",
                table: "AccountingSettings",
                column: "SingletonKey",
                unique: true);
        }
    }
}
