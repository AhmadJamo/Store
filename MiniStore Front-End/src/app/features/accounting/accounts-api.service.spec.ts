import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AccountsApiService } from './accounts-api.service';
describe('AccountsApiService', () => {
  let api: AccountsApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(AccountsApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('creates through the bounded accounts endpoint', () => {
    const input = { code: '1101', name: 'Cash', type: 1, parentAccountId: 1 };
    api.create(input).subscribe();
    const request = http.expectOne('/api/v1/accounts');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(input);
    request.flush(null);
  });
});
