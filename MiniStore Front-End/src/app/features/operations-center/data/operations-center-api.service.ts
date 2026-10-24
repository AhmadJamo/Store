import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OperationsCenter } from './operations-center.models';

@Injectable({ providedIn: 'root' })
export class OperationsCenterApiService {
  constructor(private readonly http: HttpClient) {}
  get() { return this.http.get<OperationsCenter>('/api/v1/operations-center'); }
}
