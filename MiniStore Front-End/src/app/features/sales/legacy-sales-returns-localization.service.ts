import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const c = {
  en: {
    area: 'Sales',
    title: 'Immediate-sale returns',
    description:
      'Return quantities from posted immediate sales with server-owned stock, tracking and accounting.',
    back: 'Back',
    view: 'View return',
    empty: 'No immediate-sale returns found.',
    create: 'Create sales return',
    original: 'Original sale',
    date: 'Date',
    reason: 'Return reason',
    product: 'Product',
    sold: 'Sold quantity',
    returned: 'Already returned',
    remaining: 'Remaining',
    quantity: 'Return quantity',
    tracking: 'Lot or serial allocations',
    trackingHelp: 'Enter only identities issued by the original sale.',
    post: 'Create and post return',
    refund: 'Refund amount',
    tax: 'Sales tax',
    restocked: 'Restocked',
    cost: 'Restocked cost',
    yes: 'Yes',
    no: 'No',
    required: 'Enter a reason and at least one return quantity.',
    unsaved: 'Discard unsaved return changes?',
  },
  ar: {
    area: 'المبيعات',
    title: 'مرتجعات البيع الفوري',
    description: 'أرجع كميات من مبيعات فورية مُرحلة مع بقاء المخزون والتتبع والمحاسبة داخل الخادم.',
    back: 'رجوع',
    view: 'عرض المرتجع',
    empty: 'لا توجد مرتجعات بيع فوري.',
    create: 'إنشاء مرتجع بيع',
    original: 'البيع الأصلي',
    date: 'التاريخ',
    reason: 'سبب المرتجع',
    product: 'المنتج',
    sold: 'الكمية المباعة',
    returned: 'المرتجع سابقاً',
    remaining: 'المتبقي',
    quantity: 'كمية الإرجاع',
    tracking: 'تخصيصات اللوط أو الرقم التسلسلي',
    trackingHelp: 'أدخل فقط الهويات التي خرجت مع البيع الأصلي.',
    post: 'إنشاء وترحيل المرتجع',
    refund: 'قيمة الاسترداد',
    tax: 'ضريبة المبيعات',
    restocked: 'أعيد للمخزون',
    cost: 'تكلفة المخزون المعاد',
    yes: 'نعم',
    no: 'لا',
    required: 'أدخل السبب وكمية مرتجع واحدة على الأقل.',
    unsaved: 'هل تريد تجاهل تغييرات المرتجع؟',
  },
} as const;
export type ReturnKey = keyof typeof c.en;
@Injectable({ providedIn: 'root' })
export class LegacySalesReturnsLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: ReturnKey) {
    return c[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
}
