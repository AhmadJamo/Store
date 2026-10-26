import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TenantLoginRequest, TenantLoginResult } from './api.models';

@Injectable({ providedIn: 'root' })
export class AccountApiService {
  constructor(private readonly http: HttpClient) {}

  login(request: TenantLoginRequest): Observable<TenantLoginResult> {
    return this.http.post<TenantLoginResult>('/api/v1/auth/login', request);
  }

  logout(): Observable<void> {
    return this.http.post<void>('/api/v1/auth/logout', {});
  }
}
