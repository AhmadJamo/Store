import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiSessionService } from '../../core/api/api-session.service';
import { CustomerApiService } from '../customers/data/customer-api.service';
import { SupplierApiService } from '../suppliers/data/supplier-api.service';
import { CommandCenterRegistryService } from './command-center-registry.service';

describe('CommandCenterRegistryService', () => {
  const customers = { getPage: vi.fn(() => of({ items: [{ id: 3, name: 'Acme', customerCode: 'C-3', isActive: true }], totalCount: 12 })) };
  const suppliers = { getPage: vi.fn(() => of({ items: [], totalCount: 0 })) };

  beforeEach(() => {
    vi.clearAllMocks();
    TestBed.configureTestingModule({ providers: [
      CommandCenterRegistryService,
      { provide: ApiSessionService, useValue: { can: (permission: string) => permission === 'Customers.View' } },
      { provide: CustomerApiService, useValue: customers },
      { provide: SupplierApiService, useValue: suppliers },
    ] });
  });

  it('exposes only providers permitted by the backend session', () => {
    expect(TestBed.inject(CommandCenterRegistryService).availableProviders().map((provider) => provider.id)).toEqual(['customers']);
  });

  it('uses bounded server paging and maps a limited preview contract', () => {
    let result: any;
    TestBed.inject(CommandCenterRegistryService).load('customers', ' Acme ').subscribe((value) => result = value);
    expect(customers.getPage).toHaveBeenCalledWith({ page: 1, pageSize: 10, search: 'Acme' });
    expect(result.rows[0]).toEqual(expect.objectContaining({ title: 'Acme', route: '/customers/3' }));
    expect(result.totalCount).toBe(12);
  });
});
