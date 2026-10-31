import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product, ProductCommand, ProductFormPage, ProductPage } from './product.models';

@Injectable({ providedIn: 'root' })
export class ProductApiService {
  constructor(private readonly http: HttpClient) {}

  getPage(query: { page: number; search: string; isActive?: boolean }) {
    let params = new HttpParams().set('page', query.page).set('pageSize', 25);
    if (query.search) params = params.set('search', query.search);
    if (query.isActive !== undefined) params = params.set('isActive', query.isActive);
    return this.http.get<ProductPage>('/api/v1/products', { params });
  }

  get(id: number) { return this.http.get<Product>(`/api/v1/products/${id}`); }
  getForm(id?: number) { return this.http.get<ProductFormPage>(id ? `/api/v1/products/${id}/form` : '/api/v1/products/form'); }
  create(command: ProductCommand) { return this.http.post<Product>('/api/v1/products', command); }
  update(id: number, command: ProductCommand) { return this.http.put<Product>(`/api/v1/products/${id}`, command); }
  delete(id: number) { return this.http.delete<void>(`/api/v1/products/${id}`); }
}
