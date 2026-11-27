import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { PurchaseAccountingReconciliationApiService } from './purchase-accounting-reconciliation-api.service';
import { PurchaseAccountingReconciliationLocalizationService } from './purchase-accounting-reconciliation-localization.service';
import { PurchaseAccountingReconciliationPageComponent } from './purchase-accounting-reconciliation-page';

describe('PurchaseAccountingReconciliationPageComponent', () => {
  const report = {
    asOfDate: '2026-10-31',
    exceptionsOnly: true,
    usesCurrentInventorySnapshot: false,
    inventoryExceptionCount: 1,
    grniOpenReceiptCount: 1,
    operationalInventoryValue: 1234.56,
    ledgerInventoryBalance: 1200,
    inventoryDifference: 34.56,
    grniAccountId: 18,
    grniAccountDisplay: '2180 — Goods received not invoiced',
    operationalGrniBalance: 475.25,
    ledgerGrniBalance: 450,
    grniDifference: 25.25,
    hasGrniAccount: true,
    inventoryIsReconciled: false,
    grniIsReconciled: false,
    inventory: [
      {
        warehouseId: 3,
        warehouseName: 'Main warehouse',
        inventoryAccountId: 14,
        inventoryAccountDisplay: '1310 — Inventory asset',
        operationalValue: 1234.56,
        ledgerBalance: 1200,
        difference: 34.56,
        hasInventoryAccount: true,
        isReconciled: false,
      },
    ],
    grniReceipts: [
      {
        goodsReceiptId: 71,
        receiptNumber: 'GR-2026-0071',
        receiptDate: '2026-10-28',
        purchaseOrderId: 41,
        purchaseOrderNumber: 'PO-2026-0041',
        supplierId: 9,
        supplierName: 'Levant Coffee Supplies',
        warehouseId: 3,
        warehouseName: 'Main warehouse',
        receiptValue: 600,
        returnedValue: 50,
        billedClearingValue: 74.75,
        outstandingValue: 475.25,
        settlements: [
          {
            documentType: 'VendorBill',
            documentId: 81,
            documentNumber: 'VB-2026-0081',
            documentDate: '2026-10-29',
            amount: 74.75,
          },
          {
            documentType: 'GoodsReceiptReturn',
            documentId: 91,
            documentNumber: 'GRR-2026-0091',
            documentDate: '2026-10-30',
            amount: 50,
          },
        ],
      },
    ],
  };
  const emptyReport = {
    ...report,
    usesCurrentInventorySnapshot: true,
    inventoryExceptionCount: 0,
    grniOpenReceiptCount: 0,
    operationalInventoryValue: 0,
    ledgerInventoryBalance: 0,
    inventoryDifference: 0,
    operationalGrniBalance: 0,
    ledgerGrniBalance: 0,
    grniDifference: 0,
    inventoryIsReconciled: true,
    grniIsReconciled: true,
    inventory: [],
    grniReceipts: [],
  };
  const api = { get: vi.fn(() => of(report)) };
  const permissions = new Set<string>();
  const session = { can: (permission: string) => permissions.has(permission) };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.get.mockReturnValue(of(report));
    permissions.clear();
    permissions.add('ProductStock.View');
    permissions.add('Warehouses.Edit');
    permissions.add('GoodsReceipts.View');
    permissions.add('PurchaseOrders.View');
    permissions.add('VendorBills.View');
    permissions.add('GoodsReceiptReturns.View');
    permissions.add('Settings.Edit');

    await TestBed.configureTestingModule({
      imports: [PurchaseAccountingReconciliationPageComponent],
      providers: [
        provideRouter([]),
        { provide: PurchaseAccountingReconciliationApiService, useValue: api },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ApiSessionService, useValue: session },
        {
          provide: PurchaseAccountingReconciliationLocalizationService,
          useValue: { text: (key: string) => key },
        },
      ],
    }).compileComponents();
  });

  it('renders server-authored inventory, ledger, GRNI, settlement, and historical evidence', () => {
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('readOnly');
    expect(text).toContain('historicalTitle');
    expect(text).toContain('Main warehouse');
    expect(text).toContain('1310 — Inventory asset');
    expect(text).toContain('2180 — Goods received not invoiced');
    expect(text).toContain('GR-2026-0071');
    expect(text).toContain('PO-2026-0041');
    expect(text).toContain('Levant Coffee Supplies');
    expect(text).toContain('VB-2026-0081');
    expect(text).toContain('GRR-2026-0091');
    expect(text).toContain('1,234.56');
    expect(text).toContain('475.25');
  });

  it('uses localized Purchasing context, V4 filters, and four server summary items', () => {
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('.page-header .eyebrow')?.textContent).toContain(
      'purchasingNavigation',
    );
    expect(element.querySelector('app-filter-bar')).not.toBeNull();
    expect(element.querySelectorAll('app-summary-strip article')).toHaveLength(4);
    expect(element.querySelector('app-summary-strip')?.textContent).toContain('1,234.56');
    expect(element.querySelector('app-summary-strip')?.textContent).toContain('475.25');
    expect(element.querySelector('.metrics')).toBeNull();
  });
  it('sends only the selected as-of date and exception flag to the read API', () => {
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    const component = fixture.componentInstance as any;
    api.get.mockClear();

    component.form.setValue({ asOfDate: '2026-09-30', exceptionsOnly: false });
    component.load();

    expect(api.get).toHaveBeenCalledTimes(1);
    expect(api.get).toHaveBeenCalledWith('2026-09-30', false);
  });

  it('keeps every source-document link behind its independent permission', () => {
    permissions.clear();
    permissions.add('GoodsReceipts.View');
    permissions.add('VendorBills.View');
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('a[href="/purchases/receipts/71"]')).not.toBeNull();
    expect(element.querySelector('a[href="/purchases/bills/81"]')).not.toBeNull();
    expect(element.querySelector('a[href="/purchases/orders/41"]')).toBeNull();
    expect(element.querySelector('a[href="/purchases/receipt-returns/91"]')).toBeNull();
    expect(element.querySelector('a[href^="/inventory/stocks"]')).toBeNull();
    expect(element.querySelector('a[href^="/warehouses"]')).toBeNull();
    expect(element.textContent).toContain('PO-2026-0041');
    expect(element.textContent).toContain('GRR-2026-0091');
  });

  it('keeps accounting, stock, and warehouse remediation links permission-gated', () => {
    permissions.clear();
    permissions.add('Settings.Edit');
    permissions.add('ProductStock.View');
    permissions.add('Warehouses.Edit');
    api.get.mockReturnValue(
      of({
        ...report,
        grniAccountId: null,
        grniAccountDisplay: '',
        hasGrniAccount: false,
        grniIsReconciled: false,
        inventory: [
          {
            ...report.inventory[0],
            inventoryAccountId: null,
            inventoryAccountDisplay: '',
            hasInventoryAccount: false,
          },
        ],
      }) as any,
    );
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('a[href=\"/settings/accounting\"]')).not.toBeNull();
    expect(element.querySelector('a[href^=\"/inventory/stocks\"]')).not.toBeNull();
    expect(element.querySelector('a[href^=\"/warehouses\"]')).not.toBeNull();
    expect(element.querySelector('a[href^=\"/purchases/receipts\"]')).toBeNull();
  });
  it('renders honest empty states without inventing inventory or receipt evidence', () => {
    api.get.mockReturnValue(of(emptyReport));
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('noInventoryExceptions');
    expect(fixture.nativeElement.textContent).toContain('noOpenReceipts');
    expect(fixture.nativeElement.textContent).not.toContain('GR-2026-0071');
    expect(fixture.nativeElement.textContent).not.toContain('Main warehouse');
  });

  it('shows the shared recovery state when the read request fails', () => {
    api.get.mockReturnValue(throwError(() => ({ status: 503, title: 'Unavailable' })) as any);
    const fixture = TestBed.createComponent(PurchaseAccountingReconciliationPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('app-api-recovery')).not.toBeNull();
    expect(fixture.nativeElement.textContent).not.toContain('GR-2026-0071');
  });
});
