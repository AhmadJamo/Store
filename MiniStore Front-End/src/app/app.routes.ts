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
    ],
  },
  { path: '**', redirectTo: '' },
];
