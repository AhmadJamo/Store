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
});
