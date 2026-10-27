import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { InventoryAvailabilityApiService } from './data/inventory-availability-api.service';
import { InventoryAvailabilityPageComponent } from './inventory-availability-page';
import { InventoryLocalizationService } from './inventory-localization.service';

describe('InventoryAvailabilityPageComponent', () => {
  const page = { items: [], warehouses: [{ id: 3, name: 'Main' }], totalCount: 0, page: 1, pageSize: 50, totalPages: 1 };
  const api = { getBalances: vi.fn(() => of(page)) };
  beforeEach(async () => { vi.clearAllMocks(); api.getBalances.mockReturnValue(of(page)); await TestBed.configureTestingModule({ imports: [InventoryAvailabilityPageComponent], providers: [{ provide: InventoryAvailabilityApiService, useValue: api }, { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } }] }).compileComponents(); });
  it('keeps warehouse filtering and paging on the server contract', () => { const component = TestBed.createComponent(InventoryAvailabilityPageComponent).componentInstance as any; component.filterWarehouse('3'); component.move(1); expect(api.getBalances).toHaveBeenLastCalledWith(expect.objectContaining({ warehouseId: 3, page: 1, pageSize: 50 })); });
});
