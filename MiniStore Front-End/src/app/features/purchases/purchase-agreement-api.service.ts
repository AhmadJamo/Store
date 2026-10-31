import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  AgreementCreatePage,
  PurchaseAgreement,
  PurchaseTemplate,
} from './purchase-agreement.models';
@Injectable({ providedIn: 'root' })
export class PurchaseAgreementApiService {
  private readonly agreements = '/api/v1/purchase-agreements';
  private readonly templates = '/api/v1/purchase-templates';
  constructor(private readonly http: HttpClient) {}
  getAll() {
    return this.http.get<PurchaseAgreement[]>(this.agreements);
  }
  get(id: number) {
    return this.http.get<PurchaseAgreement>(`${this.agreements}/${id}`);
  }
  getCreateForm(sourcingEventId: number) {
    return this.http.get<AgreementCreatePage>(`${this.agreements}/create-form`, {
      params: { sourcingEventId },
    });
  }
  create(input: unknown) {
    return this.http.post<{ id: number }>(this.agreements, input);
  }
  activate(id: number, rowVersion: string) {
    return this.http.post(`${this.agreements}/${id}/activate`, { rowVersion });
  }
  cancel(id: number, rowVersion: string, reason: string) {
    return this.http.post(`${this.agreements}/${id}/cancel`, { rowVersion, reason });
  }
  release(id: number, input: unknown) {
    return this.http.post<{ id: number }>(`${this.agreements}/${id}/release`, input);
  }
  getTemplates() {
    return this.http.get<PurchaseTemplate[]>(this.templates);
  }
  createTemplate(input: unknown) {
    return this.http.post<{ id: number }>(`${this.templates}/from-order`, input);
  }
  generate(id: number) {
    return this.http.post<{ id: number }>(`${this.templates}/${id}/generate`, {});
  }
  setActive(id: number, active: boolean) {
    return this.http.post(`${this.templates}/${id}/active`, { active });
  }
}
