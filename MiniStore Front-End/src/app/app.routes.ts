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
    ],
  },
  { path: '**', redirectTo: '' },
];
