import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AccountApiService } from '../../core/api/account-api.service';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LoginPageComponent } from './login-page';

describe('LoginPageComponent', () => {
  const account = { login: vi.fn() };
  const session = { load: vi.fn(() => Promise.resolve()) };
  const router = { navigateByUrl: vi.fn(() => Promise.resolve(true)) };

  beforeEach(async () => {
    vi.clearAllMocks();
    await TestBed.configureTestingModule({ imports: [LoginPageComponent], providers: [
      { provide: AccountApiService, useValue: account },
      { provide: ApiSessionService, useValue: session },
      { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
      { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: { get: () => '/settings/accounting' } } } },
      { provide: Router, useValue: router },
    ] }).compileComponents();
  });

  it('preserves company and user fields but clears the password after a rejected login', () => {
    account.login.mockReturnValue(throwError(() => ({ detail: 'Invalid credentials.' })));
    const component = TestBed.createComponent(LoginPageComponent).componentInstance as any;
    component.form.setValue({ companyCode: 'amman-cafe', userName: 'operator', password: 'wrong' });
    component.submit();
    expect(component.form.controls.companyCode.value).toBe('amman-cafe');
    expect(component.form.controls.userName.value).toBe('operator');
    expect(component.form.controls.password.value).toBe('');
    expect(component.problem()).toBe('Invalid credentials.');
  });

  it('reloads the server session and returns to the requested local route', async () => {
    account.login.mockReturnValue(of({ requiresCompanyOnboarding: false }));
    const component = TestBed.createComponent(LoginPageComponent).componentInstance as any;
    component.form.setValue({ companyCode: '', userName: 'operator', password: 'correct' });
    component.submit();
    await Promise.resolve();
    await Promise.resolve();
    expect(session.load).toHaveBeenCalled();
    expect(router.navigateByUrl).toHaveBeenCalledWith('/settings/accounting');
    expect(component.form.controls.password.value).toBe('');
  });

  it('opens Angular onboarding when the server says setup is pending', async () => {
    account.login.mockReturnValue(of({ requiresCompanyOnboarding: true }));
    const component = TestBed.createComponent(LoginPageComponent).componentInstance as any;
    component.form.setValue({ companyCode: 'amman-cafe', userName: 'owner', password: 'correct' });
    component.submit();
    await Promise.resolve();
    await Promise.resolve();
    expect(session.load).toHaveBeenCalled();
    expect(router.navigateByUrl).toHaveBeenCalledWith('/onboarding');
  });
});
