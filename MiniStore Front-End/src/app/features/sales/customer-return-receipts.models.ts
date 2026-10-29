export interface ReturnReceiptSummary {
  id: number;
  receiptNumber: string;
  requestId: number;
  requestNumber: string;
  receiptDate: string;
  status: number;
  quantity: number;
}
export interface ReturnReceiptLine {
  id: number;
  productCode: string;
  productName: string;
  warehouseName: string;
  locationName: string;
  receivedQuantity: number;
  disposition: number;
  inspectionNotes: string;
  trackingAllocations: string | null;
  unitCostSnapshot: number;
  restockedQuantity: number;
  restockedCost: number;
}
export interface ReturnReceiptHistory {
  fromStatus: number;
  toStatus: number;
  action: number;
  userId: string;
  reason: string | null;
  createdAtUtc: string;
}
export interface ReturnReceiptDetails {
  id: number;
  receiptNumber: string;
  requestId: number;
  requestNumber: string;
  receiptDate: string;
  notes: string | null;
  status: number;
  cancellationReason: string | null;
  postedByUserId: string | null;
  postedAtUtc: string | null;
  journalEntryId: number | null;
  journalEntryNumber: string | null;
  totalRestockedQuantity: number;
  totalRestockedCost: number;
  rowVersion: string;
  lines: ReturnReceiptLine[];
  history: ReturnReceiptHistory[];
}
export interface ReturnReceiptEligibleLine {
  requestLineId: number;
  productCode: string;
  productName: string;
  warehouseName: string;
  locationName: string;
  trackingPolicy: number;
  approvedQuantity: number;
  alreadyReceivedQuantity: number;
  remainingQuantity: number;
}
export interface CreateReturnReceiptLine {
  customerReturnRequestLineId: number;
  receivedQuantity: number;
  disposition: number;
  inspectionNotes: string;
  trackingAllocations: string | null;
}
export interface CreateReturnReceipt {
  customerReturnRequestId: number;
  receiptDate: string;
  notes: string | null;
  lines: CreateReturnReceiptLine[];
}
export interface ReturnReceiptCreatePage {
  requestId: number;
  requestNumber: string;
  requestDate: string;
  customerName: string;
  form: CreateReturnReceipt;
  lines: ReturnReceiptEligibleLine[];
}
