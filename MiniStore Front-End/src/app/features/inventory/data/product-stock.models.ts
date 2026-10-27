export interface StockSelectionOption { id: number; name: string; }
export interface ProductStock { id: number; productId: number; productName: string; warehouseId: number; warehouseName: string; quantity: number; averageUnitCost: number; inventoryValue: number; }
export interface ProductStockCommand { productId: number; warehouseId: number; quantity: number; }
export interface ProductStockFormPage { form: ProductStockCommand; products: readonly StockSelectionOption[]; warehouses: readonly StockSelectionOption[]; }
export interface ManualStockAdjustmentCommand { productId: number; warehouseId: number; quantity: number; type: number; reference: string; }
export interface ManualStockAdjustmentFormPage { form: ManualStockAdjustmentCommand; products: readonly StockSelectionOption[]; warehouses: readonly StockSelectionOption[]; }
