export interface SalesReturnLine {
  saleItemId: number;
  productId: number;
  productName: string;
  quantity: number;
  refundAmount: number;
  restocked: boolean;
  restockedCostAmount: number;
}
export interface SalesReturn {
  id: number;
  returnNumber: string;
  saleId: number;
  saleInvoiceNumber: string;
  date: string;
  reason: string;
  refundAmount: number;
  taxAmount: number;
  restockedCostAmount: number;
  items: SalesReturnLine[];
}
export interface SalesReturnCreateLine {
  saleItemId: number;
  productId: number;
  productName: string;
  soldQuantity: number;
  returnedQuantity: number;
  remainingQuantity: number;
  trackingPolicy: number;
}
export interface SalesReturnCreatePage {
  saleId: number;
  saleInvoiceNumber: string;
  date: string;
  lines: SalesReturnCreateLine[];
}
export interface SalesReturnCreateInput {
  saleId: number;
  date: string;
  reason: string;
  items: { saleItemId: number; quantity: number; trackingAllocations: string | null }[];
}
