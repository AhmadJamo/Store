import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PurchaseAccountingReconciliationApiService } from './purchase-accounting-reconciliation-api.service';

describe('PurchaseAccountingReconciliationApiService', () => {
  it('sends the as-of date and exception filter to the read endpoint', () => {
    TestBed.configureTestingModule({
      providers: [
        PurchaseAccountingReconciliationApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });
    const api = TestBed.inject(PurchaseAccountingReconciliationApiService);
    const http = TestBed.inject(HttpTestingController);
    api.get('2026-10-28', true).subscribe();
    const request = http.expectOne((value) =>
      value.url.endsWith('/api/v1/purchase-accounting-reconciliation'),
    );
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('AsOfDate')).toBe('2026-10-28');
    expect(request.request.params.get('ExceptionsOnly')).toBe('true');
    http.verify();
  });
});
