import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  LegacySaleCreateInput,
  LegacySaleCreatePage,
  LegacySaleDetailsPage,
  LegacySaleSummary,
  LegacyPosCreatePage,
  PosCashPage,
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
  posForm() {
    return this.http.get<LegacyPosCreatePage>(`${this.base}/pos-form`);
  }
  createPos(input: LegacySaleCreateInput) {
    return this.http.post<{ id: number }>(`${this.base}/pos`, input);
  }
  post(id: number) {
    return this.http.post<void>(`${this.base}/${id}/post`, {});
  }
  cashPage(terminalId:number) { return this.http.get<PosCashPage>(`/api/v1/pos-cash-sessions/terminal/${terminalId}`); }
  openCash(value:unknown) { return this.http.post<{id:number}>(`/api/v1/pos-cash-sessions/open`,value); }
  cashMovement(id:number,value:unknown) { return this.http.post<void>(`/api/v1/pos-cash-sessions/${id}/movements`,value); }
  cashCount(id:number,declaredCash:number) { return this.http.post<void>(`/api/v1/pos-cash-sessions/${id}/count`,{declaredCash}); }
  approveCash(id:number,value:unknown) { return this.http.post<void>(`/api/v1/pos-cash-sessions/${id}/approve`,value); }
  linkCashSale(id:number,saleId:number) { return this.http.post<void>(`/api/v1/pos-cash-sessions/${id}/link-sale/${saleId}`,{}); }
  configureCashPolicy(value:unknown) { return this.http.post<void>(`/api/v1/pos-cash-sessions/policy`,value); }
  createCashDrawer(value:unknown) { return this.http.post<{id:number}>(`/api/v1/pos-cash-sessions/drawers`,value); }
}
