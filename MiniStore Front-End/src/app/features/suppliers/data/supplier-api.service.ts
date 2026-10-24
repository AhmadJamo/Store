import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SupplierCommand, SupplierDetails, SupplierFormPage, SupplierListPage, SupplierListQuery } from './supplier.models';

@Injectable({ providedIn: 'root' })
export class SupplierApiService {
  constructor(private readonly http: HttpClient) {}
  getPage(query: SupplierListQuery) {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) if (value !== undefined && value !== '') params = params.set(key, String(value));
    return this.http.get<SupplierListPage>('/api/v1/suppliers', { params });
  }
  get(id: number) { return this.http.get<SupplierDetails>(`/api/v1/suppliers/${id}`); }
  getForm(id?: number) { return this.http.get<SupplierFormPage>(id ? `/api/v1/suppliers/${id}/form` : '/api/v1/suppliers/form'); }
  create(form: SupplierCommand) { return this.http.post<SupplierDetails>('/api/v1/suppliers', form); }
  update(id: number, form: SupplierCommand) { return this.http.put<SupplierDetails>(`/api/v1/suppliers/${id}`, form); }
}
