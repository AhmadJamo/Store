import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SupplierIntegrationsApiService } from './supplier-integrations-api.service';
describe('SupplierIntegrationsApiService', () => {
  it('covers dashboard and protected mutations', () => {
    TestBed.configureTestingModule({
      providers: [SupplierIntegrationsApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const a = TestBed.inject(SupplierIntegrationsApiService),
      h = TestBed.inject(HttpTestingController);
    a.get().subscribe();
    expect(h.expectOne('/api/v1/supplier-integrations').request.method).toBe('GET');
    a.create({ supplierId: 1 }).subscribe();
    expect(h.expectOne('/api/v1/supplier-integrations/credentials').request.method).toBe('POST');
    a.retry(2).subscribe();
    expect(h.expectOne('/api/v1/supplier-integrations/outbox/2/retry').request.method).toBe('POST');
    h.verify();
  });
});
