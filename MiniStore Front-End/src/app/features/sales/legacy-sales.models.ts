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
export interface PosCashPolicy { currencyCode:string; varianceTolerance:number; mandatoryApprovalThreshold:number; requireApprovalForAnyVariance:boolean; cashPaymentMethodId:number|null; }
export interface PosCashDrawer { id:number; code:string; name:string; branchId:number; posTerminalId:number; isActive:boolean; }
export interface PosCashMovement { id:number; type:number; amount:number; occurredAtUtc:string; reason:string|null; sourceType:string|null; sourceReference:string|null; }
export interface PosCashSession { id:number; branchId:number; posTerminalId:number; drawerId:number; currencyCode:string; openedBy:string; openedAtUtc:string; openingFloat:number; status:number; expectedCash:number|null; declaredCash:number|null; variance:number|null; movements:PosCashMovement[]; rowVersion:string; }
export interface PosCashPage { policy:PosCashPolicy|null; drawers:PosCashDrawer[]; currentSession:PosCashSession|null; terminalName:string; accounts:{id:number;code:string;name:string}[]; }
