import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  InventoryInsightsPage,
  InventoryInsightsQuery,
  InventoryReconciliationPage,
} from './inventory-analysis.models';

@Injectable({ providedIn: 'root' })
export class InventoryAnalysisApiService {
  constructor(private readonly http: HttpClient) {}

  getInsights(query: InventoryInsightsQuery) {
    let params = new HttpParams()
      .set('attentionOnly', query.attentionOnly)
      .set('slowDays', query.slowDays)
      .set('deadDays', query.deadDays);
    if (query.warehouseId) params = params.set('warehouseId', query.warehouseId);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.state !== null && query.state !== undefined) params = params.set('state', query.state);
    return this.http.get<InventoryInsightsPage>('/api/v1/inventory-insights', { params });
  }

  getReconciliation(query: {
    warehouseId?: number | null;
    search: string;
    exceptionsOnly: boolean;
    page: number;
    pageSize: number;
  }) {
    let params = new HttpParams()
      .set('exceptionsOnly', query.exceptionsOnly)
      .set('page', query.page)
      .set('pageSize', query.pageSize);
    if (query.warehouseId) params = params.set('warehouseId', query.warehouseId);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    return this.http.get<InventoryReconciliationPage>('/api/v1/inventory-reconciliation', { params });
  }
}
