import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { InventoryAvailabilityApiService } from './data/inventory-availability-api.service';
import { InventoryAvailabilityPageComponent } from './inventory-availability-page';
import { InventoryLocalizationService } from './inventory-localization.service';

describe('InventoryAvailabilityPageComponent', () => {
  const page = {
    items: [{ productName: 'Roasted coffee', productCode: 'COF-1', warehouseId: 3, warehouseName: 'Main warehouse', locationName: 'A-01', isUnassigned: false, onHand: 12, reserved: 2, available: 10 }],
    warehouses: [{ id: 3, name: 'Main warehouse' }],
    totalCount: 1,
    page: 1,
    pageSize: 50,
    totalPages: 1,
  };
  const api = { getBalances: vi.fn(() => of(page)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getBalances.mockReturnValue(of(page));
    await TestBed.configureTestingModule({
      imports: [InventoryAvailabilityPageComponent],
      providers: [
        { provide: InventoryAvailabilityApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored product, warehouse, location, and quantity evidence', () => {
    const fixture = TestBed.createComponent(InventoryAvailabilityPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('COF-1');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('A-01');
    expect(fixture.nativeElement.querySelectorAll('app-page-header')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('.filters')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-data-table')).toHaveLength(1);
  });

  it('keeps warehouse filtering and paging on the server contract', () => {
    const component = TestBed.createComponent(InventoryAvailabilityPageComponent).componentInstance as any;
    component.filterWarehouse('3');
    component.move(1);

    expect(api.getBalances).toHaveBeenLastCalledWith(expect.objectContaining({ warehouseId: 3, page: 1, pageSize: 50 }));
  });

  it('debounces trimmed product search into the server query', () => {
    vi.useFakeTimers();
    try {
      const component = TestBed.createComponent(InventoryAvailabilityPageComponent).componentInstance as any;
      component.search('  coffee  ');
      vi.advanceTimersByTime(300);

      expect(api.getBalances).toHaveBeenLastCalledWith({ search: 'coffee', page: 1, pageSize: 50 });
    } finally {
      vi.useRealTimers();
    }
  });

  it('resets warehouse, search, page size, and page to the bounded defaults', () => {
    const component = TestBed.createComponent(InventoryAvailabilityPageComponent).componentInstance as any;
    component.query = { warehouseId: 3, search: 'coffee', page: 2, pageSize: 100 };
    component.clearFilters();

    expect(api.getBalances).toHaveBeenLastCalledWith({ page: 1, pageSize: 50 });
  });

  it('keeps the empty result recoverable without inventing client balances', () => {
    api.getBalances.mockReturnValue(of({ ...page, items: [], totalCount: 0 }));
    const fixture = TestBed.createComponent(InventoryAvailabilityPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('noBalances');
    expect(fixture.nativeElement.querySelector('app-data-table')).toBeNull();
  });
});