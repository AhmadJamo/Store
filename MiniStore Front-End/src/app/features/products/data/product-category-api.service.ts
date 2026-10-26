import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateProductCategoryCommand, ProductCategory } from './product-category.models';

@Injectable({ providedIn: 'root' })
export class ProductCategoryApiService {
  constructor(private readonly http: HttpClient) {}
  getAll() { return this.http.get<readonly ProductCategory[]>('/api/v1/product-categories'); }
  create(command: CreateProductCategoryCommand) { return this.http.post<ProductCategory>('/api/v1/product-categories', command); }
  setStatus(id: number, isActive: boolean) { return this.http.put<ProductCategory>(`/api/v1/product-categories/${id}/status`, { isActive }); }
}
