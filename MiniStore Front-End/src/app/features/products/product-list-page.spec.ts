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
  it('keeps create primary and consolidates catalog destinations in one menu', async () => {
    await TestBed.configureTestingModule({
      imports: [ProductListPageComponent],
      providers: [
        provideRouter([]),
        { provide: ProductApiService, useValue: { getPage: () => of({ query: { page: 1, pageSize: 25, sortBy: 'name', sortDescending: false }, items: [], totalCount: 0, totalPages: 0, categories: [] }) } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: PageContextService, useValue: { watch: () => of({ page: 1, search: '', isActive: undefined }), replace: vi.fn() } },
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
  });
});
