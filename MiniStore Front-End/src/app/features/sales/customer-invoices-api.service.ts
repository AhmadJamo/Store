import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InvoiceCreatePage, InvoiceDetails, InvoiceSummary } from './customer-invoices.models';
@Injectable({ providedIn: 'root' })
export class CustomerInvoicesApiService {
  private readonly base = '/api/v1/customer-invoices';
  constructor(private readonly http: HttpClient) {}
  list(status?: number, search?: string) {
    const params: any = {};
    if (status) params.status = status;
    if (search) params.search = search;
    return this.http.get<InvoiceSummary[]>(this.base, { params });
  }
  get(id: number) {
    return this.http.get<InvoiceDetails>(`${this.base}/${id}`);
  }
  createForm(salesOrderId: number) {
    return this.http.get<InvoiceCreatePage>(`${this.base}/create-form`, {
      params: { salesOrderId },
    });
  }
  create(input: any) {
    return this.http.post<{ id: number }>(this.base, input);
  }
  command(
    id: number,
    action: 'approve' | 'cancel' | 'post',
    rowVersion: string,
    reason: string | null = null,
  ) {
    return this.http.post<void>(`${this.base}/${id}/${action}`, { id, rowVersion, reason });
  }
  reverse(id: number, rowVersion: string, reversalDate: string, reason: string) {
    return this.http.post<void>(`${this.base}/${id}/reverse`, {
      id,
      rowVersion,
      reversalDate,
      reason,
    });
  }
}
