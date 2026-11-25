import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { BranchContextService } from '../../core/branch-context/branch-context.service';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { BranchesApiService } from './branches-api.service';
import { BranchesLocalizationService } from './branches-localization.service';
import { BranchesPageComponent } from './branches-page';

describe('BranchesPageComponent', () => {
  const branch = {
    id: 1,
    code: 'AMM',
    name: 'Amman',
    isActive: true,
    warehouses: [{ id: 7, name: 'Main warehouse', isDefaultForPos: true, allowPosSales: true, allowPurchases: true, allowTransferOut: true, allowTransferIn: true }],
    posTerminals: [{ id: 4, name: 'Front POS', defaultWarehouseName: 'Main warehouse', isActive: true }],
    operations: { salesAmount: 125, salesCount: 3, stockQuantity: 50, stockValue: 800, openTransfers: 2, transferExceptions: 1, openCashSessions: 1, cashSessionsAwaitingApproval: 0, cashVariance: 0 },
  };
  const workspace = {
    branches: [branch],
    periodFromUtc: '2026-11-01T00:00:00Z',
    periodToUtc: '2026-11-25T00:00:00Z',
    company: branch.operations,
  };
  const management = {
    branches: [
      { id: 1, code: 'AMM', name: 'Amman', isActive: true },
      { id: 2, code: 'AQB', name: 'Aqaba', isActive: true },
    ],
    users: [{ id: 'user-1', displayName: 'Mona', email: 'mona@example.test', branchIds: [1] }],
  };
  const api = {
    getWorkspace: vi.fn(() => of(workspace)),
    getAccessManagement: vi.fn(() => of(management)),
    create: vi.fn(() => of({ id: 2, code: 'AQB', name: 'Aqaba', isActive: true })),
    update: vi.fn(),
    setActive: vi.fn(() => of({ id: 1, code: 'AMM', name: 'Amman', isActive: false })),
    replaceUserGrants: vi.fn(() => of(management)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getWorkspace.mockReturnValue(of(workspace));
    api.getAccessManagement.mockReturnValue(of(management));
    api.create.mockReturnValue(of({ id: 2, code: 'AQB', name: 'Aqaba', isActive: true }));
    api.setActive.mockReturnValue(of({ id: 1, code: 'AMM', name: 'Amman', isActive: false }));
    api.replaceUserGrants.mockReturnValue(of(management));
    await TestBed.configureTestingModule({
      imports: [BranchesPageComponent],
      providers: [
        provideRouter([]),
        { provide: BranchesApiService, useValue: api },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: BranchContextService, useValue: { selectedId: signal(1) } },
        { provide: BranchesLocalizationService, useValue: { text: (key: string) => key } },
        { provide: FormFocusService, useValue: { focusFirstInvalid: vi.fn() } },
      ],
    }).compileComponents();
  });

  it('renders only the server-returned branch workspace with its current operations and names', () => {
    const fixture = TestBed.createComponent(BranchesPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Amman');
    expect(fixture.nativeElement.textContent).toContain('Main warehouse');
    expect(fixture.nativeElement.textContent).toContain('Front POS');
    expect(fixture.nativeElement.textContent).toContain('currentOperationalBranch');
    expect(fixture.nativeElement.querySelectorAll('.branch-card')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('.branch-actions a')).toHaveLength(4);
  });

  it('renders the shared Odoo-inspired workspace hierarchy with localized context', () => {
    const fixture = TestBed.createComponent(BranchesPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('administrationNavigation');
    expect(fixture.nativeElement.textContent).not.toContain('ADMIN');
    expect(fixture.nativeElement.querySelectorAll('app-summary-strip')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-detail-section')).toHaveLength(3);

    (fixture.componentInstance as any).startCreate();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('app-form-section')).toHaveLength(1);
    expect(fixture.nativeElement.querySelectorAll('app-field-cue')).toHaveLength(2);
    expect(fixture.nativeElement.querySelectorAll('app-workflow-footer')).toHaveLength(1);
  });

  it('preserves branch create and active-state commands', () => {
    const component = TestBed.createComponent(BranchesPageComponent).componentInstance as any;
    component.startCreate();
    component.form.patchValue({ code: 'AQB', name: 'Aqaba' });
    component.saveBranch();
    component.setBranchActive(branch);

    expect(api.create).toHaveBeenCalledWith({ code: 'AQB', name: 'Aqaba' });
    expect(api.setActive).toHaveBeenCalledWith(1, false);
  });

  it('replaces user branch grants with the selected internal IDs', () => {
    const component = TestBed.createComponent(BranchesPageComponent).componentInstance as any;
    component.selectUser({ target: { value: 'user-1' } } as unknown as Event);
    component.toggleBranch(2, { target: { checked: true } } as unknown as Event);
    component.saveAccess();

    expect(api.replaceUserGrants).toHaveBeenCalledWith('user-1', [1, 2]);
  });
});
