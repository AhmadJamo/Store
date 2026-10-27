import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PurchaseRequestApiService } from './purchase-request-api.service';
describe('PurchaseRequestApiService', () => {
  let api: PurchaseRequestApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PurchaseRequestApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(PurchaseRequestApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('uses every request endpoint and verb', () => {
    api.getAll(2, 'PR-').subscribe();
    let request = http.expectOne((r) => r.url === '/api/v1/purchase-requests');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('status')).toBe('2');
    api.get(4).subscribe();
    expect(http.expectOne('/api/v1/purchase-requests/4').request.method).toBe('GET');
    api.getCreateForm().subscribe();
    expect(http.expectOne('/api/v1/purchase-requests/create-form').request.method).toBe('GET');
    api
      .create({
        warehouseId: 1,
        neededByDate: '2026-11-01',
        priority: 2,
        justification: 'Need',
        lines: [],
      })
      .subscribe();
    expect(http.expectOne('/api/v1/purchase-requests').request.method).toBe('POST');
    api.submit(4, 'AQID').subscribe();
    request = http.expectOne('/api/v1/purchase-requests/4/submit');
    expect(request.request.method).toBe('POST');
    expect(request.request.body.rowVersion).toBe('AQID');
    api.cancel(4, 'AQID', 'No longer needed').subscribe();
    request = http.expectOne('/api/v1/purchase-requests/4/cancel');
    expect(request.request.method).toBe('POST');
    expect(request.request.body.reason).toBe('No longer needed');
  });
});
