import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductAttributeApiService } from './data/product-attribute-api.service';
import { ProductAttributesPageComponent } from './product-attributes-page';
import { ProductLocalizationService } from './product-localization.service';

describe('ProductAttributesPageComponent', () => {
  const page: any = { form: {}, categories: [{ id: 2, name: 'Clothes', code: 'CLOTHES', isActive: true }], definitions: [] }; const created = { id: 3, name: 'Color', code: 'COLOR', dataType: 4 as const, isRequired: true, isVariantDefining: true, isActive: true, displayOrder: 1, categories: ['Clothes'], options: ['Red'] };
  const api = { getPage: vi.fn(() => of(page)), create: vi.fn(), setStatus: vi.fn() }; const workspace = { activeTab: vi.fn(() => ({ id: 'attributes::default' })), markDirty: vi.fn() };
  beforeEach(async () => { vi.clearAllMocks(); api.getPage.mockReturnValue(of(page)); await TestBed.configureTestingModule({ imports: [ProductAttributesPageComponent], providers: [{ provide: ProductAttributeApiService, useValue: api }, { provide: ApiSessionService, useValue: { can: () => true } }, { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: WorkspaceService, useValue: workspace }, { provide: ProductLocalizationService, useValue: { text: (key: string) => key } }, provideRouter([])] }).compileComponents(); });
  it('creates a normalized selection attribute with categories', () => { api.create.mockReturnValue(of(created)); const component = TestBed.createComponent(ProductAttributesPageComponent).componentInstance as any; component.form.setValue({ name: ' Color ', code: 'color', dataType: 4, isRequired: true, isVariantDefining: true, displayOrder: 1, productCategoryIds: [2], optionsText: 'Red|RED' }); component.form.markAsDirty(); component.create(); expect(api.create).toHaveBeenCalledWith(expect.objectContaining({ name: 'Color', code: 'COLOR', productCategoryIds: [2] })); expect(component.definitions()[0].id).toBe(3); expect(component.form.dirty).toBe(false); });
  it('preserves the form when the server rejects creation', () => { api.create.mockReturnValue(throwError(() => ({ title: 'Duplicate', detail: 'Code exists', errors: { Code: ['Used'] } }))); const component = TestBed.createComponent(ProductAttributesPageComponent).componentInstance as any; component.form.patchValue({ name: 'Color', code: 'COLOR' }); component.form.markAsDirty(); component.create(); expect(component.problem()).toBe('Code exists'); expect(component.form.controls.name.value).toBe('Color'); expect(component.form.dirty).toBe(true); });
  it('changes status after confirmation', () => { vi.spyOn(globalThis, 'confirm').mockReturnValue(true); api.getPage.mockReturnValue(of({ ...page, definitions: [created] })); api.setStatus.mockReturnValue(of({ ...created, isActive: false })); const component = TestBed.createComponent(ProductAttributesPageComponent).componentInstance as any; component.setStatus(created); expect(api.setStatus).toHaveBeenCalledWith(3, false); expect(component.definitions()[0].isActive).toBe(false); });
});
