import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  CreateSubscriptionCheckout,
  SubscriptionCheckoutQuote,
  SubscriptionPage,
} from './subscription.models';

@Injectable({ providedIn: 'root' })
export class SubscriptionApiService {
  private readonly base = '/api/v1/subscription';

  constructor(private readonly http: HttpClient) {}

  get() {
    return this.http.get<SubscriptionPage>(this.base);
  }

  checkout(input: CreateSubscriptionCheckout) {
    return this.http.post<SubscriptionCheckoutQuote>(`${this.base}/checkout`, input);
  }
}
