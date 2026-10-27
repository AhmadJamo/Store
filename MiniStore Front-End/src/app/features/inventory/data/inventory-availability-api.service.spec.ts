import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { InventoryAvailabilityApiService } from './inventory-availability-api.service';

describe('InventoryAvailabilityApiService', () => {
  let api: InventoryAvailabilityApiService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [InventoryAvailabilityApiService, provideHttpClient(), provideHttpClientTesting()] }); api = TestBed.inject(InventoryAvailabilityApiService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());
  it('queries availability with named warehouse and paging filters', () => { api.getBalances({ warehouseId: 3, search: 'coffee', page: 2, pageSize: 25 }).subscribe(); const request = http.expectOne(r => r.url === '/api/v1/inventory-balances'); expect(request.request.method).toBe('GET'); expect(request.request.params.get('warehouseId')).toBe('3'); expect(request.request.params.get('search')).toBe('coffee'); expect(request.request.params.get('page')).toBe('2'); });
  it('queries reservation history with status and paging filters', () => { api.getReservations({ status: 1, search: 'SO-', page: 1, pageSize: 50 }).subscribe(); const request = http.expectOne(r => r.url === '/api/v1/inventory-reservations'); expect(request.request.method).toBe('GET'); expect(request.request.params.get('status')).toBe('1'); expect(request.request.params.get('search')).toBe('SO-'); });
});
