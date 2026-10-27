import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductAttributeApiService } from './data/product-attribute-api.service';
import { ProductAttributeValuesPageComponent } from './product-attribute-values-page';
import { ProductLocalizationService } from './product-localization.service';

describe('ProductAttributeValuesPageComponent', () => {
  const page = { productId: 9, productName: 'Shirt', productCode: 'SH-1', categoryName: 'Clothes', fields: [{ definitionId: 3, name: 'Color', code: 'COLOR', dataType: 4 as const, isRequired: true, isVariantDefining: true, value: '2', options: [{ id: 2, name: 'Red', code: 'RED' }] }] };
  const api = { getValues: vi.fn(() => of(page)), saveValues: vi.fn() }; const workspace = { activeTab: vi.fn(() => ({ id: 'product::9' })), markDirty: vi.fn() };
  beforeEach(async () => { vi.clearAllMocks(); api.getValues.mockReturnValue(of(page)); await TestBed.configureTestingModule({ imports: [ProductAttributeValuesPageComponent], providers: [provideRouter([]), { provide: ProductAttributeApiService, useValue: api }, { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '9' } } } }, { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: WorkspaceService, useValue: workspace }, { provide: ProductLocalizationService, useValue: { text: (key: string) => key } }] }).compileComponents(); });
  it('loads applicable fields and saves every value', () => { api.saveValues.mockReturnValue(of(page)); const component = TestBed.createComponent(ProductAttributeValuesPageComponent).componentInstance as any; component.form.controls.attribute_3.setValue('2'); component.form.markAsDirty(); component.save(); expect(api.saveValues).toHaveBeenCalledWith(9, [{ definitionId: 3, value: '2' }]); expect(component.form.dirty).toBe(false); });
  it('preserves rejected values', () => { api.saveValues.mockReturnValue(throwError(() => ({ title: 'Invalid', detail: 'Invalid option' }))); const component = TestBed.createComponent(ProductAttributeValuesPageComponent).componentInstance as any; component.form.controls.attribute_3.setValue('999'); component.form.markAsDirty(); component.save(); expect(component.problem()).toBe('Invalid option'); expect(component.form.controls.attribute_3.value).toBe('999'); expect(component.form.dirty).toBe(true); });
});
