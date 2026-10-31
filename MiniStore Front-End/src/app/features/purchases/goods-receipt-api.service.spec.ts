import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { GoodsReceiptApiService } from './goods-receipt-api.service';

describe('GoodsReceiptApiService', () => {
  let api: GoodsReceiptApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [GoodsReceiptApiService, provideHttpClient(), provideHttpClientTesting()] });
    api = TestBed.inject(GoodsReceiptApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('uses every goods receipt endpoint and verb', () => {
    api.getAll('GR').subscribe();
    expect(http.expectOne(x => x.url === '/api/v1/goods-receipts').request.method).toBe('GET');
    api.get(3).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/3').request.method).toBe('GET');
    api.getCreateForm(4).subscribe();
    expect(http.expectOne(x => x.url === '/api/v1/goods-receipts/create-form').request.method).toBe('GET');
    api.create({}).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts').request.method).toBe('POST');
    api.createDraft({}).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/draft').request.method).toBe('POST');
    api.getScan(3).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/3/scan').request.method).toBe('GET');
    api.scan(3, {}).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/3/scan').request.method).toBe('POST');
    api.post(3).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/3/post').request.method).toBe('POST');
    api.inspect(3, {}).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/3/inspect').request.method).toBe('POST');
    api.putAway(3).subscribe();
    expect(http.expectOne('/api/v1/goods-receipts/3/put-away').request.method).toBe('POST');
  });
});
