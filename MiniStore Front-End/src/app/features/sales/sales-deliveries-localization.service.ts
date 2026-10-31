import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const m = {
  en: {
    title: 'Sales deliveries',
    description: 'Prepare and post physical deliveries from active sales-order reservations.',
    search: 'Delivery or order number',
    empty: 'No sales deliveries were found.',
    view: 'View details',
    order: 'Sales order',
    warehouse: 'Warehouse',
    date: 'Delivery date',
    quantity: 'Quantity',
    cogs: 'Cost of goods sold',
    draft: 'Draft',
    posted: 'Posted',
    cancelled: 'Cancelled',
    prepare: 'Prepare sales delivery',
    back: 'Back',
    reservedAt: 'Reserved at',
    openDraft: 'View prepared delivery',
    notes: 'Notes',
    remaining: 'Reserved remaining',
    deliveryQuantity: 'Delivery quantity',
    tracking: 'Lot or serial allocation',
    trackingHelp:
      'Leave empty for warehouse picking policy, or enter lot quantities / one serial per line.',
    create: 'Prepare delivery draft',
    noReservations: 'No active reserved quantity is available for delivery.',
    safe: 'Posting consumes the reservation, reduces physical stock and records cost of goods sold. It does not create an invoice or payment.',
    unitCost: 'Unit cost',
    cost: 'Cost',
    post: 'Post sales delivery',
    cancel: 'Cancel delivery draft',
    reason: 'Cancellation reason',
    history: 'History',
    problem: 'The operation could not be completed.',
    unsaved: 'Discard unsaved delivery changes?',
    completed: 'Posting completed',
    returnRequest: 'Create customer return request',
    prepareRemaining: 'Prepare remaining delivery',
  },
  ar: {
    title: 'تسليمات المبيعات',
    description: 'حضّر ورحّل التسليم الفعلي من حجوزات طلبات البيع النشطة.',
    search: 'رقم التسليم أو الطلب',
    empty: 'لا توجد تسليمات مبيعات.',
    view: 'عرض التفاصيل',
    order: 'طلب البيع',
    warehouse: 'المستودع',
    date: 'تاريخ التسليم',
    quantity: 'الكمية',
    cogs: 'تكلفة البضاعة المباعة',
    draft: 'مسودة',
    posted: 'مرحّل',
    cancelled: 'ملغي',
    prepare: 'تحضير تسليم مبيعات',
    back: 'رجوع',
    reservedAt: 'تاريخ الحجز',
    openDraft: 'عرض التسليم المحضّر',
    notes: 'ملاحظات',
    remaining: 'المتبقي المحجوز',
    deliveryQuantity: 'كمية التسليم',
    tracking: 'تخصيص اللوط أو الرقم التسلسلي',
    trackingHelp: 'اتركه فارغًا لسياسة السحب، أو أدخل كميات اللوط / رقمًا تسلسليًا بكل سطر.',
    create: 'تحضير مسودة التسليم',
    noReservations: 'لا توجد كمية محجوزة نشطة متاحة للتسليم.',
    safe: 'الترحيل يستهلك الحجز وينقص المخزون الفعلي ويسجل تكلفة البضاعة المباعة، ولا ينشئ فاتورة أو دفعة.',
    unitCost: 'كلفة الوحدة',
    cost: 'الكلفة',
    post: 'ترحيل تسليم المبيعات',
    cancel: 'إلغاء مسودة التسليم',
    reason: 'سبب الإلغاء',
    history: 'السجل',
    problem: 'تعذر إكمال العملية.',
    unsaved: 'هل تريد تجاهل تغييرات التسليم غير المحفوظة؟',
    completed: 'اكتمل الترحيل',
    returnRequest: 'إنشاء طلب مرتجع عميل',
    prepareRemaining: 'تحضير التسليم المتبقي',
  },
} as const;
export type DeliveryKey = keyof typeof m.en;
@Injectable({ providedIn: 'root' })
export class SalesDeliveriesLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: DeliveryKey) {
    return m[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
  status(v: number) {
    return this.text((['', 'draft', 'posted', 'cancelled'][v] ?? 'draft') as DeliveryKey);
  }
}
