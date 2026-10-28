import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PurchaseAccountingReconciliationApiService {
  constructor(private readonly http: HttpClient) {}

  get(asOfDate: string, exceptionsOnly: boolean) {
    const params = new HttpParams()
      .set('AsOfDate', asOfDate)
      .set('ExceptionsOnly', String(exceptionsOnly));
    return this.http.get<any>('/api/v1/purchase-accounting-reconciliation', { params });
  }
}
