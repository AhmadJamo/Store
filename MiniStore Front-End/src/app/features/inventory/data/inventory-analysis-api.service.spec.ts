import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { InventoryAnalysisApiService } from './inventory-analysis-api.service';

describe('InventoryAnalysisApiService', () => {
  let api: InventoryAnalysisApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [InventoryAnalysisApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(InventoryAnalysisApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('keeps insight thresholds and reconciliation paging server-side', () => {
    api.getInsights({ warehouseId: 2, search: 'milk', state: 1, attentionOnly: true, slowDays: 20, deadDays: 80 }).subscribe();
    const insights = http.expectOne(request => request.url === '/api/v1/inventory-insights');
    expect(insights.request.method).toBe('GET');
    expect(insights.request.params.get('slowDays')).toBe('20');
    expect(insights.request.params.get('deadDays')).toBe('80');
    insights.flush({});

    api.getReconciliation({ warehouseId: 2, search: 'milk', exceptionsOnly: true, page: 3, pageSize: 100 }).subscribe();
    const reconciliation = http.expectOne(request => request.url === '/api/v1/inventory-reconciliation');
    expect(reconciliation.request.method).toBe('GET');
    expect(reconciliation.request.params.get('page')).toBe('3');
    expect(reconciliation.request.params.get('pageSize')).toBe('100');
  });
});
