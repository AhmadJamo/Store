import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { InventoryAvailabilityApiService } from './data/inventory-availability-api.service';
import { InventoryLocalizationService } from './inventory-localization.service';
import { InventoryReservationsPageComponent } from './inventory-reservations-page';

describe('InventoryReservationsPageComponent', () => {
  const page = {
    items: [{ id: 9, sourceType: 1, sourceReference: 'TR-2026-004', status: 1, createdAt: '2026-11-25T08:00:00Z', lineCount: 2, totalQuantity: 5 }],
    totalCount: 1,
    page: 1,
    pageSize: 50,
    totalPages: 1,
  };
  const api = { getReservations: vi.fn(() => of(page)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getReservations.mockReturnValue(of(page));
    await TestBed.configureTestingModule({
      imports: [InventoryReservationsPageComponent],
      providers: [
        { provide: InventoryAvailabilityApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored reservation reference, source, status, quantity, and lifecycle evidence', () => {
    const fixture = TestBed.createComponent(InventoryReservationsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('TR-2026-004');
    expect(fixture.nativeElement.textContent).toContain('stockTransfer');
    expect(fixture.nativeElement.textContent).toContain('reservationActive');
    expect(fixture.nativeElement.querySelectorAll('app-page-header')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('.filters')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-data-table')).toHaveLength(1);
  });

  it('keeps reservation status filtering and paging on the server contract', () => {
    const component = TestBed.createComponent(InventoryReservationsPageComponent).componentInstance as any;
    component.filterStatus('2');
    component.move(1);

    expect(api.getReservations).toHaveBeenLastCalledWith(expect.objectContaining({ status: 2, page: 1, pageSize: 50 }));
    expect(component.statusName(2)).toBe('reservationConsumed');
    expect(component.statusName(3)).toBe('reservationReleased');
    expect(component.sourceName(2)).toBe('saleOrder');
  });

  it('debounces trimmed source-reference search into the server query', () => {
    vi.useFakeTimers();
    try {
      const component = TestBed.createComponent(InventoryReservationsPageComponent).componentInstance as any;
      component.search('  TR-2026  ');
      vi.advanceTimersByTime(300);

      expect(api.getReservations).toHaveBeenLastCalledWith({ search: 'TR-2026', page: 1, pageSize: 50 });
    } finally {
      vi.useRealTimers();
    }
  });

  it('resets status, search, page size, and page to the bounded defaults', () => {
    const component = TestBed.createComponent(InventoryReservationsPageComponent).componentInstance as any;
    component.query = { status: 2, search: 'TR-', page: 2, pageSize: 100 };
    component.clearFilters();

    expect(api.getReservations).toHaveBeenLastCalledWith({ page: 1, pageSize: 50 });
  });

  it('keeps an empty response free of invented reservation rows', () => {
    api.getReservations.mockReturnValue(of({ ...page, items: [], totalCount: 0 }));
    const fixture = TestBed.createComponent(InventoryReservationsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('noReservations');
    expect(fixture.nativeElement.querySelector('app-data-table')).toBeNull();
  });
});