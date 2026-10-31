import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductCategoryApiService } from './data/product-category-api.service';
import { ProductCategoriesPageComponent } from './product-categories-page';
import { ProductLocalizationService } from './product-localization.service';

describe('ProductCategoriesPageComponent', () => {
  const category = { id: 4, name: 'Beverages', code: 'BEVERAGES', isActive: true };
  const api = { getAll: vi.fn(() => of([category])), create: vi.fn(), setStatus: vi.fn() };
  const workspace = { activeTab: vi.fn(() => ({ id: 'products::default' })), markDirty: vi.fn() };

  beforeEach(async () => {
    vi.clearAllMocks(); api.getAll.mockReturnValue(of([category]));
    await TestBed.configureTestingModule({ imports: [ProductCategoriesPageComponent], providers: [
      { provide: ProductCategoryApiService, useValue: api }, { provide: ApiSessionService, useValue: { can: () => true } },
      { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: WorkspaceService, useValue: workspace },
      { provide: ProductLocalizationService, useValue: { text: (key: string) => key } },
      provideRouter([]),
    ] }).compileComponents();
  });

  it('creates a normalized category and clears the dirty state', () => {
    api.create.mockReturnValue(of({ id: 5, name: 'Hot drinks', code: 'HOT_DRINKS', isActive: true }));
    const component = TestBed.createComponent(ProductCategoriesPageComponent).componentInstance as any;
    component.form.setValue({ name: ' Hot drinks ', code: 'hot_drinks' }); component.form.markAsDirty(); component.create();
    expect(api.create).toHaveBeenCalledWith({ name: 'Hot drinks', code: 'HOT_DRINKS' }); expect(component.items().some((item: any) => item.id === 5)).toBe(true); expect(component.form.dirty).toBe(false); expect(workspace.markDirty).toHaveBeenLastCalledWith('products::default', false);
  });

  it('preserves input and exposes a duplicate rejection', () => {
    api.create.mockReturnValue(throwError(() => ({ title: 'Duplicate', detail: 'Code exists', errors: { Code: ['Already used'] } })));
    const component = TestBed.createComponent(ProductCategoriesPageComponent).componentInstance as any;
    component.form.setValue({ name: 'Beverages', code: 'BEVERAGES' }); component.form.markAsDirty(); component.create();
    expect(component.problem()).toBe('Code exists'); expect(component.fieldErrors().Code[0]).toBe('Already used'); expect(component.form.controls.code.value).toBe('BEVERAGES'); expect(component.form.dirty).toBe(true);
  });

  it('changes category status only after confirmation', () => {
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true); api.setStatus.mockReturnValue(of({ ...category, isActive: false }));
    const component = TestBed.createComponent(ProductCategoriesPageComponent).componentInstance as any; component.setStatus(category);
    expect(api.setStatus).toHaveBeenCalledWith(4, false); expect(component.items()[0].isActive).toBe(false);
  });
});
