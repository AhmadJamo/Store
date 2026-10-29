export interface InvoiceSummary {
  id: number;
  invoiceNumber: string;
  salesOrderId: number;
  orderNumber: string;
  customerCode: string;
  customerName: string;
  invoiceDate: string;
  dueDate: string;
  currencyCode: string;
  totalAmount: number;
  status: number;
  paidAmount: number;
  creditedAmount: number;
  outstandingAmount: number;
}
export interface InvoiceSource {
  salesDeliveryId: number;
  salesDeliveryLineId: number;
  deliveryNumber: string;
  stockQuantity: number;
}
export interface InvoiceLine {
  id: number;
  salesOrderLineId: number;
  productCode: string;
  productName: string;
  unitName: string;
  quantity: number;
  stockQuantity: number;
  quantityBasis: number;
  unitPrice: number;
  netAmount: number;
  taxName: string | null;
  taxRatePercent: number | null;
  taxAmount: number;
  totalAmount: number;
  companyNetAmount: number;
  companyTaxAmount: number;
  companyTotalAmount: number;
  sources: InvoiceSource[];
}
export interface InvoiceHistory {
  fromStatus: number;
  toStatus: number;
  action: number;
  userId: string;
  reason: string | null;
  createdAtUtc: string;
}
export interface JournalLine {
  accountDisplay: string;
  branchName: string | null;
  warehouseName: string | null;
  debit: number;
  credit: number;
  description: string | null;
}
export interface InvoiceDetails {
  id: number;
  invoiceNumber: string;
  salesOrderId: number;
  orderNumber: string;
  quantityPolicy: number;
  customerCode: string;
  customerName: string;
  customerLegalName: string | null;
  customerTaxNumber: string | null;
  address: string | null;
  invoiceDate: string;
  dueDate: string;
  currencyCode: string;
  exchangeRate: number;
  exchangeRateDate: string;
  exchangeRateSource: string;
  paymentTermDays: number;
  notes: string | null;
  status: number;
  netAmount: number;
  taxAmount: number;
  totalAmount: number;
  paidAmount: number;
  creditedAmount: number;
  outstandingAmount: number;
  paymentStatus: string;
  companyCurrencyCode: string | null;
  companyNetAmount: number;
  companyTaxAmount: number;
  companyTotalAmount: number;
  receivableAccountDisplay: string | null;
  salesRevenueAccountDisplay: string | null;
  createdByUserId: string;
  createdAtUtc: string;
  approvedByUserId: string | null;
  approvedAtUtc: string | null;
  postedByUserId: string | null;
  postedAtUtc: string | null;
  reversedByUserId: string | null;
  reversedAtUtc: string | null;
  reversalDate: string | null;
  reversalReason: string | null;
  journalEntryId: number | null;
  journalEntryNumber: string | null;
  reversalJournalEntryId: number | null;
  reversalJournalEntryNumber: string | null;
  journalLines: JournalLine[];
  cancellationReason: string | null;
  rowVersion: string;
  lines: InvoiceLine[];
  history: InvoiceHistory[];
}
export interface EligibleInvoiceLine {
  salesOrderLineId: number;
  productCode: string;
  productName: string;
  unitName: string;
  quantityBasis: number;
  orderedQuantity: number;
  sourceEligibleQuantity: number;
  committedQuantity: number;
  remainingQuantity: number;
  unitPrice: number;
}
export interface InvoiceCreatePage {
  salesOrderId: number;
  orderNumber: string;
  customerCode: string;
  customerName: string;
  currencyCode: string;
  quantityPolicy: number;
  lines: EligibleInvoiceLine[];
  form: {
    salesOrderId: number;
    requestKey: string;
    invoiceDate: string;
    notes: string | null;
    lines: { salesOrderLineId: number; quantity: number }[];
  };
}
