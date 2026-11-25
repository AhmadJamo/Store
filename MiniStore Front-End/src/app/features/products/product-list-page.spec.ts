import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { PageContextService } from '../../shared/state/page-context.service';
import { ProductApiService } from './data/product-api.service';
import { ProductListPageComponent } from './product-list-page';
import { ProductLocalizationService } from './product-localization.service';

describe('ProductListPageComponent', () => {
  it('keeps create primary, consolidates catalog links, and explains filtered-empty results', async () => {
    const replace = vi.fn();
    await TestBed.configureTestingModule({
      imports: [ProductListPageComponent],
      providers: [
        provideRouter([]),
        { provide: ProductApiService, useValue: { getPage: () => of({ query: { page: 1, pageSize: 25, sortBy: 'name', sortDescending: false }, items: [], totalCount: 0, totalPages: 0, categories: [] }) } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: PageContextService, useValue: { watch: () => of({ page: 1, search: 'coffee', isActive: true }), replace } },
        { provide: ProductLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(ProductListPageComponent);
    fixture.detectChanges();

    const menu = fixture.nativeElement.querySelector('.catalog-menu') as HTMLDetailsElement;
    expect(menu.querySelector('summary')?.textContent).toContain('catalogNavigation');
    expect(menu.querySelectorAll('nav a')).toHaveLength(6);
    expect(fixture.nativeElement.querySelectorAll('[primary-actions]')).toHaveLength(1);
    expect(fixture.nativeElement.textContent).not.toContain('CATALOG');
    expect(fixture.nativeElement.querySelector('.table-state.empty')?.textContent).toContain('filteredProductsEmpty');
    const clear = fixture.nativeElement.querySelector('[filter-actions]') as HTMLButtonElement;
    expect(clear.textContent).toContain('clearFilters');
    clear.click();
    expect(replace).toHaveBeenCalledWith('products-list', expect.anything(), expect.anything(), { page: 1, search: '', isActive: undefined });

    (fixture.componentInstance as any).page.set({
      query: { page: 1, pageSize: 25, sortBy: 'name', sortDescending: false },
      items: [{ id: 4, productCode: 'P-004', name: 'Coffee', barcode: '1234', salePrice: 2, isActive: true }],
      totalCount: 1,
      totalPages: 1,
      categories: [],
    });
    fixture.detectChanges();
    const actions = fixture.nativeElement.querySelectorAll('.row-action') as NodeListOf<HTMLAnchorElement>;
    expect(actions).toHaveLength(3);
    expect([...actions].map(action => action.getAttribute('aria-label'))).toEqual(['view', 'edit', 'attributes']);
  });
});
