import { computed, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationItem } from '../state/workspace.models';
import { WorkspaceProfileService } from '../../core/preferences/workspace-profile.service';

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
        'operations-center',
        'design-system',
      ],
      operator: [
        'work-center',
        'operations-center',
        'purchase-requests',
        'stock-transfers',
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
        'design-system',
      ],
      analyst: [
        'analytics',
        'purchase-requests',
        'inventory-availability',
        'inventory-reservations',
        'stock-transfers',
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
        'design-system',
      ],
    };
    const order = priorities[this.profiles.profile()];
    return [...visible].sort((a, b) => order.indexOf(a.id) - order.indexOf(b.id));
  });

  constructor(
    private readonly session: ApiSessionService,
    private readonly profiles: WorkspaceProfileService,
  ) {}

  findById(id: string): NavigationItem | undefined {
    return this.items.find((item) => item.id === id);
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
