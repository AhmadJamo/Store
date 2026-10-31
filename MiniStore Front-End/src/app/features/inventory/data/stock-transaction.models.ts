export type StockTransactionSort = 'id' | 'product' | 'warehouse' | 'quantity' | 'type' | 'reference' | 'date';
export type StockMovementFilter = 'incoming' | 'outgoing' | '';

export interface StockTransactionQuery {
  search?: string;
  type?: number;
  movement?: StockMovementFilter;
  fromDate?: string;
  toDate?: string;
  page: number;
  pageSize: 25 | 50 | 100 | 250;
  sortColumn: StockTransactionSort;
  sortDescending: boolean;
}

export interface StockTransaction {
  id: number;
  productId: number;
  productName: string;
  warehouseId: number;
  warehouseName: string;
  quantity: number;
  type: number;
  reference?: string;
  createdAt: string;
  quantityBefore: number;
  quantityAfter: number;
  averageUnitCostBefore: number;
  averageUnitCostAfter: number;
  inventoryValueBefore: number;
  inventoryValueAfter: number;
  unitCost: number;
  transactionValue: number;
  costVariance: number;
}

export interface StockTransactionPage {
  items: StockTransaction[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
