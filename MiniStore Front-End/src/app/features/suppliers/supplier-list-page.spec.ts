import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeListPreferencesService } from '../../shared/forms/safe-list-preferences.service';
import { PageContextService } from '../../shared/state/page-context.service';
import { SupplierApiService } from './data/supplier-api.service';
import { SupplierListPageComponent } from './supplier-list-page';

describe('SupplierListPageComponent', () => {
  it('uses the shared list hierarchy without losing filters, preferences, or actions', async () => {
    const replace = vi.fn();
    const preferences = { loadSuppliers: () => ({ includeInactive: false, pageSize: 25 }), saveSuppliers: vi.fn(), reset: vi.fn() };
    await TestBed.configureTestingModule({
      imports: [SupplierListPageComponent],
      providers: [
        provideRouter([]),
        { provide: SupplierApiService, useValue: { getPage: () => of({ query: { page: 1, pageSize: 25, search: 'coffee', includeInactive: false }, items: [], totalCount: 0, totalPages: 1 }) } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: LocalizationService, useValue: { text: (key: string) => key } },
        { provide: SafeListPreferencesService, useValue: preferences },
        { provide: PageContextService, useValue: { watch: () => of({ page: 1, search: 'coffee', includeInactive: false, pageSize: 25 }), replace } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(SupplierListPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('P2P');
    expect(fixture.nativeElement.querySelectorAll('[primary-actions]')).toHaveLength(1);
    expect(fixture.nativeElement.querySelector('.table-state.empty')?.textContent).toContain('filteredSuppliersEmpty');
    const clear = [...fixture.nativeElement.querySelectorAll('[filter-actions]')].find((button: Element) => button.textContent?.includes('clearFilters')) as HTMLButtonElement;
    clear.click();
    expect(replace).toHaveBeenCalledWith('suppliers-list', expect.anything(), expect.anything(), expect.objectContaining({ page: 1, search: '', includeInactive: false }));

    (fixture.componentInstance as any).result.set({
      query: { page: 1, pageSize: 25, search: '', includeInactive: false },
      items: [{ id: 9, name: 'Coffee Supply', contactPerson: 'Sam', phone: '123', paymentTermsDays: 30, isActive: true }],
      totalCount: 1, totalPages: 1,
    });
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelectorAll('.row-action')).toHaveLength(2);
    const save = [...fixture.nativeElement.querySelectorAll('[filter-actions]')].find((button: Element) => button.textContent?.includes('saveView')) as HTMLButtonElement;
    save.click();
    expect(preferences.saveSuppliers).toHaveBeenCalledWith(expect.objectContaining({ search: 'coffee', includeInactive: false, pageSize: 25 }));
  });
});