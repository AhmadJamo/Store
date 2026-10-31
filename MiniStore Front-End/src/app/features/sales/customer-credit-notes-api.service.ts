import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CreateCreditNote,
  CreditNoteCreatePage,
  CreditNoteDetails,
  CreditNoteSummary,
} from './customer-credit-notes.models';
@Injectable({ providedIn: 'root' })
export class CustomerCreditNotesApiService {
  private readonly base = '/api/v1/customer-credit-notes';
  constructor(private readonly http: HttpClient) {}
  list() {
    return this.http.get<CreditNoteSummary[]>(this.base);
  }
  get(id: number) {
    return this.http.get<CreditNoteDetails>(`${this.base}/${id}`);
  }
  createForm(customerInvoiceId: number) {
    return this.http.get<CreditNoteCreatePage>(`${this.base}/create-form`, {
      params: { customerInvoiceId },
    });
  }
  create(input: CreateCreditNote) {
    return this.http.post<{ id: number }>(this.base, input);
  }
}
