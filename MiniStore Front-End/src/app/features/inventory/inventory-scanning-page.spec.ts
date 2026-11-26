import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { InventoryScanningApiService } from './data/inventory-scanning-api.service';
import { InventoryScanningLocalizationService } from './inventory-scanning-localization.service';
import { InventoryScanningPageComponent } from './inventory-scanning-page';

describe('InventoryScanningPageComponent', () => {
  const permissions = new Set<string>();
  const basePage: any = {
    query: { adjustmentScan: '', productScan: 'COF-7', sourceLocationScan: '', locationScan: 'DST-A' },
    product: { normalizedValue: 'COF-7', status: 1, entityId: 7, displayName: 'Roasted coffee', secondaryText: 'COF-7' },
    adjustment: { normalizedValue: '', status: 0, displayName: 'Waiting', secondaryText: '' },
    sourceLocation: { normalizedValue: '', status: 0, displayName: 'Waiting', secondaryText: '' },
    location: { normalizedValue: 'DST-A', status: 1, entityId: 22, displayName: 'Destination rack', secondaryText: 'Main warehouse' },
    balances: [{ warehouseName: 'Main warehouse', locationName: 'Destination rack', isUnassigned: false, onHand: 10, reserved: 2, available: 8 }],
    putawayIdempotencyKey: 'putaway-key-1', relocationIdempotencyKey: 'relocation-key-1', isBlindCount: false, canRecordCount: false,
  };
  const api = {
    resolve: vi.fn<any>(() => of(basePage)), putaway: vi.fn<any>(() => of(void 0)),
    relocate: vi.fn<any>(() => of(void 0)), count: vi.fn<any>(() => of(void 0)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    permissions.clear();
    ['ProductStock.Edit', 'LocationMovements.Create', 'InventoryAdjustments.Count'].forEach(permission => permissions.add(permission));
    api.resolve.mockReturnValue(of(basePage));
    api.putaway.mockReturnValue(of(void 0));
    api.relocate.mockReturnValue(of(void 0));
    api.count.mockReturnValue(of(void 0));
    await TestBed.configureTestingModule({
      imports: [InventoryScanningPageComponent],
      providers: [
        { provide: InventoryScanningApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: any) => error } },
        { provide: InventoryScanningLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-resolved scan identities, statuses, and balance evidence', () => {
    const fixture = TestBed.createComponent(InventoryScanningPageComponent);
    fixture.detectChanges();
    expect(api.resolve).toHaveBeenCalledWith({ adjustmentScan: '', productScan: '', sourceLocationScan: '', locationScan: '' });
    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('Destination rack');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('10');
    const component = fixture.componentInstance as any;
    expect([0, 1, 2, 3].map(status => component.statusName(status))).toEqual(['waiting', 'resolved', 'notFound', 'ambiguous']);
  });

  it('keeps putaway, relocation, and count independently permission and scan-state gated', () => {
    const component = TestBed.createComponent(InventoryScanningPageComponent).componentInstance as any;
    const relocation = { ...basePage, sourceLocation: { ...basePage.location, entityId: 11, displayName: 'Source rack' } };
    const count = { ...basePage, adjustment: { status: 1, entityLongId: 41, displayName: 'ADJ-41' }, canRecordCount: true };
    expect(component.canPutaway(basePage)).toBe(true);
    expect(component.canRelocate(relocation)).toBe(true);
    expect(component.canCount(count)).toBe(true);
    permissions.clear();
    expect(component.canPutaway(basePage)).toBe(false);
    expect(component.canRelocate(relocation)).toBe(false);
    expect(component.canCount(count)).toBe(false);
  });

  it('submits exact putaway, relocation, and count commands with server idempotency keys', () => {
    const component = TestBed.createComponent(InventoryScanningPageComponent).componentInstance as any;
    component.quantity.setValue(3);
    component.putaway(basePage);
    expect(api.putaway).toHaveBeenCalledWith({ productScan: 'COF-7', locationScan: 'DST-A', quantity: 3, idempotencyKey: 'putaway-key-1' });

    const relocation = { ...basePage, query: { ...basePage.query, sourceLocationScan: 'SRC-A' } };
    component.quantity.setValue(2);
    component.relocate(relocation);
    expect(api.relocate).toHaveBeenCalledWith({ productScan: 'COF-7', sourceLocationScan: 'SRC-A', destinationLocationScan: 'DST-A', quantity: 2, idempotencyKey: 'relocation-key-1' });

    const count = { ...basePage, query: { ...basePage.query, adjustmentScan: 'ADJ-41' }, canRecordCount: true };
    component.countedQuantity.setValue(0);
    component.count(count);
    expect(api.count).toHaveBeenCalledWith({ adjustmentNumber: 'ADJ-41', productScan: 'COF-7', locationScan: 'DST-A', countedQuantity: 0 });
  });

  it('blocks invalid operation quantities before issuing inventory commands', () => {
    const component = TestBed.createComponent(InventoryScanningPageComponent).componentInstance as any;
    component.quantity.setValue(0);
    component.putaway(basePage);
    component.relocate(basePage);
    component.countedQuantity.setValue(-1);
    component.count(basePage);
    expect(api.putaway).not.toHaveBeenCalled();
    expect(api.relocate).not.toHaveBeenCalled();
    expect(api.count).not.toHaveBeenCalled();
    expect(component.problem()).toBe('invalidQuantity');
  });

  it('hides balances for blind counts and preserves server-authored command errors', () => {
    api.resolve.mockReturnValue(of({ ...basePage, isBlindCount: true }));
    const fixture = TestBed.createComponent(InventoryScanningPageComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('blindCount');
    expect(fixture.nativeElement.querySelector('app-data-table')).toBeNull();

    api.putaway.mockReturnValue(throwError(() => ({ detail: 'Destination capacity exceeded.' })));
    const component = fixture.componentInstance as any;
    component.quantity.setValue(3);
    component.putaway(component.page());
    expect(component.problem()).toBe('Destination capacity exceeded.');
  });
});
