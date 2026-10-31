import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const c = {
  en: {
    area: 'Accounting',
    title: 'Chart of accounts',
    description: 'Maintain the account hierarchy used by operational and financial posting.',
    add: 'Add account',
    code: 'Code',
    name: 'Name',
    type: 'Type',
    parent: 'Parent account',
    top: 'Top-level account',
    save: 'Save account',
    empty: 'No accounts found.',
    active: 'Active',
    inactive: 'Inactive',
    asset: 'Asset',
    liability: 'Liability',
    equity: 'Equity',
    revenue: 'Revenue',
    expense: 'Expense',
    unsaved: 'Discard unsaved account changes?',
  },
  ar: {
    area: 'المحاسبة',
    title: 'شجرة الحسابات',
    description: 'أدر تسلسل الحسابات المستخدم في الترحيل التشغيلي والمالي.',
    add: 'إضافة حساب',
    code: 'الرمز',
    name: 'الاسم',
    type: 'النوع',
    parent: 'الحساب الأب',
    top: 'حساب رئيسي',
    save: 'حفظ الحساب',
    empty: 'لا توجد حسابات.',
    active: 'نشط',
    inactive: 'غير نشط',
    asset: 'أصل',
    liability: 'التزام',
    equity: 'حقوق ملكية',
    revenue: 'إيراد',
    expense: 'مصروف',
    unsaved: 'هل تريد تجاهل تغييرات الحساب؟',
  },
} as const;
export type AccountKey = keyof typeof c.en;
@Injectable({ providedIn: 'root' })
export class AccountsLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: AccountKey) {
    return c[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
  type(v: number) {
    return this.text(
      (['', 'asset', 'liability', 'equity', 'revenue', 'expense'][v] ?? 'asset') as AccountKey,
    );
  }
}
