import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CreateReturnReceipt,
  ReturnReceiptCreatePage,
  ReturnReceiptDetails,
  ReturnReceiptSummary,
} from './customer-return-receipts.models';

@Injectable({ providedIn: 'root' })
export class CustomerReturnReceiptsApiService {
  private readonly base = '/api/v1/customer-return-receipts';
  constructor(private readonly http: HttpClient) {}
  list(search?: string) {
    return this.http.get<ReturnReceiptSummary[]>(this.base, { params: search ? { search } : {} });
  }
  get(id: number) {
    return this.http.get<ReturnReceiptDetails>(`${this.base}/${id}`);
  }
  createForm(customerReturnRequestId: number) {
    return this.http.get<ReturnReceiptCreatePage>(`${this.base}/create-form`, {
      params: { customerReturnRequestId },
    });
  }
  create(input: CreateReturnReceipt) {
    return this.http.post<{ id: number }>(this.base, input);
  }
  command(
    id: number,
    action: 'inspect' | 'post' | 'cancel',
    rowVersion: string,
    reason: string | null = null,
  ) {
    return this.http.post<void>(`${this.base}/${id}/${action}`, { id, rowVersion, reason });
  }
}
