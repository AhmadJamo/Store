export interface Product {
  id: number;
  productCode: string;
  name: string;
  barcode?: string;
  purchasePrice: number;
  salePrice: number;
  wholesalePrice: number;
  inventoryBehavior: number;
  productType: number;
  stockUnit: number;
  measurementUnitId?: number;
  measurementUnitName?: string;
  allowNegativeRecipeConsumption: boolean;
  isSellableInPos: boolean;
  isSellableInSales: boolean;
  isActive: boolean;
  productCategoryId?: number;
  productCategoryName?: string;
  productTemplateId?: number;
  variantLabel?: string;
  netWeight?: number;
  grossWeight?: number;
  weightMeasurementUnitId?: number;
  length?: number;
  width?: number;
  height?: number;
  dimensionMeasurementUnitId?: number;
  trackingPolicy: number;
  defaultShelfLifeDays?: number;
  requireExpirationDate: boolean;
  expirationWarningDays: number;
  handlingRequirements: number;
  supplierIds: readonly number[];
  supplierNames: readonly string[];
  purchaseBillingPolicy: number;
  purchaseExpenseAccountId?: number;
}

export interface ProductPage {
  query: { page: number; pageSize: number; search?: string; isActive?: boolean; sortBy: string; sortDescending: boolean };
  items: readonly Product[];
  totalCount: number;
  totalPages: number;
  categories: readonly ProductCategoryOption[];
}

export interface ProductCategoryOption { id: number; name: string; code: string; isActive: boolean; }
export interface ProductUnitOption { id: number; code: string; name: string; symbol: string; dimension: number; isActive: boolean; }
export interface ProductSupplierOption { id: number; name: string; }
export interface ProductAccountOption { id: number; code: string; name: string; type: number; }

export interface ProductFormPage {
  form: Product;
  measurementUnits: readonly ProductUnitOption[];
  categories: readonly ProductCategoryOption[];
  suppliers: readonly ProductSupplierOption[];
  expenseAccounts: readonly ProductAccountOption[];
}

export interface ProductCommand {
  name: string;
  barcode?: string;
  purchasePrice: number;
  salePrice: number;
  wholesalePrice: number;
  measurementUnitId: number;
  allowNegativeRecipeConsumption: boolean;
  productType: number;
  isSellableInPos: boolean;
  isSellableInSales: boolean;
  isActive: boolean;
  supplierIds: readonly number[];
  purchaseBillingPolicy: number;
  purchaseExpenseAccountId?: number;
  productCategoryId?: number;
  netWeight?: number;
  grossWeight?: number;
  weightMeasurementUnitId?: number;
  length?: number;
  width?: number;
  height?: number;
  dimensionMeasurementUnitId?: number;
  trackingPolicy: number;
  defaultShelfLifeDays?: number;
  requireExpirationDate: boolean;
  expirationWarningDays: number;
  isFragile: boolean;
  keepDry: boolean;
  requiresRefrigeration: boolean;
  requiresFrozenStorage: boolean;
  isHazardous: boolean;
}
