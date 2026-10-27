import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { InventoryAdjustmentApiService } from './inventory-adjustment-api.service';

describe('InventoryAdjustmentApiService', () => {
  let api: InventoryAdjustmentApiService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [InventoryAdjustmentApiService, provideHttpClient(), provideHttpClientTesting()] }); api = TestBed.inject(InventoryAdjustmentApiService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());
  it('uses the complete inventory-adjustment workflow', () => {
    api.getAll().subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments').request.method).toBe('GET');
    api.getCreateForm().subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments/create-form').request.method).toBe('GET');
    api.create({ warehouseId: 1, isBlindCount: true, reason: 'Cycle', productIds: [2] }).subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments').request.method).toBe('POST');
    api.getDetails(4).subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments/4').request.method).toBe('GET');
    api.count(4, { lines: [{ productId: 2, countedQuantity: 3 }] }).subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments/4/count').request.method).toBe('POST');
    api.approve(4).subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments/4/approve').request.method).toBe('POST');
    api.post(4).subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments/4/post').request.method).toBe('POST');
    api.cancel(4).subscribe(); expect(http.expectOne('/api/v1/inventory-adjustments/4/cancel').request.method).toBe('POST');
  });
});
