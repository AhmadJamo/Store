import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const copy = {
  en: {
    product: 'MiniStore ERP',
    navHome: 'Overview',
    navPricing: 'Plans & pricing',
    signIn: 'Sign in',
    start: 'Start free trial',
    badge: 'SaaS ERP + POS',
    hero: 'Run your entire business from one place',
    heroDescription:
      'Accounting, sales, purchasing, inventory and POS in one secure platform built for branches and warehouses.',
    explore: 'Explore features',
    accounting: 'Connected accounting',
    accountingText: 'Automated journals and a detailed chart of accounts.',
    inventory: 'Professional inventory',
    inventoryText: 'Locations, racks, transfers and replenishment.',
    pos: 'Flexible POS',
    posText: 'Experiences for restaurants, cafés and retail.',
    pricingTitle: 'A plan that grows with your business',
    pricingDescription: 'Clear server-authored pricing and limits before you subscribe.',
    month: 'month',
    year: 'year',
    monthly: 'Monthly',
    annual: 'Annual',
    unlimited: 'Unlimited',
    users: 'Users',
    warehouses: 'Warehouses',
    branches: 'Branches',
    terminals: 'POS terminals',
    products: 'Products',
    plansEmpty: 'No public plans are currently available.',
    language: 'العربية',
    skip: 'Skip to content',
    primaryNavigation: 'Primary navigation',
    featuresLabel: 'Features',
    plansLabel: 'Plans',
    sales: 'Sales',
    inventoryMetric: 'Inventory',
  },
  ar: {
    product: 'MiniStore ERP',
    navHome: 'نظرة عامة',
    navPricing: 'الخطط والأسعار',
    signIn: 'تسجيل الدخول',
    start: 'ابدأ التجربة',
    badge: 'نظام سحابي + نقاط بيع',
    hero: 'أدر شركتك من مكان واحد',
    heroDescription:
      'المحاسبة والمبيعات والمشتريات والمخزون ونقاط البيع ضمن منصة آمنة تدعم الفروع والمستودعات.',
    explore: 'اكتشف المزايا',
    accounting: 'محاسبة مترابطة',
    accountingText: 'قيود تلقائية وشجرة حسابات تفصيلية.',
    inventory: 'مخزون احترافي',
    inventoryText: 'مواقع ورفوف وتحويلات وتزويد.',
    pos: 'نقاط بيع مرنة',
    posText: 'واجهات للمطاعم والمقاهي والمتاجر.',
    pricingTitle: 'خطة تناسب حجم عملك',
    pricingDescription: 'أسعار وحدود واضحة من الخادم قبل الاشتراك.',
    month: 'شهر',
    year: 'سنة',
    monthly: 'شهري',
    annual: 'سنوي',
    unlimited: 'غير محدود',
    users: 'المستخدمون',
    warehouses: 'المستودعات',
    branches: 'الفروع',
    terminals: 'أجهزة نقاط البيع',
    products: 'المنتجات',
    plansEmpty: 'لا توجد خطط عامة متاحة حاليًا.',
    language: 'English',
    skip: 'تخطي إلى المحتوى',
    primaryNavigation: 'التنقل الرئيسي',
    featuresLabel: 'المزايا',
    plansLabel: 'الخطط',
    sales: 'المبيعات',
    inventoryMetric: 'المخزون',
  },
} as const;

export type PublicSiteCopyKey = keyof typeof copy.en;

@Injectable({ providedIn: 'root' })
export class PublicSiteLocalizationService {
  readonly locale = inject(LocalizationService);

  text(key: PublicSiteCopyKey) {
    return copy[this.locale.locale() === 'ar-JO' ? 'ar' : 'en'][key];
  }

  limit(key: string) {
    const keys: Record<string, PublicSiteCopyKey> = {
      'Users.Max': 'users',
      'Warehouses.Max': 'warehouses',
      'Branches.Max': 'branches',
      'PosTerminals.Max': 'terminals',
      'Products.Max': 'products',
    };
    return keys[key] ? this.text(keys[key]) : key;
  }
}
