import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ApprovalRulesPage,
  CreateRequest,
  CreateRequestPage,
  PurchaseRequest,
} from './purchase-request.models';
@Injectable({ providedIn: 'root' })
export class PurchaseRequestApiService {
  private readonly base = '/api/v1/purchase-requests';
  constructor(private readonly http: HttpClient) {}
  getAll(status?: number, search?: string) {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    if (search?.trim()) params = params.set('search', search.trim());
    return this.http.get<PurchaseRequest[]>(this.base, { params });
  }
  get(id: number) {
    return this.http.get<PurchaseRequest>(`${this.base}/${id}`);
  }
  getCreateForm() {
    return this.http.get<CreateRequestPage>(`${this.base}/create-form`);
  }
  create(input: CreateRequest) {
    return this.http.post<{ id: number }>(this.base, input);
  }
  submit(id: number, rowVersion: string) {
    return this.http.post<void>(`${this.base}/${id}/submit`, { id, rowVersion });
  }
  cancel(id: number, rowVersion: string, reason: string) {
    return this.http.post<void>(`${this.base}/${id}/cancel`, { id, rowVersion, reason });
  }
  approve(id: number, note?: string) {
    return this.http.post<void>(`${this.base}/${id}/approve`, { id, note });
  }
  reject(id: number, note: string) {
    return this.http.post<void>(`${this.base}/${id}/reject`, { id, note });
  }
  getApprovalRules() {
    return this.http.get<ApprovalRulesPage>('/api/v1/purchase-approval-rules');
  }
  createApprovalRule(input: {
    name: string;
    warehouseId?: number;
    minimumPriority: number;
    maximumPriority: number;
    approverRoles: string[];
  }) {
    return this.http.post<void>('/api/v1/purchase-approval-rules', input);
  }
  setApprovalRuleActive(id: number, active: boolean, rowVersion: string) {
    return this.http.put<void>(`/api/v1/purchase-approval-rules/${id}/active`, {
      id,
      active,
      rowVersion,
    });
  }
}
