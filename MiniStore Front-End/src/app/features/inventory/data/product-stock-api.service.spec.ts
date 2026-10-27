import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductStockApiService } from './product-stock-api.service';

describe('ProductStockApiService', () => {
  let api: ProductStockApiService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [ProductStockApiService, provideHttpClient(), provideHttpClientTesting()] }); api = TestBed.inject(ProductStockApiService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());
  it('uses complete product-stock operations', () => {
    api.getAll().subscribe(); expect(http.expectOne('/api/v1/product-stocks').request.method).toBe('GET');
    api.getForm().subscribe(); expect(http.expectOne('/api/v1/product-stocks/form').request.method).toBe('GET');
    api.getForm(4).subscribe(); expect(http.expectOne('/api/v1/product-stocks/4/form').request.method).toBe('GET');
    api.create({ productId: 1, warehouseId: 2, quantity: 3 }).subscribe(); expect(http.expectOne('/api/v1/product-stocks').request.method).toBe('POST');
    api.update(4, 5).subscribe(); const update = http.expectOne('/api/v1/product-stocks/4'); expect(update.request.method).toBe('PUT'); expect(update.request.body).toEqual({ quantity: 5 });
    api.delete(4).subscribe(); expect(http.expectOne('/api/v1/product-stocks/4').request.method).toBe('DELETE');
  });
  it('loads and creates a complete manual adjustment', () => {
    api.getAdjustmentForm().subscribe(); expect(http.expectOne('/api/v1/stock-transactions/manual/form').request.method).toBe('GET');
    api.createAdjustment({ productId: 1, warehouseId: 2, type: 7, quantity: 2, reference: 'Count' }).subscribe();
    const request = http.expectOne('/api/v1/stock-transactions/manual'); expect(request.request.method).toBe('POST'); expect(request.request.body.reference).toBe('Count');
  });
});
