import { signal } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceProfileService } from '../../core/preferences/workspace-profile.service';
import { NavigationRegistryService } from './navigation-registry.service';

describe('NavigationRegistryService', () => {
  function createRegistry(permissions: readonly string[]): NavigationRegistryService {
    const granted = new Set(permissions);
    const session = { can: (permission: string) => granted.has(permission) } as ApiSessionService;
    const profiles = { profile: signal('focused') } as unknown as WorkspaceProfileService;
    return new NavigationRegistryService(session, profiles);
  }

  it('keeps the sidebar concise while exposing permitted sales workspaces to search and tabs', () => {
    const registry = createRegistry(['SalesOrders.View']);
    const salesGroup = registry.visibleGroups().find((group) => group.id === 'sales');

    expect(salesGroup?.items.map((item) => item.id)).toEqual(['sales-center']);
    expect(registry.visibleItems().some((item) => item.id === 'sales-orders')).toBe(true);
    expect(registry.findByRoute('/sales/orders/42')?.id).toBe('sales-orders');
  });

  it('filters module destinations and hubs using the current permission set', () => {
    const registry = createRegistry(['Administration.Access']);

    expect(registry.visibleGroups().some((group) => group.id === 'accounting')).toBe(true);
    expect(registry.visibleItems().some((item) => item.id === 'chart-of-accounts')).toBe(true);
    expect(registry.visibleItems().some((item) => item.id === 'customer-payments')).toBe(false);
    expect(registry.visibleGroups().some((group) => group.id === 'sales')).toBe(false);
  });

  it('opens the accounting hub for journal readers without exposing administration settings', () => {
    const registry = createRegistry(['JournalEntries.View']);
    const accounting = registry.visibleGroups().find((group) => group.id === 'accounting');

    expect(accounting?.items.map((item) => item.id)).toEqual(['accounting-center']);
    expect(registry.visibleItems().some((item) => item.id === 'journal-entries')).toBe(true);
    expect(registry.visibleItems().some((item) => item.id === 'settings')).toBe(false);
  });
});
