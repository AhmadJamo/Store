import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentFormPageComponent } from './inventory-adjustment-form-page';
import { InventoryLocalizationService } from './inventory-localization.service';

describe('InventoryAdjustmentFormPageComponent', () => {
  const page = {
    input: { warehouseId: 2, storageLocationId: 21, isBlindCount: false, reason: '', productIds: [] },
    warehouses: [{ id: 2, name: 'Main warehouse' }, { id: 3, name: 'Reserve warehouse' }],
    locations: [
      { id: 21, warehouseId: 2, name: 'A-01 — Coffee rack' },
      { id: 31, warehouseId: 3, name: 'B-01 — Reserve rack' },
    ],
    products: [{ id: 7, name: 'Roasted coffee' }, { id: 8, name: 'Green coffee' }],
  };
  const api = { getCreateForm: vi.fn(() => of(page)), create: vi.fn(() => of({ id: 44 })) };
  const workspace = { activeTab: vi.fn(() => ({ id: 'adjustment-create' })), markDirty: vi.fn() };
  const focus = { focusFirstInvalid: vi.fn() };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getCreateForm.mockReturnValue(of(page));
    api.create.mockReturnValue(of({ id: 44 }));
    vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    await TestBed.configureTestingModule({
      imports: [InventoryAdjustmentFormPageComponent],
      providers: [
        { provide: InventoryAdjustmentApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: () => ({ detail: 'Save failed.' }) } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: FormFocusService, useValue: focus },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
        provideRouter([]),
      ],
    }).compileComponents();
  });

  it('renders server-authored named warehouses, countable locations, and stocked products', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentFormPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('Reserve warehouse');
    expect(fixture.nativeElement.textContent).toContain('A-01 — Coffee rack');
    expect(fixture.nativeElement.textContent).not.toContain('B-01 — Reserve rack');
    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('Green coffee');
  });

  it('filters named locations by warehouse and clears a stale location selection', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentFormPageComponent);
    const component = fixture.componentInstance as any;
    fixture.detectChanges();

    component.form.controls.warehouseId.setValue(3);
    fixture.detectChanges();

    expect(component.form.controls.storageLocationId.value).toBeNull();
    expect(component.locations()).toEqual([{ id: 31, warehouseId: 3, name: 'B-01 — Reserve rack' }]);
    expect(fixture.nativeElement.textContent).toContain('B-01 — Reserve rack');
    expect(fixture.nativeElement.textContent).not.toContain('A-01 — Coffee rack');
  });

  it('submits the exact named-choice IDs, blind-count flag, reason, and selected products then opens the record', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentFormPageComponent);
    const component = fixture.componentInstance as any;
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.detectChanges();

    component.form.setValue({ warehouseId: 2, storageLocationId: 21, isBlindCount: true, reason: 'Monthly cycle count' });
    component.toggleProduct(7, true);
    component.toggleProduct(8, true);
    component.save();

    expect(api.create).toHaveBeenCalledWith({ warehouseId: 2, storageLocationId: 21, isBlindCount: true, reason: 'Monthly cycle count', productIds: [7, 8] });
    expect(workspace.markDirty).toHaveBeenLastCalledWith('adjustment-create', false);
    expect(navigate).toHaveBeenCalledWith(['/inventory/adjustments', 44]);
  });

  it('blocks incomplete input, exposes localized guidance, and focuses the first invalid field', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentFormPageComponent);
    const component = fixture.componentInstance as any;
    fixture.detectChanges();

    component.form.setValue({ warehouseId: 0, storageLocationId: null, isBlindCount: false, reason: '' });
    component.save();
    fixture.detectChanges();

    expect(api.create).not.toHaveBeenCalled();
    expect(component.problem()).toBe('correctInventoryCountFields');
    expect(focus.focusFirstInvalid).toHaveBeenCalled();
  });

  it('marks product edits dirty and keeps the unsaved-leave confirmation boundary', () => {
    const component = TestBed.createComponent(InventoryAdjustmentFormPageComponent).componentInstance as any;

    component.toggleProduct(7, true);
    expect(workspace.markDirty).toHaveBeenLastCalledWith('adjustment-create', true);
    expect(component.canDeactivate()).toBe(false);
    expect(globalThis.confirm).toHaveBeenCalledWith('unsavedDescription');
  });
});
