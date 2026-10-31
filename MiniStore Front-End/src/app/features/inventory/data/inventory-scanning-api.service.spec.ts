import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { InventoryScanningApiService } from './inventory-scanning-api.service';

describe('InventoryScanningApiService', () => {
  let api: InventoryScanningApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [InventoryScanningApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(InventoryScanningApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('uses exact scan resolution and three explicit command routes', () => {
    api.resolve({ adjustmentScan: 'ADJ-1', productScan: 'P-1', sourceLocationScan: 'A', locationScan: 'B' }).subscribe();
    const resolve = http.expectOne(request => request.url === '/api/v1/inventory-scanning');
    expect(resolve.request.method).toBe('GET');
    expect(resolve.request.params.get('productScan')).toBe('P-1');
    resolve.flush({});

    api.putaway({ productScan: 'P-1', locationScan: 'B', quantity: 2, idempotencyKey: 'SCAN-PUTAWAY:1' }).subscribe();
    expect(http.expectOne('/api/v1/inventory-scanning/putaway').request.method).toBe('POST');
    api.relocate({ productScan: 'P-1', sourceLocationScan: 'A', destinationLocationScan: 'B', quantity: 2, idempotencyKey: 'SCAN-RELOCATION:1' }).subscribe();
    expect(http.expectOne('/api/v1/inventory-scanning/relocate').request.method).toBe('POST');
    api.count({ adjustmentNumber: 'ADJ-1', productScan: 'P-1', locationScan: 'B', countedQuantity: 2 }).subscribe();
    expect(http.expectOne('/api/v1/inventory-scanning/count').request.method).toBe('POST');
  });
});
