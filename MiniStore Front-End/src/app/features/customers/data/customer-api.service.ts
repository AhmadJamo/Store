import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CustomerDetails, CustomerForm, CustomerFormPage, CustomerListPage, CustomerListQuery } from './customer.models';

@Injectable({ providedIn: 'root' })
export class CustomerApiService {
  private readonly baseUrl = '/api/v1/customers';
  constructor(private readonly http: HttpClient) {}
  getPage(query: CustomerListQuery) {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) if (value !== undefined && value !== '') params = params.set(key, String(value));
    return this.http.get<CustomerListPage>(this.baseUrl, { params });
  }
  get(id: number) { return this.http.get<CustomerDetails>(`${this.baseUrl}/${id}`); }
  getForm(id?: number) { return this.http.get<CustomerFormPage>(id ? `${this.baseUrl}/${id}/form` : `${this.baseUrl}/form`); }
  create(form: CustomerForm) { return this.http.post<CustomerDetails>(this.baseUrl, form); }
  update(form: CustomerForm) { return this.http.put<CustomerDetails>(`${this.baseUrl}/${form.id}`, form); }
}
