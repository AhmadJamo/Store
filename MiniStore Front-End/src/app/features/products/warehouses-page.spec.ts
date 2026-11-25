import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { WarehouseApiService } from './data/warehouse-api.service';
import { WarehousesPageComponent } from './warehouses-page';
import { ProductLocalizationService } from './product-localization.service';

describe('WarehousesPageComponent', () => {
  const warehouse: any = {
    id: 2, name: 'Main', branchId: 1, branchName: 'Amman', inventoryAccountId: 3, inventoryAccountName: 'Inventory',
    type: 1, controlMode: 3, pickingStrategy: 4, allowPosSales: true, enforceLocationCapacity: true,
    requireSourceLocationForTransfers: false, requireDestinationLocationForTransfers: false, purchaseReceivingRoute: 1,
  };
  const form: any = {
    form: { ...warehouse, id: 0, name: '' },
    branches: [{ id: 1, code: 'AMM', name: 'Amman', isActive: true }],
    accounts: [{ id: 3, code: '1100', name: 'Inventory', type: 1, parentAccountId: null }],
  };
  const api = {
    getAll: vi.fn(() => of([warehouse])), getForm: vi.fn(() => of(form)), create: vi.fn(), update: vi.fn(), delete: vi.fn(),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getAll.mockReturnValue(of([warehouse]));
    api.getForm.mockReturnValue(of(form));
    await TestBed.configureTestingModule({
      imports: [WarehousesPageComponent],
      providers: [
        { provide: WarehouseApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: WorkspaceService, useValue: { activeTab: () => ({ id: 'warehouses' }), markDirty: vi.fn() } },
        { provide: ProductLocalizationService, useValue: { text: (key: string) => key } },
        provideRouter([]),
      ],
    }).compileComponents();
  });

  it('creates a warehouse with all operation settings', () => {
    api.create.mockReturnValue(of(warehouse));
    const component = TestBed.createComponent(WarehousesPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Main', branchId: 1, inventoryAccountId: 3 });
    component.save();
    expect(api.create).toHaveBeenCalledWith(expect.objectContaining({ name: 'Main', branchId: 1, inventoryAccountId: 3, controlMode: 3 }));
  });

  it('preserves a warehouse when delete is rejected', () => {
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    api.delete.mockReturnValue(throwError(() => ({ detail: 'Warehouse is in use' })));
    const component = TestBed.createComponent(WarehousesPageComponent).componentInstance as any;
    component.delete(warehouse);
    expect(component.problem()).toBe('Warehouse is in use');
    expect(component.items()).toContain(warehouse);
  });

  it('preserves a rejected edit', () => {
    api.update.mockReturnValue(throwError(() => ({ detail: 'Cannot change' })));
    const component = TestBed.createComponent(WarehousesPageComponent).componentInstance as any;
    component.editingId.set(2);
    component.form.patchValue({ name: 'Main', branchId: 1, inventoryAccountId: 3 });
    component.form.markAsDirty();
    component.save();
    expect(component.problem()).toBe('Cannot change');
    expect(component.form.controls.name.value).toBe('Main');
  });

  it('uses the shared warehouse form and typed table hierarchy', () => {
    const fixture = TestBed.createComponent(WarehousesPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('INVENTORY');
    expect(fixture.nativeElement.querySelectorAll('app-form-section')).toHaveLength(3);
    expect(fixture.nativeElement.querySelectorAll('app-workflow-footer')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-data-table table tbody tr')).toHaveLength(1);
    expect(fixture.nativeElement.querySelector('.danger-action')?.textContent).toContain('deleteWarehouse');
    expect(fixture.nativeElement.textContent).toContain('Amman');
    expect(fixture.nativeElement.textContent).toContain('Inventory');
  });
});