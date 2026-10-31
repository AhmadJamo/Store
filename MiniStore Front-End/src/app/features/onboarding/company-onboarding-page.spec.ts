import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { CompanyOnboardingPageComponent } from './company-onboarding-page';
import { OnboardingApiService } from './onboarding-api.service';

describe('CompanyOnboardingPageComponent', () => {
  const api = {
    getCompanyOnboarding: vi.fn(() => of({ tenantId: 7, tenantName: 'Cafe', status: 1, businessTypes: [{ value: 2, nameEnglish: 'Grocery', nameArabic: 'بقالة', descriptionEnglish: 'Fast', descriptionArabic: 'سريع', recommendedPos: true }] })),
    complete: vi.fn(() => of(undefined)),
    skip: vi.fn(() => of(undefined)),
  };
  const session = { load: vi.fn(() => Promise.resolve()) };
  const router = { navigateByUrl: vi.fn(() => Promise.resolve(true)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    await TestBed.configureTestingModule({ imports: [CompanyOnboardingPageComponent], providers: [
      { provide: OnboardingApiService, useValue: api },
      { provide: ApiSessionService, useValue: session },
      { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
      { provide: Router, useValue: router },
    ] }).compileComponents();
  });

  it('submits every setup choice to the protected complete mutation', () => {
    const component = TestBed.createComponent(CompanyOnboardingPageComponent).componentInstance as any;
    component.form.setValue({ businessType: 2, countryCode: 'JO', currency: 'JOD', language: 2, fiscalYearStartMonth: 4, isTaxRegistered: true, defaultTaxRate: 16, usePos: true, inventoryControlMode: 3 });
    component.complete();
    expect(api.complete).toHaveBeenCalledWith({ businessType: 2, countryCode: 'JO', currency: 'JOD', language: 2, fiscalYearStartMonth: 4, isTaxRegistered: true, defaultTaxRate: 16, usePos: true, inventoryControlMode: 3 });
  });

  it('requires confirmation and executes the explicit skip mutation', () => {
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    const component = TestBed.createComponent(CompanyOnboardingPageComponent).componentInstance as any;
    component.skip();
    expect(api.skip).toHaveBeenCalled();
  });
});
