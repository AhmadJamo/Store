import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeListPreferencesService } from '../../shared/forms/safe-list-preferences.service';
import { PageContextService } from '../../shared/state/page-context.service';
import { CustomerApiService } from './data/customer-api.service';
import { CustomerListPageComponent } from './customer-list-page';

describe('CustomerListPageComponent', () => {
  it('uses the shared list hierarchy without losing filters, sort, preferences, or actions', async () => {
    const replace = vi.fn();
    const preferences = {
      loadCustomers: () => ({ includeArchived: false, sortBy: 'name', sortDescending: false, pageSize: 25 }),
      saveCustomers: vi.fn(), reset: vi.fn(),
    };
    await TestBed.configureTestingModule({
      imports: [CustomerListPageComponent],
      providers: [
        provideRouter([]),
        { provide: CustomerApiService, useValue: { getPage: () => of({ query: { page: 1, pageSize: 25, search: 'acme', includeArchived: false, sortBy: 'name', sortDescending: false }, items: [], totalCount: 0, totalPages: 1 }) } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: LocalizationService, useValue: { text: (key: string) => key } },
        { provide: SafeListPreferencesService, useValue: preferences },
        { provide: PageContextService, useValue: { watch: () => of({ page: 1, search: 'acme', includeArchived: false, sortBy: 'name', sortDescending: false, pageSize: 25 }), replace } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CustomerListPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('CRM');
    expect(fixture.nativeElement.querySelectorAll('[primary-actions]')).toHaveLength(1);
    expect(fixture.nativeElement.querySelector('.table-state.empty')?.textContent).toContain('filteredCustomersEmpty');
    const clear = [...fixture.nativeElement.querySelectorAll('[filter-actions]')].find((button: Element) => button.textContent?.includes('clearFilters')) as HTMLButtonElement;
    clear.click();
    expect(replace).toHaveBeenCalledWith('customers-list', expect.anything(), expect.anything(), expect.objectContaining({ page: 1, search: '', includeArchived: false }));

    (fixture.componentInstance as any).page.set({
      query: { page: 1, pageSize: 25, search: '', includeArchived: false, sortBy: 'name', sortDescending: false },
      items: [{ id: 7, customerCode: 'C-007', type: 1, name: 'Acme', phone: '123', accountName: 'Receivables', isActive: true }],
      totalCount: 1, totalPages: 1,
    });
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelectorAll('.row-action')).toHaveLength(2);
    const codeSort = fixture.nativeElement.querySelector('.sort-button') as HTMLButtonElement;
    codeSort.click();
    expect(replace).toHaveBeenLastCalledWith('customers-list', expect.anything(), expect.anything(), expect.objectContaining({ sortBy: 'code', sortDescending: false, page: 1 }));
  });
});