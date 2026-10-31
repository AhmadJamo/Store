import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { CustomerFormPageComponent } from './customer-form-page';
import { CustomerApiService } from './data/customer-api.service';

describe('CustomerFormPageComponent', () => {
  const page = {
    form: { id: 0, name: '', type: 1, preferredLanguage: 'ar-JO', timeZoneId: 'Asia/Amman', paymentTermDays: 0, isTaxExempt: false, rowVersion: '' },
    accounts: [], currencies: ['JOD'], taxRates: [], priceLists: [],
  };
  const api = { getForm: vi.fn(() => of(page)), create: vi.fn(), update: vi.fn() };
  const router = { navigate: vi.fn(() => Promise.resolve(true)) };
  const workspace = { activeTab: vi.fn(() => ({ id: 'customers::default' })), markDirty: vi.fn() };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getForm.mockReturnValue(of(page));
    await TestBed.configureTestingModule({
      imports: [CustomerFormPageComponent],
      providers: [
        { provide: CustomerApiService, useValue: api },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => null } } } },
        { provide: Router, useValue: router },
        { provide: WorkspaceService, useValue: workspace },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ApiSessionService, useValue: { can: () => true } },
      ],
    }).compileComponents();
  });

  it('clears dirty state and navigates after a successful save', () => {
    api.create.mockReturnValue(of({ id: 7 }));
    const component = TestBed.createComponent(CustomerFormPageComponent).componentInstance as any;
    component.form.controls.name.setValue('Acme');
    component.form.markAsDirty();
    component.save();
    expect(api.create).toHaveBeenCalled();
    expect(workspace.markDirty).toHaveBeenLastCalledWith('customers::default', false);
    expect(router.navigate).toHaveBeenCalledWith(['/customers', 7]);
  });

  it('keeps the form dirty and exposes a failed save problem', () => {
    api.create.mockReturnValue(throwError(() => ({ title: 'Invalid customer', errors: { Name: ['Required'] } })));
    const component = TestBed.createComponent(CustomerFormPageComponent).componentInstance as any;
    component.form.controls.name.setValue('Acme');
    component.form.markAsDirty();
    component.save();
    expect(component.problem()).toBe('Invalid customer');
    expect(component.form.dirty).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
