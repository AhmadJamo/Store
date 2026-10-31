import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CreateReturnRequest,
  ReturnRequestCreatePage,
  ReturnRequestDetails,
  ReturnRequestSummary,
} from './customer-return-requests.models';
@Injectable({ providedIn: 'root' })
export class CustomerReturnRequestsApiService {
  private readonly base = '/api/v1/customer-return-requests';
  constructor(private readonly http: HttpClient) {}
  list(search?: string) {
    return this.http.get<ReturnRequestSummary[]>(this.base, { params: search ? { search } : {} });
  }
  get(id: number) {
    return this.http.get<ReturnRequestDetails>(`${this.base}/${id}`);
  }
  createForm(salesDeliveryId: number) {
    return this.http.get<ReturnRequestCreatePage>(`${this.base}/create-form`, {
      params: { salesDeliveryId },
    });
  }
  create(input: CreateReturnRequest) {
    return this.http.post<{ id: number }>(this.base, input);
  }
  command(
    id: number,
    action: 'submit' | 'approve' | 'reject' | 'cancel',
    rowVersion: string,
    reason: string | null = null,
  ) {
    return this.http.post<void>(`${this.base}/${id}/${action}`, { id, rowVersion, reason });
  }
}
