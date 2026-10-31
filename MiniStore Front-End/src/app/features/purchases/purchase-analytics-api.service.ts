import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class PurchaseAnalyticsApiService {
  constructor(private readonly http: HttpClient) {}
  get(q: Record<string, unknown>) {
    let p = new HttpParams();
    for (const [k, v] of Object.entries(q))
      if (v !== null && v !== undefined && v !== '') p = p.set(k, String(v));
    return this.http.get<any>('/api/v1/purchase-analytics', { params: p });
  }
}
