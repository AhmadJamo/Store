import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentDetailsPageComponent } from './inventory-adjustment-details-page';
import { InventoryLocalizationService } from './inventory-localization.service';

describe('InventoryAdjustmentDetailsPageComponent', () => {
  const details: any = {
    id: 4, number: 'ADJ-4', warehouse: 'Main warehouse', location: 'A-01 — Coffee rack', status: 1,
    isBlindCount: true, reason: 'Monthly cycle count', createdAt: '2026-10-27T10:00:00Z', lineCount: 2,
    lines: [
      { productId: 2, productName: 'Roasted coffee', expectedQuantity: null },
      { productId: 3, productName: 'Green coffee', expectedQuantity: null },
    ],
  };
  const api = {
    getDetails: vi.fn<any>(() => of(details)), count: vi.fn(() => of(void 0)), approve: vi.fn(() => of(void 0)),
    post: vi.fn(() => of(void 0)), cancel: vi.fn(() => of(void 0)),
  };
  const permissions = new Set(['InventoryAdjustments.Count', 'InventoryAdjustments.Approve', 'InventoryAdjustments.Post', 'InventoryAdjustments.Cancel']);
  const workspace = { activeTab: vi.fn(() => ({ id: 'adjustment' })), markDirty: vi.fn() };

  beforeEach(async () => {
    vi.clearAllMocks();
    permissions.clear();
    ['InventoryAdjustments.Count', 'InventoryAdjustments.Approve', 'InventoryAdjustments.Post', 'InventoryAdjustments.Cancel'].forEach(permission => permissions.add(permission));
    api.getDetails.mockReturnValue(of(details));
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    await TestBed.configureTestingModule({
      imports: [InventoryAdjustmentDetailsPageComponent],
      providers: [
        { provide: InventoryAdjustmentApiService, useValue: api },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '4' } } } },
        { provide: ApiSessionService, useValue: { can: (permission: string) => permissions.has(permission) } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: WorkspaceService, useValue: workspace },
        { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } },
        provideRouter([]),
      ],
    }).compileComponents();
  });

  it('renders server-authored record evidence and hides expected quantities during an authorized blind draft count', () => {
    const fixture = TestBed.createComponent(InventoryAdjustmentDetailsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('ADJ-4');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('A-01 — Coffee rack');
    expect(fixture.nativeElement.textContent).toContain('Monthly cycle count');
    expect(fixture.nativeElement.textContent).toContain('Roasted coffee');
    expect(fixture.nativeElement.textContent).toContain('expectedQuantity: hidden');
    expect(fixture.nativeElement.querySelector('form.count-form')).not.toBeNull();
  });

  it('renders the exact workflow action and completed-state matrix for every server status', () => {
    const cases = [
      { status: 1, expected: ['cancelAdjustment'], absent: ['approve', 'post', 'countCompleted'] },
      { status: 2, expected: ['approve', 'cancelAdjustment', 'countCompleted'], absent: ['post'] },
      { status: 3, expected: ['post', 'cancelAdjustment', 'countCompleted', 'approvalCompleted'], absent: ['approve'] },
      { status: 4, expected: ['countCompleted', 'approvalCompleted', 'adjustmentPosted'], absent: ['approve', 'cancelAdjustment'] },
      { status: 5, expected: ['finishedCancelled'], absent: ['approve', 'post', 'cancelAdjustment', 'countCompleted'] },
    ];

    for (const testCase of cases) {
      api.getDetails.mockReturnValue(of({ ...details, status: testCase.status, isBlindCount: false, lines: [{ productId: 2, productName: 'Roasted coffee', expectedQuantity: 5, countedQuantity: 6, varianceQuantity: 1 }] }));
      const fixture = TestBed.createComponent(InventoryAdjustmentDetailsPageComponent);
      fixture.detectChanges();
      const text = fixture.nativeElement.textContent as string;
      testCase.expected.forEach(value => expect(text).toContain(value));
      testCase.absent.forEach(value => expect(text).not.toContain(value));
      fixture.destroy();
    }
  });

  it('keeps every workflow action independently permission-gated', () => {
    api.getDetails.mockReturnValue(of({ ...details, status: 2, isBlindCount: false, lines: [{ productId: 2, productName: 'Coffee', expectedQuantity: 5, countedQuantity: 5, varianceQuantity: 0 }] }));
    permissions.clear();
    const fixture = TestBed.createComponent(InventoryAdjustmentDetailsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('approve');
    expect(fixture.nativeElement.textContent).not.toContain('cancelAdjustment');
  });

  it('records every line exactly and keeps approve, post, and cancel as separate confirmed commands', () => {
    const component = TestBed.createComponent(InventoryAdjustmentDetailsPageComponent).componentInstance as any;
    component.id = 4;
    component.lines.at(0).controls.countedQuantity.setValue(6);
    component.lines.at(1).controls.countedQuantity.setValue(4);
    component.recordCount();
    component.approve();
    component.post();
    component.cancel();

    expect(api.count).toHaveBeenCalledWith(4, { lines: [{ productId: 2, countedQuantity: 6 }, { productId: 3, countedQuantity: 4 }] });
    expect(api.approve).toHaveBeenCalledWith(4);
    expect(api.post).toHaveBeenCalledWith(4);
    expect(api.cancel).toHaveBeenCalledWith(4);
    expect(globalThis.confirm).toHaveBeenCalledWith('confirmCompleteCount');
  });

  it('blocks an invalid count and preserves the dirty unsaved-leave boundary', () => {
    const component = TestBed.createComponent(InventoryAdjustmentDetailsPageComponent).componentInstance as any;
    component.lines.at(0).controls.countedQuantity.setValue(-1);
    component.recordCount();

    expect(api.count).not.toHaveBeenCalled();
    expect(component.problem()).toBe('countEveryLine');
    component.form.markAsDirty();
    expect(component.canDeactivate()).toBe(true);
    expect(globalThis.confirm).toHaveBeenCalledWith('unsavedDescription');
  });
});
