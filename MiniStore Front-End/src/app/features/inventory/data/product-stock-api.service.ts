import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ManualStockAdjustmentCommand, ManualStockAdjustmentFormPage, ProductStock, ProductStockCommand, ProductStockFormPage } from './product-stock.models';

@Injectable({ providedIn: 'root' })
export class ProductStockApiService {
  private readonly baseUrl = '/api/v1/product-stocks';
  constructor(private readonly http: HttpClient) {}
  getAll() { return this.http.get<readonly ProductStock[]>(this.baseUrl); }
  getForm(id?: number) { return this.http.get<ProductStockFormPage>(id ? `${this.baseUrl}/${id}/form` : `${this.baseUrl}/form`); }
  create(command: ProductStockCommand) { return this.http.post<ProductStock>(this.baseUrl, command); }
  update(id: number, quantity: number) { return this.http.put<ProductStock>(`${this.baseUrl}/${id}`, { quantity }); }
  delete(id: number) { return this.http.delete<void>(`${this.baseUrl}/${id}`); }
  getAdjustmentForm() { return this.http.get<ManualStockAdjustmentFormPage>('/api/v1/stock-transactions/manual/form'); }
  createAdjustment(command: ManualStockAdjustmentCommand) { return this.http.post<void>('/api/v1/stock-transactions/manual', command); }
}
