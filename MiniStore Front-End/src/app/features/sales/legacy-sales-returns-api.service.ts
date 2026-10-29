import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  SalesReturn,
  SalesReturnCreateInput,
  SalesReturnCreatePage,
} from './legacy-sales-returns.models';
@Injectable({ providedIn: 'root' })
export class LegacySalesReturnsApiService {
  private readonly base = '/api/v1/legacy-sales-returns';
  constructor(private readonly http: HttpClient) {}
  list() {
    return this.http.get<SalesReturn[]>(this.base);
  }
  get(id: number) {
    return this.http.get<SalesReturn>(`${this.base}/${id}`);
  }
  createForm(saleId: number) {
    return this.http.get<SalesReturnCreatePage>(`${this.base}/create-form/${saleId}`);
  }
  create(input: SalesReturnCreateInput) {
    return this.http.post<{ id: number }>(this.base, input);
  }
}
