import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { PageContextService } from '../../shared/state/page-context.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { StockTransferApiService } from './data/stock-transfer-api.service';
import { StockTransferLocalizationService } from './stock-transfer-localization.service';
import { StockTransfersPageComponent } from './stock-transfers-page';

describe('StockTransfersPageComponent', () => {
  let routePath = 'inventory/transfers';
  let routeId: string | null = null;
  const permissions = new Set<string>();
  const row: any = {
    id: 41, transferNumber: 'TR-0041', fromWarehouseId: 1, fromWarehouseName: 'Source branch',
    toWarehouseId: 2, toWarehouseName: 'Destination branch', status: 1, createdByUserId: 'user-1',
    createdAt: '2026-11-20T08:00:00Z', reference: 'Restock', itemsCount: 1,
  };
  const details: any = {
    ...row, createdByUserName: 'Operations user', items: [{
      id: 501, productId: 7, productName: 'Roasted coffee', barcode: 'COF-7', quantity: 10,
      sourceLocationId: 11, destinationLocationId: 22, sourceLocationCode: 'SRC-A',
      destinationLocationCode: 'DST-A', receivedQuantity: 0, remainingQuantity: 10,
      reportedShortQuantity: 0, reportedDamagedQuantity: 0,
    }],
    history: [{ id: 1, fromStatus: 1, toStatus: 2, action: 1, userId: 'user-1', userName: 'Operations user', reason: 'Ready', createdAt: '2026-11-20T09:00:00Z' }],
  };
  const formPage: any = {
    form: { fromWarehouseId: 0, toWarehouseId: 0, reference: '', notes: '', items: [] },
    warehouses: [
      { id: 1, name: 'Source branch', controlMode: 2, requireSourceLocation: true, requireDestinationLocation: false },
      { id: 2, name: 'Destination branch', controlMode: 2, requireSourceLocation: false, requireDestinationLocation: true },
    ],
    products: [{ id: 7, name: 'Roasted coffee', barcode: 'COF-7', trackingPolicy: 0 }],
    locations: [
      { id: 11, warehouseId: 1, code: 'SRC-A', isPickable: true, isReceivable: false },
      { id: 22, warehouseId: 2, code: 'DST-A', isPickable: false, isReceivable: true },
    ],
  };
  const api = {
    getAll: vi.fn<any>(() => of([row])), getDetails: vi.fn<any>(() => of(details)),
    getCreateForm: vi.fn<any>(() => of(formPage)), getEditForm: vi.fn<any>(() => of({ ...formPage, id: 41, transferNumber: 'TR-0041' })),
    create: vi.fn<any>(() => of({ id: 41 })), update: vi.fn<any>(() => of(void 0)),
    submit: vi.fn<any>(() => of(void 0)), approve: vi.fn<any>(() => of(void 0)), reject: vi.fn<any>(() => of(void 0)),
    returnToDraft: vi.fn<any>(() => of(void 0)), dispatch: vi.fn<any>(() => of(void 0)),
    receive: vi.fn<any>(() => of(void 0)), cancel: vi.fn<any>(() => of(void 0)),
  };
  const context = {
    watch: vi.fn(() => of({ search: 'coffee', status: 0 })), replace: vi.fn(() => Promise.resolve(false)),
    saveDraft: vi.fn(), loadDraft: vi.fn(() => null), clearDraft: vi.fn(),
  };
  const workspace = { activeTab: vi.fn(() => ({ id: 'transfers' })), markDirty: vi.fn() };

  beforeEach(async () => {
    routePath = 'inventory/transfers';
    routeId = null;
    vi.clearAllMocks();
    permissions.clear();
    ['StockTransfers.View', 'StockTransfers.Create', 'StockTransfers.Edit', 'StockTransfers.Submit', 'StockTransfers.Approve', 'StockTransfers.Reject', 'StockTransfers.Dispatch', 'StockTransfers.Receive', 'StockTransfers.Cancel'].forEach(permission => permissions.add(permission));
    api.getAll.mockReturnValue(of([row]));
    api.getDetails.mockReturnValue(of(details));
    api.getCreateForm.mockReturnValue(of(formPage));
    context.loadDraft.mockReturnValue(null);
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    vi.spyOn(globalThis, 'prompt').mockReturnValue('Operational reason');
    await TestBed.configureTestingModule({
      imports: [StockTransfersPageComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => routeId }, routeConfig: { get path() { return routePath; } } } } },
        { provide: StockTransferApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: any) => error } },
        { provide: PageContextService, useValue: context },
        { provide: WorkspaceService, useValue: workspace },
        { provide: StockTransferLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored list evidence and preserves search, status, and create permission contracts', () => {
    const fixture = TestBed.createComponent(StockTransfersPageComponent);
    fixture.detectChanges();

    expect(api.getAll).toHaveBeenCalledWith('coffee', undefined);
    expect(fixture.nativeElement.textContent).toContain('TR-0041');
    expect(fixture.nativeElement.textContent).toContain('Source branch');
    expect(fixture.nativeElement.textContent).toContain('Destination branch');
    expect(fixture.nativeElement.querySelectorAll('app-data-table th')).toHaveLength(7);
    expect(fixture.nativeElement.querySelector('[primary-actions]')).not.toBeNull();
    const component = fixture.componentInstance as any;
    expect([1,2,3,4,5,6,7,8,9,10].map(status => component.statusName(status))).toEqual([
      'draft', 'submitted', 'approved', 'rejected', 'posted', 'cancelled', 'inTransit', 'partiallyReceived', 'receiptException', 'received',
    ]);
  });

  it('keeps named warehouses, required locations, distinct warehouses, exact create command, and dirty boundary', () => {
    routePath = 'inventory/transfers/new';
    const fixture = TestBed.createComponent(StockTransfersPageComponent);
    const component = fixture.componentInstance as any;
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('inventoryNavigation');
    expect(fixture.nativeElement.querySelectorAll('form.transfer-form app-form-section')).toHaveLength(2);
    expect(fixture.nativeElement.querySelectorAll('.line-card app-field-cue')).toHaveLength(5);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate');
    const line = component.items.at(0);
    component.form.patchValue({ fromWarehouseId: 1, toWarehouseId: 1, reference: 'Restock', notes: 'Branch demand' });
    line.patchValue({ productId: 7, quantity: 4, sourceLocationId: 11, destinationLocationId: 22, trackingAllocations: '' });
    component.save();
    expect(api.create).not.toHaveBeenCalled();
    expect(component.problem()).toBe('correctFields');

    component.form.controls.toWarehouseId.setValue(2);
    line.controls.destinationLocationId.setValue(22);
    component.form.markAsDirty();
    expect(component.canDeactivate()).toBe(true);
    component.save();

    expect(api.create).toHaveBeenCalledWith({
      fromWarehouseId: 1, toWarehouseId: 2, reference: 'Restock', notes: 'Branch demand',
      items: [{ productId: 7, quantity: 4, sourceLocationId: 11, destinationLocationId: 22, trackingAllocations: '' }],
    });
    expect(navigate).toHaveBeenCalledWith(['/inventory/transfers', 41]);
    expect(globalThis.confirm).toHaveBeenCalledWith('unsaved');
  });

  it('renders the exact action and receipt matrix for all ten server statuses', () => {
    routePath = 'inventory/transfers/:id';
    routeId = '41';
    const cases = [
      { status: 1, actions: ['edit', 'submit'], receipt: false },
      { status: 2, actions: ['approve', 'reject'], receipt: false },
      { status: 3, actions: ['post'], receipt: false },
      { status: 4, actions: ['returnToDraft'], receipt: false },
      { status: 5, actions: ['cancel'], receipt: false },
      { status: 6, actions: [], receipt: false },
      { status: 7, actions: [], receipt: true },
      { status: 8, actions: [], receipt: true },
      { status: 9, actions: [], receipt: true },
      { status: 10, actions: [], receipt: false },
    ];

    for (const testCase of cases) {
      api.getDetails.mockReturnValue(of({ ...details, status: testCase.status }));
      const fixture = TestBed.createComponent(StockTransfersPageComponent);
      fixture.detectChanges();
      const actions = [...fixture.nativeElement.querySelectorAll('app-workflow-footer a, app-workflow-footer button')]
        .map((element: Element) => element.textContent?.trim());
      expect(actions).toEqual(testCase.actions);
      expect(fixture.nativeElement.textContent.includes('receiptEvidence')).toBe(testCase.receipt);
      expect(fixture.nativeElement.querySelectorAll('app-workflow-progress')).toHaveLength(1);
      if (testCase.status === 1) {
        const component = fixture.componentInstance as any;
        expect(component.itemColumns()).toHaveLength(7);
        expect(component.historyColumns()).toHaveLength(5);
        expect(fixture.nativeElement.querySelectorAll('app-detail-section app-data-table th')).toHaveLength(12);
      }
      fixture.destroy();
    }
  });

  it('keeps every workflow action independently permission-gated', () => {
    routePath = 'inventory/transfers/:id';
    routeId = '41';
    api.getDetails.mockReturnValue(of({ ...details, status: 2 }));
    permissions.clear();
    const fixture = TestBed.createComponent(StockTransfersPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('app-workflow-footer a, app-workflow-footer button')).toHaveLength(0);
  });

  it('keeps submit, approve, reject, return, dispatch, and cancel as exact separate commands', () => {
    routePath = 'inventory/transfers/:id';
    routeId = '41';
    const component = TestBed.createComponent(StockTransfersPageComponent).componentInstance as any;
    component.submit();
    component.approve();
    component.reject();
    component.returnToDraft();
    component.post();
    component.cancel();

    expect(api.submit).toHaveBeenCalledWith(41);
    expect(api.approve).toHaveBeenCalledWith(41);
    expect(api.reject).toHaveBeenCalledWith(41, 'Operational reason');
    expect(api.returnToDraft).toHaveBeenCalledWith(41);
    expect(api.dispatch).toHaveBeenCalledWith(41);
    expect(api.cancel).toHaveBeenCalledWith(41, 'Operational reason');
  });

  it('blocks an empty receipt and submits only explicit received, short, and damaged evidence', () => {
    routePath = 'inventory/transfers/:id';
    routeId = '41';
    api.getDetails.mockReturnValue(of({ ...details, status: 7 }));
    const component = TestBed.createComponent(StockTransfersPageComponent).componentInstance as any;
    component.receive();
    expect(api.receive).not.toHaveBeenCalled();
    expect(component.problem()).toBe('correctFields');

    component.setReceiptValue(501, 'receivedQuantity', '6');
    component.setReceiptValue(501, 'shortQuantity', '1');
    component.setReceiptValue(501, 'damagedQuantity', '0.5');
    component.receive();

    expect(api.receive).toHaveBeenCalledWith(41, [{
      stockTransferItemId: 501, receivedQuantity: 6, shortQuantity: 1, damagedQuantity: 0.5, destinationLocationId: 22,
    }]);
  });
});
