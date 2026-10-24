import { computed, Injectable, signal } from '@angular/core';
import { WorkspaceState, WorkspaceTab } from './workspace.models';

@Injectable({ providedIn: 'root' })
export class WorkspaceService {
  private readonly state = signal<WorkspaceState>({ tabs: [], activeTabId: null });
  readonly tabs = computed(() => this.state().tabs);
  readonly activeTab = computed(() => this.state().tabs.find((tab) => tab.id === this.state().activeTabId) ?? null);

  open(tab: WorkspaceTab): void {
    this.state.update((current) => ({
      tabs: current.tabs.some((item) => item.id === tab.id) ? current.tabs : [...current.tabs, tab],
      activeTabId: tab.id,
    }));
  }

  activate(id: string): void {
    if (this.state().tabs.some((tab) => tab.id === id))
      this.state.update((current) => ({ ...current, activeTabId: id }));
  }
}
