import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const m = {
  en: {
    area: 'Sales',
    title: 'Customer credit notes',
    description: 'Reverse receivables, revenue and output tax for posted billed returns.',
    empty: 'No customer credit notes were found.',
    view: 'Details',
    back: 'Back',
    create: 'Create customer credit note',
    safe: 'This posts a financial credit only. Inventory was handled by the return disposition and no cash refund is created.',
    invoice: 'Customer invoice',
    customer: 'Customer',
    creditDate: 'Credit date',
    reason: 'Reason',
    returned: 'Returned',
    credited: 'Credited',
    available: 'Available to credit',
    quantity: 'Credit quantity',
    post: 'Post customer credit note',
    posted: 'Posted',
    receipt: 'Return receipt',
    net: 'Net amount',
    tax: 'Tax',
    total: 'Total',
    companyTotal: 'Company total',
    journal: 'Journal entry',
    noEligible: 'No posted returned quantities remain eligible for this invoice.',
    unsaved: 'Discard unsaved customer credit note changes?',
  },
  ar: {
    area: 'المبيعات',
    title: 'الإشعارات الدائنة للعملاء',
    description: 'اعكس الذمم والإيراد وضريبة المخرجات للمرتجعات المفوترة المرحلة.',
    empty: 'لا توجد إشعارات دائنة للعملاء.',
    view: 'التفاصيل',
    back: 'رجوع',
    create: 'إنشاء إشعار دائن للعميل',
    safe: 'هذا ترحيل دائن مالي فقط. عولج المخزون عبر تصرف المرتجع ولا يُنشأ استرداد نقدي.',
    invoice: 'فاتورة العميل',
    customer: 'العميل',
    creditDate: 'تاريخ الإشعار',
    reason: 'السبب',
    returned: 'المرتجع',
    credited: 'الدائن سابقاً',
    available: 'المتاح للإشعار',
    quantity: 'كمية الإشعار',
    post: 'ترحيل إشعار العميل الدائن',
    posted: 'مرحّل',
    receipt: 'استلام المرتجع',
    net: 'الصافي',
    tax: 'الضريبة',
    total: 'الإجمالي',
    companyTotal: 'إجمالي عملة الشركة',
    journal: 'قيد اليومية',
    noEligible: 'لا توجد كميات مرتجعة مرحلة متبقية مؤهلة لهذه الفاتورة.',
    unsaved: 'هل تريد تجاهل تغييرات الإشعار الدائن؟',
  },
} as const;
export type CreditKey = keyof typeof m.en;
@Injectable({ providedIn: 'root' })
export class CustomerCreditNotesLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: CreditKey) {
    return m[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
  status(_v: number) {
    return this.text('posted');
  }
}
