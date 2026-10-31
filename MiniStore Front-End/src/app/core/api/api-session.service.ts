import { HttpClient } from '@angular/common/http';
import { computed, effect, Injectable, signal, untracked } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiErrorService } from './api-error.service';
import { ApiProblem, ApiSession } from './api.models';
import { UiCultureStateService } from '../localization/ui-culture-state.service';

@Injectable({ providedIn: 'root' })
export class ApiSessionService {
  private readonly sessionState = signal<ApiSession | null>(null);
  private readonly errorState = signal<ApiProblem | null>(null);
  private readonly loadingState = signal(false);

  readonly session = this.sessionState.asReadonly();
  readonly error = this.errorState.asReadonly();
  readonly loading = this.loadingState.asReadonly();
  readonly isAuthenticated = computed(() => this.sessionState() !== null);

  constructor(
    private readonly http: HttpClient,
    private readonly errors: ApiErrorService,
    private readonly cultureState: UiCultureStateService,
  ) {
    effect(() => {
      const culture = this.cultureState.locale();
      untracked(() => this.sessionState.update(session => session
        ? { ...session, culture, direction: culture === 'ar-JO' ? 'rtl' : 'ltr' }
        : session));
    });
  }

  async load(): Promise<void> {
    this.loadingState.set(true);
    this.errorState.set(null);
    try {
      const session = await firstValueFrom(this.http.get<ApiSession>('/api/v1/session'));
      if (session.culture === 'ar-JO' || session.culture === 'en-US') this.cultureState.set(session.culture);
      this.sessionState.set(session);
    } catch (error) {
      this.sessionState.set(null);
      this.errorState.set(this.errors.normalize(error));
      throw error;
    } finally {
      this.loadingState.set(false);
    }
  }

  can(permission: string): boolean {
    return this.sessionState()?.permissions.includes(permission) ?? false;
  }

  clear(): void {
    this.sessionState.set(null);
    this.errorState.set(null);
  }
}
