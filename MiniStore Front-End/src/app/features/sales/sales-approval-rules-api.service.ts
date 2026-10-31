import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApprovalRulesPage, CreateApprovalRule } from './sales-approval-rules.models';
@Injectable({ providedIn: 'root' })
export class SalesApprovalRulesApiService {
  private readonly base = '/api/v1/sales-approval-rules';
  constructor(private readonly http: HttpClient) {}
  page() {
    return this.http.get<ApprovalRulesPage>(this.base);
  }
  create(input: CreateApprovalRule) {
    return this.http.post<void>(this.base, input);
  }
  setActive(id: number, active: boolean, rowVersion: string) {
    return this.http.post<void>(`${this.base}/${id}/active`, { id, active, rowVersion });
  }
}
