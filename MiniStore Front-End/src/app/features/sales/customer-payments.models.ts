export interface CustomerPaymentLine {
  customerInvoiceId: number;
  invoiceNumber: string;
  amount: number;
  companyCurrencyAmount: number;
  receivableCarryingAmount: number;
}
export interface CustomerPayment {
  id: number;
  paymentNumber: string;
  customerId: number;
  customerName: string;
  paymentMethodName: string;
  paymentDate: string;
  currencyCode: string;
  companyCurrencyCode: string;
  exchangeRate: number;
  exchangeRateDate: string;
  exchangeRateSource: string;
  externalReference: string | null;
  notes: string | null;
  status: number;
  totalAmount: number;
  companyCurrencyAmount: number;
  receivableCarryingAmount: number;
  rowVersion: string;
  journalEntryId: number | null;
  journalEntryNumber: string | null;
  reversalJournalEntryId: number | null;
  reversalJournalEntryNumber: string | null;
  reversalDate: string | null;
  reversalReason: string | null;
  lines: CustomerPaymentLine[];
}
export interface OpenCustomerInvoice {
  customerInvoiceId: number;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  totalAmount: number;
  paidAmount: number;
  outstandingAmount: number;
}
export interface CustomerPaymentCreatePage {
  customerName: string;
  currencyCode: string;
  openInvoices: OpenCustomerInvoice[];
  paymentMethods: { id: number; name: string }[];
  form: {
    sourceCustomerInvoiceId: number;
    paymentMethodId: number;
    paymentDate: string;
    externalReference: string | null;
    notes: string | null;
    lines: { customerInvoiceId: number; amount: number }[];
  };
}
export interface CreateCustomerPayment {
  sourceCustomerInvoiceId: number;
  paymentMethodId: number;
  paymentDate: string;
  externalReference: string | null;
  notes: string | null;
  lines: { customerInvoiceId: number; amount: number }[];
}
