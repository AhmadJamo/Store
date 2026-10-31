export type InventoryAdjustmentStatus = 1 | 2 | 3 | 4 | 5;
export interface InventoryAdjustmentRow { id: number; number: string; warehouse: string; location: string; status: InventoryAdjustmentStatus; isBlindCount: boolean; reason: string; createdAt: string; lineCount: number; }
export interface InventoryAdjustmentLine { productId: number; productName: string; expectedQuantity?: number; countedQuantity?: number; varianceQuantity?: number; }
export interface InventoryAdjustmentDetails extends InventoryAdjustmentRow { lines: InventoryAdjustmentLine[]; }
export interface InventoryAdjustmentWarehouseOption { id: number; name: string; }
export interface InventoryAdjustmentLocationOption { id: number; warehouseId: number; name: string; }
export interface InventoryAdjustmentProductOption { id: number; name: string; }
export interface CreateInventoryAdjustmentCommand { warehouseId: number; storageLocationId?: number; isBlindCount: boolean; reason: string; productIds: number[]; }
export interface InventoryAdjustmentCreatePage { input: CreateInventoryAdjustmentCommand; warehouses: InventoryAdjustmentWarehouseOption[]; locations: InventoryAdjustmentLocationOption[]; products: InventoryAdjustmentProductOption[]; }
export interface RecordInventoryCountsCommand { lines: { productId: number; countedQuantity: number }[]; }
