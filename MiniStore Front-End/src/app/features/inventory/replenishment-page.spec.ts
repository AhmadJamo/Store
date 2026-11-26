import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ReplenishmentApiService } from './data/replenishment-api.service';
import { ReplenishmentLocalizationService } from './replenishment-localization.service';
import { ReplenishmentPageComponent } from './replenishment-page';

describe('ReplenishmentPageComponent', () => {
  const permissions = new Set<string>();
  const query = new Map<string, string>();
  const workspace = { activeTab: vi.fn(() => ({ id: 'replenishment' })), markDirty: vi.fn() };
  const page: any = {
    rows: [
      { ruleId: 7, productId: 1, warehouseId: 2, product: 'Coffee', warehouse: 'Main', sourceWarehouse: 'Source', onHand: 11, reserved: 2, available: 9, confirmedIncoming: 1, confirmedOutgoing: 3, projectedOnHand: 12, projectedAvailable: 10, minimum: 12, maximum: 20, safetyStock: 4, leadTimeDays: 3, planningMethod: 1, coverageDays: 30, useSeasonality: false, suggestedQuantity: 10, sourceAvailable: 30, isActive: true, needsReplenishment: true },
      { ruleId: 8, productId: 2, warehouseId: 2, product: 'Milk', warehouse: 'Main', onHand: 2, reserved: 1, available: 1, confirmedIncoming: 0, confirmedOutgoing: 0, projectedOnHand: 2, projectedAvailable: 1, minimum: 4, maximum: 8, safetyStock: 1, leadTimeDays: 5, planningMethod: 2, coverageDays: 14, useSeasonality: true, suggestedQuantity: 7, isActive: true, needsReplenishment: true },
      { ruleId: 9, productId: 3, warehouseId: 2, product: 'Tea', warehouse: 'Main', onHand: 5, reserved: 0, available: 5, confirmedIncoming: 2, confirmedOutgoing: 1, projectedOnHand: 7, projectedAvailable: 7, minimum: 3, maximum: 9, safetyStock: 1, leadTimeDays: 2, planningMethod: 1, coverageDays: 30, useSeasonality: false, suggestedQuantity: 2, openPurchaseRequestId: 44, openPurchaseRequestNumber: 'PRQ-44', isActive: false, needsReplenishment: false },
    ],
    products: [{ id: 1, name: 'Coffee' }, { id: 2, name: 'Milk' }],
    warehouses: [{ id: 2, name: 'Main' }, { id: 3, name: 'Source' }],
    search: '',
  };
  const api = {
    getPage: vi.fn<any>(() => of(page)),
    saveRule: vi.fn<any>(() => of(void 0)),
    setActive: vi.fn<any>(() => of(void 0)),
    createTransferDraft: vi.fn<any>(() => of({ id: 9 })),
    createPurchaseRequestDraft: vi.fn<any>(() => of({ id: 10 })),
  };
  const router = { navigate: vi.fn<any>(() => Promise.resolve(true)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    permissions.clear();
    ['Inventory.Replenishment.View', 'Inventory.Replenishment.Manage', 'StockTransfers.Create', 'PurchaseRequests.Create'].forEach(permission => permissions.add(permission));
    query.clear();
    api.getPage.mockReturnValue(of(page));
    api.saveRule.mockReturnValue(of(void 0));
    api.setActive.mockReturnValue(of(void 0));
    api.createTransferDraft.mockReturnValue(of({ id: 9 }));
    api.createPurchaseRequestDraft.mockReturnValue(of({ id: 10 }));
    router.navigate.mockReturnValue(Promise.resolve(true));
    await TestBed.configureTestingModule({
      imports: [ReplenishmentPageComponent],
      providers: [
        { provide: ReplenishmentApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: any) => error } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: ReplenishmentLocalizationService, useValue: { text: (key: string) => key } },
        { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: { get: (key: string) => query.get(key) ?? null } } } },
        { provide: Router, useValue: router },
      ],
    }).compileComponents();
  });

  it('renders only named server-authored replenishment and generated-demand evidence', () => {
    const fixture = TestBed.createComponent(ReplenishmentPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    ['Coffee', 'Milk', 'Tea', 'Main', 'Source', 'PRQ-44', '9', '30', '12', '20', '10'].forEach(value => expect(text).toContain(value));
    expect(fixture.componentInstance['page']()?.rows[0]).toEqual(expect.objectContaining({ onHand: 11, reserved: 2, available: 9, confirmedIncoming: 1, confirmedOutgoing: 3, projectedOnHand: 12, projectedAvailable: 10, minimum: 12, maximum: 20, suggestedQuantity: 10, sourceAvailable: 30 }));
    expect(fixture.nativeElement.querySelectorAll('tbody tr')).toHaveLength(3);
    expect(fixture.nativeElement.querySelectorAll('thead th')).toHaveLength(11);
    expect(fixture.nativeElement.textContent).toContain('inventoryNavigation');
    expect(fixture.nativeElement.querySelectorAll('.rule-form app-form-section')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('.rule-form app-field-cue')).toHaveLength(10);
    expect(fixture.nativeElement.querySelectorAll('.rule-form app-workflow-footer')).toHaveLength(1);
    expect(fixture.nativeElement.querySelector('a')).not.toBeNull();
  });

  it('preserves warehouse, search, exception and reset query contracts', async () => {
    query.set('warehouseId', '2'); query.set('search', 'milk'); query.set('exceptionsOnly', 'true');
    const component = TestBed.createComponent(ReplenishmentPageComponent).componentInstance as any;
    expect(api.getPage).toHaveBeenLastCalledWith({ warehouseId: 2, search: 'milk', exceptionsOnly: true });
    component.filter.setValue({ warehouseId: 3, search: ' Coffee ', exceptionsOnly: false });
    component.applyFilters();
    await Promise.resolve();
    expect(router.navigate).toHaveBeenCalledWith([], { relativeTo: expect.anything(), queryParams: { warehouseId: 3, search: 'Coffee', exceptionsOnly: null } });
    expect(api.getPage).toHaveBeenLastCalledWith({ warehouseId: 3, search: ' Coffee ', exceptionsOnly: false });
    component.clearFilters();
    await Promise.resolve();
    expect(router.navigate).toHaveBeenLastCalledWith([], { relativeTo: expect.anything(), queryParams: { warehouseId: null, search: null, exceptionsOnly: null } });
  });

  it('keeps rule, transfer, purchase-request, and existing-document actions independently gated', () => {
    permissions.clear(); permissions.add('Inventory.Replenishment.View');
    const viewOnly = TestBed.createComponent(ReplenishmentPageComponent); viewOnly.detectChanges();
    expect(viewOnly.nativeElement.querySelector('.rule-form')).toBeNull();
    expect(viewOnly.nativeElement.textContent).toContain('PRQ-44');
    expect(viewOnly.nativeElement.textContent).not.toContain('createTransfer');
    expect(viewOnly.nativeElement.textContent).not.toContain('createPurchaseRequest');

    permissions.add('Inventory.Replenishment.Manage'); permissions.add('StockTransfers.Create');
    const transfer = TestBed.createComponent(ReplenishmentPageComponent); transfer.detectChanges();
    expect(transfer.nativeElement.querySelector('.rule-form')).not.toBeNull();
    expect(transfer.nativeElement.textContent).toContain('createTransfer');
    expect(transfer.nativeElement.textContent).not.toContain('createPurchaseRequest');

    permissions.delete('StockTransfers.Create'); permissions.add('PurchaseRequests.Create');
    const purchase = TestBed.createComponent(ReplenishmentPageComponent); purchase.detectChanges();
    expect(purchase.nativeElement.textContent).not.toContain('createTransfer');
    expect(purchase.nativeElement.textContent).toContain('createPurchaseRequest');
  });

  it('submits exact rule, status, and generated-document commands', () => {
    const component = TestBed.createComponent(ReplenishmentPageComponent).componentInstance as any;
    component.form.setValue({ productId: 1, warehouseId: 2, preferredSourceWarehouseId: 3, planningMethod: 1, minimumQuantity: 2, maximumQuantity: 8, safetyStock: 1, leadTimeDays: 3, coverageDays: 30, useSeasonality: false });
    component.save();
    expect(api.saveRule).toHaveBeenCalledWith({ productId: 1, warehouseId: 2, preferredSourceWarehouseId: 3, planningMethod: 1, minimumQuantity: 2, maximumQuantity: 8, safetyStock: 1, leadTimeDays: 3, coverageDays: 30, useSeasonality: false });
    component.toggle(7, false);
    expect(api.setActive).toHaveBeenCalledWith(7, false);
    component.createTransfer(7);
    expect(api.createTransferDraft).toHaveBeenCalledWith(7);
    expect(router.navigate).toHaveBeenCalledWith(['/inventory/transfers', 9]);
    component.createPurchaseRequest(8);
    expect(api.createPurchaseRequestDraft).toHaveBeenCalledWith(8);
    expect(router.navigate).toHaveBeenCalledWith(['/purchases/requests', 10]);
  });

  it('blocks invalid rules, preserves server errors, and guards dirty work', () => {
    const component = TestBed.createComponent(ReplenishmentPageComponent).componentInstance as any;
    component.form.patchValue({ productId: 1, warehouseId: 2, preferredSourceWarehouseId: 2, minimumQuantity: 8, maximumQuantity: 2 });
    component.save();
    expect(api.saveRule).not.toHaveBeenCalled();
    expect(component.problem()).toBe('fixFields');

    api.setActive.mockReturnValue(throwError(() => ({ detail: 'Rule is already inactive.' })));
    component.toggle(7, false);
    expect(component.problem()).toBe('Rule is already inactive.');

    component.form.markAsDirty();
    const confirm = vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    expect(component.canDeactivate()).toBe(false);
    expect(confirm).toHaveBeenCalledWith('unsaved');
    expect(workspace.markDirty).toHaveBeenCalled();
    confirm.mockRestore();
  });
});
