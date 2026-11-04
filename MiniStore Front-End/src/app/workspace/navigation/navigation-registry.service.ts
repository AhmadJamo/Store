import { computed, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationItem } from '../state/workspace.models';
import { WorkspaceProfileService } from '../../core/preferences/workspace-profile.service';

export type NavigationGroupId = 'overview' | 'partners' | 'catalog' | 'inventory' | 'purchasing' | 'insights' | 'administration';

export interface NavigationGroup {
  readonly id: NavigationGroupId;
  readonly icon: string;
  readonly items: readonly NavigationItem[];
}

const navigationGroups: readonly Omit<NavigationGroup, 'items'>[] = [
  { id: 'overview', icon: '⌂' },
  { id: 'partners', icon: '♙' },
  { id: 'catalog', icon: '▦' },
  { id: 'inventory', icon: '▥' },
  { id: 'purchasing', icon: '▣' },
  { id: 'insights', icon: '◫' },
  { id: 'administration', icon: '⚙' },
];

const navigationItems: readonly NavigationItem[] = [
  { id: 'home', route: '/', labelKey: 'dashboard', icon: '⌂' },
  {
    id: 'customers',
    route: '/customers',
    labelKey: 'customersTitle',
    icon: '♙',
    requiredPermission: 'Customers.View',
  },
  {
    id: 'suppliers',
    route: '/suppliers',
    labelKey: 'suppliersTitle',
    icon: '♧',
    requiredPermission: 'Suppliers.View',
  },
  {
    id: 'documents',
    route: '/documents',
    labelKey: 'documentEvidence',
    icon: '▤',
    requiredAnyPermissions: ['SalesOrders.View', 'CustomerInvoices.View'],
  },
  {
    id: 'work-center',
    route: '/work-center',
    labelKey: 'workCenter',
    icon: '!',
    requiredAnyPermissions: [
      'SalesOrders.Confirm',
      'CustomerInvoices.Approve',
      'CustomerInvoices.Post',
      'CustomerPayments.Create',
    ],
  },
  {
    id: 'analytics',
    route: '/analytics',
    labelKey: 'analyticsCenter',
    icon: '◫',
    requiredPermission: 'AnalyticsCenter.View',
  },
  {
    id: 'settings',
    route: '/settings',
    labelKey: 'settings',
    icon: '⚙',
    requiredPermission: 'Administration.Access',
  },
  {
    id: 'users',
    route: '/users',
    labelKey: 'usersAdministration',
    icon: '♟',
    requiredPermission: 'Users.View',
  },
  {
    id: 'roles',
    route: '/roles',
    labelKey: 'rolesPermissions',
    icon: '♜',
    requiredPermission: 'Roles.View',
  },
  {
    id: 'products',
    route: '/products',
    labelKey: 'products',
    icon: '▦',
    requiredPermission: 'Products.View',
  },
  {
    id: 'audit-events',
    route: '/audit-events',
    labelKey: 'auditEvents',
    icon: '◎',
    requiredPermission: 'AuditEvents.View',
  },
  {
    id: 'inventory-ledger',
    route: '/inventory/transactions',
    labelKey: 'inventoryLedger',
    icon: '⇄',
    requiredPermission: 'StockTransactions.View',
  },
  {
    id: 'inventory-stock',
    route: '/inventory/stocks',
    labelKey: 'inventoryStock',
    icon: '▥',
    requiredPermission: 'ProductStock.View',
  },
  {
    id: 'inventory-availability',
    route: '/inventory/availability',
    labelKey: 'inventoryAvailability',
    icon: '▦',
    requiredPermission: 'InventoryBalances.View',
  },
  {
    id: 'inventory-reservations',
    route: '/inventory/reservations',
    labelKey: 'inventoryReservations',
    icon: '⌁',
    requiredPermission: 'InventoryReservations.View',
  },
  {
    id: 'stock-transfers',
    route: '/inventory/transfers',
    labelKey: 'stockTransfers',
    icon: '⇄',
    requiredPermission: 'StockTransfers.View',
  },
  {
    id: 'inventory-scanning',
    route: '/inventory/scanning',
    labelKey: 'inventoryScanning',
    icon: '▣',
    requiredPermission: 'InventoryScanning.View',
  },
  { id: 'inventory-replenishment', route: '/inventory/replenishment', labelKey: 'replenishment', icon: '↥', requiredPermission: 'Inventory.Replenishment.View' },
  {
    id: 'inventory-insights',
    route: '/inventory/insights',
    labelKey: 'inventoryInsights',
    icon: '◫',
    requiredPermission: 'InventoryInsights.View',
  },
  {
    id: 'inventory-reconciliation',
    route: '/inventory/reconciliation',
    labelKey: 'inventoryReconciliation',
    icon: '≋',
    requiredPermission: 'InventoryReconciliation.View',
  },
  {
    id: 'inventory-adjustments',
    route: '/inventory/adjustments',
    labelKey: 'inventoryAdjustments',
    icon: '✓',
    requiredPermission: 'InventoryAdjustments.View',
  },
  {
    id: 'inventory-tracking',
    route: '/inventory/tracking',
    labelKey: 'inventoryTracking',
    icon: '⌘',
    requiredPermission: 'InventoryTracking.View',
  },
  {
    id: 'warehouse-locations',
    route: '/inventory/locations',
    labelKey: 'warehouseLocations',
    icon: '⌗',
    requiredPermission: 'Warehouses.View',
  },
  {
    id: 'inventory-putaway',
    route: '/inventory/putaway',
    labelKey: 'inventoryPutaway',
    icon: '⇲',
    requiredPermission: 'ProductStock.View',
  },
  {
    id: 'location-movements',
    route: '/inventory/location-movements',
    labelKey: 'locationMovements',
    icon: '⇆',
    requiredAnyPermissions: ['LocationMovements.View', 'StockMovements.View'],
  },
  {
    id: 'purchase-requests',
    route: '/purchases/requests',
    labelKey: 'purchaseRequests',
    icon: '▣',
    requiredPermission: 'PurchaseRequests.View',
  },
  {
    id: 'operations-center',
    route: '/operations-center',
    labelKey: 'operationsCenter',
    icon: '◉',
    requiredAnyPermissions: [
      'PurchaseReplenishment.View',
      'SupplierIntegrations.View',
      'AuditEvents.View',
      'AnalyticsCenter.View',
    ],
  },
  { id: 'design-system', route: '/design-system', labelKey: 'designSystem', icon: '◇' },
];

@Injectable({ providedIn: 'root' })
export class NavigationRegistryService {
  readonly items = navigationItems;
  readonly visibleItems = computed(() => {
    const visible = this.items.filter((item) => this.canAccess(item));
    const priorities: Record<string, readonly string[]> = {
      focused: [
        'home',
        'customers',
        'suppliers',
        'purchase-requests',
        'products',
        'inventory-stock',
        'inventory-availability',
        'inventory-reservations',
        'stock-transfers',
        'inventory-scanning',
        'inventory-replenishment',
        'inventory-insights',
        'inventory-reconciliation',
        'inventory-adjustments',
        'inventory-tracking',
        'warehouse-locations',
        'inventory-putaway',
        'location-movements',
        'inventory-ledger',
        'documents',
        'work-center',
        'analytics',
        'settings',
        'users',
        'roles',
        'audit-events',
        'operations-center',
        'design-system',
      ],
      operator: [
        'work-center',
        'operations-center',
        'purchase-requests',
        'stock-transfers',
        'inventory-scanning',
        'inventory-replenishment',
        'inventory-insights',
        'inventory-reconciliation',
        'inventory-adjustments',
        'inventory-putaway',
        'location-movements',
        'inventory-tracking',
        'warehouse-locations',
        'inventory-stock',
        'inventory-availability',
        'inventory-reservations',
        'inventory-ledger',
        'documents',
        'products',
        'customers',
        'suppliers',
        'home',
        'analytics',
        'settings',
        'users',
        'roles',
        'audit-events',
        'design-system',
      ],
      analyst: [
        'analytics',
        'purchase-requests',
        'inventory-availability',
        'inventory-reservations',
        'stock-transfers',
        'inventory-scanning',
        'inventory-replenishment',
        'inventory-insights',
        'inventory-reconciliation',
        'inventory-adjustments',
        'inventory-tracking',
        'location-movements',
        'inventory-ledger',
        'inventory-stock',
        'warehouse-locations',
        'inventory-putaway',
        'operations-center',
        'documents',
        'work-center',
        'home',
        'products',
        'customers',
        'suppliers',
        'settings',
        'users',
        'roles',
        'audit-events',
        'design-system',
      ],
    };
    const order = priorities[this.profiles.profile()];
    return [...visible].sort((a, b) => order.indexOf(a.id) - order.indexOf(b.id));
  });
  readonly visibleGroups = computed<readonly NavigationGroup[]>(() => {
    const visible = this.visibleItems();
    return navigationGroups.flatMap((group): NavigationGroup[] => {
      const items = visible.filter((item) => this.groupForItem(item) === group.id);
      return items.length ? [{ ...group, items }] : [];
    });
  });

  constructor(
    private readonly session: ApiSessionService,
    private readonly profiles: WorkspaceProfileService,
  ) {}

  findById(id: string): NavigationItem | undefined {
    return this.items.find((item) => item.id === id);
  }

  groupForItem(item: NavigationItem): NavigationGroupId {
    if (item.route.startsWith('/inventory/')) return 'inventory';
    if (item.route.startsWith('/purchases/')) return 'purchasing';
    if (item.id === 'customers' || item.id === 'suppliers') return 'partners';
    if (item.id === 'products') return 'catalog';
    if (item.id === 'documents' || item.id === 'analytics') return 'insights';
    if (['settings', 'users', 'roles', 'audit-events', 'design-system'].includes(item.id)) return 'administration';
    return 'overview';
  }

  groupForRoute(route: string): NavigationGroupId | null {
    const item = this.findByRoute(route);
    return item ? this.groupForItem(item) : null;
  }

  findByRoute(route: string): NavigationItem | undefined {
    const cleanRoute = this.normalizeRoute(route);
    return [...this.items]
      .sort((left, right) => right.route.length - left.route.length)
      .find((item) =>
        item.route === '/'
          ? cleanRoute === '/'
          : cleanRoute === item.route || cleanRoute.startsWith(`${item.route}/`),
      );
  }

  canAccess(item: NavigationItem): boolean {
    return (
      (!item.requiredPermission || this.session.can(item.requiredPermission)) &&
      (!item.requiredAnyPermissions?.length ||
        item.requiredAnyPermissions.some((permission) => this.session.can(permission)))
    );
  }

  normalizeRoute(route: string): string {
    const path = route.split(/[?#]/, 1)[0] || '/';
    const normalized = `/${path}`.replace(/\/{2,}/g, '/').replace(/\/$/, '');
    return normalized || '/';
  }
}
