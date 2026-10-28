export interface PurchasingOption {
  id: number;
  label: string;
}
export interface SupplierPurchasingRow {
  id: number;
  supplierId: number;
  supplierName: string;
  productId: number;
  productName: string;
  productCode: string;
  purchaseMeasurementUnitId: number;
  purchaseUnit: string;
  supplierProductCode: string;
  supplierDescription?: string;
  minimumOrderQuantity: number;
  orderMultiple: number;
  leadTimeDays: number;
  unitPrice: number;
  currencyCode: string;
  validFrom: string;
  validTo?: string;
  isPreferred: boolean;
  priority: number;
  isActive: boolean;
  priceBreakCount: number;
  rowVersion: string;
}
export interface BulkPurchasingRow {
  selected: boolean;
  id: number;
  productId: number;
  productCode: string;
  productName: string;
  purchaseMeasurementUnitId: number;
  unitOptions: PurchasingOption[];
  supplierProductCode: string;
  supplierDescription?: string;
  minimumOrderQuantity: number;
  orderMultiple: number;
  leadTimeDays: number;
  unitPrice: number;
  currencyCode: string;
  validFrom: string;
  validTo?: string;
  isPreferred: boolean;
  priority: number;
  isActive: boolean;
  rowVersion: string;
}
export interface SupplierPurchasingPage {
  rows: SupplierPurchasingRow[];
  suppliers: PurchasingOption[];
  products: PurchasingOption[];
  units: PurchasingOption[];
  selectedSupplierId?: number;
  bulkForm: { supplierId: number; rows: BulkPurchasingRow[] };
  baseCurrency: string;
  priceExceptionCount: number;
}
export interface PriceBreakPage {
  purchasingInfoId: number;
  supplierId: number;
  supplierName: string;
  productLabel: string;
  purchaseUnit: string;
  minimumOrderQuantity: number;
  baseUnitPrice: number;
  currencyCode: string;
  isActive: boolean;
  form: {
    purchasingInfoId: number;
    supplierId: number;
    rowVersion: string;
    rows: {
      selected: boolean;
      minimumQuantity: number;
      unitPrice: number;
      discountPercent: number;
    }[];
  };
}
export interface PriceHistoryPage {
  purchasingInfoId: number;
  supplierId: number;
  supplierName: string;
  productLabel: string;
  purchaseUnit: string;
  rows: {
    revision: number;
    source: number;
    minimumOrderQuantity: number;
    baseUnitPrice: number;
    currencyCode: string;
    validFrom: string;
    validTo?: string;
    changedByUserId: string;
    changedAtUtc: string;
    priceBreaks: {
      minimumQuantity: number;
      unitPrice: number;
      discountPercent: number;
      netUnitPrice: number;
    }[];
  }[];
}
export interface PriceExceptionPage {
  asOfDate: string;
  expiringWithinDays: number;
  selectedType?: number;
  rows: {
    purchasingInfoId: number;
    supplierId: number;
    supplierName: string;
    productLabel: string;
    purchaseUnit: string;
    supplierProductCode: string;
    unitPrice: number;
    currencyCode: string;
    validTo?: string;
    isPreferred: boolean;
    issues: number[];
  }[];
  totalFindingCount: number;
  expiredCount: number;
  expiringSoonCount: number;
  duplicateSupplierProductCodeCount: number;
  multiplePreferredRowsCount: number;
  pageNumber: number;
  totalPages: number;
}
export interface ImportPreview {
  supplierId: number;
  supplierName: string;
  fileName: string;
  fileFormat: string;
  csvContent: string;
  encodedContent: string;
  validRowCount: number;
  errorRowCount: number;
  canImport: boolean;
  rows: {
    rowNumber: number;
    changeType: string;
    productCode: string;
    productName: string;
    purchaseUnitCode: string;
    supplierProductCode: string;
    unitPrice?: number;
    currencyCode: string;
    errors: string[];
  }[];
}
