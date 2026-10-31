import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { StockTransferApiService } from './stock-transfer-api.service';

describe('StockTransferApiService', () => {
  let api: StockTransferApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StockTransferApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(StockTransferApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('uses the complete stock-transfer workflow', () => {
    api.getAll('TR', 2).subscribe();
    const list = http.expectOne(request => request.url === '/api/v1/stock-transfers');
    expect(list.request.method).toBe('GET');
    expect(list.request.params.get('search')).toBe('TR');
    expect(list.request.params.get('status')).toBe('2');
    list.flush([]);

    api.getCreateForm().subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/create-form').request.method).toBe('GET');
    api.getEditForm(4).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/edit-form').request.method).toBe('GET');
    api.getDetails(4).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4').request.method).toBe('GET');

    const input = { fromWarehouseId: 1, toWarehouseId: 2, items: [{ productId: 3, quantity: 4 }] };
    api.create(input).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers').request.method).toBe('POST');
    api.update(4, input).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4').request.method).toBe('PUT');

    api.submit(4).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/submit').request.method).toBe('POST');
    api.approve(4).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/approve').request.method).toBe('POST');
    api.returnToDraft(4).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/return-to-draft').request.method).toBe('POST');
    api.post(4).subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/post').request.method).toBe('POST');
    api.reject(4, 'Damaged').subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/reject').request.body.reason).toBe('Damaged');
    api.cancel(4, 'Reverse').subscribe();
    expect(http.expectOne('/api/v1/stock-transfers/4/cancel').request.body.reason).toBe('Reverse');
  });
});
