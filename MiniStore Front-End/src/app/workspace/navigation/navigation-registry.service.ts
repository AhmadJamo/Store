import { computed, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationItem } from '../state/workspace.models';
import { WorkspaceProfileService } from '../../core/preferences/workspace-profile.service';

export type NavigationGroupId = 'overview' | 'partners' | 'catalog' | 'inventory' | 'purchasing' | 'sales' | 'accounting' | 'insights' | 'administration';

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
  { id: 'sales', icon: '◈' },
  { id: 'accounting', icon: '▤' },
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
    id: 'purchasing-center',
    route: '/purchases',
    labelKey: 'purchasingCenter',
    icon: '▣',
    requiredAnyPermissions: ['PurchaseRequests.View', 'PurchaseSourcing.View', 'PurchaseOrders.View', 'GoodsReceipts.View', 'VendorBills.View', 'SupplierPayments.View', 'PurchaseAnalytics.View'],
  },
  {
    id: 'purchase-requests',
    route: '/purchases/requests',
    labelKey: 'purchaseRequests',
    icon: '▣',
    requiredPermission: 'PurchaseRequests.View',
    showInSidebar: false,
    hubSectionKey: 'planningAndSourcing',
  },
  { id: 'purchase-approval-rules', route: '/purchases/approval-rules', labelKey: 'purchaseApprovalRules', icon: '✓', requiredPermission: 'PurchaseApprovalRules.View', showInSidebar: false, hubSectionKey: 'planningAndSourcing' },
  { id: 'purchase-sourcing', route: '/purchases/sourcing', labelKey: 'purchaseSourcingWorkspace', icon: '⌕', requiredPermission: 'PurchaseSourcing.View', showInSidebar: false, hubSectionKey: 'planningAndSourcing' },
  { id: 'purchase-orders', route: '/purchases/orders', labelKey: 'purchaseOrders', icon: '▤', requiredPermission: 'PurchaseOrders.View', showInSidebar: false, hubSectionKey: 'planningAndSourcing' },
  { id: 'purchase-agreements', route: '/purchases/agreements', labelKey: 'purchaseAgreements', icon: '≡', requiredPermission: 'PurchaseAgreements.View', showInSidebar: false, hubSectionKey: 'planningAndSourcing' },
  { id: 'purchase-templates', route: '/purchases/templates', labelKey: 'purchaseTemplates', icon: '▧', requiredPermission: 'PurchaseTemplates.View', showInSidebar: false, hubSectionKey: 'planningAndSourcing' },
  { id: 'purchase-replenishment', route: '/purchases/replenishment', labelKey: 'purchaseReplenishment', icon: '↥', requiredPermission: 'PurchaseReplenishment.View', showInSidebar: false, hubSectionKey: 'planningAndSourcing' },
  { id: 'goods-receipts', route: '/purchases/receipts', labelKey: 'goodsReceipts', icon: '⇲', requiredPermission: 'GoodsReceipts.View', showInSidebar: false, hubSectionKey: 'receivingAndFulfillment' },
  { id: 'purchase-service-acceptances', route: '/purchases/service-acceptances', labelKey: 'serviceAcceptances', icon: '✓', requiredPermission: 'PurchaseServiceAcceptances.View', showInSidebar: false, hubSectionKey: 'receivingAndFulfillment' },
  { id: 'purchase-receipt-returns', route: '/purchases/receipt-returns', labelKey: 'purchaseReturns', icon: '↶', requiredPermission: 'GoodsReceiptReturns.View', showInSidebar: false, hubSectionKey: 'receivingAndFulfillment' },
  { id: 'landed-costs', route: '/purchases/landed-costs', labelKey: 'landedCosts', icon: '＋', requiredPermission: 'LandedCosts.View', showInSidebar: false, hubSectionKey: 'receivingAndFulfillment' },
  { id: 'vendor-bills', route: '/purchases/bills', labelKey: 'vendorBills', icon: '▥', requiredPermission: 'VendorBills.View', showInSidebar: false, hubSectionKey: 'payablesAndControl' },
  { id: 'supplier-credit-notes', route: '/purchases/credit-notes', labelKey: 'supplierCreditNotes', icon: '−', requiredPermission: 'SupplierCreditNotes.View', showInSidebar: false, hubSectionKey: 'payablesAndControl' },
  { id: 'supplier-payments', route: '/purchases/payments', labelKey: 'supplierPayments', icon: '¤', requiredPermission: 'SupplierPayments.View', showInSidebar: false, hubSectionKey: 'payablesAndControl' },
  { id: 'purchase-reversals', route: '/purchases/reversals', labelKey: 'purchaseReversals', icon: '↺', requiredPermission: 'PurchaseDocumentReversals.View', showInSidebar: false, hubSectionKey: 'payablesAndControl' },
  { id: 'purchase-reconciliation', route: '/purchases/accounting-reconciliation', labelKey: 'purchaseAccountingReconciliation', icon: '≋', requiredPermission: 'PurchaseAccountingReconciliation.View', showInSidebar: false, hubSectionKey: 'payablesAndControl' },
  { id: 'supplier-terms', route: '/purchases/supplier-terms', labelKey: 'supplierTerms', icon: '♧', requiredPermission: 'Purchases.SupplierTerms.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  { id: 'supplier-term-exceptions', route: '/purchases/supplier-terms/exceptions', labelKey: 'supplierTermExceptions', icon: '!', requiredPermission: 'Purchases.SupplierTerms.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  { id: 'supplier-communications', route: '/purchases/communications/prepare', labelKey: 'supplierCommunications', icon: '✉', requiredPermission: 'SupplierCommunications.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  { id: 'supplier-integrations', route: '/purchases/integrations', labelKey: 'supplierIntegrations', icon: '⇆', requiredPermission: 'SupplierIntegrations.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  { id: 'purchase-analytics', route: '/purchases/analytics', labelKey: 'purchaseAnalytics', icon: '◫', requiredPermission: 'PurchaseAnalytics.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  { id: 'legacy-purchases', route: '/purchases/legacy', labelKey: 'legacyPurchases', icon: '◷', requiredPermission: 'Purchases.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  { id: 'legacy-purchase-returns', route: '/purchases/legacy-returns', labelKey: 'legacyPurchaseReturns', icon: '↶', requiredPermission: 'PurchaseReturns.View', showInSidebar: false, hubSectionKey: 'supplierOperations' },
  {
    id: 'sales-center',
    route: '/sales',
    labelKey: 'salesCenter',
    icon: '◈',
    requiredAnyPermissions: ['Sales.View', 'SalesPricing.View', 'SalesQuotations.View', 'SalesOrders.View', 'SalesDeliveries.View', 'CustomerInvoices.View', 'CustomerPayments.View'],
  },
  { id: 'sales-pricing', route: '/sales/pricing', labelKey: 'salesPricing', icon: '¤', requiredPermission: 'SalesPricing.View', showInSidebar: false, hubSectionKey: 'commercialPreparation' },
  { id: 'sales-quotation-templates', route: '/sales/quotation-templates', labelKey: 'salesQuotationTemplates', icon: '▧', requiredPermission: 'SalesQuotationTemplates.View', showInSidebar: false, hubSectionKey: 'commercialPreparation' },
  { id: 'sales-quotations', route: '/sales/quotations', labelKey: 'salesQuotations', icon: '▤', requiredPermission: 'SalesQuotations.View', showInSidebar: false, hubSectionKey: 'commercialPreparation' },
  { id: 'sales-approval-rules', route: '/sales/approval-rules', labelKey: 'salesApprovalRules', icon: '✓', requiredPermission: 'SalesApprovalRules.View', showInSidebar: false, hubSectionKey: 'commercialPreparation' },
  { id: 'sales-orders', route: '/sales/orders', labelKey: 'salesOrders', icon: '▣', requiredPermission: 'SalesOrders.View', showInSidebar: false, hubSectionKey: 'commercialPreparation' },
  { id: 'sales-deliveries', route: '/sales/deliveries', labelKey: 'salesDeliveries', icon: '⇱', requiredPermission: 'SalesDeliveries.View', showInSidebar: false, hubSectionKey: 'salesFulfillment' },
  { id: 'customer-invoices', route: '/sales/invoices', labelKey: 'customerInvoices', icon: '▥', requiredPermission: 'CustomerInvoices.View', showInSidebar: false, hubSectionKey: 'receivablesAndCollection' },
  { id: 'customer-payments', route: '/sales/payments', labelKey: 'customerPayments', icon: '¤', requiredPermission: 'CustomerPayments.View', showInSidebar: false, hubSectionKey: 'receivablesAndCollection' },
  { id: 'customer-return-requests', route: '/sales/return-requests', labelKey: 'customerReturnRequests', icon: '↶', requiredPermission: 'CustomerReturnRequests.View', showInSidebar: false, hubSectionKey: 'returnsAndCredits' },
  { id: 'customer-return-receipts', route: '/sales/return-receipts', labelKey: 'customerReturnReceipts', icon: '⇲', requiredPermission: 'CustomerReturnReceipts.View', showInSidebar: false, hubSectionKey: 'returnsAndCredits' },
  { id: 'customer-credit-notes', route: '/sales/credit-notes', labelKey: 'customerCreditNotes', icon: '−', requiredPermission: 'CustomerCreditNotes.View', showInSidebar: false, hubSectionKey: 'returnsAndCredits' },
  { id: 'direct-sales', route: '/sales/legacy', labelKey: 'directSales', icon: '◷', requiredPermission: 'Sales.View', showInSidebar: false, hubSectionKey: 'immediateSales' },
  { id: 'point-of-sale', route: '/sales/pos', labelKey: 'pointOfSale', icon: '▦', requiredPermission: 'Sales.View', showInSidebar: false, hubSectionKey: 'immediateSales' },
  { id: 'direct-sales-returns', route: '/sales/legacy-returns', labelKey: 'directSalesReturns', icon: '↶', requiredPermission: 'SalesReturns.View', showInSidebar: false, hubSectionKey: 'immediateSales' },
  {
    id: 'accounting-center',
    route: '/accounting',
    labelKey: 'accountingCenter',
    icon: '▤',
    requiredAnyPermissions: ['Administration.Access', 'JournalEntries.View'],
  },
  { id: 'chart-of-accounts', route: '/accounting/accounts', labelKey: 'chartOfAccounts', icon: '⌘', requiredPermission: 'Administration.Access', showInSidebar: false, hubSectionKey: 'accountingSetup' },
  { id: 'journal-entries', route: '/accounting/journal-entries', labelKey: 'journalEntries', icon: '▤', requiredPermission: 'JournalEntries.View', showInSidebar: false, hubSectionKey: 'accountingReview' },
  { id: 'accounting-settings-link', route: '/settings/accounting', labelKey: 'accountingSettings', icon: '⚙', requiredPermission: 'Administration.Access', showInSidebar: false, hubSectionKey: 'accountingSetup' },
  { id: 'fiscal-periods-link', route: '/settings/fiscal-periods', labelKey: 'fiscalPeriods', icon: '▦', requiredPermission: 'Administration.Access', showInSidebar: false, hubSectionKey: 'accountingSetup' },
  { id: 'currency-rates-link', route: '/settings/currency-exchange-rates', labelKey: 'currencyExchangeRates', icon: '¤', requiredPermission: 'Administration.Access', showInSidebar: false, hubSectionKey: 'accountingSetup' },
  { id: 'branch-accounts-link', route: '/settings/branch-sales-accounts', labelKey: 'branchSalesAccounts', icon: '⌂', requiredPermission: 'Administration.Access', showInSidebar: false, hubSectionKey: 'accountingSetup' },
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
    const rank = (id: string) => { const index = order.indexOf(id); return index < 0 ? Number.MAX_SAFE_INTEGER : index; };
    return [...visible].sort((a, b) => rank(a.id) - rank(b.id));
  });
  readonly visibleGroups = computed<readonly NavigationGroup[]>(() => {
    const visible = this.visibleItems();
    return navigationGroups.flatMap((group): NavigationGroup[] => {
      const items = visible.filter((item) => item.showInSidebar !== false && this.groupForItem(item) === group.id);
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
    if (item.route === '/purchases') return 'purchasing';
    if (item.route === '/sales' || item.route.startsWith('/sales/')) return 'sales';
    if (item.route === '/accounting' || item.route.startsWith('/accounting/')) return 'accounting';
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
