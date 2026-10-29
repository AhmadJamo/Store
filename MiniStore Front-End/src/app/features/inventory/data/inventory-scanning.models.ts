export type InventoryScanStatus = 0 | 1 | 2 | 3;

export interface InventoryScanQuery {
  adjustmentScan: string;
  productScan: string;
  sourceLocationScan: string;
  locationScan: string;
}

export interface InventoryScanResult {
  normalizedValue: string;
  status: InventoryScanStatus;
  entityId?: number;
  entityLongId?: number;
  displayName: string;
  secondaryText?: string;
}

export interface InventoryScanBalance {
  warehouseName: string;
  locationName: string;
  isUnassigned: boolean;
  onHand: number;
  reserved: number;
  available: number;
}

export interface InventoryScanPage {
  query: InventoryScanQuery;
  product: InventoryScanResult;
  adjustment: InventoryScanResult;
  sourceLocation: InventoryScanResult;
  location: InventoryScanResult;
  balances: InventoryScanBalance[];
  putawayIdempotencyKey: string;
  relocationIdempotencyKey: string;
  isBlindCount: boolean;
  canRecordCount: boolean;
}

export interface ScannedPutaway {
  productScan: string;
  locationScan: string;
  quantity: number;
  idempotencyKey: string;
}

export interface ScannedRelocation {
  productScan: string;
  sourceLocationScan: string;
  destinationLocationScan: string;
  quantity: number;
  idempotencyKey: string;
}

export interface ScannedInventoryCount {
  adjustmentNumber: string;
  productScan: string;
  locationScan: string;
  countedQuantity: number;
}
