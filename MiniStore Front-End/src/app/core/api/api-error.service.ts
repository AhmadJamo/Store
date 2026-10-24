import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiProblem, ApiProblemKind, NormalizedApiProblem } from './api.models';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  normalize(error: unknown): NormalizedApiProblem {
    if (error instanceof HttpErrorResponse && this.isProblem(error.error)) {
      const problem = { ...error.error, status: error.status || error.error.status };
      const kind = this.classify(problem);
      return { ...problem, kind, canRetry: kind === 'transient' };
    }

    return { status: 0, kind: 'transient', canRetry: true };
  }

  private classify(problem: ApiProblem): ApiProblemKind {
    const byCode: Record<string, ApiProblemKind> = {
      validation: 'validation', duplicate: 'duplicate', concurrency: 'concurrency',
      not_found: 'notFound', unauthorized: 'unauthorized', forbidden: 'forbidden', transient: 'transient',
    };
    if (problem.code && byCode[problem.code]) return byCode[problem.code];
    if (problem.status === 400 || problem.errors) return 'validation';
    if (problem.status === 401) return 'unauthorized';
    if (problem.status === 403) return 'forbidden';
    if (problem.status === 404) return 'notFound';
    if (problem.status === 409) return 'concurrency';
    if (problem.status === 0 || problem.status === 408 || problem.status === 429 || (problem.status ?? 0) >= 500) return 'transient';
    return 'unknown';
  }

  private isProblem(value: unknown): value is ApiProblem {
    return typeof value === 'object' && value !== null;
  }
}
