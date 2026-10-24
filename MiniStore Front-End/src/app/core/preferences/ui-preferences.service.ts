import { DOCUMENT } from '@angular/common';
import { computed, inject, Injectable, signal } from '@angular/core';

export type ThemePreference = 'system' | 'light' | 'dark';
export type ResolvedTheme = 'light' | 'dark';
export type DensityPreference = 'comfortable' | 'compact';

interface StoredPreferences {
  version: 1;
  theme: ThemePreference;
  density: DensityPreference;
}

const storageKey = 'ministore.ui.preferences.v1';

@Injectable({ providedIn: 'root' })
export class UiPreferencesService {
  private readonly document = inject(DOCUMENT);
  private readonly systemDark = signal(false);
  readonly theme = signal<ThemePreference>('system');
  readonly density = signal<DensityPreference>('comfortable');
  readonly resolvedTheme = computed<ResolvedTheme>(() => {
    const preference = this.theme();
    return preference === 'system' ? (this.systemDark() ? 'dark' : 'light') : preference;
  });

  private readonly mediaQuery = globalThis.matchMedia?.('(prefers-color-scheme: dark)');
  private readonly onSystemThemeChange = (event: MediaQueryListEvent): void => {
    this.systemDark.set(event.matches);
    if (this.theme() === 'system') this.apply();
  };

  constructor() {
    this.systemDark.set(this.mediaQuery?.matches ?? false);
    this.restore();
    this.mediaQuery?.addEventListener('change', this.onSystemThemeChange);
    this.apply();
  }

  setTheme(theme: ThemePreference): void {
    if (!['system', 'light', 'dark'].includes(theme)) return;
    this.theme.set(theme);
    this.persistAndApply();
  }

  setDensity(density: DensityPreference): void {
    if (!['comfortable', 'compact'].includes(density)) return;
    this.density.set(density);
    this.persistAndApply();
  }

  reset(): void {
    this.theme.set('system');
    this.density.set('comfortable');
    this.removeStoredValue();
    this.apply();
  }

  private restore(): void {
    try {
      const raw = globalThis.localStorage?.getItem(storageKey);
      if (!raw) return;
      const stored = JSON.parse(raw) as Partial<StoredPreferences>;
      if (stored.version !== 1) return;
      if (stored.theme && ['system', 'light', 'dark'].includes(stored.theme)) this.theme.set(stored.theme);
      if (stored.density && ['comfortable', 'compact'].includes(stored.density)) this.density.set(stored.density);
    } catch {
      this.removeStoredValue();
    }
  }

  private persistAndApply(): void {
    try {
      const value: StoredPreferences = { version: 1, theme: this.theme(), density: this.density() };
      globalThis.localStorage?.setItem(storageKey, JSON.stringify(value));
    } catch {
      // Preferences remain active in memory when storage is unavailable.
    }
    this.apply();
  }

  private removeStoredValue(): void {
    try { globalThis.localStorage?.removeItem(storageKey); } catch { /* in-memory fallback */ }
  }

  private apply(): void {
    const root = this.document.documentElement;
    root.dataset['theme'] = this.resolvedTheme();
    root.dataset['themePreference'] = this.theme();
    root.dataset['density'] = this.density();
  }
}
