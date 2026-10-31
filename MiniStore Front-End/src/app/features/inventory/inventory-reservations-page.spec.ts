import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { InventoryAvailabilityApiService } from './data/inventory-availability-api.service';
import { InventoryLocalizationService } from './inventory-localization.service';
import { InventoryReservationsPageComponent } from './inventory-reservations-page';

describe('InventoryReservationsPageComponent', () => {
  const page = { items: [], totalCount: 0, page: 1, pageSize: 50, totalPages: 1 };
  const api = { getReservations: vi.fn(() => of(page)) };
  beforeEach(async () => { vi.clearAllMocks(); api.getReservations.mockReturnValue(of(page)); await TestBed.configureTestingModule({ imports: [InventoryReservationsPageComponent], providers: [{ provide: InventoryAvailabilityApiService, useValue: api }, { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } }] }).compileComponents(); });
  it('keeps reservation status filtering on the server contract', () => { const component = TestBed.createComponent(InventoryReservationsPageComponent).componentInstance as any; component.filterStatus('2'); expect(api.getReservations).toHaveBeenLastCalledWith(expect.objectContaining({ status: 2, page: 1, pageSize: 50 })); expect(component.statusName(2)).toBe('reservationConsumed'); });
});
