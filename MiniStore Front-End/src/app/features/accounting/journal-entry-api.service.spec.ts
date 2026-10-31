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
});
