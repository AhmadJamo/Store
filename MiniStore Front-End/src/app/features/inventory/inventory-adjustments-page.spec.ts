import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Subject, of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentsPageComponent } from './inventory-adjustments-page';
import { InventoryLocalizationService } from './inventory-localization.service';

describe('InventoryAdjustmentsPageComponent', () => {
  const rows = [
    { id: 42, number: 'ADJ-2026-0042', warehouse: 'Main warehouse', location: 'A-01 — Coffee rack', status: 2 as const, isBlindCount: true, reason: 'Monthly cycle count', createdAt: '2026-11-25T08:30:00Z', lineCount: 3 },
    { id: 41, number: 'ADJ-2026-0041', warehouse: 'Reserve warehouse', location: 'Unassigned (system)', status: 4 as const, isBlindCount: false, reason: 'Opening balance review', createdAt: '2026-11-24T07:00:00Z', lineCount: 1 },
  ];
  const api = { getAll: vi.fn<any>(() => of(rows)) };
  const session = { can: vi.fn((permission: string) => permission === 'InventoryAdjustments.Create') };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getAll.mockReturnValue(of(rows));
    session.can.mockImplementation((permission: string) => permission === 'InventoryAdjustments.Create');
    await TestBed.configureTestingModule({
      imports: [InventoryAdjustmentsPageComponent],
      providers: [
        { provide: InventoryAdjustmentApiService, useValue: api },
        { provide: ApiSessionService, useValue: session },
        { provide: ApiErrorService, useValue: { normalize: () => ({ title: 'Load failed', detail: 'Try again.' }) } },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
        provideRouter([]),
      ],
    }).compileComponents();
  });

  it('renders only server-authored document, warehouse, location, count, and creation evidence', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('ADJ-2026-0042');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('A-01 — Coffee rack');
    expect(fixture.nativeElement.textContent).toContain('adjustmentCounted');
    expect(fixture.nativeElement.textContent).toContain('yes');
    expect(fixture.nativeElement.textContent).toContain('3');
    expect(fixture.nativeElement.querySelectorAll('app-page-header')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-data-table th')).toHaveLength(8);
    expect(fixture.nativeElement.querySelector('a[href="/inventory/adjustments/42"]')).not.toBeNull();
  });

  it('maps all five server workflow states without deriving a new lifecycle', () => {
    const component = TestBed.createComponent(InventoryAdjustmentsPageComponent).componentInstance as any;

    expect([1, 2, 3, 4, 5].map(status => component.statusName(status))).toEqual([
      'adjustmentDraft',
      'adjustmentCounted',
      'adjustmentApproved',
      'adjustmentPosted',
      'adjustmentCancelled',
    ]);
  });

  it('shows the create action only with the independent create permission', () => {
    const allowed = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    allowed.detectChanges();
    expect(allowed.nativeElement.querySelector('a[href="/inventory/adjustments/new"]')).not.toBeNull();

    session.can.mockReturnValue(false);
    const denied = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    denied.detectChanges();
    expect(denied.nativeElement.querySelector('a[href="/inventory/adjustments/new"]')).toBeNull();
  });

  it('preserves the service response order and does not invent client-side filters or paging', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(api.getAll).toHaveBeenCalledTimes(1);
    expect(text.indexOf('ADJ-2026-0042')).toBeLessThan(text.indexOf('ADJ-2026-0041'));
  });

  it('keeps loading, recovery, and true-empty states distinct without invented rows', () => {
    const pending = new Subject<typeof rows>();
    api.getAll.mockReturnValue(pending);
    const loading = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    loading.detectChanges();
    expect(loading.nativeElement.textContent).toContain('loadingAdjustments');

    api.getAll.mockReturnValue(throwError(() => new Error('offline')));
    const failed = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    failed.detectChanges();
    expect(failed.nativeElement.querySelector('app-api-recovery')).not.toBeNull();
    expect(failed.nativeElement.textContent).toContain('Try again.');

    api.getAll.mockReturnValue(of([]));
    const empty = TestBed.createComponent(InventoryAdjustmentsPageComponent);
    empty.detectChanges();
    expect(empty.nativeElement.textContent).toContain('noAdjustments');
    expect(empty.nativeElement.querySelectorAll('app-data-table tbody tr')).toHaveLength(0);
  });
});
