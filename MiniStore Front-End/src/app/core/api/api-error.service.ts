import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiProblem } from './api.models';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  normalize(error: unknown): ApiProblem {
    if (error instanceof HttpErrorResponse && this.isProblem(error.error)) {
      return { ...error.error, status: error.status || error.error.status };
    }

    return { status: 0 };
  }

  private isProblem(value: unknown): value is ApiProblem {
    return typeof value === 'object' && value !== null;
  }
}
