import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductApiService } from './data/product-api.service';
import { ProductFormPageComponent } from './product-form-page';
import { ProductLocalizationService } from './product-localization.service';

describe('ProductFormPageComponent', () => {
  const product = { id: 0, productCode: '', name: '', purchasePrice: 0, wholesalePrice: 0, salePrice: 0, inventoryBehavior: 1, productType: 2, stockUnit: 1, measurementUnitId: 1, allowNegativeRecipeConsumption: false, isSellableInPos: true, isSellableInSales: true, isActive: true, trackingPolicy: 0, requireExpirationDate: false, expirationWarningDays: 30, handlingRequirements: 0, supplierIds: [], supplierNames: [], purchaseBillingPolicy: 2 };
  const page = { form: product, measurementUnits: [{ id: 1, code: 'PC', name: 'Piece', symbol: 'pc', dimension: 1, isActive: true }], categories: [], suppliers: [{ id: 8, name: 'Supplier' }], expenseAccounts: [{ id: 9, code: '5100', name: 'Expense', type: 5 }] };
  const api = { getForm: vi.fn(() => of(page)), create: vi.fn(), update: vi.fn() };
  const router = { navigate: vi.fn(() => Promise.resolve(true)) };
  const workspace = { activeTab: vi.fn(() => ({ id: 'products::default' })), markDirty: vi.fn() };
  let routeId: string | null;

  beforeEach(async () => {
    vi.clearAllMocks(); routeId = null; api.getForm.mockReturnValue(of(page));
    await TestBed.configureTestingModule({ imports: [ProductFormPageComponent], providers: [
      { provide: ProductApiService, useValue: api }, { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => routeId } } } },
      { provide: Router, useValue: router }, { provide: WorkspaceService, useValue: workspace },
      { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: ProductLocalizationService, useValue: { text: (key: string) => key } },
    ] }).compileComponents();
  });

  it('creates a complete product command and clears dirty state', () => {
    api.create.mockReturnValue(of({ ...product, id: 7, name: 'Coffee' }));
    const component = TestBed.createComponent(ProductFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Coffee', purchasePrice: 2, wholesalePrice: 3, salePrice: 4, supplierIds: [8] }); component.form.markAsDirty(); component.save();
    expect(api.create).toHaveBeenCalledWith(expect.objectContaining({ name: 'Coffee', measurementUnitId: 1, supplierIds: [8], purchasePrice: 2 }));
    expect(workspace.markDirty).toHaveBeenLastCalledWith('products::default', false); expect(router.navigate).toHaveBeenCalledWith(['/products', 7]);
  });

  it('keeps entered values and server validation after a rejected save', () => {
    api.create.mockReturnValue(throwError(() => ({ title: 'Validation', detail: 'Barcode exists', errors: { Barcode: ['Duplicate'] } })));
    const component = TestBed.createComponent(ProductFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Coffee', barcode: '123', salePrice: 4 }); component.form.markAsDirty(); component.save();
    expect(component.problem()).toBe('Barcode exists'); expect(component.fieldErrors().Barcode[0]).toBe('Duplicate'); expect(component.form.controls.barcode.value).toBe('123'); expect(component.form.dirty).toBe(true);
  });

  it('enforces raw and prepared presentation rules before sending', () => {
    api.create.mockReturnValue(of({ ...product, id: 8 }));
    const component = TestBed.createComponent(ProductFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Ingredient', productType: 1, isSellableInPos: true, isSellableInSales: true }); component.save();
    expect(api.create).toHaveBeenLastCalledWith(expect.objectContaining({ isSellableInPos: false, isSellableInSales: false }));
    component.form.patchValue({ name: 'Prepared', productType: 3, purchasePrice: 9, supplierIds: [8] }); component.save();
    expect(api.create).toHaveBeenLastCalledWith(expect.objectContaining({ purchasePrice: 0, supplierIds: [] }));
  });

  it('updates the selected product through the edit route', () => {
    routeId = '12'; api.update.mockReturnValue(of({ ...product, id: 12, name: 'Updated' }));
    const component = TestBed.createComponent(ProductFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Updated' }); component.save();
    expect(api.update).toHaveBeenCalledWith(12, expect.objectContaining({ name: 'Updated' }));
  });
});
