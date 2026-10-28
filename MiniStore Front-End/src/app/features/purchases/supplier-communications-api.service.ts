import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SupplierCommunicationsApiService {
  constructor(private readonly http: HttpClient) {}
  get(id: number) {
    return this.http.get<any>(`/api/v1/supplier-communications/${id}`);
  }
  byDocument(type: number, id: number) {
    return this.http.get<any[]>(`/api/v1/supplier-communications/document/${type}/${id}`);
  }
  prepare(value: {
    documentType: number;
    documentId: number;
    supplierId: number;
    preferredLanguage: string;
  }) {
    return this.http.post<{ id: number }>('/api/v1/supplier-communications', value);
  }
  record(id: number, value: any) {
    return this.http.post<any>(`/api/v1/supplier-communications/${id}/activities`, value);
  }
  attachmentUrl(id: number, attachmentId: number) {
    return `/api/v1/supplier-communications/${id}/attachments/${attachmentId}`;
  }
}
