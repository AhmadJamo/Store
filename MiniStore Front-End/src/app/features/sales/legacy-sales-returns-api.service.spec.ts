import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { LegacySalesReturnsApiService } from './legacy-sales-returns-api.service';
describe('LegacySalesReturnsApiService', () => {
  let api: LegacySalesReturnsApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(LegacySalesReturnsApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('creates a posted immediate-sale return', () => {
    const input = {
      saleId: 4,
      date: '2026-10-29',
      reason: 'Damaged',
      items: [{ saleItemId: 8, quantity: 1, trackingAllocations: null }],
    };
    api.create(input).subscribe();
    const request = http.expectOne('/api/v1/legacy-sales-returns');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(input);
    request.flush({ id: 5 });
  });
});
