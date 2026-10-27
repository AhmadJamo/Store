import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StockTransactionPage, StockTransactionQuery } from './stock-transaction.models';

@Injectable({ providedIn: 'root' })
export class StockTransactionApiService {
  private readonly baseUrl = '/api/v1/stock-transactions';

  constructor(private readonly http: HttpClient) {}

  getPage(query: StockTransactionQuery) {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    }
    return this.http.get<StockTransactionPage>(this.baseUrl, { params });
  }
}
