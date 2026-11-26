import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { StockTransactionApiService } from './data/stock-transaction-api.service';
import { InventoryLocalizationService } from './inventory-localization.service';
import { StockTransactionsPageComponent } from './stock-transactions-page';

describe('StockTransactionsPageComponent', () => {
  const page = {
    items: [{
      id: 71,
      productId: 12,
      productName: 'Roasted coffee',
      warehouseId: 3,
      warehouseName: 'Main warehouse',
      quantity: -2.5,
      type: 3,
      reference: 'SALE-2026-008',
      createdAt: '2026-11-25T08:00:00Z',
      quantityBefore: 20,
      quantityAfter: 17.5,
      averageUnitCostBefore: 4.25,
      averageUnitCostAfter: 4.25,
      inventoryValueBefore: 85,
      inventoryValueAfter: 74.375,
      unitCost: 4.25,
      transactionValue: -10.625,
      costVariance: 0.125,
    }],
    totalCount: 1,
    page: 1,
    pageSize: 50,
    totalPages: 1,
  };
  const api = { getPage: vi.fn(() => of(page)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getPage.mockReturnValue(of(page));
    await TestBed.configureTestingModule({
      imports: [StockTransactionsPageComponent],
      providers: [
        { provide: StockTransactionApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders only server-authored transaction identity, names, quantity, cost, type, reference, and date evidence', () => {
    const fixture = TestBed.createComponent(StockTransactionsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('71');
    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('SALE-2026-008');
    expect(fixture.nativeElement.textContent).toContain('sale');
    expect(fixture.nativeElement.querySelectorAll('app-page-header')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-filter-bar')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-data-table')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-data-table th')).toHaveLength(10);
  });

  it('keeps type, movement, date, and page-size filters on the server query', () => {
    const component = TestBed.createComponent(StockTransactionsPageComponent).componentInstance as any;
    component.filterType('12');
    component.filterMovement('incoming');
    component.filterFrom('2026-11-01');
    component.filterTo('2026-11-25');
    component.resize('100');

    expect(api.getPage).toHaveBeenLastCalledWith(expect.objectContaining({
      type: 12,
      movement: 'incoming',
      fromDate: '2026-11-01',
      toDate: '2026-11-25',
      page: 1,
      pageSize: 100,
    }));
  });

  it('debounces trimmed product, warehouse, or reference search into the server query', () => {
    vi.useFakeTimers();
    try {
      const component = TestBed.createComponent(StockTransactionsPageComponent).componentInstance as any;
      component.search('  SALE-2026  ');
      vi.advanceTimersByTime(300);

      expect(api.getPage).toHaveBeenLastCalledWith(expect.objectContaining({ search: 'SALE-2026', page: 1 }));
    } finally {
      vi.useRealTimers();
    }
  });

  it('keeps sorting and bounded paging server-side and resets to ledger defaults', () => {
    const component = TestBed.createComponent(StockTransactionsPageComponent).componentInstance as any;
    component.sort('quantity');
    expect(api.getPage).toHaveBeenLastCalledWith(expect.objectContaining({ sortColumn: 'quantity', sortDescending: false, page: 1 }));

    component.move(1);
    expect(api.getPage).toHaveBeenLastCalledWith(expect.objectContaining({ page: 1 }));

    component.query = { ...component.query, search: 'SALE', type: 3, movement: 'outgoing', page: 2, pageSize: 100 };
    component.clearFilters();
    expect(api.getPage).toHaveBeenLastCalledWith({ page: 1, pageSize: 50, sortColumn: 'date', sortDescending: true, movement: '' });
  });

  it('distinguishes true empty from filtered empty without inventing ledger rows', () => {
    api.getPage.mockReturnValue(of({ ...page, items: [], totalCount: 0, totalPages: 0 }));
    const fixture = TestBed.createComponent(StockTransactionsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('noTransactions');
    expect(fixture.nativeElement.textContent).not.toContain('filteredTransactionsEmpty');
    expect(fixture.nativeElement.querySelector('app-data-table')).not.toBeNull();

    (fixture.componentInstance as any).filterMovement('outgoing');
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('filteredTransactionsEmpty');
  });
});
