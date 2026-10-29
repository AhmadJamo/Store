export interface LegacySaleOption {
  id: number;
  label: string;
}
export interface LegacySaleProductOption {
  id: number;
  code: string;
  name: string;
  barcode: string | null;
  wholesalePrice: number;
  retailPrice: number;
  trackingPolicy: number;
}
export interface LegacySaleTaxOption extends LegacySaleOption {
  rate: number;
  isPriceInclusive: boolean;
}
export interface LegacySaleLineInput {
  productId: number;
  quantity: number;
  salePrice: number;
  discountType: number;
  discountValue: number;
  notes: string | null;
  trackingAllocations: string | null;
}
export interface LegacySaleCreateInput {
  invoiceNumber: string;
  warehouseId: number;
  posTerminalId: number | null;
  customerId: number | null;
  paymentMethodId: number;
  taxRateId: number | null;
  posOrderType: number | null;
  serviceReference: string | null;
  guestCount: number | null;
  date: string;
  notes: string | null;
  invoiceDiscountType: number;
  invoiceDiscountValue: number;
  items: LegacySaleLineInput[];
}
export interface LegacySaleCreatePage {
  form: LegacySaleCreateInput;
  products: LegacySaleProductOption[];
  warehouses: LegacySaleOption[];
  customers: LegacySaleOption[];
  paymentMethods: LegacySaleOption[];
  taxRates: LegacySaleTaxOption[];
}
export interface LegacyPosWarehouse {
  warehouseId: number;
  warehouseName: string;
  priority: number;
}
export interface LegacyPosTerminal {
  id: number;
  name: string;
  branchId: number;
  branchName: string;
  defaultWarehouseId: number;
  defaultWarehouseName: string;
  isActive: boolean;
  warehouses: LegacyPosWarehouse[];
}
export interface LegacyPosRuntimeSettings {
  posTerminalId: number;
  productLayout: number;
  theme: number;
  cartPosition: number;
  accentColor: string;
  headerTitle: string;
  productColumns: number;
  compactProductCards: boolean;
  showBarcode: boolean;
  showPrice: boolean;
  showStock: boolean;
  autoFocusSearch: boolean;
  touchOptimized: boolean;
  enabledOrderTypes: number;
  defaultOrderType: number;
  requireServiceReference: boolean;
  enableGuestCount: boolean;
  enableItemNotes: boolean;
  quickAddOnBarcodeScan: boolean;
}
export interface LegacyPosStock {
  productId: number;
  warehouseId: number;
  quantity: number;
}
export interface LegacyPosCreatePage extends LegacySaleCreatePage {
  terminals: LegacyPosTerminal[];
  runtimeSettings: LegacyPosRuntimeSettings[];
  stocks: LegacyPosStock[];
}
export interface LegacySaleSummary {
  id: number;
  invoiceNumber: string;
  channel: number;
  date: string;
  warehouseName: string;
  customerName: string | null;
  totalAmount: number;
}
export interface LegacySaleItemDetails {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  salePrice: number;
  grossTotal: number;
  discountType: number;
  discountValue: number;
  discountAmount: number;
  total: number;
  notes: string | null;
  unitCost: number;
  costOfGoodsSold: number;
}
export interface LegacySaleDetails {
  id: number;
  invoiceNumber: string;
  warehouseId: number;
  posTerminalId: number | null;
  posOrderType: number | null;
  serviceReference: string | null;
  guestCount: number | null;
  date: string;
  notes: string | null;
  subtotal: number;
  invoiceDiscountType: number;
  invoiceDiscountValue: number;
  invoiceDiscountAmount: number;
  taxRateId: number | null;
  taxRatePercent: number;
  isTaxInclusive: boolean;
  taxAmount: number;
  totalAmount: number;
  items: LegacySaleItemDetails[];
}
export interface LegacySaleDetailsPage {
  sale: LegacySaleDetails;
  channel: number;
  warehouseName: string;
  customerName: string | null;
  paymentMethodName: string;
  isPosted: boolean;
}
