import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { DocumentDesignDetails, DocumentDesignFilters, DocumentDesignForm, DocumentDesignSummary } from './document-design.models';

@Injectable({ providedIn: 'root' })
export class DocumentDesignsApiService {
  private readonly http = inject(HttpClient);
  getAll(filters: DocumentDesignFilters = {}) { let params = new HttpParams(); if (filters.status) params = params.set('status', filters.status); if (filters.documentType) params = params.set('documentType', filters.documentType); if (filters.search?.trim()) params = params.set('search', filters.search.trim()); return this.http.get<DocumentDesignSummary[]>('/api/v1/document-designs', { params }); }
  getForm(sourceDesignId?: number, documentType?: number) { let params = new HttpParams(); if (sourceDesignId) params = params.set('sourceDesignId', sourceDesignId); if (documentType) params = params.set('documentType', documentType); return this.http.get<DocumentDesignForm>('/api/v1/document-designs/form', { params }); }
  get(id: number) { return this.http.get<DocumentDesignDetails>(`/api/v1/document-designs/${id}`); }
  create(value: DocumentDesignForm) { return this.http.post<DocumentDesignDetails>('/api/v1/document-designs', value); }
  activate(id: number, rowVersion: string) { return this.command(id, 'activate', rowVersion); }
  archive(id: number, rowVersion: string) { return this.command(id, 'archive', rowVersion); }
  setDefault(id: number, rowVersion: string) { return this.command(id, 'set-default', rowVersion); }
  private command(id: number, action: string, rowVersion: string) { return this.http.post<DocumentDesignDetails>(`/api/v1/document-designs/${id}/${action}`, { id, rowVersion }); }
}
