import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AnalyticsPage, AnalyticsWorkspaceBuilder, AnalyticsWorkspaceCommand } from './analytics.models';
@Injectable({ providedIn: 'root' })
export class AnalyticsApiService {
  constructor(private readonly http: HttpClient) {}
  get(query: { workspaceId?: number; moduleKey?: string; from?: string; to?: string; dimension?: string; comparisonMode?: number }) { let params = new HttpParams(); for (const [key, value] of Object.entries(query)) if (value !== undefined && value !== '') params = params.set(key, String(value)); return this.http.get<AnalyticsPage>('/api/v1/analytics-center', { params }); }
  getBuilder() { return this.http.get<AnalyticsWorkspaceBuilder>('/api/v1/analytics-center/builder'); }
  createWorkspace(command: AnalyticsWorkspaceCommand) { return this.http.post<{ id: number }>('/api/v1/analytics-center/workspaces', command); }
}
