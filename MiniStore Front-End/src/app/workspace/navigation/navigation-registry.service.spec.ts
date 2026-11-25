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

  it('exposes permission-filtered applications with a safe entry destination', () => {
    const registry = createRegistry(['Suppliers.View', 'ProductStock.View']);

    expect(registry.visibleApplications().map((application) => application.id)).toEqual([
      'inventory',
      'contacts',
    ]);
    expect(
      registry.visibleApplications().find((application) => application.id === 'contacts')?.entry.id,
    ).toBe('suppliers');
    expect(
      registry.visibleApplications().find((application) => application.id === 'inventory')?.entry
        .id,
    ).toBe('inventory-stock');
  });

  it('maps nested routes to an explicit application while keeping point of sale distinct from sales', () => {
    const registry = createRegistry(['Sales.View', 'SalesOrders.View']);

    expect(registry.applicationForRoute('/sales/orders/42')?.id).toBe('sales');
    expect(registry.applicationForRoute('/sales/pos')?.id).toBe('pos');
    expect(registry.applicationForRoute('/settings/accounting')?.id).toBe('settings');
  });
  it('limits current-app navigation without hiding a dedicated entry marked out of the global sidebar', () => {
    const registry = createRegistry(['Sales.View', 'SalesOrders.View']);

    expect(
      registry
        .visibleGroupsForApplication('sales')
        .flatMap((group) => group.items)
        .map((item) => item.id),
    ).toEqual(['sales-center']);
    expect(
      registry
        .visibleGroupsForApplication('pos')
        .flatMap((group) => group.items)
        .map((item) => item.id),
    ).toEqual(['point-of-sale']);
  });

  it('classifies every registered destination for section, archetype, and mobile navigation', () => {
    const registry = createRegistry([]);
    const allowedArchetypes = [
      'list',
      'record',
      'form',
      'kanban',
      'dashboard',
      'workspace',
      'settings',
    ];
    const allowedMobilePriorities = ['primary', 'secondary', 'hidden'];

    for (const item of registry.items) {
      const metadata = registry.metadataForItem(item);
      expect(metadata.sectionId, item.id).toBeTruthy();
      expect(allowedArchetypes, item.id).toContain(metadata.archetype);
      expect(allowedMobilePriorities, item.id).toContain(metadata.mobilePriority);
    }
  });

  it('keeps application entries prominent while hiding secondary hub-only destinations on mobile', () => {
    const registry = createRegistry([]);

    expect(registry.metadataForItem(registry.findById('point-of-sale')!)).toEqual({
      appId: 'pos',
      sectionId: 'immediateSales',
      archetype: 'workspace',
      mobilePriority: 'primary',
    });
    expect(registry.metadataForItem(registry.findById('sales-orders')!)).toEqual({
      appId: 'sales',
      sectionId: 'commercialPreparation',
      archetype: 'list',
      mobilePriority: 'hidden',
    });
  });
});
