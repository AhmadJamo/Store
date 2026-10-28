import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

export interface SupplierPortalLine {
  id: number;
  productCode: string;
  productName: string;
  unitName: string;
  quantity: number;
  unitPrice: number | null;
}
export interface SupplierPortalPage {
  token: string;
  documentType: 1 | 2;
  documentId: number;
  documentNumber: string;
  supplierName: string;
  currencyCode: string;
  responseDeadline: string | null;
  expectedDate: string | null;
  paymentTerms: string | null;
  canSubmitQuotation: boolean;
  canConfirmOrder: boolean;
  canSubmitDeliveryNotice: boolean;
  lines: SupplierPortalLine[];
}
export interface SupplierPortalQuotationCommand {
  token: string;
  idempotencyKey: string;
  currencyCode: string;
  supplierReference: string;
  quotationDate: string;
  validUntilDate: string;
  leadTimeDays: number;
  paymentTerms: string;
  notes: string;
  lines: {
    documentLineId: number;
    quantity: number;
    unitPrice: number;
    discountPercent: number;
    taxPercent: number;
  }[];
}
export interface SupplierPortalConfirmationCommand {
  token: string;
  idempotencyKey: string;
  confirmedExpectedDate: string;
  confirmationReference: string;
  note: string;
}
export interface SupplierPortalDeliveryCommand {
  token: string;
  idempotencyKey: string;
  deliveryDate: string;
  reference: string;
  carrier: string;
  trackingNumber: string;
  note: string;
}
export interface SupplierPortalResult {
  replayed: boolean;
  submissionId: number;
  resultType: string | null;
  resultId: number | null;
}

@Injectable({ providedIn: 'root' })
export class SupplierPortalApiService {
  constructor(private readonly h: HttpClient) {}
  get(token: string) {
    return this.h.get<SupplierPortalPage>(
      `/api/public/supplier-portal/${encodeURIComponent(token)}`,
    );
  }
  quotation(v: SupplierPortalQuotationCommand) {
    return this.h.post<SupplierPortalResult>('/api/public/supplier-portal/quotation', v);
  }
  confirm(v: SupplierPortalConfirmationCommand) {
    return this.h.post<SupplierPortalResult>('/api/public/supplier-portal/order-confirmation', v);
  }
  delivery(v: SupplierPortalDeliveryCommand) {
    return this.h.post<SupplierPortalResult>('/api/public/supplier-portal/delivery-notice', v);
  }
}
