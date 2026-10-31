import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateInventoryAdjustmentCommand, InventoryAdjustmentCreatePage, InventoryAdjustmentDetails, InventoryAdjustmentRow, RecordInventoryCountsCommand } from './inventory-adjustment.models';

@Injectable({ providedIn: 'root' })
export class InventoryAdjustmentApiService {
  private readonly baseUrl = '/api/v1/inventory-adjustments';
  constructor(private readonly http: HttpClient) {}
  getAll() { return this.http.get<InventoryAdjustmentRow[]>(this.baseUrl); }
  getCreateForm() { return this.http.get<InventoryAdjustmentCreatePage>(`${this.baseUrl}/create-form`); }
  create(command: CreateInventoryAdjustmentCommand) { return this.http.post<{ id: number }>(this.baseUrl, command); }
  getDetails(id: number) { return this.http.get<InventoryAdjustmentDetails>(`${this.baseUrl}/${id}`); }
  count(id: number, command: RecordInventoryCountsCommand) { return this.http.post<void>(`${this.baseUrl}/${id}/count`, command); }
  approve(id: number) { return this.http.post<void>(`${this.baseUrl}/${id}/approve`, {}); }
  post(id: number) { return this.http.post<void>(`${this.baseUrl}/${id}/post`, {}); }
  cancel(id: number) { return this.http.post<void>(`${this.baseUrl}/${id}/cancel`, {}); }
}
