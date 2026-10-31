import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AuditEventsApiService } from './audit-events-api.service';

describe('AuditEventsApiService', () => {
  it('uses the tenant audit endpoints and preserves approved export evidence', () => {
    TestBed.configureTestingModule({
      providers: [AuditEventsApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const api = TestBed.inject(AuditEventsApiService);
    const http = TestBed.inject(HttpTestingController);
    const query = { page: 2, pageSize: 50, eventType: 'Inventory.Posted' };

    api.getPage(query).subscribe();
    const page = http.expectOne(
      request => request.url === '/api/v1/audit-events' && request.params.get('page') === '2',
    );
    expect(page.request.method).toBe('GET');
    expect(page.request.params.get('eventType')).toBe('Inventory.Posted');

    api.export(query, 'ComplianceReview').subscribe();
    const exported = http.expectOne('/api/v1/audit-events/export');
    expect(exported.request.method).toBe('POST');
    expect(exported.request.body).toEqual({ query, purpose: 'ComplianceReview' });
    expect(exported.request.responseType).toBe('blob');
    http.verify();
  });
});
