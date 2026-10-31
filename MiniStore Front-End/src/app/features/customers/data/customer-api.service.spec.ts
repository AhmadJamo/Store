import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { CustomerApiService } from './customer-api.service';
import { CustomerForm } from './customer.models';

describe('CustomerApiService', () => {
  let api: CustomerApiService;
  let http: HttpTestingController;
  const form: CustomerForm = { id: 0, name: 'Acme', type: 1, preferredLanguage: 'ar-JO', timeZoneId: 'Asia/Amman', paymentTermDays: 0, isTaxExempt: false, rowVersion: '' };

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [CustomerApiService, provideHttpClient(), provideHttpClientTesting()] });
    api = TestBed.inject(CustomerApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('uses list query parameters', () => {
    api.getPage({ page: 2, pageSize: 25, search: 'Acme' }).subscribe();
    const request = http.expectOne((candidate) => candidate.url === '/api/v1/customers');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('page')).toBe('2');
    expect(request.request.params.get('search')).toBe('Acme');
    request.flush({ items: [], query: {}, totalCount: 0, totalPages: 0 });
  });

  it('creates without requiring a generated customer code', () => {
    api.create(form).subscribe();
    const request = http.expectOne('/api/v1/customers');
    expect(request.request.method).toBe('POST');
    expect(request.request.body.customerCode).toBeUndefined();
    request.flush({ id: 7 });
  });

  it('sends the row version when updating', () => {
    api.update({ ...form, id: 7, rowVersion: 'AQID' }).subscribe();
    const request = http.expectOne('/api/v1/customers/7');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body.rowVersion).toBe('AQID');
    request.flush({ id: 7 });
  });
});
