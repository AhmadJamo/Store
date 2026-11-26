import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductStockApiService } from './data/product-stock-api.service';
import { InventoryLocalizationService } from './inventory-localization.service';
import { ProductStocksPageComponent } from './product-stocks-page';

describe('ProductStocksPageComponent', () => {
  let routeId: string | null = null;
  const item: any = { id: 4, productId: 1, productName: 'Roasted coffee', warehouseId: 2, warehouseName: 'Main warehouse', quantity: 5, averageUnitCost: 2.25, inventoryValue: 11.25 };
  const stockPage: any = { form: { productId: 0, warehouseId: 0, quantity: 0 }, products: [{ id: 1, name: 'Roasted coffee' }], warehouses: [{ id: 2, name: 'Main warehouse' }] };
  const adjustmentPage: any = { form: { productId: 0, warehouseId: 0, type: 6, quantity: 0, reference: '' }, products: stockPage.products, warehouses: stockPage.warehouses };
  const api = {
    getAll: vi.fn<any>(() => of([item])), getForm: vi.fn<any>(() => of(stockPage)), create: vi.fn<any>(), update: vi.fn<any>(), delete: vi.fn<any>(),
    getAdjustmentForm: vi.fn<any>(() => of(adjustmentPage)), createAdjustment: vi.fn<any>(),
  };
  const permissions = new Set(['ProductStock.Create', 'ProductStock.Edit', 'ProductStock.Delete', 'StockTransactions.Create']);
  const workspace = { activeTab: vi.fn(() => ({ id: 'stocks' })), markDirty: vi.fn() };
  const focus = { focusFirstInvalid: vi.fn() };

  beforeEach(async () => {
    routeId = null;
    vi.clearAllMocks();
    permissions.clear();
    ['ProductStock.Create', 'ProductStock.Edit', 'ProductStock.Delete', 'StockTransactions.Create'].forEach(permission => permissions.add(permission));
    api.getAll.mockReturnValue(of([item]));
    api.getForm.mockReturnValue(of(stockPage));
    api.getAdjustmentForm.mockReturnValue(of(adjustmentPage));
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    await TestBed.configureTestingModule({
      imports: [ProductStocksPageComponent],
      providers: [
        { provide: ProductStockApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: FormFocusService, useValue: focus },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => routeId } } } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored named stock, quantity, cost, and value evidence', () => {
    const fixture = TestBed.createComponent(ProductStocksPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('2.25');
    expect(fixture.nativeElement.textContent).toContain('11.25');
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip article')).toHaveLength(2);
    expect(fixture.nativeElement.querySelectorAll('app-data-table th')).toHaveLength(7);
    expect(fixture.nativeElement.querySelectorAll('form.stock-operation app-form-section')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('form.stock-operation app-field-cue')).toHaveLength(3);
    expect(fixture.nativeElement.querySelectorAll('form.stock-operation app-workflow-footer')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('form.adjustment-operation app-form-section')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('form.adjustment-operation app-field-cue')).toHaveLength(5);
    expect(fixture.nativeElement.querySelectorAll('form.adjustment-operation app-workflow-footer')).toHaveLength(1);
  });

  it('keeps opening stock, edit, delete, and manual adjustment independently permission-gated', () => {
    permissions.clear();
    const fixture = TestBed.createComponent(ProductStocksPageComponent);
    fixture.detectChanges();

    expect(api.getForm).not.toHaveBeenCalled();
    expect(api.getAdjustmentForm).not.toHaveBeenCalled();
    expect(fixture.nativeElement.querySelectorAll('form.operation-form')).toHaveLength(0);
    expect(fixture.nativeElement.textContent).not.toContain('edit');
    expect(fixture.nativeElement.textContent).not.toContain('delete');
  });

  it('loads the requested stock through the edit contract and locks its product and warehouse identity', () => {
    routeId = '4';
    const component = TestBed.createComponent(ProductStocksPageComponent).componentInstance as any;

    expect(api.getForm).toHaveBeenCalledWith(4);
    expect(component.editingId()).toBe(4);
    expect(component.stockForm.controls.productId.disabled).toBe(true);
    expect(component.stockForm.controls.warehouseId.disabled).toBe(true);
  });

  it('keeps opening-stock creation and current-quantity edit as separate commands', () => {
    api.create.mockReturnValue(of(item));
    api.update.mockReturnValue(of({ ...item, quantity: 8, inventoryValue: 18 }));
    const component = TestBed.createComponent(ProductStocksPageComponent).componentInstance as any;

    component.stockForm.patchValue({ productId: 1, warehouseId: 2, quantity: 5 });
    component.saveStock();
    expect(api.create).toHaveBeenCalledWith({ productId: 1, warehouseId: 2, quantity: 5 });

    component.editingId.set(4);
    component.stockForm.patchValue({ productId: 1, warehouseId: 2, quantity: 8 });
    component.saveStock();
    expect(api.update).toHaveBeenCalledWith(4, 8);
  });

  it('submits only adjustment-in, adjustment-out, and kitchen-variance manual movement types with a reason', () => {
    api.createAdjustment.mockReturnValue(of(void 0));
    const component = TestBed.createComponent(ProductStocksPageComponent).componentInstance as any;

    for (const type of [6, 7, 8]) {
      component.adjustmentForm.setValue({ productId: 1, warehouseId: 2, type, quantity: 2, reference: `Count ${type}` });
      component.saveAdjustment();
    }

    expect(api.createAdjustment.mock.calls.map((call: any[]) => call[0].type)).toEqual([6, 7, 8]);
    expect(api.createAdjustment).toHaveBeenLastCalledWith({ productId: 1, warehouseId: 2, type: 8, quantity: 2, reference: 'Count 8' });
  });

  it('blocks invalid stock and adjustment commands, focuses the first invalid field, and preserves dirty leave confirmation', () => {
    const component = TestBed.createComponent(ProductStocksPageComponent).componentInstance as any;
    component.stockForm.setValue({ productId: 0, warehouseId: 0, quantity: -1 });
    component.saveStock();
    component.adjustmentForm.setValue({ productId: 0, warehouseId: 0, type: 6, quantity: 0, reference: '' });
    component.saveAdjustment();

    expect(api.create).not.toHaveBeenCalled();
    expect(api.createAdjustment).not.toHaveBeenCalled();
    expect(focus.focusFirstInvalid).toHaveBeenCalledTimes(2);
    component.stockForm.markAsDirty();
    expect(component.canDeactivate()).toBe(true);
    expect(globalThis.confirm).toHaveBeenCalledWith('unsavedDescription');
  });

  it('preserves rejected edits and delete restrictions with the server-authored reason', () => {
    api.update.mockReturnValue(throwError(() => ({ detail: 'Insufficient stock.' })));
    api.delete.mockReturnValue(throwError(() => ({ detail: 'Adjust the quantity to zero instead.' })));
    const component = TestBed.createComponent(ProductStocksPageComponent).componentInstance as any;
    component.editingId.set(4);
    component.stockForm.patchValue({ productId: 1, warehouseId: 2, quantity: 8 });
    component.saveStock();
    expect(component.stockForm.controls.quantity.value).toBe(8);
    expect(component.problem()).toBe('Insufficient stock.');

    component.delete(item);
    expect(api.delete).toHaveBeenCalledWith(4);
    expect(component.problem()).toBe('Adjust the quantity to zero instead.');
  });
});
