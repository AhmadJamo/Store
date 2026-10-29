import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreatePlatformAuditHold, CreatePlatformPlan, CreatePlatformPromotion, PlatformAuditPage, PlatformAuditQuery, PlatformAuditRetentionStatus, PlatformManagementPage, PlatformOverview, PlatformSession, UpdatePlatformCompany } from './platform.models';

@Injectable({ providedIn: 'root' })
export class PlatformApiService {
  private readonly base = '/api/v1/platform';
  constructor(private readonly http: HttpClient) {}
  login(input: { username: string; password: string }) {
    return this.http.post<PlatformSession>(`${this.base}/login`, input);
  }
  session() {
    return this.http.get<PlatformSession>(`${this.base}/session`);
  }
  overview() {
    return this.http.get<PlatformOverview>(`${this.base}/overview`);
  }
  logout() {
    return this.http.post<void>(`${this.base}/logout`, {});
  }
  management() { return this.http.get<PlatformManagementPage>(`${this.base}/management`); }
  updateCompany(input: UpdatePlatformCompany) { return this.http.put<void>(`${this.base}/management/companies/${input.tenantId}`, input); }
  createPlan(input: CreatePlatformPlan) { return this.http.post<void>(`${this.base}/management/plans`, input); }
  createPromotion(input: CreatePlatformPromotion) { return this.http.post<void>(`${this.base}/management/promotions`, input); }
  confirmPayment(sessionId: string, providerReference: string) { return this.http.post<void>(`${this.base}/management/payments/${sessionId}/confirm`, { providerReference }); }
  auditEvents(query: PlatformAuditQuery) { return this.http.get<PlatformAuditPage>(`${this.base}/audit-events`, { params: this.auditParams(query) }); }
  exportAudit(query: PlatformAuditQuery, purpose: string) { return this.http.post(`${this.base}/audit-events/export`, { query, purpose }, { responseType: 'blob' }); }
  auditRetention() { return this.http.get<PlatformAuditRetentionStatus>(`${this.base}/audit-retention`); }
  createAuditHold(input: CreatePlatformAuditHold) { return this.http.post<void>(`${this.base}/audit-retention/holds`, input); }
  releaseAuditHold(id: number, reason: string) { return this.http.post<void>(`${this.base}/audit-retention/holds/${id}/release`, { reason }); }
  createAuditCheckpoint() { return this.http.post<void>(`${this.base}/audit-retention/checkpoint`, {}); }
  runAuditRetention() { return this.http.post<void>(`${this.base}/audit-retention/run`, {}); }
  private auditParams(query: PlatformAuditQuery) {
    const params: Record<string,string|number> = { page: query.page, pageSize: query.pageSize };
    for (const [key,value] of Object.entries(query)) if (value !== undefined && value !== null && value !== '' && key !== 'page' && key !== 'pageSize') params[key] = value;
    return params;
  }
}
