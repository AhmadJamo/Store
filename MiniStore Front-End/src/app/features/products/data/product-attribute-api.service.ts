import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateProductAttributeCommand, ProductAttributeDefinition, ProductAttributePage, ProductAttributeValueInput, ProductAttributeValuesPage } from './product-attribute.models';

@Injectable({ providedIn: 'root' })
export class ProductAttributeApiService {
  constructor(private readonly http: HttpClient) {}
  getPage() { return this.http.get<ProductAttributePage>('/api/v1/product-attributes'); }
  create(command: CreateProductAttributeCommand) { return this.http.post<ProductAttributeDefinition>('/api/v1/product-attributes', command); }
  setStatus(id: number, isActive: boolean) { return this.http.put<ProductAttributeDefinition>(`/api/v1/product-attributes/${id}/status`, { isActive }); }
  getValues(productId: number) { return this.http.get<ProductAttributeValuesPage>(`/api/v1/product-attributes/products/${productId}/values`); }
  saveValues(productId: number, values: readonly ProductAttributeValueInput[]) { return this.http.put<ProductAttributeValuesPage>(`/api/v1/product-attributes/products/${productId}/values`, { values }); }
}
