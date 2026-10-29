import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreatePlatformPlan, CreatePlatformPromotion, PlatformManagementPage, PlatformOverview, PlatformSession, UpdatePlatformCompany } from './platform.models';

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
}
