import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PurchaseReplenishmentApiService } from './purchase-replenishment-api.service';
describe('PurchaseReplenishmentApiService', () => {
  it('uses read, settings and run endpoints', () => {
    TestBed.configureTestingModule({
      providers: [PurchaseReplenishmentApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const a = TestBed.inject(PurchaseReplenishmentApiService),
      h = TestBed.inject(HttpTestingController);
    a.get().subscribe();
    expect(h.expectOne('/api/v1/purchase-replenishment').request.method).toBe('GET');
    a.save({}).subscribe();
    expect(h.expectOne('/api/v1/purchase-replenishment/settings').request.method).toBe('POST');
    a.run().subscribe();
    expect(h.expectOne('/api/v1/purchase-replenishment/run').request.method).toBe('POST');
    h.verify();
  });
});
