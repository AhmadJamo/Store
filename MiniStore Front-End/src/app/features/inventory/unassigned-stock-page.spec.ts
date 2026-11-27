import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { WarehouseOperationsApiService } from './data/warehouse-operations-api.service';
import { WarehouseOperationsLocalizationService } from './warehouse-operations-localization.service';
import { UnassignedStockPageComponent } from './unassigned-stock-page';

describe('UnassignedStockPageComponent', () => {
  const permissions = new Set<string>();
  const workspace = { activeTab: vi.fn(() => ({ id: 'putaway' })), markDirty: vi.fn() };
  const page: any = {
    rows: [{ productId: 1, productName: 'Coffee', barcode: 'COF-1', warehouseId: 2, warehouseName: 'Main warehouse', warehouseQuantity: 10, assignedQuantity: 6, unassignedQuantity: 4, suggestedLocationId: 4, suggestedLocationCode: 'A-01' }],
    warehouses: [{ id: 2, name: 'Main warehouse' }, { id: 3, name: 'Reserve warehouse' }],
    locations: [{ id: 4, warehouseId: 2, code: 'A-01' }, { id: 5, warehouseId: 3, code: 'B-01' }],
    products: [{ id: 1, name: 'Coffee' }, { id: 6, name: 'Milk' }],
    categories: [{ id: 7, name: 'Beverages' }],
    rules: [{ id: 8, warehouseId: 2, warehouseName: 'Main warehouse', storageLocationId: 4, storageLocationCode: 'A-01', target: 'Coffee', priority: 10, isActive: true }],
  };
  const api = {
    getUnassigned: vi.fn<any>(() => of(page)),
    assign: vi.fn<any>(() => of(void 0)),
    createRule: vi.fn<any>(() => of(void 0)),
    setRuleActive: vi.fn<any>(() => of(void 0)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    permissions.clear();
    permissions.add('ProductStock.View');
    permissions.add('ProductStock.Edit');
    api.getUnassigned.mockReturnValue(of(page));
    api.assign.mockReturnValue(of(void 0));
    api.createRule.mockReturnValue(of(void 0));
    api.setRuleActive.mockReturnValue(of(void 0));
    await TestBed.configureTestingModule({
      imports: [UnassignedStockPageComponent],
      providers: [
        { provide: WarehouseOperationsApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: any) => error } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: WarehouseOperationsLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored unassigned stock, suggestion, rule, and named option evidence', () => {
    const fixture = TestBed.createComponent(UnassignedStockPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    ['Coffee', 'COF-1', 'Main warehouse', 'A-01', '4'].forEach(value => expect(text).toContain(value));
    expect(fixture.componentInstance['page']()?.categories).toEqual([{ id: 7, name: 'Beverages' }]);
    expect(fixture.componentInstance['page']()?.rows[0]).toEqual(expect.objectContaining({ warehouseQuantity: 10, assignedQuantity: 6, unassignedQuantity: 4, suggestedLocationId: 4 }));
    expect(fixture.nativeElement.querySelectorAll('tbody tr')).toHaveLength(2);
  });

  it('preserves warehouse, query, and sort request contracts', () => {
    const component = TestBed.createComponent(UnassignedStockPageComponent).componentInstance as any;
    expect(api.getUnassigned).toHaveBeenLastCalledWith(undefined, '', 'product');
    component.filter.setValue({ warehouseId: 2, query: 'coffee', sort: 'warehouse' });
    component.load();
    expect(api.getUnassigned).toHaveBeenLastCalledWith(2, 'coffee', 'warehouse');
  });

  it('keeps assignment, rule creation, row choice, and rule status gated by ProductStock.Edit', () => {
    permissions.delete('ProductStock.Edit');
    const viewOnly = TestBed.createComponent(UnassignedStockPageComponent); viewOnly.detectChanges();
    expect(viewOnly.nativeElement.querySelectorAll('form.operation-form')).toHaveLength(0);
    expect(viewOnly.nativeElement.querySelectorAll('tbody tr')[0].querySelector('button')).toBeNull();
    expect(viewOnly.nativeElement.querySelectorAll('tbody tr')[1].querySelector('button').disabled).toBe(true);

    permissions.add('ProductStock.Edit');
    const editor = TestBed.createComponent(UnassignedStockPageComponent); editor.detectChanges();
    expect(editor.nativeElement.querySelectorAll('form.operation-form')).toHaveLength(2);
    expect(editor.nativeElement.querySelectorAll('tbody tr')[0].querySelector('button')).not.toBeNull();
    expect(editor.nativeElement.querySelectorAll('tbody tr')[1].querySelector('button').disabled).toBe(false);
  });

  it('submits exact assignment, default/product/category rule, and status commands', () => {
    const component = TestBed.createComponent(UnassignedStockPageComponent).componentInstance as any;
    component.choose(page.rows[0]);
    const assignment = component.assignForm.getRawValue();
    expect(assignment).toEqual(expect.objectContaining({ productId: 1, warehouseId: 2, storageLocationId: 4, quantity: 4 }));
    expect(assignment.idempotencyKey.length).toBeGreaterThan(15);
    component.assign();
    expect(api.assign).toHaveBeenCalledWith(assignment);

    component.ruleForm.setValue({ warehouseId: 2, storageLocationId: 4, targetType: 'default', targetId: 0, priority: 10 });
    component.createRule();
    expect(api.createRule).toHaveBeenLastCalledWith({ warehouseId: 2, storageLocationId: 4, productId: undefined, productCategoryId: undefined, priority: 10 });
    component.ruleForm.setValue({ warehouseId: 2, storageLocationId: 4, targetType: 'product', targetId: 1, priority: 20 });
    component.createRule();
    expect(api.createRule).toHaveBeenLastCalledWith({ warehouseId: 2, storageLocationId: 4, productId: 1, productCategoryId: undefined, priority: 20 });
    component.ruleForm.setValue({ warehouseId: 2, storageLocationId: 4, targetType: 'category', targetId: 7, priority: 30 });
    component.createRule();
    expect(api.createRule).toHaveBeenLastCalledWith({ warehouseId: 2, storageLocationId: 4, productId: undefined, productCategoryId: 7, priority: 30 });
    component.toggle(8, false);
    expect(api.setRuleActive).toHaveBeenCalledWith(8, false);
  });

  it('uses the shared V4 hierarchy for the assignment form only', () => {
    const fixture = TestBed.createComponent(UnassignedStockPageComponent);
    fixture.detectChanges();
    const form = fixture.nativeElement.querySelector('form.assignment-form');
    expect(fixture.nativeElement.textContent).toContain('inventoryNavigation');
    expect(form.querySelectorAll('app-form-section')).toHaveLength(1);
    expect(form.querySelectorAll('app-field-cue')).toHaveLength(4);
    expect(form.querySelectorAll('app-workflow-footer')).toHaveLength(1);
  });
  it('uses the shared V4 hierarchy for default and targeted putaway rules', () => {
    const fixture = TestBed.createComponent(UnassignedStockPageComponent);
    fixture.detectChanges();
    const component = fixture.componentInstance as any;
    const form = () => fixture.nativeElement.querySelector('form.rule-form');
    expect(form().querySelectorAll('app-form-section')).toHaveLength(1);
    expect(form().querySelectorAll('app-field-cue')).toHaveLength(4);
    expect(form().querySelectorAll('app-workflow-footer')).toHaveLength(1);
    component.ruleForm.controls.targetType.setValue('product');
    fixture.detectChanges();
    expect(form().querySelectorAll('app-field-cue')).toHaveLength(5);
  });
  it('uses FilterBar cues and resets the exact server filter contract', () => {
    const fixture = TestBed.createComponent(UnassignedStockPageComponent);
    fixture.detectChanges();
    const component = fixture.componentInstance as any;
    expect(fixture.nativeElement.querySelectorAll('app-filter-bar')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-filter-bar app-field-cue')).toHaveLength(3);
    component.filter.setValue({ warehouseId: 2, query: 'coffee', sort: 'barcode' });
    component.clearFilters();
    expect(component.filter.getRawValue()).toEqual({ warehouseId: 0, query: '', sort: 'product' });
    expect(api.getUnassigned).toHaveBeenLastCalledWith(undefined, '', 'product');
  });
  it('blocks invalid commands, preserves server errors, and guards dirty work', () => {
    const component = TestBed.createComponent(UnassignedStockPageComponent).componentInstance as any;
    component.assignForm.patchValue({ productId: 0, warehouseId: 0, storageLocationId: 0, quantity: 0 });
    component.assign();
    expect(api.assign).not.toHaveBeenCalled();
    component.ruleForm.setValue({ warehouseId: 2, storageLocationId: 4, targetType: 'product', targetId: 0, priority: 10 });
    component.createRule();
    expect(api.createRule).not.toHaveBeenCalled();
    expect(component.problem()).toBe('fixFields');

    api.setRuleActive.mockReturnValue(throwError(() => ({ detail: 'Putaway rule was changed.' })));
    component.toggle(8, false);
    expect(component.problem()).toBe('Putaway rule was changed.');

    component.assignForm.markAsDirty();
    const confirm = vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    expect(component.canDeactivate()).toBe(false);
    expect(confirm).toHaveBeenCalledWith('unsaved');
    confirm.mockRestore();
  });
});
