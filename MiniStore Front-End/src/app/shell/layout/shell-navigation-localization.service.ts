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

const searchLabels = {
  'ar-JO': {
    searchPages: 'ابحث في الصفحات والأوامر', searchResults: 'نتائج البحث',
    noSearchResults: 'لا توجد وجهة مسموحة مطابقة.', recentDestinations: 'وجهات حديثة',
    searchStartHint: 'اكتب اسم صفحة أو مجموعة، أو استخدم Ctrl+K من أي مكان.',
  },
  'en-US': {
    searchPages: 'Search pages and commands', searchResults: 'Search results',
    noSearchResults: 'No permitted destination matches.', recentDestinations: 'Recent destinations',
    searchStartHint: 'Type a page or group name, or use Ctrl+K from anywhere.',
  },
} as const;

export type ShellSearchKey = keyof typeof searchLabels['en-US'];

@Injectable({ providedIn: 'root' })
export class ShellNavigationLocalizationService {
  private readonly i18n = inject(LocalizationService);

  group(groupId: NavigationGroupId): string {
    const locale = this.i18n.locale() === 'ar-JO' ? 'ar-JO' : 'en-US';
    return navigationGroupLabels[groupId][locale];
  }

  search(key: ShellSearchKey): string {
    const locale = this.i18n.locale() === 'ar-JO' ? 'ar-JO' : 'en-US';
    return searchLabels[locale][key];
  }
}
