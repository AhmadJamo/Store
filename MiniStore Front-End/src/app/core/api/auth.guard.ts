import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ApiSessionService } from './api-session.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const session = inject(ApiSessionService);
  if (session.isAuthenticated()) return true;
  return inject(Router).createUrlTree(['/login'], {
    queryParams: state.url && state.url !== '/' ? { returnUrl: state.url } : undefined,
  });
};

export const anonymousOnlyGuard: CanActivateFn = () => {
  const session = inject(ApiSessionService);
  return session.isAuthenticated() ? inject(Router).createUrlTree(['/']) : true;
};
