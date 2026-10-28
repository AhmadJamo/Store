import { Routes } from '@angular/router';
import { anonymousOnlyGuard, authGuard } from './core/api/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [anonymousOnlyGuard],
    loadComponent: () => import('./features/account/login-page').then(module => module.LoginPageComponent),
  },
  {
    path: 'register',
    canActivate: [anonymousOnlyGuard],
    loadComponent: () => import('./features/onboarding/registration-page').then(module => module.RegistrationPageComponent),
  },
  {
    path: 'onboarding',
    canActivate: [authGuard],
    canDeactivate: [(component) => component.canDeactivate()],
    loadComponent: () => import('./features/onboarding/company-onboarding-page').then(module => module.CompanyOnboardingPageComponent),
  },
  {
    path: 'access-denied',
    loadComponent: () => import('./features/account/access-denied-page').then(module => module.AccessDeniedPageComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    loadComponent: () =>
      import('./shell/layout/app-shell').then((module) => module.AppShell),
    children: [
      {
        path: '',
        title: 'MiniStore Workspace',
        loadComponent: () =>
          import('./features/home/home-page').then((module) => module.HomePage),
      },
      {
        path: 'design-system',
        title: 'MiniStore Design System',
        loadComponent: () =>
          import('./features/design-system/design-system-page').then((module) => module.DesignSystemPage),
      },
      {
        path: 'customers',
        loadComponent: () => import('./features/customers/customer-list-page').then((module) => module.CustomerListPageComponent),
      },
      {
        path: 'customers/new',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/customers/customer-form-page').then((module) => module.CustomerFormPageComponent),
      },
      {
        path: 'customers/:id/edit',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/customers/customer-form-page').then((module) => module.CustomerFormPageComponent),
      },
      {
        path: 'customers/:id',
        loadComponent: () => import('./features/customers/customer-details-page').then((module) => module.CustomerDetailsPageComponent),
      },
      {
        path: 'suppliers',
        loadComponent: () => import('./features/suppliers/supplier-list-page').then((module) => module.SupplierListPageComponent),
      },
      {
        path: 'suppliers/new',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/suppliers/supplier-form-page').then((module) => module.SupplierFormPageComponent),
      },
      {
        path: 'suppliers/:id/edit',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/suppliers/supplier-form-page').then((module) => module.SupplierFormPageComponent),
      },
      {
        path: 'suppliers/:id',
        loadComponent: () => import('./features/suppliers/supplier-details-page').then((module) => module.SupplierDetailsPageComponent),
      },
      {
        path: 'documents',
        loadComponent: () => import('./features/document-evidence/document-evidence-page').then((module) => module.DocumentEvidencePageComponent),
      },
      {
        path: 'documents/:kind/:id',
        loadComponent: () => import('./features/document-evidence/document-evidence-page').then((module) => module.DocumentEvidencePageComponent),
      },
      {
        path: 'work-center',
        loadComponent: () => import('./features/work-center/work-center-page').then((module) => module.WorkCenterPageComponent),
      },
      {
        path: 'analytics',
        loadComponent: () => import('./features/analytics/analytics-page').then((module) => module.AnalyticsPageComponent),
      },
      {
        path: 'analytics/builder',
        loadComponent: () => import('./features/analytics/report-builder-page').then((module) => module.ReportBuilderPageComponent),
      },
      {
        path: 'analytics/workspaces/:id/access',
        loadComponent: () => import('./features/analytics/workspace-access-page').then((module) => module.WorkspaceAccessPageComponent),
      },
      {
        path: 'settings',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/settings-overview-page').then((module) => module.SettingsOverviewPageComponent),
      },
      { path: 'users', loadComponent: () => import('./features/identity/user-list-page').then(m => m.UserListPageComponent) },
      { path: 'users/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/identity/user-form-page').then(m => m.UserFormPageComponent) },
      { path: 'roles', loadComponent: () => import('./features/identity/role-list-page').then(m => m.RoleListPageComponent) },
      { path: 'roles/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/identity/role-form-page').then(m => m.RoleFormPageComponent) },
      { path: 'roles/:id/edit', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/identity/role-form-page').then(m => m.RoleFormPageComponent) },
      {
        path: 'settings/identity', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/organization-identity-page').then((module) => module.OrganizationIdentityPageComponent),
      },
      {
        path: 'settings/discounts', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/discount-settings-page').then((module) => module.DiscountSettingsPageComponent),
      },
      {
        path: 'settings/purchase-matching', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/purchase-matching-settings-page').then((module) => module.PurchaseMatchingSettingsPageComponent),
      },
      {
        path: 'settings/inventory', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/inventory-settings-page').then((module) => module.InventorySettingsPageComponent),
      },
      {
        path: 'settings/purchase-sourcing', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/purchase-sourcing-settings-page').then((module) => module.PurchaseSourcingSettingsPageComponent),
      },
      {
        path: 'settings/document-numbers', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/document-number-settings-page').then((module) => module.DocumentNumberSettingsPageComponent),
      },
      {
        path: 'settings/fiscal-periods', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/fiscal-periods-page').then((module) => module.FiscalPeriodsPageComponent),
      },
      {
        path: 'settings/currency-exchange-rates', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/currency-exchange-rates-page').then((module) => module.CurrencyExchangeRatesPageComponent),
      },
      {
        path: 'settings/measurement-units', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/measurement-units-page').then((module) => module.MeasurementUnitsPageComponent),
      },
      {
        path: 'settings/accounting', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/accounting-settings-page').then((module) => module.AccountingSettingsPageComponent),
      },
      {
        path: 'settings/branch-sales-accounts', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/branch-sales-accounts-page').then((module) => module.BranchSalesAccountsPageComponent),
      },
      {
        path: 'settings/inventory-access', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/inventory-access-page').then((module) => module.InventoryAccessPageComponent),
      },
      {
        path: 'settings/pos-experience', canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/settings/pos-experience-page').then((module) => module.PosExperiencePageComponent),
      },
      {
        path: 'settings/workflow-catalog',
        loadComponent: () => import('./features/settings/workflow-catalog-page').then((module) => module.WorkflowCatalogPageComponent),
      },
      { path: 'settings/document-designs', loadComponent: () => import('./features/document-designs/document-design-list-page').then(m => m.DocumentDesignListPageComponent) },
      { path: 'settings/document-designs/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/document-designs/document-design-form-page').then(m => m.DocumentDesignFormPageComponent) },
      { path: 'settings/document-designs/:id', loadComponent: () => import('./features/document-designs/document-design-details-page').then(m => m.DocumentDesignDetailsPageComponent) },
      {
        path: 'products',
        loadComponent: () => import('./features/products/product-list-page').then((module) => module.ProductListPageComponent),
      },
      {
        path: 'products/new',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/product-form-page').then((module) => module.ProductFormPageComponent),
      },
      {
        path: 'products/categories',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/product-categories-page').then((module) => module.ProductCategoriesPageComponent),
      },
      {
        path: 'products/attributes',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/product-attributes-page').then((module) => module.ProductAttributesPageComponent),
      },
      {
        path: 'products/templates',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/product-templates-page').then((module) => module.ProductTemplatesPageComponent),
      },
      {
        path: 'products/recipes',
        loadComponent: () => import('./features/products/recipe-list-page').then((module) => module.RecipeListPageComponent),
      },
      {
        path: 'products/catalog-masters',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/catalog-masters-page').then((module) => module.CatalogMastersPageComponent),
      },
      { path: 'warehouses', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/products/warehouses-page').then(module => module.WarehousesPageComponent) },
      { path: 'inventory/transactions', loadComponent: () => import('./features/inventory/stock-transactions-page').then(module => module.StockTransactionsPageComponent) },
      { path: 'inventory/stocks', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/product-stocks-page').then(module => module.ProductStocksPageComponent) },
      { path: 'inventory/availability', loadComponent: () => import('./features/inventory/inventory-availability-page').then(module => module.InventoryAvailabilityPageComponent) },
      { path: 'inventory/reservations', loadComponent: () => import('./features/inventory/inventory-reservations-page').then(module => module.InventoryReservationsPageComponent) },
      { path: 'inventory/adjustments', loadComponent: () => import('./features/inventory/inventory-adjustments-page').then(module => module.InventoryAdjustmentsPageComponent) },
      { path: 'inventory/adjustments/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/inventory-adjustment-form-page').then(module => module.InventoryAdjustmentFormPageComponent) },
      { path: 'inventory/adjustments/:id', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/inventory-adjustment-details-page').then(module => module.InventoryAdjustmentDetailsPageComponent) },
      { path: 'inventory/tracking', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/inventory-tracking-page').then(module => module.InventoryTrackingPageComponent) },
      { path: 'inventory/locations', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/warehouse-locations-page').then(module => module.WarehouseLocationsPageComponent) },
      { path: 'inventory/putaway', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/unassigned-stock-page').then(module => module.UnassignedStockPageComponent) },
      { path: 'inventory/location-movements', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/inventory/location-movements-page').then(module => module.LocationMovementsPageComponent) },
      { path: 'purchases/requests', loadComponent: () => import('./features/purchases/purchase-requests-page').then(module => module.PurchaseRequestsPageComponent) },
      { path: 'purchases/requests/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/purchase-requests-page').then(module => module.PurchaseRequestsPageComponent) },
      { path: 'purchases/requests/:id', loadComponent: () => import('./features/purchases/purchase-requests-page').then(module => module.PurchaseRequestsPageComponent) },
      { path: 'purchases/approval-rules', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/purchase-requests-page').then(module => module.PurchaseRequestsPageComponent) },
      { path: 'purchases/sourcing', loadComponent: () => import('./features/purchases/purchase-sourcing-page').then(module => module.PurchaseSourcingPageComponent) },
      { path: 'purchases/sourcing/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/purchase-sourcing-page').then(module => module.PurchaseSourcingPageComponent) },
      { path: 'purchases/sourcing/:id', loadComponent: () => import('./features/purchases/purchase-sourcing-page').then(module => module.PurchaseSourcingPageComponent) },
      { path: 'purchases/sourcing/:id/quotation', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/purchase-sourcing-page').then(module => module.PurchaseSourcingPageComponent) },
      { path: 'purchases/sourcing/:id/comparison', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/purchase-sourcing-page').then(module => module.PurchaseSourcingPageComponent) },
      { path: 'purchases/orders', loadComponent: () => import('./features/purchases/purchase-orders-page').then(module => module.PurchaseOrdersPageComponent) },
      { path: 'purchases/orders/:id', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/purchase-orders-page').then(module => module.PurchaseOrdersPageComponent) },
      { path: 'purchases/receipts', loadComponent: () => import('./features/purchases/goods-receipts-page').then(module => module.GoodsReceiptsPageComponent) },
      { path: 'purchases/receipts/new', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/goods-receipts-page').then(module => module.GoodsReceiptsPageComponent) },
      { path: 'purchases/receipts/:id', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/goods-receipts-page').then(module => module.GoodsReceiptsPageComponent) },
      { path: 'purchases/receipts/:id/scan', canDeactivate: [(component) => component.canDeactivate()], loadComponent: () => import('./features/purchases/goods-receipts-page').then(module => module.GoodsReceiptsPageComponent) },
      {
        path: 'products/recipes/:productId',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/recipe-editor-page').then((module) => module.RecipeEditorPageComponent),
      },
      {
        path: 'products/:id/attributes',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/product-attribute-values-page').then((module) => module.ProductAttributeValuesPageComponent),
      },
      {
        path: 'products/:id/edit',
        canDeactivate: [(component) => component.canDeactivate()],
        loadComponent: () => import('./features/products/product-form-page').then((module) => module.ProductFormPageComponent),
      },
      {
        path: 'products/:id',
        loadComponent: () => import('./features/products/product-details-page').then((module) => module.ProductDetailsPageComponent),
      },
      {
        path: 'operations-center',
        loadComponent: () => import('./features/operations-center/operations-center-page').then((module) => module.OperationsCenterPageComponent),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
