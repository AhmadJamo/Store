import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { ApiSessionService } from './api-session.service';
import { authFailureInterceptor } from './auth-failure.interceptor';

describe('authFailureInterceptor', () => {
  it('clears an expired session and routes back to login with the current local URL', async () => {
    const session = { clear: vi.fn() };
    const router = { url: '/settings/accounting', navigate: vi.fn(() => Promise.resolve(true)) };
    TestBed.configureTestingModule({ providers: [
      { provide: ApiSessionService, useValue: session },
      { provide: Router, useValue: router },
    ] });
    TestBed.runInInjectionContext(() => authFailureInterceptor(
      new HttpRequest('GET', '/api/v1/settings/accounting'),
      () => throwError(() => new HttpErrorResponse({ status: 401 })),
    ).subscribe({ error: () => undefined }));
    await new Promise<void>(resolve => queueMicrotask(() => resolve()));
    expect(session.clear).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login'], { queryParams: { returnUrl: '/settings/accounting' } });
  });

  it('does not redirect rejected credentials from the login form', () => {
    const session = { clear: vi.fn() };
    const router = { url: '/login', navigate: vi.fn() };
    TestBed.configureTestingModule({ providers: [
      { provide: ApiSessionService, useValue: session },
      { provide: Router, useValue: router },
    ] });
    TestBed.runInInjectionContext(() => authFailureInterceptor(
      new HttpRequest('POST', '/api/v1/auth/login', {}),
      () => throwError(() => new HttpErrorResponse({ status: 401 })),
    ).subscribe({ error: () => undefined }));
    expect(session.clear).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('allows anonymous session bootstrap to fail without replacing public routes', () => {
    const session = { clear: vi.fn() };
    const router = { url: '/register', navigate: vi.fn() };
    TestBed.configureTestingModule({ providers: [
      { provide: ApiSessionService, useValue: session },
      { provide: Router, useValue: router },
    ] });
    TestBed.runInInjectionContext(() => authFailureInterceptor(
      new HttpRequest('GET', '/api/v1/session'),
      () => throwError(() => new HttpErrorResponse({ status: 401 })),
    ).subscribe({ error: () => undefined }));
    expect(session.clear).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('leaves platform authentication failures to the isolated platform flow', () => {
    const session = { clear: vi.fn() };
    const router = { url: '/platform-login', navigate: vi.fn() };
    TestBed.configureTestingModule({ providers: [
      { provide: ApiSessionService, useValue: session },
      { provide: Router, useValue: router },
    ] });
    TestBed.runInInjectionContext(() => authFailureInterceptor(
      new HttpRequest('GET', '/api/v1/platform/session'),
      () => throwError(() => new HttpErrorResponse({ status: 401 })),
    ).subscribe({ error: () => undefined }));
    expect(session.clear).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
