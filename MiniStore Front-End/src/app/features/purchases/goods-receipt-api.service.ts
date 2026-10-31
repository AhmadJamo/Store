import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GoodsReceipt, GoodsReceiptCreatePage, GoodsReceiptScanPage } from './goods-receipt.models';

@Injectable({ providedIn: 'root' })
export class GoodsReceiptApiService {
  private readonly base = '/api/v1/goods-receipts';
  constructor(private readonly http: HttpClient) {}
  getAll(search?: string) {
    let params = new HttpParams();
    if (search?.trim()) params = params.set('search', search.trim());
    return this.http.get<GoodsReceipt[]>(this.base, { params });
  }
  get(id: number) { return this.http.get<GoodsReceipt>(`${this.base}/${id}`); }
  getCreateForm(purchaseOrderId: number) {
    return this.http.get<GoodsReceiptCreatePage>(`${this.base}/create-form`, { params: { purchaseOrderId } });
  }
  create(input: unknown) { return this.http.post<{ id: number }>(this.base, input); }
  createDraft(input: unknown) { return this.http.post<{ id: number }>(`${this.base}/draft`, input); }
  getScan(id: number) { return this.http.get<GoodsReceiptScanPage>(`${this.base}/${id}/scan`); }
  scan(id: number, input: unknown) { return this.http.post<void>(`${this.base}/${id}/scan`, input); }
  post(id: number) { return this.http.post<void>(`${this.base}/${id}/post`, {}); }
  inspect(id: number, input: unknown) { return this.http.post<void>(`${this.base}/${id}/inspect`, input); }
  putAway(id: number) { return this.http.post<void>(`${this.base}/${id}/put-away`, {}); }
}
