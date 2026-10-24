import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
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
        path: 'operations-center',
        loadComponent: () => import('./features/operations-center/operations-center-page').then((module) => module.OperationsCenterPageComponent),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
