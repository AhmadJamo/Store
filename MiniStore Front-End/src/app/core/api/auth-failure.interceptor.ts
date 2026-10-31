import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ApiSessionService } from './api-session.service';

export const authFailureInterceptor: HttpInterceptorFn = (request, next) => {
  const session = inject(ApiSessionService);
  const router = inject(Router);
  return next(request).pipe(catchError(error => {
    const anonymousBootstrap =
      request.url === '/api/v1/auth/login' ||
      request.url === '/api/v1/session' ||
      request.url.startsWith('/api/v1/platform');
    if (error instanceof HttpErrorResponse && error.status === 401 && !anonymousBootstrap) {
      session.clear();
      const returnUrl = router.url.startsWith('/') && router.url !== '/login' ? router.url : undefined;
      queueMicrotask(() => void router.navigate(['/login'], {
        queryParams: returnUrl && returnUrl !== '/' ? { returnUrl } : undefined,
      }));
    }
    return throwError(() => error);
  }));
};
