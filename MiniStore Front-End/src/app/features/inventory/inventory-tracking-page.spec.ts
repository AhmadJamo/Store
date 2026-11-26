import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { InventoryTrackingApiService } from './data/inventory-tracking-api.service';
import { InventoryLocalizationService } from './inventory-localization.service';
import { InventoryTrackingPageComponent } from './inventory-tracking-page';

describe('InventoryTrackingPageComponent', () => {
  const permissions = new Set<string>();
  const workspace = { activeTab: vi.fn(() => ({ id: 'tracking' })), markDirty: vi.fn() };
  const page: any = {
    input: { productId: 0, policy: 1, sourceReference: '', lines: [] },
    products: [{ id: 1, name: 'Coffee' }],
    trackedProducts: [{ id: 2, name: 'Milk' }],
    warehouses: [{ id: 3, name: 'Main warehouse' }],
    locations: [{ id: 4, warehouseId: 3, name: 'Rack A' }],
    balances: [{ id: 8, product: 'Milk', warehouse: 'Main warehouse', location: 'Rack A', policy: 1, identifier: 'LOT-1', quantity: 2, expirationDate: '2026-12-01', status: 1, expirationState: 'Expiring soon', daysUntilExpiration: 5 }],
    transactions: [{ product: 'Milk', identifier: 'LOT-1', quantity: -1, type: 3, pickingStrategy: 1, reference: 'SALE-9', createdAt: '2026-11-26T10:00:00Z' }],
    expiredCount: 1,
    expiringSoonCount: 2,
    expirationAlerts: [{ product: 'Milk', warehouse: 'Main warehouse', location: 'Rack A', identifier: 'LOT-1', quantity: 2, expirationDate: '2026-12-01', daysUntilExpiration: 5, severity: 'Warning', status: 1 }],
    recalls: [{ id: 12, reference: 'REC-1', product: 'Milk', identifier: 'LOT-1', reason: 'Safety review', status: 1, createdAt: '2026-11-25T10:00:00Z', impacts: [{ documentType: 'Sale', reference: 'SALE-9', party: 'Customer A', contact: '0790000000', quantity: 1, lastActivityAt: '2026-11-26T10:00:00Z' }], communications: [{ partyName: 'Customer A', channelAddress: '0790000000', channel: 1, outcome: 2, notes: 'Reached customer', createdAt: '2026-11-26T11:00:00Z' }] }],
  };
  const api = {
    getPage: vi.fn<any>(() => of(page)), open: vi.fn<any>(() => of(void 0)),
    quarantine: vi.fn<any>(() => of(void 0)), release: vi.fn<any>(() => of(void 0)),
    createRecall: vi.fn<any>(() => of(void 0)), closeRecall: vi.fn<any>(() => of(void 0)),
    recordCommunication: vi.fn<any>(() => of(void 0)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    permissions.clear();
    ['InventoryTracking.View', 'InventoryTracking.Open', 'InventoryTracking.ManageQuarantine', 'InventoryTracking.ManageRecall'].forEach(permission => permissions.add(permission));
    api.getPage.mockReturnValue(of(page));
    api.open.mockReturnValue(of(void 0));
    api.quarantine.mockReturnValue(of(void 0));
    api.release.mockReturnValue(of(void 0));
    api.createRecall.mockReturnValue(of(void 0));
    api.closeRecall.mockReturnValue(of(void 0));
    api.recordCommunication.mockReturnValue(of(void 0));
    await TestBed.configureTestingModule({
      imports: [InventoryTrackingPageComponent],
      providers: [
        { provide: InventoryTrackingApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: any) => error } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored balance, expiration, recall, communication, and transaction evidence', () => {
    const fixture = TestBed.createComponent(InventoryTrackingPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    ['Milk', 'Main warehouse', 'Rack A', 'LOT-1', 'REC-1', 'Safety review', 'SALE-9', 'Customer A', 'Reached customer'].forEach(value => expect(text).toContain(value));
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip')).toHaveLength(1);
    const alerts = fixture.nativeElement.querySelector('app-data-table.expiration-alert-table');
    expect(alerts).not.toBeNull();
    expect(alerts.querySelectorAll('thead th')).toHaveLength(9);
    expect(fixture.nativeElement.querySelectorAll('.recalls app-detail-section')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('.recall-actions app-form-section')).toHaveLength(2);
    expect(fixture.nativeElement.querySelectorAll('.recall-actions app-field-cue')).toHaveLength(6);
    expect(fixture.nativeElement.querySelectorAll('.recall-actions app-workflow-footer')).toHaveLength(2);
    const balances = fixture.nativeElement.querySelector('app-data-table.tracking-balance-table');
    expect(balances).not.toBeNull();
    expect(balances.querySelectorAll('thead th')).toHaveLength(10);
    expect(balances.querySelectorAll('app-field-cue')).toHaveLength(1);
    const history = fixture.nativeElement.querySelector('app-data-table.tracking-history-table');
    expect(history).not.toBeNull();
    expect(history.querySelectorAll('thead th')).toHaveLength(6);
    const component = fixture.componentInstance as any;
    expect([1, 2].map(value => component.policyName(value))).toEqual(['lotTracking', 'serialTracking']);
    expect([1, 2, 3].map(value => component.statusName(value))).toEqual(['trackingAvailable', 'trackingDepleted', 'trackingQuarantined']);
    expect([1, 2, 3, 4, 5, 6, 7, 8].map(value => component.transactionName(value))).toEqual(['openingAllocation', 'receipt', 'issue', 'transfer', 'returnMovement', 'adjustment', 'quarantine', 'release']);
  });

  it('opens lot or serial tracking with the complete named allocation command', () => {
    const fixture = TestBed.createComponent(InventoryTrackingPageComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('inventoryNavigation');
    expect(fixture.nativeElement.querySelectorAll('.tracking-open-form app-form-section')).toHaveLength(2);
    expect(fixture.nativeElement.querySelectorAll('.tracking-open-form app-field-cue')).toHaveLength(9);
    expect(fixture.nativeElement.querySelectorAll('.tracking-open-form app-workflow-footer')).toHaveLength(1);
    const component = fixture.componentInstance as any;
    component.openForm.patchValue({ productId: 1, policy: 2, sourceReference: 'OPEN-1' });
    component.lines.at(0).patchValue({ warehouseId: 3, storageLocationId: 4, identifier: 'SER-1', quantity: 1, manufactureDate: '2026-01-01', expirationDate: '2027-01-01' });
    component.openTracking();
    expect(api.open).toHaveBeenCalledWith({ productId: 1, policy: 2, sourceReference: 'OPEN-1', lines: [{ warehouseId: 3, storageLocationId: 4, identifier: 'SER-1', quantity: 1, manufactureDate: '2026-01-01', expirationDate: '2027-01-01' }] });
    expect(api.getPage).toHaveBeenCalledTimes(2);
  });

  it('keeps open, quarantine, and recall surfaces independently permission gated', () => {
    permissions.clear();
    const none = TestBed.createComponent(InventoryTrackingPageComponent); none.detectChanges();
    expect(none.nativeElement.querySelectorAll('form.section-form')).toHaveLength(0);
    expect(none.nativeElement.querySelector('.inline-action')).toBeNull();

    permissions.add('InventoryTracking.Open');
    const open = TestBed.createComponent(InventoryTrackingPageComponent); open.detectChanges();
    expect(open.nativeElement.querySelectorAll('form.section-form')).toHaveLength(1);

    permissions.clear(); permissions.add('InventoryTracking.ManageRecall');
    const recall = TestBed.createComponent(InventoryTrackingPageComponent); recall.detectChanges();
    expect(recall.nativeElement.querySelectorAll('form.section-form')).toHaveLength(1);
    expect(recall.nativeElement.querySelectorAll('.recall-create-form app-form-section')).toHaveLength(1);
    expect(recall.nativeElement.querySelectorAll('.recall-create-form app-field-cue')).toHaveLength(4);
    expect(recall.nativeElement.querySelectorAll('.recall-create-form app-workflow-footer')).toHaveLength(1);
    expect(recall.nativeElement.textContent).toContain('recordCommunication');

    permissions.clear(); permissions.add('InventoryTracking.ManageQuarantine');
    const quarantine = TestBed.createComponent(InventoryTrackingPageComponent); quarantine.detectChanges();
    expect(quarantine.nativeElement.querySelector('.inline-action')).not.toBeNull();
  });

  it('submits exact recall, communication, quarantine, release, and close commands', () => {
    const component = TestBed.createComponent(InventoryTrackingPageComponent).componentInstance as any;
    component.recallForm.setValue({ reference: 'REC-2', productId: 2, identifier: 'LOT-2', reason: 'Safety' });
    component.createRecall();
    expect(api.createRecall).toHaveBeenCalledWith({ reference: 'REC-2', productId: 2, identifier: 'LOT-2', reason: 'Safety' });
    component.changeQuarantine(8, ' Quality hold ', true);
    component.changeQuarantine(8, ' Cleared ', false);
    expect(api.quarantine).toHaveBeenCalledWith(8, 'Quality hold');
    expect(api.release).toHaveBeenCalledWith(8, 'Cleared');
    component.communicate(12, ' Customer A ', ' 0790000000 ', '2', '3', ' Confirmed ');
    expect(api.recordCommunication).toHaveBeenCalledWith(12, { partyName: 'Customer A', channelAddress: '0790000000', channel: 2, outcome: 3, notes: 'Confirmed' });
    component.closeRecall(12, ' Completed ');
    expect(api.closeRecall).toHaveBeenCalledWith(12, 'Completed');
  });

  it('blocks invalid commands, preserves server errors, and guards dirty tracking work', () => {
    const component = TestBed.createComponent(InventoryTrackingPageComponent).componentInstance as any;
    component.changeQuarantine(8, 'x', true);
    component.communicate(12, '', '', '1', '1', '');
    component.closeRecall(12, '');
    expect(api.quarantine).not.toHaveBeenCalled();
    expect(api.recordCommunication).not.toHaveBeenCalled();
    expect(api.closeRecall).not.toHaveBeenCalled();
    expect(component.problem()).toBe('closureNotesRequired');

    api.quarantine.mockReturnValue(throwError(() => ({ detail: 'Tracked balance is already quarantined.' })));
    component.changeQuarantine(8, 'Quality hold', true);
    expect(component.problem()).toBe('Tracked balance is already quarantined.');

    component.openForm.controls.sourceReference.setValue('DIRTY');
    component.openForm.markAsDirty();
    const confirm = vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    expect(component.canDeactivate()).toBe(false);
    expect(confirm).toHaveBeenCalledWith('unsavedDescription');
    confirm.mockRestore();
  });
});