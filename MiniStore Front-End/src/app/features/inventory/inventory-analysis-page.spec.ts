import { TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { InventoryAnalysisApiService } from './data/inventory-analysis-api.service';
import { InventoryAnalysisLocalizationService } from './inventory-analysis-localization.service';
import { InventoryAnalysisPageComponent } from './inventory-analysis-page';

describe('InventoryAnalysisPageComponent insights mode', () => {
  const report = {
    query: { warehouseId: null, search: '', state: null, attentionOnly: false, slowDays: 30, deadDays: 90 },
    rows: [{
      productId: 12,
      productName: 'Roasted coffee',
      productCode: 'COF-12',
      warehouseId: 3,
      warehouseName: 'Main warehouse',
      onHand: 18,
      inventoryValue: 76.5,
      lastOutboundAt: '2026-10-01T08:00:00Z',
      firstInboundAt: '2026-09-01T08:00:00Z',
      activityAnchorAt: '2026-10-01T08:00:00Z',
      daysSinceActivity: 55,
      state: 1 as const,
    }],
    warehouses: [{ id: 3, name: 'Main warehouse' }],
    totalValue: 76.5,
    attentionValue: 76.5,
    slowCount: 1,
    deadCount: 0,
    neverIssuedCount: 0,
    noMovementHistoryCount: 0,
  };
  const api = {
    getInsights: vi.fn(() => of(report)),
    getReconciliation: vi.fn(),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getInsights.mockReturnValue(of(report));
    await TestBed.configureTestingModule({
      imports: [InventoryAnalysisPageComponent],
      providers: [
        { provide: ActivatedRoute, useValue: { snapshot: { routeConfig: { path: 'inventory/insights' } } } },
        { provide: InventoryAnalysisApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permission === 'InventoryReconciliation.View' } },
        { provide: InventoryAnalysisLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders server-authored KPI, product, warehouse, quantity, value, activity, and classification evidence', () => {
    const fixture = TestBed.createComponent(InventoryAnalysisPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('COF-12');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('slow');
    expect(fixture.nativeElement.querySelectorAll('app-filter-bar')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip article')).toHaveLength(5);
    expect(fixture.nativeElement.querySelectorAll('app-data-table th')).toHaveLength(7);
    expect(fixture.nativeElement.querySelector('a[href="/inventory/reconciliation"]')).not.toBeNull();
  });

  it('submits named warehouse, search, state, attention, and classification thresholds to the API', () => {
    const component = TestBed.createComponent(InventoryAnalysisPageComponent).componentInstance as any;
    component.insightsForm.setValue({ warehouseId: 3, search: 'coffee', state: 2, attentionOnly: true, slowDays: 45, deadDays: 120 });
    component.load();

    expect(api.getInsights).toHaveBeenLastCalledWith({ warehouseId: 3, search: 'coffee', state: 2, attentionOnly: true, slowDays: 45, deadDays: 120 });
  });

  it('does not query the API when classification thresholds are invalid', () => {
    const fixture = TestBed.createComponent(InventoryAnalysisPageComponent);
    const component = fixture.componentInstance as any;
    const callsBefore = api.getInsights.mock.calls.length;
    component.insightsForm.controls.slowDays.setValue(0);
    component.load();

    expect(api.getInsights).toHaveBeenCalledTimes(callsBefore);
    expect(component.loading()).toBe(false);
  });

  it('maps every server activity state without classifying rows in Angular', () => {
    const component = TestBed.createComponent(InventoryAnalysisPageComponent).componentInstance as any;
    expect([0, 1, 2, 3, 4].map(state => component.stateName(state))).toEqual(['active', 'slow', 'dead', 'neverIssued', 'noMovement']);
  });

  it('keeps an empty insights response free of invented inventory rows', () => {
    api.getInsights.mockReturnValue(of({ ...report, rows: [], totalValue: 0, attentionValue: 0, slowCount: 0 }));
    const fixture = TestBed.createComponent(InventoryAnalysisPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('noInsights');
    expect(fixture.nativeElement.querySelector('app-data-table')).not.toBeNull();
    expect(fixture.nativeElement.querySelectorAll('app-data-table tbody tr')).toHaveLength(0);

    const component = fixture.componentInstance as any;
    component.insightsForm.controls.search.setValue('coffee');
    expect(component.insightsEmptyTitle()).toBe('filteredInsightsEmpty');
  });
});
