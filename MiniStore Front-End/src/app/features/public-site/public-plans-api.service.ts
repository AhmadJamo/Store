import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PublicPlan } from './public-site.models';

@Injectable({ providedIn: 'root' })
export class PublicPlansApiService {
  constructor(private readonly http: HttpClient) {}

  getPlans() {
    return this.http.get<PublicPlan[]>('/api/v1/public/plans');
  }
}
