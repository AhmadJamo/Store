import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SupplierPurchasingApiService } from './supplier-purchasing-api.service';
describe('SupplierPurchasingApiService', () => {
  let a: SupplierPurchasingApiService, h: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SupplierPurchasingApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    a = TestBed.inject(SupplierPurchasingApiService);
    h = TestBed.inject(HttpTestingController);
  });
  afterEach(() => h.verify());
  it('uses all supplier purchasing endpoints', () => {
    a.page(2).subscribe();
    expect(h.expectOne((x) => x.url === '/api/v1/supplier-purchasing').request.method).toBe('GET');
    a.exceptions(1, 2).subscribe();
    expect(h.expectOne((x) => x.url.endsWith('/exceptions')).request.method).toBe('GET');
    a.breaks(3).subscribe();
    expect(h.expectOne('/api/v1/supplier-purchasing/3/price-breaks').request.method).toBe('GET');
    a.history(3).subscribe();
    expect(h.expectOne('/api/v1/supplier-purchasing/3/history').request.method).toBe('GET');
    a.saveBulk({}).subscribe();
    expect(h.expectOne('/api/v1/supplier-purchasing/bulk').request.method).toBe('POST');
    a.saveBreaks(3, {}).subscribe();
    expect(h.expectOne('/api/v1/supplier-purchasing/3/price-breaks').request.method).toBe('POST');
    a.setActive(3, false, 'AQID').subscribe();
    expect(h.expectOne('/api/v1/supplier-purchasing/3/active').request.method).toBe('POST');
    a.download(2, 'csv').subscribe();
    expect(h.expectOne((x) => x.url.endsWith('/export/csv')).request.method).toBe('GET');
  });
});
