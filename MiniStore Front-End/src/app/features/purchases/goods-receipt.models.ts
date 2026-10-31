export interface GoodsReceiptRemainingLine {
  purchaseOrderLineId: number;
  productCode: string;
  productName: string;
  unitName: string;
  remainingQuantity: number;
}

export interface GoodsReceiptCreatePage {
  purchaseOrder: { id: number; orderNumber: string; supplierName: string; warehouseName: string };
  receivingRoute: number;
  form: { purchaseOrderId: number; receiptDate: string; notes?: string };
  remainingLines: GoodsReceiptRemainingLine[];
}

export interface GoodsReceiptLine {
  id: number;
  productCode: string;
  productName: string;
  unitName: string;
  receivedQuantity: number;
  deliveredQuantity: number;
  acceptedQuantity: number;
  rejectedQuantity: number;
  damagedQuantity: number;
  shortQuantity: number;
  discrepancyReason?: string;
  stockQuantity: number;
  unitCost: number;
  lotNumber?: string;
  serialNumbers?: string;
  manufactureDate?: string;
  expirationDate?: string;
}

export interface GoodsReceipt {
  id: number;
  receiptNumber: string;
  purchaseOrderNumber: string;
  warehouseName: string;
  receiptDate: string;
  currencyCode: string;
  companyCurrencyCode: string;
  exchangeRate: number;
  exchangeRateDate: string;
  exchangeRateSource: string;
  status: number;
  receivingRoute: number;
  inspectedByUserId?: string;
  inspectedAtUtc?: string;
  inspectionNotes?: string;
  notes?: string;
  lines: GoodsReceiptLine[];
}

export interface GoodsReceiptScanPage {
  receipt: GoodsReceipt;
  idempotencyKey: string;
  lines: {
    purchaseOrderLineId: number;
    productCode: string;
    productName: string;
    barcode?: string;
    unitName: string;
    trackingPolicy: number;
    orderedQuantity: number;
    previouslyPostedQuantity: number;
    draftQuantity: number;
    remainingQuantity: number;
  }[];
}
