import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { OnboardingApiService } from './onboarding-api.service';
import { RegistrationPageComponent } from './registration-page';

describe('RegistrationPageComponent', () => {
  const plan = { id: 2, code: 'PRO', nameEnglish: 'Pro', nameArabic: 'احترافية', descriptionEnglish: 'Plan', descriptionArabic: 'خطة', monthlyPrice: 10, annualPrice: 100, currency: 'JOD', limits: {}, features: [] };
  const api = { getRegistrationPage: vi.fn(() => of({ plans: [plan] })), register: vi.fn() };
  const session = { load: vi.fn(() => Promise.resolve()) };
  const router = { navigateByUrl: vi.fn(() => Promise.resolve(true)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getRegistrationPage.mockReturnValue(of({ plans: [plan] }));
    await TestBed.configureTestingModule({ imports: [RegistrationPageComponent], providers: [
      { provide: OnboardingApiService, useValue: api },
      { provide: ApiSessionService, useValue: session },
      { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
      { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: { get: () => '2' } } } },
      { provide: Router, useValue: router },
    ] }).compileComponents();
  });

  it('clears only the password after rejected registration', () => {
    api.register.mockReturnValue(throwError(() => ({ kind: 'validation', title: 'Rejected', canRetry: false })));
    const component = TestBed.createComponent(RegistrationPageComponent).componentInstance as any;
    component.form.setValue({ companyName: 'Amman Cafe', companySlug: 'amman-cafe', ownerUsername: 'owner', ownerEmail: 'owner@example.test', password: 'Wrong1!', planId: 2 });
    component.submit();
    expect(component.form.controls.companyName.value).toBe('Amman Cafe');
    expect(component.form.controls.companySlug.value).toBe('amman-cafe');
    expect(component.form.controls.password.value).toBe('');
  });

  it('reloads the cookie-backed session and opens Angular onboarding after registration', async () => {
    api.register.mockReturnValue(of({ requiresCompanyOnboarding: true }));
    const component = TestBed.createComponent(RegistrationPageComponent).componentInstance as any;
    component.form.setValue({ companyName: 'Amman Cafe', companySlug: 'amman-cafe', ownerUsername: 'owner', ownerEmail: 'owner@example.test', password: 'Secret1!', planId: 2 });
    component.submit();
    await Promise.resolve();
    await Promise.resolve();
    expect(session.load).toHaveBeenCalled();
    expect(router.navigateByUrl).toHaveBeenCalledWith('/onboarding');
    expect(component.form.controls.password.value).toBe('');
  });
});
