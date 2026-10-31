import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';

const copy = {
  en: {
    area: 'Company subscription',
    title: 'Subscription & plan',
    description: 'Review the current subscription or create a secure checkout request.',
    current: 'Current subscription',
    noCurrent: 'No subscription is assigned to this company.',
    plan: 'Plan',
    status: 'Status',
    cycle: 'Billing cycle',
    period: 'Current period',
    available: 'Company access',
    availableYes: 'Available',
    availableNo: 'Unavailable',
    plans: 'Available plans',
    monthly: 'Monthly',
    annual: 'Annual',
    choose: 'Choose plan',
    promo: 'Promotion code',
    promoHint: 'Optional',
    continue: 'Create checkout request',
    cancel: 'Choose another plan',
    quote: 'Checkout request created',
    quoteDescription:
      'The server calculated this quote. The plan activates only after platform billing confirms payment.',
    subtotal: 'Subtotal',
    discount: 'Discount',
    total: 'Total',
    expires: 'Expires',
    reference: 'Request reference',
    manual:
      'No online payment provider is configured. Platform billing will confirm this request after payment is received.',
    empty: 'No public plans are currently available.',
    trial: 'Trial',
    active: 'Active',
    pastDue: 'Past due',
    grace: 'Grace period',
    suspended: 'Suspended',
    cancelled: 'Cancelled',
    expired: 'Expired',
    unsaved: 'Discard the checkout information you entered?',
  },
  ar: {
    area: 'اشتراك الشركة',
    title: 'الاشتراك والخطة',
    description: 'راجع الاشتراك الحالي أو أنشئ طلب دفع آمنًا.',
    current: 'الاشتراك الحالي',
    noCurrent: 'لا يوجد اشتراك مرتبط بهذه الشركة.',
    plan: 'الخطة',
    status: 'الحالة',
    cycle: 'دورة الفوترة',
    period: 'الفترة الحالية',
    available: 'دخول الشركة',
    availableYes: 'متاح',
    availableNo: 'غير متاح',
    plans: 'الخطط المتاحة',
    monthly: 'شهري',
    annual: 'سنوي',
    choose: 'اختيار الخطة',
    promo: 'رمز العرض',
    promoHint: 'اختياري',
    continue: 'إنشاء طلب الدفع',
    cancel: 'اختيار خطة أخرى',
    quote: 'تم إنشاء طلب الدفع',
    quoteDescription:
      'حسب الخادم هذا العرض. لا تتفعّل الخطة إلا بعد تأكيد الدفع من فريق فوترة المنصة.',
    subtotal: 'المبلغ',
    discount: 'الخصم',
    total: 'الإجمالي',
    expires: 'ينتهي',
    reference: 'مرجع الطلب',
    manual:
      'لا يوجد مزود دفع إلكتروني حاليًا. سيؤكد فريق فوترة المنصة الطلب بعد استلام الدفعة.',
    empty: 'لا توجد خطط عامة متاحة حاليًا.',
    trial: 'تجريبي',
    active: 'نشط',
    pastDue: 'متأخر الدفع',
    grace: 'فترة سماح',
    suspended: 'موقوف',
    cancelled: 'ملغي',
    expired: 'منتهي',
    unsaved: 'هل تريد تجاهل بيانات الدفع التي أدخلتها؟',
  },
} as const;

export type SubscriptionCopyKey = keyof typeof copy.en;

@Injectable({ providedIn: 'root' })
export class SubscriptionLocalizationService {
  private readonly session = inject(ApiSessionService);

  text(key: SubscriptionCopyKey) {
    return copy[this.isArabic() ? 'ar' : 'en'][key];
  }

  isArabic() {
    return this.session.session()?.culture?.startsWith('ar') ?? false;
  }

  status(value: number) {
    const keys: SubscriptionCopyKey[] = [
      'trial',
      'trial',
      'active',
      'pastDue',
      'grace',
      'suspended',
      'cancelled',
      'expired',
    ];
    return this.text(keys[value] ?? 'expired');
  }

  cycle(value: number) {
    return this.text(value === 2 ? 'annual' : 'monthly');
  }
}
