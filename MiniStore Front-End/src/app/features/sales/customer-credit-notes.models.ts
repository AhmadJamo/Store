export interface CreditNoteSummary {
  id: number;
  number: string;
  invoiceNumber: string;
  customerName: string;
  creditDate: string;
  currencyCode: string;
  totalAmount: number;
  status: number;
}
export interface CreditNoteLine {
  receiptNumber: string;
  productCode: string;
  productName: string;
  unitName: string;
  quantity: number;
  stockQuantity: number;
  netAmount: number;
  taxAmount: number;
  grossAmount: number;
}
export interface CreditNoteDetails {
  id: number;
  number: string;
  invoiceId: number;
  invoiceNumber: string;
  customerName: string;
  creditDate: string;
  currencyCode: string;
  reason: string;
  status: number;
  netAmount: number;
  taxAmount: number;
  totalAmount: number;
  companyTotalAmount: number;
  journalEntryId: number | null;
  journalEntryNumber: string | null;
  lines: CreditNoteLine[];
}
export interface CreditNoteAvailableLine {
  customerInvoiceLineId: number;
  customerReturnReceiptLineId: number;
  receiptNumber: string;
  productCode: string;
  productName: string;
  unitName: string;
  returnedStockQuantity: number;
  alreadyCreditedStockQuantity: number;
  remainingStockQuantity: number;
  remainingQuantity: number;
}
export interface CreateCreditNote {
  customerInvoiceId: number;
  requestKey: string;
  creditDate: string;
  reason: string;
  lines: { customerInvoiceLineId: number; customerReturnReceiptLineId: number; quantity: number }[];
}
export interface CreditNoteCreatePage {
  invoiceId: number;
  invoiceNumber: string;
  customerName: string;
  currencyCode: string;
  lines: CreditNoteAvailableLine[];
  form: CreateCreditNote;
}
