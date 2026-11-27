import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { WarehouseOperationsApiService } from './data/warehouse-operations-api.service';
import { LocationMovementsPageComponent } from './location-movements-page';
import { WarehouseOperationsLocalizationService } from './warehouse-operations-localization.service';

describe('LocationMovementsPageComponent', () => {
  const permissions = new Set<string>();
  const workspace = { activeTab: vi.fn(() => ({ id: 'location-movements' })), markDirty: vi.fn() };
  const page: any = {
    warehouseFilterId: 2,
    query: 'coffee',
    movements: [{ id: 11, productName: 'Coffee', barcode: 'COF-1', warehouseName: 'Main warehouse', fromLocationCode: 'A-01', toLocationCode: 'A-02', quantity: 3, type: 2, reference: 'MOVE-11', notes: 'Re-slot', createdByUserId: 'user-1', createdAt: '2026-11-27T08:00:00Z' }],
    warehouses: [{ id: 2, name: 'Main warehouse' }],
    products: [{ id: 1, name: 'Coffee', barcode: 'COF-1' }],
    locations: [{ id: 4, warehouseId: 2, code: 'A-01' }, { id: 5, warehouseId: 2, code: 'A-02' }],
    sourceStocks: [{ productId: 1, warehouseId: 2, storageLocationId: 4, quantity: 9 }],
  };
  const ledger: any = {
    rows: [{ id: 21, locationMovementId: 11, productName: 'Coffee', warehouseName: 'Main warehouse', fromLocation: 'A-01', toLocation: 'A-02', quantity: 3, type: 2, status: 2, reference: 'MOVE-11', createdAt: '2026-11-27T08:00:00Z', postedAt: '2026-11-27T08:00:01Z', sourceDocumentType: 'LocationMovement', sourceDocumentId: 11, stageSequence: 1 }],
    legacyMovementCount: 4, linkedMovementCount: 1, totalMovementCount: 5, isReconciled: true,
  };
  const api = {
    getLocationMovements: vi.fn<any>(() => of(page)),
    getStockMovements: vi.fn<any>(() => of(ledger)),
    move: vi.fn<any>(() => of(void 0)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    permissions.clear();
    ['LocationMovements.View', 'LocationMovements.Create', 'StockMovements.View'].forEach(x => permissions.add(x));
    api.getLocationMovements.mockReturnValue(of(page));
    api.getStockMovements.mockReturnValue(of(ledger));
    api.move.mockReturnValue(of(void 0));
    await TestBed.configureTestingModule({
      imports: [LocationMovementsPageComponent],
      providers: [
        { provide: WarehouseOperationsApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: any) => error } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: WarehouseOperationsLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored movement, ledger, reconciliation, and named-option evidence', () => {
    const fixture = TestBed.createComponent(LocationMovementsPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    ['Coffee', 'COF-1', 'Main warehouse', 'A-01', 'A-02', 'MOVE-11', '4', '1', '5', 'reconciled'].forEach(value => expect(text).toContain(value));
    const component = fixture.componentInstance as any;
    component.form.patchValue({ warehouseId: 2, productId: 1, fromStorageLocationId: 4 });
    expect(component.sourceQuantity()).toBe(9);
    expect(component.page()?.products[0]).toEqual(expect.objectContaining({ name: 'Coffee', barcode: 'COF-1' }));
    expect(component.ledger()?.rows[0]).toEqual(expect.objectContaining({ locationMovementId: 11, sourceDocumentId: 11, stageSequence: 1 }));
  });

  it('preserves warehouse/query requests and independent read permissions', () => {
    const component = TestBed.createComponent(LocationMovementsPageComponent).componentInstance as any;
    expect(api.getLocationMovements).toHaveBeenLastCalledWith(undefined, '');
    expect(api.getStockMovements).toHaveBeenCalledTimes(1);
    component.filter.setValue({ warehouseId: 2, query: 'coffee' });
    component.load();
    expect(api.getLocationMovements).toHaveBeenLastCalledWith(2, 'coffee');

    permissions.delete('StockMovements.View');
    const operationalOnly = TestBed.createComponent(LocationMovementsPageComponent).componentInstance as any;
    expect(operationalOnly.ledger()).toBeNull();
    permissions.add('StockMovements.View');
    permissions.delete('LocationMovements.View');
    const ledgerOnly = TestBed.createComponent(LocationMovementsPageComponent).componentInstance as any;
    expect(ledgerOnly.page()).toBeNull();
    expect(ledgerOnly.ledger()).toEqual(ledger);
  });

  it('keeps the movement form gated by LocationMovements.Create', () => {
    permissions.delete('LocationMovements.Create');
    const viewOnly = TestBed.createComponent(LocationMovementsPageComponent); viewOnly.detectChanges();
    expect(viewOnly.nativeElement.querySelector('form.operation-form')).toBeNull();
    permissions.add('LocationMovements.Create');
    const creator = TestBed.createComponent(LocationMovementsPageComponent); creator.detectChanges();
    expect(creator.nativeElement.querySelector('form.operation-form')).not.toBeNull();
  });

  it('submits the exact relocation payload with a fresh idempotency key', () => {
    const component = TestBed.createComponent(LocationMovementsPageComponent).componentInstance as any;
    component.form.setValue({ idempotencyKey: '1234567890123456', warehouseId: 2, productId: 1, fromStorageLocationId: 4, toStorageLocationId: 5, quantity: 3, reference: ' MOVE-12 ', notes: ' Re-slot ' });
    component.move();
    expect(api.move).toHaveBeenCalledWith({ idempotencyKey: '1234567890123456', warehouseId: 2, productId: 1, fromStorageLocationId: 4, toStorageLocationId: 5, quantity: 3, reference: 'MOVE-12', notes: 'Re-slot' });
    expect(component.form.controls.idempotencyKey.value.length).toBeGreaterThan(15);
    expect(component.form.pristine).toBe(true);
  });

  it('uses the shared V4 hierarchy for the relocation form only', () => {
    const fixture = TestBed.createComponent(LocationMovementsPageComponent);
    fixture.detectChanges();
    const form = fixture.nativeElement.querySelector('form.movement-form');
    expect(fixture.nativeElement.textContent).toContain('inventoryNavigation');
    expect(form.querySelectorAll('app-form-section')).toHaveLength(1);
    expect(form.querySelectorAll('app-field-cue')).toHaveLength(7);
    expect(form.querySelectorAll('app-workflow-footer')).toHaveLength(1);
  });
  it('uses FilterBar cues and resets the exact movement query contract', () => {
    const fixture = TestBed.createComponent(LocationMovementsPageComponent);
    fixture.detectChanges();
    const component = fixture.componentInstance as any;
    expect(fixture.nativeElement.querySelectorAll('app-filter-bar')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-filter-bar app-field-cue')).toHaveLength(2);
    component.filter.setValue({ warehouseId: 2, query: 'coffee' });
    component.clearFilters();
    expect(component.filter.getRawValue()).toEqual({ warehouseId: 0, query: '' });
    expect(api.getLocationMovements).toHaveBeenLastCalledWith(undefined, '');
  });
  it('blocks invalid moves, preserves server errors, and guards dirty work', () => {
    const component = TestBed.createComponent(LocationMovementsPageComponent).componentInstance as any;
    component.form.patchValue({ warehouseId: 2, productId: 1, fromStorageLocationId: 4, toStorageLocationId: 4, quantity: 3 });
    component.move();
    expect(api.move).not.toHaveBeenCalled();
    expect(component.problem()).toBe('fixFields');

    api.move.mockReturnValue(throwError(() => ({ detail: 'Destination capacity exceeded.' })));
    component.form.patchValue({ toStorageLocationId: 5 });
    component.move();
    expect(component.problem()).toBe('Destination capacity exceeded.');

    component.form.markAsDirty();
    const confirm = vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    expect(component.canDeactivate()).toBe(false);
    expect(confirm).toHaveBeenCalledWith('unsaved');
    confirm.mockRestore();
  });
});
