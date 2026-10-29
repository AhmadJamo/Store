import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const copy = {
  en: {
    product: 'MiniStore Control',
    loginTitle: 'Platform sign in',
    loginDescription: 'Authorized platform operators only.',
    username: 'Username',
    password: 'Password',
    signIn: 'Sign in securely',
    invalid: 'The platform credentials were rejected.',
    overview: 'Platform overview',
    overviewDescription: 'Platform administration is isolated from tenant ERP administration.',
    operator: 'Operator',
    role: 'Role',
    configuredPlans: 'Configured plans',
    plans: 'Plans',
    code: 'Code',
    name: 'Name',
    monthly: 'Monthly',
    annual: 'Annual',
    limits: 'Limits',
    signOut: 'Sign out',
    language: 'العربية',
    summary: 'Platform summary',
    management: 'Management',
  },
  ar: {
    product: 'إدارة MiniStore',
    loginTitle: 'دخول إدارة المنصة',
    loginDescription: 'للمشغلين المخولين بإدارة المنصة فقط.',
    username: 'اسم المستخدم',
    password: 'كلمة المرور',
    signIn: 'دخول آمن',
    invalid: 'تم رفض بيانات دخول المنصة.',
    overview: 'نظرة عامة على المنصة',
    overviewDescription: 'إدارة المنصة منفصلة عن إدارة شركات العملاء.',
    operator: 'المشغل',
    role: 'الدور',
    configuredPlans: 'الخطط المعرفة',
    plans: 'الخطط',
    code: 'الرمز',
    name: 'الاسم',
    monthly: 'شهري',
    annual: 'سنوي',
    limits: 'الحدود',
    signOut: 'تسجيل الخروج',
    language: 'English',
    summary: 'ملخص المنصة',
    management: 'الإدارة',
  },
} as const;
export type PlatformCopyKey = keyof typeof copy.en;

@Injectable({ providedIn: 'root' })
export class PlatformLocalizationService {
  readonly locale = inject(LocalizationService);
  text(key: PlatformCopyKey) {
    return copy[this.locale.locale() === 'ar-JO' ? 'ar' : 'en'][key];
  }
}
