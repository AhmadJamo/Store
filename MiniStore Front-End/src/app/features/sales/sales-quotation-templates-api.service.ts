import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CreateTemplate,
  TemplateDetails,
  TemplatePage,
  TemplateSummary,
} from './sales-quotation-templates.models';
@Injectable({ providedIn: 'root' })
export class SalesQuotationTemplatesApiService {
  private readonly base = '/api/v1/sales-quotation-templates';
  constructor(private readonly http: HttpClient) {}
  list(status?: number, search?: string) {
    const params: any = {};
    if (status) params.status = status;
    if (search) params.search = search;
    return this.http.get<TemplateSummary[]>(this.base, { params });
  }
  get(id: number) {
    return this.http.get<TemplateDetails>(`${this.base}/${id}`);
  }
  createForm(sourceTemplateId?: number) {
    return this.http.get<TemplatePage>(`${this.base}/create-form`, {
      params: sourceTemplateId ? { sourceTemplateId } : {},
    });
  }
  create(input: CreateTemplate) {
    return this.http.post<{ id: number }>(this.base, input);
  }
  command(id: number, action: 'activate' | 'archive' | 'default', rowVersion: string) {
    return this.http.post<void>(`${this.base}/${id}/${action}`, { id, rowVersion });
  }
}
