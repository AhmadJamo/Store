import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AntiforgeryToken } from './api.models';

@Injectable({ providedIn: 'root' })
export class ApiSecurityService {
  private readonly antiforgery = signal<AntiforgeryToken | null>(null);

  constructor(private readonly http: HttpClient) {}

  async initialize(): Promise<void> {
    this.antiforgery.set(
      await firstValueFrom(this.http.get<AntiforgeryToken>('/api/v1/security/csrf'))
    );
  }

  get token(): AntiforgeryToken | null {
    return this.antiforgery();
  }
}
