import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { StockTransactionApiService } from './stock-transaction-api.service';

describe('StockTransactionApiService', () => {
  let api: StockTransactionApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [StockTransactionApiService, provideHttpClient(), provideHttpClientTesting()] });
    api = TestBed.inject(StockTransactionApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('sends every ledger filter to the read-only endpoint', () => {
    api.getPage({ search: 'coffee', type: 2, movement: 'incoming', fromDate: '2026-10-01', toDate: '2026-10-27', page: 2, pageSize: 25, sortColumn: 'product', sortDescending: false }).subscribe();
    const request = http.expectOne(candidate => candidate.url === '/api/v1/stock-transactions');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('search')).toBe('coffee');
    expect(request.request.params.get('type')).toBe('2');
    expect(request.request.params.get('movement')).toBe('incoming');
    expect(request.request.params.get('fromDate')).toBe('2026-10-01');
    expect(request.request.params.get('toDate')).toBe('2026-10-27');
    expect(request.request.params.get('page')).toBe('2');
    expect(request.request.params.get('pageSize')).toBe('25');
    expect(request.request.params.get('sortColumn')).toBe('product');
    expect(request.request.params.get('sortDescending')).toBe('false');
    request.flush({ items: [], totalCount: 0, page: 2, pageSize: 25, totalPages: 0 });
  });
});
