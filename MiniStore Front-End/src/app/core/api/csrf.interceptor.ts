import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ApiSecurityService } from './api-security.service';

const safeMethods = new Set(['GET', 'HEAD', 'OPTIONS']);

export const csrfInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(ApiSecurityService).token;
  if (!request.url.startsWith('/api/') || safeMethods.has(request.method.toUpperCase()) || !token) {
    return next(request);
  }

  return next(request.clone({ setHeaders: { [token.headerName]: token.token } }));
};
