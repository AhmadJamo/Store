import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SupplierApiService } from './data/supplier-api.service';
import { SupplierFormPageComponent } from './supplier-form-page';

describe('SupplierFormPageComponent', () => {
  const page = {
    form: {
      id: 0, name: '', paymentTermsDays: 0, isActive: true, accountId: 0,
      productIds: [], productCount: 0, rowVersion: '',
    },
    accounts: [{ id: 12, code: '2100', name: 'Payables' }],
    products: [{ id: 4, code: 'P-4', name: 'Coffee' }],
  };
  const api = { getForm: vi.fn(() => of(page)), create: vi.fn(), update: vi.fn() };
  const router = { navigate: vi.fn(() => Promise.resolve(true)) };
  const workspace = { activeTab: vi.fn(() => ({ id: 'suppliers::default' })), markDirty: vi.fn() };
  const session = { can: vi.fn(() => true) };
  let routeId: string | null;

  beforeEach(async () => {
    vi.clearAllMocks();
    routeId = null;
    api.getForm.mockReturnValue(of(page));
    session.can.mockReturnValue(true);
    await TestBed.configureTestingModule({
      imports: [SupplierFormPageComponent],
      providers: [
        { provide: SupplierApiService, useValue: api },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => routeId } } } },
        { provide: Router, useValue: router },
        { provide: WorkspaceService, useValue: workspace },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ApiSessionService, useValue: session },
      ],
    }).compileComponents();
  });

  it('sends only required and selected values then clears dirty state after save', () => {
    api.create.mockReturnValue(of({ ...page.form, id: 7, name: 'Local Foods', accountId: 12 }));
    const component = TestBed.createComponent(SupplierFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Local Foods', accountId: 12, productIds: [4] });
    component.form.markAsDirty();
    component.save();
    expect(api.create).toHaveBeenCalledWith(expect.objectContaining({
      name: 'Local Foods', accountId: 12, productIds: [4], legalName: undefined,
    }));
    expect(workspace.markDirty).toHaveBeenLastCalledWith('suppliers::default', false);
    expect(router.navigate).toHaveBeenCalledWith(['/suppliers', 7]);
  });

  it('keeps input and exposes validation details when save fails', () => {
    api.create.mockReturnValue(throwError(() => ({
      status: 400, title: 'Validation failed', detail: 'Please correct the supplier.',
      errors: { AccountId: ['Selected supplier account was not found.'] },
    })));
    const component = TestBed.createComponent(SupplierFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Local Foods', accountId: 12 });
    component.form.markAsDirty();
    component.save();
    expect(component.problem()).toBe('Please correct the supplier.');
    expect(component.fieldErrors().AccountId[0]).toContain('not found');
    expect(component.form.controls.name.value).toBe('Local Foods');
    expect(component.form.dirty).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('keeps the edit form recoverable after a concurrency conflict', () => {
    routeId = '8';
    api.update.mockReturnValue(throwError(() => ({
      status: 409, title: 'Supplier changed', detail: 'Reload the supplier and try again.', kind: 'concurrency', canRetry: false,
    })));
    const component = TestBed.createComponent(SupplierFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Edited name', accountId: 12, rowVersion: 'stale' });
    component.form.markAsDirty();
    component.save();
    expect(api.update).toHaveBeenCalledWith(8, expect.objectContaining({ rowVersion: 'stale' }));
    expect(component.problem()).toContain('Reload');
    expect(component.form.controls.name.value).toBe('Edited name');
    expect(component.form.dirty).toBe(true);
  });

  it('does not offer a latest-record link without view permission', () => {
    routeId = '8';
    session.can.mockReturnValue(false);
    api.update.mockReturnValue(throwError(() => ({ kind: 'concurrency', canRetry: false, status: 409 })));
    const component = TestBed.createComponent(SupplierFormPageComponent).componentInstance as any;
    component.form.patchValue({ name: 'Edited name', accountId: 12, rowVersion: 'stale' });
    component.save();
    expect(component.latestRecordUrl()).toBeNull();
  });
});
