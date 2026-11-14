# MOD-031A next ten-controller audit

> Completed: 2026-11-14

The next bounded cohort is the connected inventory, warehouse-operation and receipt-control group:
LocationMovements, StockTransactions, UnassignedStock, InventoryAdjustments, InventoryTracking, WarehouseLocations,
Warehouses, StockTransfers, GoodsReceipts and PurchaseDocumentReversals.

Every surface has an authenticated Angular workspace and versioned API over the same Application services. Independent
view/create/edit/workflow permissions and antiforgery remain on API mutations. The cohort has no MVC-owned download,
identity, Platform, subscription or public-token boundary. Legacy GET/HEAD navigation can be represented explicitly;
legacy mutations must terminate without redirect or replay. The owner explicitly approved selecting and executing the
next cohort without another approval pause, so MOD-031B is authorized for these exact ten controllers only.
