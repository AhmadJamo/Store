import { Injectable, signal } from '@angular/core';

export type UiLocale = 'ar-JO' | 'en-US';

@Injectable({ providedIn: 'root' })
export class UiCultureStateService {
  private readonly value = signal<UiLocale>(this.restore());
  readonly locale = this.value.asReadonly();

  set(locale: UiLocale): void {
    this.value.set(locale);
    try { globalThis.localStorage?.setItem('ministore.ui.locale', locale); } catch { /* in-memory fallback */ }
  }

  private restore(): UiLocale {
    try { return globalThis.localStorage?.getItem('ministore.ui.locale') === 'en-US' ? 'en-US' : 'ar-JO'; }
    catch { return 'ar-JO'; }
  }
}
