import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DocumentEvidence, DocumentEvidenceListItem, DocumentKind } from './document-evidence.models';

@Injectable({ providedIn: 'root' })
export class DocumentEvidenceApiService {
  private readonly baseUrl = '/api/v1/document-evidence';
  constructor(private readonly http: HttpClient) {}
  getList(kind: DocumentKind) { const segment = kind === 'sales-order' ? 'sales-orders' : 'customer-invoices'; return this.http.get<readonly DocumentEvidenceListItem[]>(`${this.baseUrl}/${segment}`); }
  get(kind: DocumentKind, id: number) { const segment = kind === 'sales-order' ? 'sales-orders' : 'customer-invoices'; return this.http.get<DocumentEvidence>(`${this.baseUrl}/${segment}/${id}`); }
}
