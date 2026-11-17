import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../../shared/ui/toast/toast.service';
import { ApiProblem } from './api.models';

export const operationErrorToastInterceptor: HttpInterceptorFn = (request, next) => {
  const toasts = inject(ToastService);
  return next(request).pipe(catchError(error => {
    if (request.method !== 'GET' && error instanceof HttpErrorResponse && error.status !== 401) {
      const problem = (typeof error.error === 'object' && error.error !== null ? error.error : {}) as ApiProblem;
      const hasFieldErrors = problem.errors && Object.keys(problem.errors).some(key => key && key !== '');
      if (!hasFieldErrors && (problem.title || problem.detail)) {
        const title = problem.title || problem.detail!;
        const detail = problem.title && problem.detail && problem.detail !== problem.title ? problem.detail : undefined;
        toasts.error(title, detail, problem.correlationId);
      }
    }
    return throwError(() => error);
  }));
};
