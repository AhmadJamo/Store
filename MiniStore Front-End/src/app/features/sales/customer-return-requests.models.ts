export interface ReturnRequestSummary {
  id: number;
  returnNumber: string;
  orderNumber: string;
  customerName: string;
  requestDate: string;
  status: number;
  quantity: number;
}
export interface ReturnRequestLine {
  id: number;
  salesDeliveryLineId: number;
  productCode: string;
  productName: string;
  warehouseName: string;
  locationName: string;
  deliveredQuantity: number;
  requestedQuantity: number;
  trackingAllocations: string | null;
}
export interface ReturnRequestHistory {
  fromStatus: number;
  toStatus: number;
  action: number;
  userId: string;
  reason: string | null;
  createdAtUtc: string;
}
export interface ReturnRequestDetails {
  id: number;
  returnNumber: string;
  salesDeliveryId: number;
  deliveryNumber: string;
  salesOrderId: number;
  orderNumber: string;
  customerName: string;
  requestDate: string;
  reason: string;
  notes: string | null;
  status: number;
  decisionReason: string | null;
  cancellationReason: string | null;
  rowVersion: string;
  lines: ReturnRequestLine[];
  history: ReturnRequestHistory[];
  hasRemainingReceipt: boolean;
  receiptIds: number[];
}
export interface ReturnEligibleLine {
  salesDeliveryLineId: number;
  productCode: string;
  productName: string;
  warehouseName: string;
  locationName: string;
  deliveredQuantity: number;
  alreadyRequestedQuantity: number;
  remainingQuantity: number;
}
export interface ReturnRequestCreatePage {
  salesDeliveryId: number;
  deliveryNumber: string;
  orderNumber: string;
  customerName: string;
  deliveryDate: string;
  form: {
    salesDeliveryId: number;
    requestDate: string;
    reason: string;
    notes: string | null;
    lines: { salesDeliveryLineId: number; stockQuantity: number }[];
  };
  lines: ReturnEligibleLine[];
}

export interface CreateReturnRequest {
  salesDeliveryId: number;
  requestDate: string;
  reason: string;
  notes: string | null;
  lines: { salesDeliveryLineId: number; stockQuantity: number }[];
}
