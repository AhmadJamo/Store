import { Injectable, inject } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';
import { NavigationGroupId } from '../../workspace/navigation/navigation-registry.service';

const navigationGroupLabels: Record<NavigationGroupId, Readonly<{ 'ar-JO': string; 'en-US': string }>> = {
  overview: { 'ar-JO': 'نظرة عامة', 'en-US': 'Overview' },
  partners: { 'ar-JO': 'العملاء والموردون', 'en-US': 'Customers and suppliers' },
  catalog: { 'ar-JO': 'الكتالوج', 'en-US': 'Catalog' },
  inventory: { 'ar-JO': 'المخزون والمستودعات', 'en-US': 'Inventory and warehouses' },
  purchasing: { 'ar-JO': 'المشتريات', 'en-US': 'Purchasing' },
  insights: { 'ar-JO': 'المستندات والتحليلات', 'en-US': 'Documents and analytics' },
  administration: { 'ar-JO': 'الإدارة والإعدادات', 'en-US': 'Administration and settings' },
};

@Injectable({ providedIn: 'root' })
export class ShellNavigationLocalizationService {
  private readonly i18n = inject(LocalizationService);

  group(groupId: NavigationGroupId): string {
    const locale = this.i18n.locale() === 'ar-JO' ? 'ar-JO' : 'en-US';
    return navigationGroupLabels[groupId][locale];
  }
}
