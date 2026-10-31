export type StockTransferStatus = 1 | 2 | 3 | 4 | 5 | 6;

export interface StockTransferRow {
  id: number;
  transferNumber: string;
  fromWarehouseId: number;
  fromWarehouseName: string;
  toWarehouseId: number;
  toWarehouseName: string;
  status: StockTransferStatus;
  createdByUserId: string;
  createdAt: string;
  reference?: string;
  notes?: string;
  itemsCount: number;
}

export interface StockTransferItem {
  id: number;
  productId: number;
  productName: string;
  barcode?: string;
  quantity: number;
  sourceLocationId?: number;
  destinationLocationId?: number;
  sourceLocationCode: string;
  destinationLocationCode: string;
  trackingAllocations?: string;
}

export interface StockTransferHistory {
  id: number;
  fromStatus: StockTransferStatus;
  toStatus: StockTransferStatus;
  action: number;
  userId: string;
  userName: string;
  reason?: string;
  createdAt: string;
}

export interface StockTransferDetails extends StockTransferRow {
  createdByUserName: string;
  submittedByUserName?: string;
  submittedAt?: string;
  approvedByUserName?: string;
  approvedAt?: string;
  rejectedByUserName?: string;
  rejectedAt?: string;
  rejectionReason?: string;
  postedByUserName?: string;
  postedAt?: string;
  cancelledByUserName?: string;
  cancelledAt?: string;
  cancellationReason?: string;
  items: StockTransferItem[];
  history: StockTransferHistory[];
}

export interface StockTransferLineInput {
  productId: number;
  quantity: number;
  sourceLocationId?: number | null;
  destinationLocationId?: number | null;
  trackingAllocations?: string;
}

export interface StockTransferInput {
  fromWarehouseId: number;
  toWarehouseId: number;
  reference?: string;
  notes?: string;
  items: StockTransferLineInput[];
}

export interface StockTransferWarehouseOption {
  id: number;
  name: string;
  controlMode: number;
  requireSourceLocation: boolean;
  requireDestinationLocation: boolean;
}

export interface StockTransferProductOption {
  id: number;
  name: string;
  barcode?: string;
  trackingPolicy: number;
}

export interface StockTransferLocationOption {
  id: number;
  warehouseId: number;
  code: string;
  barcode?: string;
  isPickable: boolean;
  isReceivable: boolean;
}

export interface StockTransferFormPage {
  id?: number;
  transferNumber?: string;
  form: StockTransferInput;
  warehouses: StockTransferWarehouseOption[];
  products: StockTransferProductOption[];
  locations: StockTransferLocationOption[];
}
