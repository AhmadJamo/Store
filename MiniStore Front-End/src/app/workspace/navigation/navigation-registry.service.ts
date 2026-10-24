import { computed, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationItem } from '../state/workspace.models';

const navigationItems: readonly NavigationItem[] = [
  { id: 'home', route: '/', labelKey: 'dashboard', icon: '⌂' },
  { id: 'customers', route: '/customers', labelKey: 'customersTitle', icon: '♙', requiredPermission: 'Customers.View' },
  { id: 'suppliers', route: '/suppliers', labelKey: 'suppliersTitle', icon: '♧', requiredPermission: 'Suppliers.View' },
  { id: 'design-system', route: '/design-system', labelKey: 'designSystem', icon: '◇' },
];

@Injectable({ providedIn: 'root' })
export class NavigationRegistryService {
  readonly items = navigationItems;
  readonly visibleItems = computed(() =>
    this.items.filter((item) => !item.requiredPermission || this.session.can(item.requiredPermission))
  );

  constructor(private readonly session: ApiSessionService) {}

  findById(id: string): NavigationItem | undefined { return this.items.find((item) => item.id === id); }

  findByRoute(route: string): NavigationItem | undefined {
    const cleanRoute = this.normalizeRoute(route);
    return [...this.items]
      .sort((left, right) => right.route.length - left.route.length)
      .find((item) => item.route === '/' ? cleanRoute === '/' : cleanRoute === item.route || cleanRoute.startsWith(`${item.route}/`));
  }

  canAccess(item: NavigationItem): boolean { return !item.requiredPermission || this.session.can(item.requiredPermission); }

  normalizeRoute(route: string): string {
    const path = route.split(/[?#]/, 1)[0] || '/';
    const normalized = `/${path}`.replace(/\/{2,}/g, '/').replace(/\/$/, '');
    return normalized || '/';
  }
}
