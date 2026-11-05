import { Injectable, signal } from '@angular/core';
import { NavigationGroupId } from './navigation-registry.service';

const storageKey = 'ministore.ui.navigation-groups.v1';
const allowedGroups: readonly NavigationGroupId[] = [
  'overview', 'partners', 'catalog', 'inventory', 'purchasing', 'sales', 'accounting', 'insights', 'administration',
];

@Injectable({ providedIn: 'root' })
export class NavigationGroupStateService {
  private readonly openGroups = signal<ReadonlySet<NavigationGroupId>>(this.restore());

  isOpen(groupId: NavigationGroupId): boolean { return this.openGroups().has(groupId); }

  setOpen(groupId: NavigationGroupId, open: boolean): void {
    if (!allowedGroups.includes(groupId)) return;
    const next = new Set(this.openGroups());
    open ? next.add(groupId) : next.delete(groupId);
    this.openGroups.set(next);
    this.persist(next);
  }

  ensureOpen(groupId: NavigationGroupId | null): void {
    if (groupId && !this.isOpen(groupId)) this.setOpen(groupId, true);
  }

  private restore(): ReadonlySet<NavigationGroupId> {
    try {
      const value = JSON.parse(globalThis.localStorage?.getItem(storageKey) ?? 'null') as { version?: number; open?: unknown } | null;
      if (value?.version === 1 && Array.isArray(value.open)) {
        const safe = value.open.filter((group): group is NavigationGroupId => allowedGroups.includes(group as NavigationGroupId));
        if (safe.length) return new Set(safe);
      }
    } catch {
      try { globalThis.localStorage?.removeItem(storageKey); } catch { /* storage unavailable */ }
    }
    return new Set<NavigationGroupId>(['overview']);
  }

  private persist(groups: ReadonlySet<NavigationGroupId>): void {
    try { globalThis.localStorage?.setItem(storageKey, JSON.stringify({ version: 1, open: [...groups] })); }
    catch { /* in-memory fallback */ }
  }
}
