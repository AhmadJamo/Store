import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { PurchaseAnalyticsApiService } from './purchase-analytics-api.service';
import { PurchaseAnalyticsLocalizationService } from './purchase-analytics-localization.service';
import { PurchaseAnalyticsPageComponent } from './purchase-analytics-page';

describe('PurchaseAnalyticsPageComponent', () => {
  const report = {
    query: {
      From: '2026-10-01',
      To: '2026-10-31',
      SupplierId: 9,
      BuyerUserId: 'buyer-1',
      OrderStatus: 3,
      GroupBy: 2,
    },
    supplierOptions: [{ id: 9, label: 'Levant Coffee Supplies' }],
    buyerOptions: [{ userId: 'buyer-1', displayName: 'procurement@example.test' }],
    openOrders: 4,
    unconfirmedOrders: 2,
    overdueOrders: 1,
    receiptValue: 5400.25,
    vendorBillSpend: 5100.5,
    outstandingPayables: 1200.75,
    openMatchExceptionCount: 1,
    averageDeliveryDays: 3.5,
    trends: [
      {
        groupKey: 's:9',
        groupLabel: 'Levant Coffee Supplies',
        orderCount: 3,
        orderedValue: 6000,
        spend: 5100.5,
        priceVariance: 125.25,
      },
    ],
    buyerWorkloads: [
      {
        buyerUserId: 'buyer-1',
        buyerDisplayName: 'procurement@example.test',
        orderCount: 3,
        openOrders: 2,
        overdueOrders: 1,
        orderedValue: 6000,
      },
    ],
    openExceptions: [
      {
        vendorBillId: 81,
        vendorBillNumber: 'VB-2026-0081',
        supplierName: 'Levant Coffee Supplies',
        exceptionCount: 2,
        maximumVariancePercent: 7.5,
      },
    ],
    acceptanceJourney: [
      {
        label: 'Purchase orders',
        count: 3,
        controller: 'PurchaseOrders',
        action: 'Index',
        requiredPermission: 'PurchaseOrders.View',
        explanation: 'Confirmed source orders.',
      },
      {
        label: 'Supplier payments',
        count: 1,
        controller: 'SupplierPayments',
        action: 'Index',
        requiredPermission: 'SupplierPayments.View',
        explanation: 'Settled payable evidence.',
      },
    ],
    suppliers: [
      {
        supplierId: 9,
        supplierName: 'Levant Coffee Supplies',
        fillRatePercent: 96.5,
        onTimeRatePercent: 92.25,
        returnRatePercent: 1.5,
        billedSpend: 5100.5,
      },
    ],
    kpis: [
      {
        label: 'Average delivery days',
        value: '3.50',
        source: 'Goods receipt dates',
        period: '2026-10-01 — 2026-10-31',
        explanation: 'Average elapsed days from confirmation to receipt.',
      },
    ],
  };
  const emptyReport = {
    ...report,
    openOrders: 0,
    unconfirmedOrders: 0,
    overdueOrders: 0,
    receiptValue: 0,
    vendorBillSpend: 0,
    outstandingPayables: 0,
    openMatchExceptionCount: 0,
    averageDeliveryDays: 0,
    trends: [],
    buyerWorkloads: [],
    openExceptions: [],
    acceptanceJourney: [],
    suppliers: [],
    kpis: [],
  };
  const api = { get: vi.fn(() => of(report)) };
  const permissions = new Set<string>();
  const session = { can: (permission: string) => permissions.has(permission) };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.get.mockReturnValue(of(report));
    permissions.clear();
    permissions.add('PurchaseOrders.View');

    await TestBed.configureTestingModule({
      imports: [PurchaseAnalyticsPageComponent],
      providers: [
        provideRouter([]),
        { provide: PurchaseAnalyticsApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ApiSessionService, useValue: session },
        { provide: PurchaseAnalyticsLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();
  });

  it('renders the server query options and summary values', () => {
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.textContent).toContain('Levant Coffee Supplies');
    expect(element.textContent).toContain('procurement@example.test');
    expect(element.textContent).toContain('5,100.50');
    expect(element.textContent).toContain('1,200.75');
    expect(element.textContent).toContain('5,400.25');
    expect((fixture.componentInstance as any).form.getRawValue()).toEqual(report.query);
  });

  it('sends the complete selected read query without adding client-side analytics inputs', () => {
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    const component = fixture.componentInstance as any;
    api.get.mockClear();
    component.form.setValue({
      From: '2026-09-01',
      To: '2026-09-30',
      SupplierId: '12',
      BuyerUserId: 'buyer-2',
      OrderStatus: '5',
      GroupBy: 3,
    });

    component.load();

    expect(api.get).toHaveBeenCalledTimes(1);
    expect(api.get).toHaveBeenCalledWith({
      From: '2026-09-01',
      To: '2026-09-30',
      SupplierId: '12',
      BuyerUserId: 'buyer-2',
      OrderStatus: '5',
      GroupBy: 3,
    });
  });

  it('renders server-authored trends, workload, exceptions, suppliers, and KPI definitions', () => {
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('125.25');
    expect(text).toContain('procurement@example.test');
    expect(text).toContain('VB-2026-0081');
    expect(text).toContain('96.5%');
    expect(text).toContain('92.25%');
    expect(text).toContain('Average delivery days');
    expect(text).toContain('Goods receipt dates');
    expect(text).toContain('2026-10-01 — 2026-10-31');
    expect(text).toContain('Average elapsed days from confirmation to receipt.');
  });

  it('keeps acceptance journey navigation behind each server-required permission', () => {
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.textContent).toContain('Purchase orders');
    expect(element.textContent).toContain('Supplier payments');
    expect(element.querySelector('a[href="/purchases/orders"]')).not.toBeNull();
    expect(element.querySelector('a[href="/purchases/payments"]')).toBeNull();
  });

  it('preserves the Vendor Bill drill-through for server-returned open exceptions', () => {
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    fixture.detectChanges();

    expect(
      (fixture.nativeElement as HTMLElement).querySelector('a[href="/purchases/bills/81"]'),
    ).not.toBeNull();
  });

  it('renders an honest empty trend state without inventing analytic evidence', () => {
    api.get.mockReturnValue(of(emptyReport));
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('empty');
    expect(text).not.toContain('VB-2026-0081');
    expect(text).not.toContain('Average delivery days');
    expect(text).not.toContain('Purchase orders');
  });

  it('shows shared recovery and hides analytic evidence when the read request fails', () => {
    api.get.mockReturnValue(throwError(() => ({ status: 503, title: 'Unavailable' })) as any);
    const fixture = TestBed.createComponent(PurchaseAnalyticsPageComponent);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('app-api-recovery')).not.toBeNull();
    expect(element.textContent).not.toContain('VB-2026-0081');
    expect(element.textContent).not.toContain('Levant Coffee Supplies');
  });
});
