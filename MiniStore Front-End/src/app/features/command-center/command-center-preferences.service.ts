import { computed, Injectable, signal } from '@angular/core';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { CommandCenterSnapshot, CommandCenterViewMode, QuickPreviewProviderId } from './command-center.models';

interface StoredCommandCenter {
  readonly version: 1;
  readonly favorites: readonly string[];
  readonly snapshots: readonly CommandCenterSnapshot[];
}

const storageKey = 'ministore.ui.command-center.v1';
const allowedProviders: readonly QuickPreviewProviderId[] = ['customers', 'suppliers'];

@Injectable({ providedIn: 'root' })
export class CommandCenterPreferencesService {
  private readonly favoriteState = signal<readonly string[]>([]);
  private readonly snapshotState = signal<readonly CommandCenterSnapshot[]>([]);
  readonly favoriteIds = this.favoriteState.asReadonly();
  readonly snapshots = this.snapshotState.asReadonly();
  readonly favoriteItems = computed(() => this.favoriteState().flatMap((id) => {
    const item = this.navigation.findById(id);
    return item && this.navigation.canAccess(item) ? [item] : [];
  }));

  constructor(private readonly navigation: NavigationRegistryService) { this.restore(); }

  isFavorite(id: string): boolean { return this.favoriteState().includes(id); }
  toggleFavorite(id: string): void {
    const item = this.navigation.findById(id);
    if (!item || !this.navigation.canAccess(item)) return;
    this.favoriteState.update((current) => current.includes(id) ? current.filter((value) => value !== id) : [...current, id].slice(-8));
    this.persist();
  }
  saveSnapshot(providerId: QuickPreviewProviderId, viewMode: CommandCenterViewMode): void {
    const item = this.navigation.findById(providerId);
    if (!allowedProviders.includes(providerId) || !item || !this.navigation.canAccess(item)) return;
    const snapshot: CommandCenterSnapshot = { id: `${providerId}-${viewMode}`, providerId, viewMode };
    this.snapshotState.update((current) => [snapshot, ...current.filter((item) => item.id !== snapshot.id)].slice(0, 6));
    this.persist();
  }
  removeSnapshot(id: string): void { this.snapshotState.update((current) => current.filter((item) => item.id !== id)); this.persist(); }
  reset(): void { this.favoriteState.set([]); this.snapshotState.set([]); try { globalThis.localStorage?.removeItem(storageKey); } catch { /* in-memory fallback */ } }

  private restore(): void {
    try {
      const raw = globalThis.localStorage?.getItem(storageKey);
      if (!raw) return;
      const stored = JSON.parse(raw) as Partial<StoredCommandCenter>;
      if (stored.version !== 1) return;
      const favorites = Array.isArray(stored.favorites) ? stored.favorites.filter((id): id is string => {
        if (typeof id !== 'string') return false;
        const item = this.navigation.findById(id);
        return !!item && this.navigation.canAccess(item);
      }).slice(0, 8) : [];
      const snapshots = Array.isArray(stored.snapshots) ? stored.snapshots.flatMap((item): CommandCenterSnapshot[] => {
        if (!item || typeof item.id !== 'string' || !allowedProviders.includes(item.providerId as QuickPreviewProviderId) || !['comfortable', 'compact'].includes(item.viewMode)) return [];
        const navigationItem = this.navigation.findById(item.providerId);
        if (!navigationItem || !this.navigation.canAccess(navigationItem)) return [];
        return [{ id: `${item.providerId}-${item.viewMode}`, providerId: item.providerId as QuickPreviewProviderId, viewMode: item.viewMode as CommandCenterViewMode }];
      }).slice(0, 6) : [];
      this.favoriteState.set([...new Set(favorites)]);
      this.snapshotState.set(snapshots.filter((item, index, all) => all.findIndex((candidate) => candidate.id === item.id) === index));
    } catch {
      this.reset();
    }
  }
  private persist(): void {
    const value: StoredCommandCenter = { version: 1, favorites: this.favoriteState(), snapshots: this.snapshotState() };
    try { globalThis.localStorage?.setItem(storageKey, JSON.stringify(value)); } catch { /* in-memory fallback */ }
  }
}
