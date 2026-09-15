using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HardenTenantIsolationRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_CostOfSalesAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_PurchaseDiscountAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesDiscountAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesRevenueAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Accounts_SalesRevenueAccountId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchWarehouseAccesses_Branches_BranchId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchWarehouseAccesses_Warehouses_WarehouseId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Accounts_AccountId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Accounts_AccountId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Branches_BranchId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_JournalEntries_JournalEntryId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Warehouses_WarehouseId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_Products_ProductId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_StorageLocations_FromStorageLocationId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_StorageLocations_ToStorageLocationId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_Warehouses_WarehouseId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Accounts_AccountId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalProducts_PosTerminals_PosTerminalId",
                table: "PosTerminalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalProducts_Products_ProductId",
                table: "PosTerminalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Branches_BranchId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Warehouses_DefaultWarehouseId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalSettings_PosTerminals_PosTerminalId",
                table: "PosTerminalSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalWarehouses_PosTerminals_PosTerminalId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalWarehouses_Warehouses_WarehouseId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_Products_ProductId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_StorageLocations_StorageLocationId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_Warehouses_WarehouseId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Products_ProductId",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Warehouses_WarehouseId",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Products_ProductId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_TaxRates_TaxRateId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Warehouses_WarehouseId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Products_ProductId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Sales_SaleId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PaymentMethods_PaymentMethodId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Warehouses_WarehouseId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Products_ProductId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Warehouses_WarehouseId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferHistories_StockTransfers_StockTransferId",
                table: "StockTransferHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_Products_ProductId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StockTransfers_StockTransferId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StorageLocations_DestinationLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StorageLocations_SourceLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Warehouses_FromWarehouseId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Warehouses_ToWarehouseId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageLocations_Warehouses_WarehouseId",
                table: "StorageLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Accounts_AccountId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxRates_Accounts_InputAccountId",
                table: "TaxRates");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxRates_Accounts_OutputAccountId",
                table: "TaxRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Accounts_InventoryAccountId",
                table: "Warehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Branches_BranchId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_BranchId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_InventoryAccountId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_InputAccountId",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_OutputAccountId",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_AccountId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_WarehouseId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_FromWarehouseId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ToWarehouseId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_DestinationLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_ProductId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_SourceLocationId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_StockTransferId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_WarehouseId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PaymentMethodId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_WarehouseId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_SupplierId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_WarehouseId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_PurchaseId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_TaxRateId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_WarehouseId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ProductId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_WarehouseId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_ProductId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_StorageLocationId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_WarehouseId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalWarehouses_WarehouseId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_BranchId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_DefaultWarehouseId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalProducts_ProductId",
                table: "PosTerminalProducts");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_AccountId",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_FromStorageLocationId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_ToStorageLocationId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_AccountId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_BranchId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_JournalEntryId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_WarehouseId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_BranchWarehouseAccesses_WarehouseId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropIndex(
                name: "IX_Branches_SalesRevenueAccountId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ParentAccountId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_CostOfSalesAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_PurchaseDiscountAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_SalesDiscountAccountId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_SalesRevenueAccountId",
                table: "AccountingSettings");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Warehouses_Id_TenantId",
                table: "Warehouses",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TaxRates_Id_TenantId",
                table: "TaxRates",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Suppliers_Id_TenantId",
                table: "Suppliers",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StorageLocations_Id_TenantId",
                table: "StorageLocations",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StockTransfers_Id_TenantId",
                table: "StockTransfers",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Sales_Id_TenantId",
                table: "Sales",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Purchases_Id_TenantId",
                table: "Purchases",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Products_Id_TenantId",
                table: "Products",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PosTerminals_Id_TenantId",
                table: "PosTerminals",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PaymentMethods_Id_TenantId",
                table: "PaymentMethods",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_JournalEntries_Id_TenantId",
                table: "JournalEntries",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Customers_Id_TenantId",
                table: "Customers",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Branches_Id_TenantId",
                table: "Branches",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Accounts_Id_TenantId",
                table: "Accounts",
                columns: new[] { "Id", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_BranchId_TenantId",
                table: "Warehouses",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_InventoryAccountId_TenantId",
                table: "Warehouses",
                columns: new[] { "InventoryAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_InputAccountId_TenantId",
                table: "TaxRates",
                columns: new[] { "InputAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_OutputAccountId_TenantId",
                table: "TaxRates",
                columns: new[] { "OutputAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AccountId_TenantId",
                table: "Suppliers",
                columns: new[] { "AccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_WarehouseId_TenantId",
                table: "StorageLocations",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_FromWarehouseId_TenantId",
                table: "StockTransfers",
                columns: new[] { "FromWarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ToWarehouseId_TenantId",
                table: "StockTransfers",
                columns: new[] { "ToWarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_DestinationLocationId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "DestinationLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_ProductId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_SourceLocationId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "SourceLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_StockTransferId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "StockTransferId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferHistories_StockTransferId_TenantId",
                table: "StockTransferHistories",
                columns: new[] { "StockTransferId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId_TenantId",
                table: "StockTransactions",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_WarehouseId_TenantId",
                table: "StockTransactions",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId_TenantId",
                table: "Sales",
                columns: new[] { "CustomerId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PaymentMethodId_TenantId",
                table: "Sales",
                columns: new[] { "PaymentMethodId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosTerminalId_TenantId",
                table: "Sales",
                columns: new[] { "PosTerminalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_WarehouseId_TenantId",
                table: "Sales",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId_TenantId",
                table: "SaleItems",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId_TenantId",
                table: "SaleItems",
                columns: new[] { "SaleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SupplierId_TenantId",
                table: "Purchases",
                columns: new[] { "SupplierId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_WarehouseId_TenantId",
                table: "Purchases",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ProductId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_PurchaseId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "PurchaseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_TaxRateId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "TaxRateId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_WarehouseId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ProductId_TenantId",
                table: "ProductStocks",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_WarehouseId_TenantId",
                table: "ProductStocks",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_ProductId_TenantId",
                table: "ProductLocationStocks",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_StorageLocationId_TenantId",
                table: "ProductLocationStocks",
                columns: new[] { "StorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_WarehouseId_TenantId",
                table: "ProductLocationStocks",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalWarehouses_PosTerminalId_TenantId",
                table: "PosTerminalWarehouses",
                columns: new[] { "PosTerminalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalWarehouses_WarehouseId_TenantId",
                table: "PosTerminalWarehouses",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalSettings_PosTerminalId_TenantId",
                table: "PosTerminalSettings",
                columns: new[] { "PosTerminalId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId_TenantId",
                table: "PosTerminals",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_DefaultWarehouseId_TenantId",
                table: "PosTerminals",
                columns: new[] { "DefaultWarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalProducts_PosTerminalId_TenantId",
                table: "PosTerminalProducts",
                columns: new[] { "PosTerminalId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalProducts_ProductId_TenantId",
                table: "PosTerminalProducts",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_AccountId_TenantId",
                table: "PaymentMethods",
                columns: new[] { "AccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_FromStorageLocationId_TenantId",
                table: "LocationMovements",
                columns: new[] { "FromStorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_ProductId_TenantId",
                table: "LocationMovements",
                columns: new[] { "ProductId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_ToStorageLocationId_TenantId",
                table: "LocationMovements",
                columns: new[] { "ToStorageLocationId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_WarehouseId_TenantId",
                table: "LocationMovements",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "AccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_BranchId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "JournalEntryId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_WarehouseId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId_TenantId",
                table: "Customers",
                columns: new[] { "AccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchWarehouseAccesses_BranchId_TenantId",
                table: "BranchWarehouseAccesses",
                columns: new[] { "BranchId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchWarehouseAccesses_WarehouseId_TenantId",
                table: "BranchWarehouseAccesses",
                columns: new[] { "WarehouseId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_SalesRevenueAccountId_TenantId",
                table: "Branches",
                columns: new[] { "SalesRevenueAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountId_TenantId",
                table: "Accounts",
                columns: new[] { "ParentAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_CostOfSalesAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "CostOfSalesAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_PurchaseDiscountAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "PurchaseDiscountAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SalesDiscountAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "SalesDiscountAccountId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SalesRevenueAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "SalesRevenueAccountId", "TenantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_CostOfSalesAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "CostOfSalesAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_PurchaseDiscountAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "PurchaseDiscountAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesDiscountAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "SalesDiscountAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesRevenueAccountId_TenantId",
                table: "AccountingSettings",
                columns: new[] { "SalesRevenueAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountId_TenantId",
                table: "Accounts",
                columns: new[] { "ParentAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Accounts_SalesRevenueAccountId_TenantId",
                table: "Branches",
                columns: new[] { "SalesRevenueAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWarehouseAccesses_Branches_BranchId_TenantId",
                table: "BranchWarehouseAccesses",
                columns: new[] { "BranchId", "TenantId" },
                principalTable: "Branches",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWarehouseAccesses_Warehouses_WarehouseId_TenantId",
                table: "BranchWarehouseAccesses",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Accounts_AccountId_TenantId",
                table: "Customers",
                columns: new[] { "AccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Accounts_AccountId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "AccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Branches_BranchId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "BranchId", "TenantId" },
                principalTable: "Branches",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_JournalEntries_JournalEntryId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "JournalEntryId", "TenantId" },
                principalTable: "JournalEntries",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Warehouses_WarehouseId_TenantId",
                table: "JournalEntryLines",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_Products_ProductId_TenantId",
                table: "LocationMovements",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_StorageLocations_FromStorageLocationId_TenantId",
                table: "LocationMovements",
                columns: new[] { "FromStorageLocationId", "TenantId" },
                principalTable: "StorageLocations",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_StorageLocations_ToStorageLocationId_TenantId",
                table: "LocationMovements",
                columns: new[] { "ToStorageLocationId", "TenantId" },
                principalTable: "StorageLocations",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_Warehouses_WarehouseId_TenantId",
                table: "LocationMovements",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Accounts_AccountId_TenantId",
                table: "PaymentMethods",
                columns: new[] { "AccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalProducts_PosTerminals_PosTerminalId_TenantId",
                table: "PosTerminalProducts",
                columns: new[] { "PosTerminalId", "TenantId" },
                principalTable: "PosTerminals",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalProducts_Products_ProductId_TenantId",
                table: "PosTerminalProducts",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Branches_BranchId_TenantId",
                table: "PosTerminals",
                columns: new[] { "BranchId", "TenantId" },
                principalTable: "Branches",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Warehouses_DefaultWarehouseId_TenantId",
                table: "PosTerminals",
                columns: new[] { "DefaultWarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalSettings_PosTerminals_PosTerminalId_TenantId",
                table: "PosTerminalSettings",
                columns: new[] { "PosTerminalId", "TenantId" },
                principalTable: "PosTerminals",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalWarehouses_PosTerminals_PosTerminalId_TenantId",
                table: "PosTerminalWarehouses",
                columns: new[] { "PosTerminalId", "TenantId" },
                principalTable: "PosTerminals",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalWarehouses_Warehouses_WarehouseId_TenantId",
                table: "PosTerminalWarehouses",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_Products_ProductId_TenantId",
                table: "ProductLocationStocks",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_StorageLocations_StorageLocationId_TenantId",
                table: "ProductLocationStocks",
                columns: new[] { "StorageLocationId", "TenantId" },
                principalTable: "StorageLocations",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_Warehouses_WarehouseId_TenantId",
                table: "ProductLocationStocks",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Products_ProductId_TenantId",
                table: "ProductStocks",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Warehouses_WarehouseId_TenantId",
                table: "ProductStocks",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Products_ProductId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "PurchaseId", "TenantId" },
                principalTable: "Purchases",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_TaxRates_TaxRateId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "TaxRateId", "TenantId" },
                principalTable: "TaxRates",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId_TenantId",
                table: "PurchaseItems",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId_TenantId",
                table: "Purchases",
                columns: new[] { "SupplierId", "TenantId" },
                principalTable: "Suppliers",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Warehouses_WarehouseId_TenantId",
                table: "Purchases",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Products_ProductId_TenantId",
                table: "SaleItems",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Sales_SaleId_TenantId",
                table: "SaleItems",
                columns: new[] { "SaleId", "TenantId" },
                principalTable: "Sales",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId_TenantId",
                table: "Sales",
                columns: new[] { "CustomerId", "TenantId" },
                principalTable: "Customers",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PaymentMethods_PaymentMethodId_TenantId",
                table: "Sales",
                columns: new[] { "PaymentMethodId", "TenantId" },
                principalTable: "PaymentMethods",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId_TenantId",
                table: "Sales",
                columns: new[] { "PosTerminalId", "TenantId" },
                principalTable: "PosTerminals",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Warehouses_WarehouseId_TenantId",
                table: "Sales",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Products_ProductId_TenantId",
                table: "StockTransactions",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Warehouses_WarehouseId_TenantId",
                table: "StockTransactions",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferHistories_StockTransfers_StockTransferId_TenantId",
                table: "StockTransferHistories",
                columns: new[] { "StockTransferId", "TenantId" },
                principalTable: "StockTransfers",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_Products_ProductId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "ProductId", "TenantId" },
                principalTable: "Products",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StockTransfers_StockTransferId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "StockTransferId", "TenantId" },
                principalTable: "StockTransfers",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StorageLocations_DestinationLocationId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "DestinationLocationId", "TenantId" },
                principalTable: "StorageLocations",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StorageLocations_SourceLocationId_TenantId",
                table: "StockTransferItems",
                columns: new[] { "SourceLocationId", "TenantId" },
                principalTable: "StorageLocations",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Warehouses_FromWarehouseId_TenantId",
                table: "StockTransfers",
                columns: new[] { "FromWarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Warehouses_ToWarehouseId_TenantId",
                table: "StockTransfers",
                columns: new[] { "ToWarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorageLocations_Warehouses_WarehouseId_TenantId",
                table: "StorageLocations",
                columns: new[] { "WarehouseId", "TenantId" },
                principalTable: "Warehouses",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Accounts_AccountId_TenantId",
                table: "Suppliers",
                columns: new[] { "AccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRates_Accounts_InputAccountId_TenantId",
                table: "TaxRates",
                columns: new[] { "InputAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRates_Accounts_OutputAccountId_TenantId",
                table: "TaxRates",
                columns: new[] { "OutputAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Accounts_InventoryAccountId_TenantId",
                table: "Warehouses",
                columns: new[] { "InventoryAccountId", "TenantId" },
                principalTable: "Accounts",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Branches_BranchId_TenantId",
                table: "Warehouses",
                columns: new[] { "BranchId", "TenantId" },
                principalTable: "Branches",
                principalColumns: new[] { "Id", "TenantId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_CostOfSalesAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_PurchaseDiscountAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesDiscountAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesRevenueAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountId_TenantId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Accounts_SalesRevenueAccountId_TenantId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchWarehouseAccesses_Branches_BranchId_TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchWarehouseAccesses_Warehouses_WarehouseId_TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Accounts_AccountId_TenantId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Accounts_AccountId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Branches_BranchId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_JournalEntries_JournalEntryId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Warehouses_WarehouseId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_Products_ProductId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_StorageLocations_FromStorageLocationId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_StorageLocations_ToStorageLocationId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMovements_Warehouses_WarehouseId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Accounts_AccountId_TenantId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalProducts_PosTerminals_PosTerminalId_TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalProducts_Products_ProductId_TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Branches_BranchId_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Warehouses_DefaultWarehouseId_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalSettings_PosTerminals_PosTerminalId_TenantId",
                table: "PosTerminalSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalWarehouses_PosTerminals_PosTerminalId_TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminalWarehouses_Warehouses_WarehouseId_TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_Products_ProductId_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_StorageLocations_StorageLocationId_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationStocks_Warehouses_WarehouseId_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Products_ProductId_TenantId",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Warehouses_WarehouseId_TenantId",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Products_ProductId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_TaxRates_TaxRateId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId_TenantId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Warehouses_WarehouseId_TenantId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Products_ProductId_TenantId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Sales_SaleId_TenantId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId_TenantId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PaymentMethods_PaymentMethodId_TenantId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId_TenantId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Warehouses_WarehouseId_TenantId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Products_ProductId_TenantId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Warehouses_WarehouseId_TenantId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferHistories_StockTransfers_StockTransferId_TenantId",
                table: "StockTransferHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_Products_ProductId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StockTransfers_StockTransferId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StorageLocations_DestinationLocationId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StorageLocations_SourceLocationId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Warehouses_FromWarehouseId_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Warehouses_ToWarehouseId_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageLocations_Warehouses_WarehouseId_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Accounts_AccountId_TenantId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxRates_Accounts_InputAccountId_TenantId",
                table: "TaxRates");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxRates_Accounts_OutputAccountId_TenantId",
                table: "TaxRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Accounts_InventoryAccountId_TenantId",
                table: "Warehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Branches_BranchId_TenantId",
                table: "Warehouses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Warehouses_Id_TenantId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_BranchId_TenantId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_InventoryAccountId_TenantId",
                table: "Warehouses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TaxRates_Id_TenantId",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_InputAccountId_TenantId",
                table: "TaxRates");

            migrationBuilder.DropIndex(
                name: "IX_TaxRates_OutputAccountId_TenantId",
                table: "TaxRates");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Suppliers_Id_TenantId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_AccountId_TenantId",
                table: "Suppliers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StorageLocations_Id_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_WarehouseId_TenantId",
                table: "StorageLocations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StockTransfers_Id_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_FromWarehouseId_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ToWarehouseId_TenantId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_DestinationLocationId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_ProductId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_SourceLocationId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferItems_StockTransferId_TenantId",
                table: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransferHistories_StockTransferId_TenantId",
                table: "StockTransferHistories");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_ProductId_TenantId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_WarehouseId_TenantId",
                table: "StockTransactions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Sales_Id_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerId_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PaymentMethodId_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PosTerminalId_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_WarehouseId_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_ProductId_TenantId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_SaleId_TenantId",
                table: "SaleItems");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Purchases_Id_TenantId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_SupplierId_TenantId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_WarehouseId_TenantId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_ProductId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_PurchaseId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_TaxRateId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_WarehouseId_TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ProductId_TenantId",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_WarehouseId_TenantId",
                table: "ProductStocks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Products_Id_TenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_ProductId_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_StorageLocationId_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductLocationStocks_WarehouseId_TenantId",
                table: "ProductLocationStocks");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalWarehouses_PosTerminalId_TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalWarehouses_WarehouseId_TenantId",
                table: "PosTerminalWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalSettings_PosTerminalId_TenantId",
                table: "PosTerminalSettings");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PosTerminals_Id_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_BranchId_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminals_DefaultWarehouseId_TenantId",
                table: "PosTerminals");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalProducts_PosTerminalId_TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropIndex(
                name: "IX_PosTerminalProducts_ProductId_TenantId",
                table: "PosTerminalProducts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PaymentMethods_Id_TenantId",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_AccountId_TenantId",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_FromStorageLocationId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_ProductId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_ToStorageLocationId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_LocationMovements_WarehouseId_TenantId",
                table: "LocationMovements");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_AccountId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_BranchId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_JournalEntryId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_WarehouseId_TenantId",
                table: "JournalEntryLines");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_JournalEntries_Id_TenantId",
                table: "JournalEntries");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Customers_Id_TenantId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountId_TenantId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_BranchWarehouseAccesses_BranchId_TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropIndex(
                name: "IX_BranchWarehouseAccesses_WarehouseId_TenantId",
                table: "BranchWarehouseAccesses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Branches_Id_TenantId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_SalesRevenueAccountId_TenantId",
                table: "Branches");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Accounts_Id_TenantId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ParentAccountId_TenantId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_CostOfSalesAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_PurchaseDiscountAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_SalesDiscountAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_AccountingSettings_SalesRevenueAccountId_TenantId",
                table: "AccountingSettings");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_BranchId",
                table: "Warehouses",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_InventoryAccountId",
                table: "Warehouses",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_InputAccountId",
                table: "TaxRates",
                column: "InputAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_OutputAccountId",
                table: "TaxRates",
                column: "OutputAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AccountId",
                table: "Suppliers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_WarehouseId",
                table: "StorageLocations",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_FromWarehouseId",
                table: "StockTransfers",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ToWarehouseId",
                table: "StockTransfers",
                column: "ToWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_DestinationLocationId",
                table: "StockTransferItems",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_ProductId",
                table: "StockTransferItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_SourceLocationId",
                table: "StockTransferItems",
                column: "SourceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_StockTransferId",
                table: "StockTransferItems",
                column: "StockTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_WarehouseId",
                table: "StockTransactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PaymentMethodId",
                table: "Sales",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_WarehouseId",
                table: "Sales",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId",
                table: "SaleItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_SaleId",
                table: "SaleItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SupplierId",
                table: "Purchases",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_WarehouseId",
                table: "Purchases",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_PurchaseId",
                table: "PurchaseItems",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_TaxRateId",
                table: "PurchaseItems",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_WarehouseId",
                table: "PurchaseItems",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ProductId",
                table: "ProductStocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_WarehouseId",
                table: "ProductStocks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_ProductId",
                table: "ProductLocationStocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_StorageLocationId",
                table: "ProductLocationStocks",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationStocks_WarehouseId",
                table: "ProductLocationStocks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalWarehouses_WarehouseId",
                table: "PosTerminalWarehouses",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId",
                table: "PosTerminals",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_DefaultWarehouseId",
                table: "PosTerminals",
                column: "DefaultWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminalProducts_ProductId",
                table: "PosTerminalProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_AccountId",
                table: "PaymentMethods",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_FromStorageLocationId",
                table: "LocationMovements",
                column: "FromStorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMovements_ToStorageLocationId",
                table: "LocationMovements",
                column: "ToStorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountId",
                table: "JournalEntryLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_BranchId",
                table: "JournalEntryLines",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryId",
                table: "JournalEntryLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_WarehouseId",
                table: "JournalEntryLines",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchWarehouseAccesses_WarehouseId",
                table: "BranchWarehouseAccesses",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_SalesRevenueAccountId",
                table: "Branches",
                column: "SalesRevenueAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountId",
                table: "Accounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_CostOfSalesAccountId",
                table: "AccountingSettings",
                column: "CostOfSalesAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_PurchaseDiscountAccountId",
                table: "AccountingSettings",
                column: "PurchaseDiscountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SalesDiscountAccountId",
                table: "AccountingSettings",
                column: "SalesDiscountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSettings_SalesRevenueAccountId",
                table: "AccountingSettings",
                column: "SalesRevenueAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_CostOfSalesAccountId",
                table: "AccountingSettings",
                column: "CostOfSalesAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_PurchaseDiscountAccountId",
                table: "AccountingSettings",
                column: "PurchaseDiscountAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesDiscountAccountId",
                table: "AccountingSettings",
                column: "SalesDiscountAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingSettings_Accounts_SalesRevenueAccountId",
                table: "AccountingSettings",
                column: "SalesRevenueAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountId",
                table: "Accounts",
                column: "ParentAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Accounts_SalesRevenueAccountId",
                table: "Branches",
                column: "SalesRevenueAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWarehouseAccesses_Branches_BranchId",
                table: "BranchWarehouseAccesses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchWarehouseAccesses_Warehouses_WarehouseId",
                table: "BranchWarehouseAccesses",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Accounts_AccountId",
                table: "Customers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Accounts_AccountId",
                table: "JournalEntryLines",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Branches_BranchId",
                table: "JournalEntryLines",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_JournalEntries_JournalEntryId",
                table: "JournalEntryLines",
                column: "JournalEntryId",
                principalTable: "JournalEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Warehouses_WarehouseId",
                table: "JournalEntryLines",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_Products_ProductId",
                table: "LocationMovements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_StorageLocations_FromStorageLocationId",
                table: "LocationMovements",
                column: "FromStorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_StorageLocations_ToStorageLocationId",
                table: "LocationMovements",
                column: "ToStorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMovements_Warehouses_WarehouseId",
                table: "LocationMovements",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Accounts_AccountId",
                table: "PaymentMethods",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalProducts_PosTerminals_PosTerminalId",
                table: "PosTerminalProducts",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalProducts_Products_ProductId",
                table: "PosTerminalProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Branches_BranchId",
                table: "PosTerminals",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Warehouses_DefaultWarehouseId",
                table: "PosTerminals",
                column: "DefaultWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalSettings_PosTerminals_PosTerminalId",
                table: "PosTerminalSettings",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalWarehouses_PosTerminals_PosTerminalId",
                table: "PosTerminalWarehouses",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminalWarehouses_Warehouses_WarehouseId",
                table: "PosTerminalWarehouses",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_Products_ProductId",
                table: "ProductLocationStocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_StorageLocations_StorageLocationId",
                table: "ProductLocationStocks",
                column: "StorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationStocks_Warehouses_WarehouseId",
                table: "ProductLocationStocks",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Products_ProductId",
                table: "ProductStocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Warehouses_WarehouseId",
                table: "ProductStocks",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Products_ProductId",
                table: "PurchaseItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseId",
                table: "PurchaseItems",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_TaxRates_TaxRateId",
                table: "PurchaseItems",
                column: "TaxRateId",
                principalTable: "TaxRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Warehouses_WarehouseId",
                table: "PurchaseItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId",
                table: "Purchases",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Warehouses_WarehouseId",
                table: "Purchases",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Products_ProductId",
                table: "SaleItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Sales_SaleId",
                table: "SaleItems",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PaymentMethods_PaymentMethodId",
                table: "Sales",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Warehouses_WarehouseId",
                table: "Sales",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Products_ProductId",
                table: "StockTransactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Warehouses_WarehouseId",
                table: "StockTransactions",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferHistories_StockTransfers_StockTransferId",
                table: "StockTransferHistories",
                column: "StockTransferId",
                principalTable: "StockTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_Products_ProductId",
                table: "StockTransferItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StockTransfers_StockTransferId",
                table: "StockTransferItems",
                column: "StockTransferId",
                principalTable: "StockTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StorageLocations_DestinationLocationId",
                table: "StockTransferItems",
                column: "DestinationLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StorageLocations_SourceLocationId",
                table: "StockTransferItems",
                column: "SourceLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Warehouses_FromWarehouseId",
                table: "StockTransfers",
                column: "FromWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Warehouses_ToWarehouseId",
                table: "StockTransfers",
                column: "ToWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorageLocations_Warehouses_WarehouseId",
                table: "StorageLocations",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Accounts_AccountId",
                table: "Suppliers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRates_Accounts_InputAccountId",
                table: "TaxRates",
                column: "InputAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRates_Accounts_OutputAccountId",
                table: "TaxRates",
                column: "OutputAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Accounts_InventoryAccountId",
                table: "Warehouses",
                column: "InventoryAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Branches_BranchId",
                table: "Warehouses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
