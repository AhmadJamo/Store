import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SupplierApiService } from './supplier-api.service';
import { SupplierCommand } from './supplier.models';

describe('SupplierApiService', () => {
  const command: SupplierCommand = {
    name: 'Local Foods', paymentTermsDays: 30, isActive: true,
    accountId: 12, productIds: [4, 9], rowVersion: 'AQID',
  };

  it('sends paging and search only to the supplier API', () => {
    TestBed.configureTestingModule({ providers: [SupplierApiService, provideHttpClient(), provideHttpClientTesting()] });
    const api = TestBed.inject(SupplierApiService);
    const http = TestBed.inject(HttpTestingController);
    api.getPage({ page: 3, pageSize: 25, search: 'Local', includeInactive: true }).subscribe();
    const request = http.expectOne((candidate) => candidate.url === '/api/v1/suppliers');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('page')).toBe('3');
    expect(request.request.params.get('includeInactive')).toBe('true');
    request.flush({ items: [], query: {}, totalCount: 0, totalPages: 0 });
    http.verify();
  });

  it('uses the dedicated form contracts for create and edit', () => {
    TestBed.configureTestingModule({ providers: [SupplierApiService, provideHttpClient(), provideHttpClientTesting()] });
    const api = TestBed.inject(SupplierApiService);
    const http = TestBed.inject(HttpTestingController);

    api.getForm().subscribe();
    http.expectOne('/api/v1/suppliers/form').flush({ form: {}, accounts: [], products: [] });
    api.getForm(8).subscribe();
    http.expectOne('/api/v1/suppliers/8/form').flush({ form: {}, accounts: [], products: [] });

    api.create(command).subscribe();
    const create = http.expectOne('/api/v1/suppliers');
    expect(create.request.method).toBe('POST');
    expect(create.request.body).toEqual(command);
    create.flush({ id: 8 });

    api.update(8, command).subscribe();
    const update = http.expectOne('/api/v1/suppliers/8');
    expect(update.request.method).toBe('PUT');
    expect(update.request.body.rowVersion).toBe('AQID');
    update.flush({ id: 8 });
    http.verify();
  });
});
