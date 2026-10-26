import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ApiSessionService } from './api-session.service';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  it('preserves the requested route when redirecting an anonymous user', () => {
    const tree = { redirected: true };
    const router = { createUrlTree: vi.fn(() => tree) };
    TestBed.configureTestingModule({ providers: [
      { provide: ApiSessionService, useValue: { isAuthenticated: () => false } },
      { provide: Router, useValue: router },
    ] });
    const result = TestBed.runInInjectionContext(() => authGuard({} as never, { url: '/settings/accounting' } as never));
    expect(result).toBe(tree);
    expect(router.createUrlTree).toHaveBeenCalledWith(['/login'], { queryParams: { returnUrl: '/settings/accounting' } });
  });

  it('allows an authenticated session', () => {
    TestBed.configureTestingModule({ providers: [
      { provide: ApiSessionService, useValue: { isAuthenticated: () => true } },
      { provide: Router, useValue: { createUrlTree: vi.fn() } },
    ] });
    expect(TestBed.runInInjectionContext(() => authGuard({} as never, { url: '/' } as never))).toBe(true);
  });
});
