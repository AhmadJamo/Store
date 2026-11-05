import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { JournalEntryApiService } from './journal-entry-api.service';
describe('JournalEntryApiService', () => {
  it('loads immutable details', () => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    const api = TestBed.inject(JournalEntryApiService),
      http = TestBed.inject(HttpTestingController);
    api.get(7).subscribe();
    const request = http.expectOne('/api/v1/journal-entries/7');
    expect(request.request.method).toBe('GET');
    request.flush({});
    http.verify();
  });
  it('loads the filtered journal directory', () => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    const api = TestBed.inject(JournalEntryApiService), http = TestBed.inject(HttpTestingController);
    api.getPage({ search: 'INV', fromDate: '2026-01-01', toDate: '2026-01-31', status: '2', accountId: 12, page: 2, pageSize: 25 }).subscribe();
    const request = http.expectOne(value => value.url === '/api/v1/journal-entries');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('search')).toBe('INV');
    expect(request.request.params.get('status')).toBe('2');
    expect(request.request.params.get('page')).toBe('2');
    expect(request.request.params.get('accountId')).toBe('12');
    request.flush({ items: [], totalCount: 0, totalPages: 0 });
    http.verify();
  });
});
