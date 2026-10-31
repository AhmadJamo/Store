import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  InventoryScanPage,
  InventoryScanQuery,
  ScannedInventoryCount,
  ScannedPutaway,
  ScannedRelocation,
} from './inventory-scanning.models';

@Injectable({ providedIn: 'root' })
export class InventoryScanningApiService {
  private readonly baseUrl = '/api/v1/inventory-scanning';
  constructor(private readonly http: HttpClient) {}

  resolve(query: InventoryScanQuery) {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value.trim()) params = params.set(key, value.trim());
    }
    return this.http.get<InventoryScanPage>(this.baseUrl, { params });
  }

  putaway(input: ScannedPutaway) {
    return this.http.post<void>(this.baseUrl + '/putaway', input);
  }

  relocate(input: ScannedRelocation) {
    return this.http.post<void>(this.baseUrl + '/relocate', input);
  }

  count(input: ScannedInventoryCount) {
    return this.http.post<void>(this.baseUrl + '/count', input);
  }
}
