import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AssignProductTemplateCommand, CreateProductTemplateCommand, ProductTemplatePage, VariantGenerationCommand, VariantGenerationResult } from './product-template.models';

@Injectable({ providedIn: 'root' })
export class ProductTemplateApiService {
  constructor(private readonly http: HttpClient) {}
  getPage() { return this.http.get<ProductTemplatePage>('/api/v1/product-templates'); }
  create(command: CreateProductTemplateCommand) { return this.http.post<ProductTemplatePage>('/api/v1/product-templates', command); }
  assign(command: AssignProductTemplateCommand) { return this.http.put<ProductTemplatePage>('/api/v1/product-templates/assignment', command); }
  preview(command: VariantGenerationCommand) { return this.http.post<ProductTemplatePage>('/api/v1/product-templates/preview', command); }
  generate(command: VariantGenerationCommand) { return this.http.post<VariantGenerationResult>('/api/v1/product-templates/generate', command); }
}
