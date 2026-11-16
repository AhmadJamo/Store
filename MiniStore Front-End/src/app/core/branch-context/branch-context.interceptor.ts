import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BranchContextService } from './branch-context.service';

export const branchContextInterceptor: HttpInterceptorFn = (request, next) => {
  const branchId = inject(BranchContextService).selectedId();
  if (!branchId || !request.url.startsWith('/api/')) return next(request);
  return next(request.clone({ setHeaders: { 'X-MiniStore-Branch-Id': String(branchId) } }));
};
