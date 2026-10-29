import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CreateCustomerPayment,
  CustomerPayment,
  CustomerPaymentCreatePage,
} from './customer-payments.models';

@Injectable({ providedIn: 'root' })
export class CustomerPaymentsApiService {
  private readonly base = '/api/v1/customer-payments';
  constructor(private readonly http: HttpClient) {}
  list(search?: string) {
    return this.http.get<CustomerPayment[]>(this.base, { params: search ? { search } : {} });
  }
  get(id: number) {
    return this.http.get<CustomerPayment>(`${this.base}/${id}`);
  }
  createForm(customerInvoiceId: number) {
    return this.http.get<CustomerPaymentCreatePage>(`${this.base}/create-form`, {
      params: { customerInvoiceId },
    });
  }
  create(input: CreateCustomerPayment) {
    return this.http.post<{ id: number }>(this.base, input);
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
