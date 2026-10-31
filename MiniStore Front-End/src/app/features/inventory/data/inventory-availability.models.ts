export interface InventoryWarehouseOption { id: number; name: string; }
export interface InventoryBalanceRow { productName: string; productCode: string; warehouseId: number; warehouseName: string; locationName: string; isUnassigned: boolean; onHand: number; reserved: number; available: number; }
export interface InventoryAvailabilityQuery { warehouseId?: number; search?: string; page: number; pageSize: 25 | 50 | 100 | 250; }
export interface InventoryAvailabilityPage { items: InventoryBalanceRow[]; warehouses: InventoryWarehouseOption[]; totalCount: number; page: number; pageSize: number; totalPages: number; }

export type InventoryReservationStatus = 1 | 2 | 3;
export interface InventoryReservationRow { id: number; sourceType: number; sourceReference: string; status: InventoryReservationStatus; createdAt: string; closedAt?: string; lineCount: number; totalQuantity: number; }
export interface InventoryReservationQuery { status?: InventoryReservationStatus; search?: string; page: number; pageSize: 25 | 50 | 100 | 250; }
export interface InventoryReservationPage { items: InventoryReservationRow[]; totalCount: number; page: number; pageSize: number; totalPages: number; }
