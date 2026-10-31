import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InventoryAvailabilityPage, InventoryAvailabilityQuery, InventoryReservationPage, InventoryReservationQuery } from './inventory-availability.models';

@Injectable({ providedIn: 'root' })
export class InventoryAvailabilityApiService {
  constructor(private readonly http: HttpClient) {}

  getBalances(query: InventoryAvailabilityQuery) {
    return this.http.get<InventoryAvailabilityPage>('/api/v1/inventory-balances', { params: this.params(query) });
  }

  getReservations(query: InventoryReservationQuery) {
    return this.http.get<InventoryReservationPage>('/api/v1/inventory-reservations', { params: this.params(query) });
  }

  private params(query: object): HttpParams {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) if (value !== undefined && value !== '') params = params.set(key, String(value));
    return params;
  }
}
