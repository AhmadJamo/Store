import { TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { InventoryAnalysisApiService } from './data/inventory-analysis-api.service';
import { InventoryAnalysisLocalizationService } from './inventory-analysis-localization.service';
import { InventoryAnalysisPageComponent } from './inventory-analysis-page';

describe('InventoryAnalysisPageComponent reconciliation mode', () => {
  const report = {
    items: [{
      productId: 12,
      productName: 'Roasted coffee',
      productCode: 'COF-12',
      barcode: '6251000000123',
      warehouseId: 3,
      warehouseName: 'Main warehouse',
      warehouseQuantity: 18,
      assignedLocationQuantity: 14,
      unassignedQuantity: 4,
      inventoryValue: 76.5,
      latestTransactionId: 71,
      latestTransactionAt: '2026-11-25T08:00:00Z',
      latestQuantityAfter: 17,
      latestInventoryValueAfter: 72.25,
      issues: [5 as const, 6 as const],
      isConsistent: false,
    }],
    warehouses: [{ id: 3, name: 'Main warehouse' }],
    warehouseId: 3,
    search: 'coffee',
    exceptionsOnly: true,
    page: 1,
    pageSize: 50,
    totalCount: 1,
    exceptionCount: 1,
    totalPages: 1,
  };
  const api = {
    getInsights: vi.fn(),
    getReconciliation: vi.fn(() => of(report)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getReconciliation.mockReturnValue(of(report));
    await TestBed.configureTestingModule({
      imports: [InventoryAnalysisPageComponent],
      providers: [
        { provide: ActivatedRoute, useValue: { snapshot: { routeConfig: { path: 'inventory/reconciliation' } } } },
        { provide: InventoryAnalysisApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permission === 'InventoryInsights.View' } },
        { provide: InventoryAnalysisLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders server-authored reconciliation identity, quantities, value, latest movement, and issues', () => {
    const fixture = TestBed.createComponent(InventoryAnalysisPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('COF-12');
    expect(fixture.nativeElement.textContent).toContain('6251000000123');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('issueQuantityMismatch');
    expect(fixture.nativeElement.textContent).toContain('issueValueMismatch');
    expect(fixture.nativeElement.textContent).toContain('readOnlyHelp');
    expect(fixture.nativeElement.querySelectorAll('app-data-table th')).toHaveLength(9);
    expect(fixture.nativeElement.querySelector('a[href="/inventory/insights"]')).not.toBeNull();
  });

  it('keeps named warehouse, search, exception, paging, and page-size filters on the API query', () => {
    const component = TestBed.createComponent(InventoryAnalysisPageComponent).componentInstance as any;
    component.reconciliationForm.setValue({ warehouseId: 3, search: 'coffee', exceptionsOnly: false, pageSize: 100 });
    component.load(2);

    expect(api.getReconciliation).toHaveBeenLastCalledWith({ warehouseId: 3, search: 'coffee', exceptionsOnly: false, pageSize: 100, page: 2 });
  });

  it('maps every server reconciliation issue without deriving consistency in Angular', () => {
    const component = TestBed.createComponent(InventoryAnalysisPageComponent).componentInstance as any;
    expect([0, 1, 2, 3, 4, 5, 6].map(issue => component.issueName(issue))).toEqual([
      'issueMissingWarehouseStock',
      'issueOverAllocated',
      'issueNegativeLocation',
      'issueSimpleLocation',
      'issueMissingTransaction',
      'issueQuantityMismatch',
      'issueValueMismatch',
    ]);
  });

  it('renders the server consistent state without inventing exception labels', () => {
    api.getReconciliation.mockReturnValue(of({ ...report, items: [{ ...report.items[0], issues: [], isConsistent: true }], exceptionCount: 0 }));
    const fixture = TestBed.createComponent(InventoryAnalysisPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('consistent');
    expect(fixture.nativeElement.querySelectorAll('.issue')).toHaveLength(0);
  });

  it('distinguishes a true empty response from filtered emptiness without inventing rows', () => {
    (api.getReconciliation as any).mockReturnValue(of({ ...report, items: [], warehouseId: null, search: '', exceptionsOnly: false, totalCount: 0, exceptionCount: 0, totalPages: 0 }));
    const fixture = TestBed.createComponent(InventoryAnalysisPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('noReconciliation');
    expect(fixture.nativeElement.querySelectorAll('app-data-table tbody tr')).toHaveLength(0);

    const component = fixture.componentInstance as any;
    component.reconciliationForm.controls.exceptionsOnly.setValue(true);
    expect(component.reconciliationEmptyTitle()).toBe('filteredReconciliationEmpty');
  });
});
