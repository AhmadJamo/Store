import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PurchaseAnalyticsApiService } from './purchase-analytics-api.service';
describe('PurchaseAnalyticsApiService', () => {
  it('sends analytics filters to the read endpoint', () => {
    TestBed.configureTestingModule({
      providers: [PurchaseAnalyticsApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const a = TestBed.inject(PurchaseAnalyticsApiService),
      h = TestBed.inject(HttpTestingController);
    a.get({ From: '2026-01-01', GroupBy: 2 }).subscribe();
    const r = h.expectOne((x) => x.url === '/api/v1/purchase-analytics');
    expect(r.request.method).toBe('GET');
    expect(r.request.params.get('GroupBy')).toBe('2');
    h.verify();
  });
});
