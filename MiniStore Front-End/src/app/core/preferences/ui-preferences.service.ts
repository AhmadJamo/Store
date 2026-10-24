import { DOCUMENT } from '@angular/common';
import { computed, inject, Injectable, signal } from '@angular/core';

export type ThemePreference = 'system' | 'light' | 'dark';
export type ResolvedTheme = 'light' | 'dark';
export type DensityPreference = 'comfortable' | 'compact';
export type SidebarPreference = 'expanded' | 'compact';
export type ContentWidthPreference = 'fluid' | 'contained';

interface StoredPreferences {
  version: 2;
  theme: ThemePreference;
  density: DensityPreference;
  sidebar: SidebarPreference;
  contentWidth: ContentWidthPreference;
}

const storageKey = 'ministore.ui.preferences.v1';

@Injectable({ providedIn: 'root' })
export class UiPreferencesService {
  private readonly document = inject(DOCUMENT);
  private readonly systemDark = signal(false);
  readonly theme = signal<ThemePreference>('system');
  readonly density = signal<DensityPreference>('comfortable');
  readonly sidebar = signal<SidebarPreference>('expanded');
  readonly contentWidth = signal<ContentWidthPreference>('fluid');
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

  setSidebar(sidebar: SidebarPreference): void {
    if (!['expanded', 'compact'].includes(sidebar)) return;
    this.sidebar.set(sidebar);
    this.persistAndApply();
  }

  setContentWidth(contentWidth: ContentWidthPreference): void {
    if (!['fluid', 'contained'].includes(contentWidth)) return;
    this.contentWidth.set(contentWidth);
    this.persistAndApply();
  }

  reset(): void {
    this.theme.set('system');
    this.density.set('comfortable');
    this.sidebar.set('expanded');
    this.contentWidth.set('fluid');
    this.removeStoredValue();
    this.apply();
  }

  private restore(): void {
    try {
      const raw = globalThis.localStorage?.getItem(storageKey);
      if (!raw) return;
      const stored = JSON.parse(raw) as { version?: number; theme?: ThemePreference; density?: DensityPreference; sidebar?: SidebarPreference; contentWidth?: ContentWidthPreference };
      if (stored.version !== 1 && stored.version !== 2) return;
      if (stored.theme && ['system', 'light', 'dark'].includes(stored.theme)) this.theme.set(stored.theme);
      if (stored.density && ['comfortable', 'compact'].includes(stored.density)) this.density.set(stored.density);
      if (stored.version === 2 && stored.sidebar && ['expanded', 'compact'].includes(stored.sidebar)) this.sidebar.set(stored.sidebar);
      if (stored.version === 2 && stored.contentWidth && ['fluid', 'contained'].includes(stored.contentWidth)) this.contentWidth.set(stored.contentWidth);
    } catch {
      this.removeStoredValue();
    }
  }

  private persistAndApply(): void {
    try {
      const value: StoredPreferences = { version: 2, theme: this.theme(), density: this.density(), sidebar: this.sidebar(), contentWidth: this.contentWidth() };
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
    root.dataset['sidebar'] = this.sidebar();
    root.dataset['contentWidth'] = this.contentWidth();
  }
}
