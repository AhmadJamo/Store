export type InventoryActivityState = 0 | 1 | 2 | 3 | 4;

export interface InventoryInsightsQuery {
  warehouseId?: number | null;
  search: string;
  state?: InventoryActivityState | null;
  attentionOnly: boolean;
  slowDays: number;
  deadDays: number;
}

export interface InventoryInsightRow {
  productId: number;
  productName: string;
  productCode: string;
  warehouseId: number;
  warehouseName: string;
  onHand: number;
  inventoryValue: number;
  lastOutboundAt?: string;
  firstInboundAt?: string;
  activityAnchorAt?: string;
  daysSinceActivity?: number;
  state: InventoryActivityState;
}

export interface NamedWarehouse { id: number; name: string; }

export interface InventoryInsightsPage {
  query: InventoryInsightsQuery;
  rows: InventoryInsightRow[];
  warehouses: NamedWarehouse[];
  totalValue: number;
  attentionValue: number;
  slowCount: number;
  deadCount: number;
  neverIssuedCount: number;
  noMovementHistoryCount: number;
}

export type InventoryReconciliationIssue = 0 | 1 | 2 | 3 | 4 | 5 | 6;

export interface InventoryReconciliationRow {
  productId: number;
  productName: string;
  productCode: string;
  barcode: string;
  warehouseId: number;
  warehouseName: string;
  warehouseQuantity: number;
  assignedLocationQuantity: number;
  unassignedQuantity: number;
  inventoryValue: number;
  latestTransactionId?: number;
  latestTransactionAt?: string;
  latestQuantityAfter?: number;
  latestInventoryValueAfter?: number;
  issues: InventoryReconciliationIssue[];
  isConsistent: boolean;
}

export interface InventoryReconciliationPage {
  items: InventoryReconciliationRow[];
  warehouses: NamedWarehouse[];
  warehouseId?: number;
  search: string;
  exceptionsOnly: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  exceptionCount: number;
  totalPages: number;
}
