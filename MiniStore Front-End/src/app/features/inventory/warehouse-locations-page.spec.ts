import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { WarehouseOperationsApiService } from './data/warehouse-operations-api.service';
import { WarehouseLocationsPageComponent } from './warehouse-locations-page';
import { WarehouseOperationsLocalizationService } from './warehouse-operations-localization.service';

describe('WarehouseLocationsPageComponent', () => {
  const page = {
    result: {
      locations: [
        { id: 1, warehouseId: 7, code: 'ROOT', name: 'Main floor', type: 1, status: 1, isReceivable: true, isPickable: true, isReservable: true, isShippable: false, isCountable: true },
        { id: 2, warehouseId: 7, parentLocationId: 1, code: 'A-01', name: 'Rack A', type: 1, status: 1, isReceivable: false, isPickable: true, isReservable: true, isShippable: true, isCountable: true },
      ],
      locationDepths: { 1: 0, 2: 1 },
      products: [],
    },
    options: {
      warehouses: [{ id: 7, name: 'Main warehouse' }],
      parentLocations: [{ id: 1, warehouseId: 7, label: 'ROOT — Main floor' }],
    },
  };
  const api = {
    getLocations: vi.fn(() => of(page)),
    getLocationForm: vi.fn(() => of({
      input: { id: 2, warehouseId: 7, code: 'A-01', name: 'Rack A', barcode: undefined, parentLocationId: 1, sequence: 10, isReceivable: false, isPickable: true, isReservable: true, isShippable: true, isCountable: true },
      parentLocations: [{ id: 1, warehouseId: 7, label: 'ROOT — Main floor' }],
    })),
    createLocation: vi.fn(() => of(void 0)),
    updateLocation: vi.fn(() => of(void 0)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getLocations.mockReturnValue(of(page));
    api.createLocation.mockReturnValue(of(void 0));
    api.updateLocation.mockReturnValue(of(void 0));
    await TestBed.configureTestingModule({
      imports: [WarehouseLocationsPageComponent],
      providers: [
        { provide: WarehouseOperationsApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: () => true, session: () => ({ culture: 'en-US' }) } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: WorkspaceService, useValue: { activeTab: () => ({ id: 'locations' }), markDirty: vi.fn() } },
        WarehouseOperationsLocalizationService,
      ],
    }).compileComponents();
  });

  it('renders warehouse names and the complete location hierarchy', () => {
    const fixture = TestBed.createComponent(WarehouseLocationsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('Main floor');
    expect(fixture.nativeElement.textContent).toContain('Rack A');
    expect(fixture.nativeElement.querySelectorAll('app-data-table tbody tr')).toHaveLength(2);
  });

  it('creates a location with the selected warehouse ID and all physical fields', () => {
    const component = TestBed.createComponent(WarehouseLocationsPageComponent).componentInstance as any;
    component.form.patchValue({ warehouseId: 7, code: ' A-02 ', name: ' Rack B ', zone: ' Z1 ', aisle: ' A ', rack: ' 02 ', level: ' 1 ', bin: ' B ', maximumQuantity: 25 });
    component.save();

    expect(api.createLocation).toHaveBeenCalledWith(expect.objectContaining({
      warehouseId: 7, code: 'A-02', name: 'Rack B', zone: 'Z1', aisle: 'A', rack: '02', level: '1', bin: 'B', maximumQuantity: 25,
    }));
  });

  it('loads an edit form and preserves the parent relationship on update', () => {
    const component = TestBed.createComponent(WarehouseLocationsPageComponent).componentInstance as any;
    component.beginEdit(2);
    component.form.patchValue({ name: 'Rack A updated' });
    component.save();

    expect(api.getLocationForm).toHaveBeenCalledWith(2);
    expect(api.updateLocation).toHaveBeenCalledWith(expect.objectContaining({ id: 2, warehouseId: 7, parentLocationId: 1, name: 'Rack A updated' }));
  });
});
