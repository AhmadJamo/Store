import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuditEventPage, AuditEventQuery } from './audit-events.models';

@Injectable({ providedIn: 'root' })
export class AuditEventsApiService {
  private readonly baseUrl = '/api/v1/audit-events';

  constructor(private readonly http: HttpClient) {}

  getPage(query: AuditEventQuery) {
    return this.http.get<AuditEventPage>(this.baseUrl, { params: this.params(query) });
  }

  export(query: AuditEventQuery, purpose: string) {
    return this.http.post(this.baseUrl + '/export', { query, purpose }, { responseType: 'blob' });
  }

  private params(query: AuditEventQuery): HttpParams {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    for (const [key, value] of Object.entries(query)) {
      if (key !== 'page' && key !== 'pageSize' && value !== undefined && value !== null && value !== '') {
        params = params.set(key, value);
      }
    }
    return params;
  }
}
