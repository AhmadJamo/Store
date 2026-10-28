import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplenishmentPage } from './purchase-replenishment.models';
@Injectable({ providedIn: 'root' })
export class PurchaseReplenishmentApiService {
  private readonly base = '/api/v1/purchase-replenishment';
  constructor(private readonly http: HttpClient) {}
  get() {
    return this.http.get<ReplenishmentPage>(this.base);
  }
  save(input: unknown) {
    return this.http.post(`${this.base}/settings`, input);
  }
  run() {
    return this.http.post<{ id: number }>(`${this.base}/run`, {});
  }
}
