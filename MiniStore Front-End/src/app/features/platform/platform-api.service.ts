import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PlatformOverview, PlatformSession } from './platform.models';

@Injectable({ providedIn: 'root' })
export class PlatformApiService {
  private readonly base = '/api/v1/platform';
  constructor(private readonly http: HttpClient) {}
  login(input: { username: string; password: string }) {
    return this.http.post<PlatformSession>(`${this.base}/login`, input);
  }
  session() {
    return this.http.get<PlatformSession>(`${this.base}/session`);
  }
  overview() {
    return this.http.get<PlatformOverview>(`${this.base}/overview`);
  }
  logout() {
    return this.http.post<void>(`${this.base}/logout`, {});
  }
}
