import { computed, Injectable } from '@angular/core';
import { map, Observable, throwError } from 'rxjs';
import { ApiSessionService } from '../../core/api/api-session.service';
import { CustomerApiService } from '../customers/data/customer-api.service';
import { SupplierApiService } from '../suppliers/data/supplier-api.service';
import { QuickPreviewProvider, QuickPreviewProviderId, QuickPreviewResult } from './command-center.models';

const providers: readonly QuickPreviewProvider[] = [
  { id: 'customers', labelKey: 'customersTitle', descriptionKey: 'quickCustomersDescription', navigationId: 'customers', requiredPermission: 'Customers.View' },
  { id: 'suppliers', labelKey: 'suppliersTitle', descriptionKey: 'quickSuppliersDescription', navigationId: 'suppliers', requiredPermission: 'Suppliers.View' },
];

@Injectable({ providedIn: 'root' })
export class CommandCenterRegistryService {
  readonly availableProviders = computed(() => providers.filter((provider) => this.session.can(provider.requiredPermission)));

  constructor(
    private readonly session: ApiSessionService,
    private readonly customers: CustomerApiService,
    private readonly suppliers: SupplierApiService,
  ) {}

  find(id: string): QuickPreviewProvider | undefined {
    return this.availableProviders().find((provider) => provider.id === id);
  }

  load(id: QuickPreviewProviderId, search = ''): Observable<QuickPreviewResult> {
    const provider = this.find(id);
    if (!provider) return throwError(() => new Error('Quick preview provider is not available.'));
    const cleanSearch = search.trim().slice(0, 100) || undefined;
    if (id === 'customers') {
      return this.customers.getPage({ page: 1, pageSize: 10, search: cleanSearch }).pipe(map((page) => ({
        providerId: id,
        totalCount: page.totalCount,
        rows: page.items.map((item) => ({ id: item.id, title: item.name, reference: item.customerCode, secondary: item.phone || item.email, route: `/customers/${item.id}`, active: item.isActive })),
      })));
    }
    return this.suppliers.getPage({ page: 1, pageSize: 10, search: cleanSearch }).pipe(map((page) => ({
      providerId: id,
      totalCount: page.totalCount,
      rows: page.items.map((item) => ({ id: item.id, title: item.name, secondary: item.contactPerson || item.phone || item.email, route: `/suppliers/${item.id}`, active: item.isActive })),
    })));
  }
}
