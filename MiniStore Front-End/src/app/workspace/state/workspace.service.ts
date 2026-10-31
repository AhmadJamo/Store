import { computed, inject, Injectable, InjectionToken, signal } from '@angular/core';
import { NavigationRegistryService } from '../navigation/navigation-registry.service';
import { CloseRequest, NavigationItem, WorkspaceOptions, WorkspaceState, WorkspaceTab } from './workspace.models';

const storageKey = 'ministore.ui.workspace.v1';
export const WORKSPACE_OPTIONS = new InjectionToken<WorkspaceOptions>('WORKSPACE_OPTIONS', {
  providedIn: 'root',
  factory: () => ({ maximumTabs: 12, maximumHistoryEntries: 25, recentlyClosedLimit: 5 }),
});

interface StoredTab {
  readonly navigationId: string;
  readonly route: string;
  readonly contextKey?: string;
  readonly history: readonly { route: string; contextKey?: string }[];
  readonly historyIndex: number;
}

interface StoredWorkspace {
  readonly version: 1;
  readonly tabs: readonly StoredTab[];
  readonly activeTabId: string | null;
}

@Injectable({ providedIn: 'root' })
export class WorkspaceService {
  private readonly options = inject(WORKSPACE_OPTIONS);
  private readonly state = signal<WorkspaceState>({ tabs: [], activeTabId: null, recentlyClosed: [] });
  private readonly pendingCloseId = signal<string | null>(null);
  private readonly pendingCloseOthersKeepId = signal<string | null>(null);

  readonly tabs = computed(() => this.state().tabs);
  readonly activeTab = computed(() => this.state().tabs.find((tab) => tab.id === this.state().activeTabId) ?? null);
  readonly recentlyClosed = computed(() => this.state().recentlyClosed);
  readonly pendingCloseTab = computed(() => this.state().tabs.find((tab) => tab.id === this.pendingCloseId()) ?? null);
  readonly canGoBack = computed(() => (this.activeTab()?.historyIndex ?? 0) > 0);
  readonly canGoForward = computed(() => {
    const tab = this.activeTab();
    return !!tab && tab.historyIndex < tab.history.length - 1;
  });
  requiresLeaveConfirmation(targetId?: string): boolean {
    const active = this.activeTab();
    return !!active?.dirty && active.id !== targetId;
  }

  constructor(private readonly registry: NavigationRegistryService) { this.restore(); }

  open(item: NavigationItem, route = item.route, contextKey?: string): WorkspaceTab | null {
    if (!this.registry.canAccess(item)) return null;
    const safeRoute = this.safeRoute(item, route);
    const id = this.tabId(item.id, contextKey);
    const existing = this.state().tabs.find((tab) => tab.id === id);
    if (existing) {
      this.state.update((current) => ({ ...current, activeTabId: id }));
      if (existing.route !== safeRoute) this.navigate(id, safeRoute, contextKey);
      else this.persist();
      return this.state().tabs.find((tab) => tab.id === id) ?? null;
    }

    const cleanContext = this.cleanContext(contextKey);
    const tab: WorkspaceTab = {
      id, navigationId: item.id, route: safeRoute, titleKey: item.labelKey, contextKey: cleanContext, dirty: false,
      history: [{ route: safeRoute, contextKey: cleanContext }], historyIndex: 0,
    };
    this.state.update((current) => {
      const removable = current.tabs.find((candidate) => !candidate.dirty && candidate.id !== current.activeTabId);
      const tabs = current.tabs.length >= this.options.maximumTabs && removable
        ? [...current.tabs.filter((candidate) => candidate.id !== removable.id), tab]
        : current.tabs.length < this.options.maximumTabs ? [...current.tabs, tab] : current.tabs;
      return tabs.includes(tab) ? { ...current, tabs, activeTabId: id } : current;
    });
    this.persist();
    return this.state().tabs.find((candidate) => candidate.id === id) ?? null;
  }

  syncRoute(route: string, contextKey?: string): WorkspaceTab | null {
    const item = this.registry.findByRoute(route);
    return item ? this.open(item, route, contextKey) : null;
  }

  activate(id: string): WorkspaceTab | null {
    const tab = this.state().tabs.find((candidate) => candidate.id === id) ?? null;
    if (tab) {
      this.state.update((current) => ({ ...current, activeTabId: id }));
      this.persist();
    }
    return tab;
  }

  navigate(id: string, route: string, contextKey?: string): WorkspaceTab | null {
    const currentTab = this.state().tabs.find((tab) => tab.id === id);
    if (!currentTab) return null;
    const item = this.registry.findById(currentTab.navigationId);
    if (!item) return null;
    const safeRoute = this.safeRoute(item, route);
    const cleanContext = this.cleanContext(contextKey);
    const currentEntry = currentTab.history[currentTab.historyIndex];
    if (currentEntry?.route === safeRoute && currentEntry.contextKey === cleanContext) return currentTab;
    const history = [...currentTab.history.slice(0, currentTab.historyIndex + 1), { route: safeRoute, contextKey: cleanContext }].slice(-this.options.maximumHistoryEntries);
    this.replaceTab(id, { ...currentTab, route: safeRoute, contextKey: cleanContext, history, historyIndex: history.length - 1 });
    return this.activate(id);
  }

  back(): WorkspaceTab | null { return this.moveHistory(-1); }
  forward(): WorkspaceTab | null { return this.moveHistory(1); }

  markDirty(id: string, dirty = true): void {
    const tab = this.state().tabs.find((candidate) => candidate.id === id);
    if (tab) this.replaceTab(id, { ...tab, dirty });
  }

  requestClose(id: string): CloseRequest {
    const tab = this.state().tabs.find((candidate) => candidate.id === id);
    if (!tab) return 'not-found';
    if (tab.dirty) {
      this.pendingCloseId.set(id);
      return 'confirmation-required';
    }
    this.closeNow(id);
    return 'closed';
  }

  confirmClose(): void {
    const id = this.pendingCloseId();
    const keepId = this.pendingCloseOthersKeepId();
    this.pendingCloseId.set(null);
    this.pendingCloseOthersKeepId.set(null);
    if (keepId) {
      for (const tab of [...this.state().tabs]) if (tab.id !== keepId) this.closeNow(tab.id);
    } else if (id) this.closeNow(id);
  }

  cancelClose(): void { this.pendingCloseId.set(null); this.pendingCloseOthersKeepId.set(null); }

  closeOthers(keepId: string): CloseRequest {
    const dirtyOther = this.state().tabs.find((tab) => tab.id !== keepId && tab.dirty);
    if (dirtyOther) {
      this.pendingCloseId.set(dirtyOther.id);
      this.pendingCloseOthersKeepId.set(keepId);
      return 'confirmation-required';
    }
    for (const tab of [...this.state().tabs]) if (tab.id !== keepId) this.closeNow(tab.id);
    return 'closed';
  }

  reopenLastClosed(): WorkspaceTab | null {
    const closed = this.state().recentlyClosed[0];
    if (!closed) return null;
    const item = this.registry.findById(closed.navigationId);
    if (!item || !this.registry.canAccess(item)) return null;
    this.state.update((current) => ({ ...current, recentlyClosed: current.recentlyClosed.slice(1) }));
    return this.open(item, closed.route, closed.contextKey);
  }

  private moveHistory(offset: number): WorkspaceTab | null {
    const tab = this.activeTab();
    if (!tab) return null;
    const index = tab.historyIndex + offset;
    const entry = tab.history[index];
    if (!entry) return null;
    const moved = { ...tab, route: entry.route, contextKey: entry.contextKey, historyIndex: index };
    this.replaceTab(tab.id, moved);
    return moved;
  }

  private closeNow(id: string): void {
    this.state.update((current) => {
      const index = current.tabs.findIndex((tab) => tab.id === id);
      if (index < 0) return current;
      const closed = current.tabs[index];
      const tabs = current.tabs.filter((tab) => tab.id !== id);
      const fallback = tabs[Math.min(index, tabs.length - 1)] ?? null;
      return {
        tabs,
        activeTabId: current.activeTabId === id ? fallback?.id ?? null : current.activeTabId,
        recentlyClosed: [{ ...closed, dirty: false }, ...current.recentlyClosed.filter((tab) => tab.id !== id)].slice(0, this.options.recentlyClosedLimit),
      };
    });
    this.persist();
  }

  private replaceTab(id: string, replacement: WorkspaceTab): void {
    this.state.update((current) => ({ ...current, tabs: current.tabs.map((tab) => tab.id === id ? replacement : tab) }));
    this.persist();
  }

  private safeRoute(item: NavigationItem, route: string): string {
    const normalized = this.registry.normalizeRoute(route);
    return normalized === item.route || (item.route !== '/' && normalized.startsWith(`${item.route}/`)) ? normalized : item.route;
  }

  private cleanContext(value?: string): string | undefined { const clean = value?.trim().slice(0, 100); return clean || undefined; }
  private tabId(navigationId: string, contextKey?: string): string { return `${navigationId}::${this.cleanContext(contextKey) ?? 'default'}`; }

  private persist(): void {
    const current = this.state();
    const stored: StoredWorkspace = {
      version: 1,
      activeTabId: current.activeTabId,
      tabs: current.tabs.map(({ navigationId, route, contextKey, history, historyIndex }) => ({ navigationId, route, contextKey, history, historyIndex })),
    };
    try { globalThis.localStorage?.setItem(storageKey, JSON.stringify(stored)); } catch { /* in-memory fallback */ }
  }

  private restore(): void {
    try {
      const raw = globalThis.localStorage?.getItem(storageKey);
      if (!raw) return;
      const stored = JSON.parse(raw) as Partial<StoredWorkspace>;
      if (stored.version !== 1 || !Array.isArray(stored.tabs)) return;
      const tabs = stored.tabs.slice(0, this.options.maximumTabs).flatMap((candidate): WorkspaceTab[] => {
        const item = typeof candidate?.navigationId === 'string' ? this.registry.findById(candidate.navigationId) : undefined;
        if (!item || !this.registry.canAccess(item) || typeof candidate.route !== 'string') return [];
        const route = this.safeRoute(item, candidate.route);
        const contextKey = this.cleanContext(candidate.contextKey);
        const history = Array.isArray(candidate.history) ? candidate.history.flatMap((entry: { route?: unknown; contextKey?: string }) => {
          if (!entry || typeof entry.route !== 'string') return [];
          return [{ route: this.safeRoute(item, entry.route), contextKey: this.cleanContext(entry.contextKey) }];
        }).slice(-this.options.maximumHistoryEntries) : [{ route, contextKey }];
        const safeHistory = history.length ? history : [{ route, contextKey }];
        const historyIndex = Math.min(Math.max(Number(candidate.historyIndex) || 0, 0), safeHistory.length - 1);
        return [{ id: this.tabId(item.id, contextKey), navigationId: item.id, route, titleKey: item.labelKey, contextKey, dirty: false, history: safeHistory, historyIndex }];
      }).filter((tab, index, all) => all.findIndex((candidate) => candidate.id === tab.id) === index);
      const activeTabId = tabs.some((tab) => tab.id === stored.activeTabId) ? stored.activeTabId! : tabs[0]?.id ?? null;
      this.state.set({ tabs, activeTabId, recentlyClosed: [] });
    } catch {
      try { globalThis.localStorage?.removeItem(storageKey); } catch { /* ignore unavailable storage */ }
    }
  }
}
