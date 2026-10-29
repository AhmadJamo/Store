import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  LegacySaleCreateInput,
  LegacySaleCreatePage,
  LegacySaleDetailsPage,
  LegacySaleSummary,
} from './legacy-sales.models';

@Injectable({ providedIn: 'root' })
export class LegacySalesApiService {
  private readonly base = '/api/v1/legacy-sales';
  constructor(private readonly http: HttpClient) {}
  list() {
    return this.http.get<LegacySaleSummary[]>(this.base);
  }
  get(id: number) {
    return this.http.get<LegacySaleDetailsPage>(`${this.base}/${id}`);
  }
  createForm() {
    return this.http.get<LegacySaleCreatePage>(`${this.base}/create-form`);
  }
  create(input: LegacySaleCreateInput) {
    return this.http.post<{ id: number }>(this.base, input);
  }
  post(id: number) {
    return this.http.post<void>(`${this.base}/${id}/post`, {});
  }
}
