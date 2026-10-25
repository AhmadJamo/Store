import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AnalyticsApiService } from './analytics-api.service';
import { AnalyticsWorkspaceCommand } from './analytics.models';

describe('AnalyticsApiService', () => {
  let service: AnalyticsApiService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [AnalyticsApiService, provideHttpClient(), provideHttpClientTesting()] }); service = TestBed.inject(AnalyticsApiService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());

  it('loads the governed builder catalog', () => { service.getBuilder().subscribe(); const request = http.expectOne('/api/v1/analytics-center/builder'); expect(request.request.method).toBe('GET'); request.flush({ form: {}, modules: [] }); });
  it('posts only the typed workspace command', () => { const command = { name: 'Ops', moduleKey: 'sales', visibility: 1, isDefault: false, layoutColumns: 2, defaultPeriodDays: 30, comparisonMode: 1, refreshIntervalMinutes: 0, showDefinitions: true, widgets: [] } satisfies AnalyticsWorkspaceCommand; service.createWorkspace(command).subscribe(); const request = http.expectOne('/api/v1/analytics-center/workspaces'); expect(request.request.method).toBe('POST'); expect(request.request.body).toEqual(command); request.flush({ id: 7 }); });
  it('loads workspace access from the governed endpoint', () => { service.getAccess(7).subscribe(); const request = http.expectOne('/api/v1/analytics-center/workspaces/7/access'); expect(request.request.method).toBe('GET'); request.flush({ workspaceId: 7, delegations: [], users: [], roles: [] }); });
  it('creates and revokes typed delegations', () => { const command = { targetType: 2, targetRoleId: 4, rowVersion: 'AQ==' }; service.createDelegation(7, command).subscribe(); const create = http.expectOne('/api/v1/analytics-center/workspaces/7/delegations'); expect(create.request.body).toEqual(command); create.flush(null); service.revokeDelegation(7, 12, 'Ag==').subscribe(); const revoke = http.expectOne('/api/v1/analytics-center/workspaces/7/delegations/revoke'); expect(revoke.request.body).toEqual({ delegationId: 12, rowVersion: 'Ag==' }); revoke.flush(null); });
});
