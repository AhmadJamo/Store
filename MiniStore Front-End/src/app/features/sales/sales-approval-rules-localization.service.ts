import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const m = {
  en: {
    area: 'Sales',
    title: 'Sales approval rules',
    description:
      'Create layered controls that warn, require sequential approval or block confirmation.',
    empty: 'No sales approval rules have been configured.',
    name: 'Rule name',
    customer: 'Customer scope',
    allCustomers: 'All customers',
    priority: 'Priority',
    action: 'Action',
    metric: 'Metric',
    comparison: 'Comparison',
    threshold: 'Threshold',
    from: 'Effective from',
    to: 'Effective to',
    roles: 'Sequential approver roles',
    addRole: 'Add approval step',
    allowRequester: 'Allow requester approval',
    distinct: 'Require different approvers',
    create: 'Create rule',
    active: 'Active',
    inactive: 'Inactive',
    activate: 'Activate',
    deactivate: 'Deactivate',
    warn: 'Warn',
    approval: 'Require approval',
    block: 'Block',
    amount: 'Order company amount',
    exposure: 'Credit exposure',
    overdue: 'Overdue days',
    discount: 'Discount percent',
    margin: 'Margin percent',
    gte: 'Greater than or equal',
    gt: 'Greater than',
    lte: 'Less than or equal',
    lt: 'Less than',
    period: 'Effective period',
    steps: 'Approval steps',
    unsaved: 'Discard unsaved approval rule changes?',
  },
  ar: {
    area: 'المبيعات',
    title: 'قواعد اعتماد المبيعات',
    description: 'أنشئ ضوابط متدرجة للتنبيه أو الاعتماد المتسلسل أو منع التأكيد.',
    empty: 'لا توجد قواعد اعتماد مبيعات.',
    name: 'اسم القاعدة',
    customer: 'نطاق العميل',
    allCustomers: 'كل العملاء',
    priority: 'الأولوية',
    action: 'الإجراء',
    metric: 'المقياس',
    comparison: 'المقارنة',
    threshold: 'الحد',
    from: 'ساري من',
    to: 'ساري إلى',
    roles: 'أدوار المعتمدين المتسلسلة',
    addRole: 'إضافة خطوة اعتماد',
    allowRequester: 'السماح باعتماد مقدم الطلب',
    distinct: 'اشتراط معتمدين مختلفين',
    create: 'إنشاء القاعدة',
    active: 'نشطة',
    inactive: 'غير نشطة',
    activate: 'تفعيل',
    deactivate: 'تعطيل',
    warn: 'تنبيه',
    approval: 'يتطلب اعتماداً',
    block: 'منع',
    amount: 'مبلغ الطلب بعملة الشركة',
    exposure: 'التعرض الائتماني',
    overdue: 'أيام التأخر',
    discount: 'نسبة الخصم',
    margin: 'نسبة الهامش',
    gte: 'أكبر من أو يساوي',
    gt: 'أكبر من',
    lte: 'أقل من أو يساوي',
    lt: 'أقل من',
    period: 'فترة السريان',
    steps: 'خطوات الاعتماد',
    unsaved: 'هل تريد تجاهل تغييرات قاعدة الاعتماد؟',
  },
} as const;
export type ApprovalKey = keyof typeof m.en;
@Injectable({ providedIn: 'root' })
export class SalesApprovalRulesLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: ApprovalKey) {
    return m[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
  metric(v: number) {
    return this.text(
      (['', 'amount', 'exposure', 'overdue', 'discount', 'margin'][v] ?? 'amount') as ApprovalKey,
    );
  }
  comparison(v: number) {
    return this.text((['', 'gte', 'gt', 'lte', 'lt'][v] ?? 'gte') as ApprovalKey);
  }
  action(v: number) {
    return this.text((['', 'warn', 'approval', 'block'][v] ?? 'warn') as ApprovalKey);
  }
}
