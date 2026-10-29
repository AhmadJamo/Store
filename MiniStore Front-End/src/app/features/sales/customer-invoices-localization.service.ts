import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const m = {
  en: {
    area: 'Sales',
    title: 'Customer invoices',
    description:
      'Create, approve, post and settle customer invoice evidence from eligible sales-order quantities.',
    search: 'Invoice, order or customer',
    status: 'Status',
    all: 'All',
    empty: 'No customer invoices found.',
    view: 'Details',
    order: 'Sales order',
    customer: 'Customer',
    invoiceDate: 'Invoice date',
    dueDate: 'Due date',
    total: 'Total',
    outstanding: 'Outstanding balance',
    draft: 'Draft',
    approved: 'Approved',
    posted: 'Posted',
    reversed: 'Reversed',
    cancelled: 'Cancelled',
    create: 'Create customer invoice',
    back: 'Back',
    safe: 'Creating or approving does not change inventory, post accounting or record payment.',
    basis: 'Invoice quantity basis',
    ordered: 'Ordered',
    eligible: 'Source eligible',
    committed: 'Already invoiced or committed',
    remaining: 'Remaining eligible',
    quantity: 'Invoice quantity',
    unitPrice: 'Unit price',
    notes: 'Notes',
    createDraft: 'Create invoice draft',
    snapshot: 'Customer snapshot',
    terms: 'Invoice terms',
    paymentDays: 'Payment days',
    lines: 'Customer invoice lines',
    net: 'Net amount',
    tax: 'Tax amount',
    sources: 'Source deliveries',
    approve: 'Approve customer invoice',
    post: 'Post customer invoice',
    cancel: 'Cancel customer invoice',
    reason: 'Cancellation reason',
    reverse: 'Reverse customer invoice',
    reversalDate: 'Reversal date',
    reversalReason: 'Reversal reason',
    paymentStatus: 'Payment status',
    paid: 'Paid amount',
    credited: 'Credited amount',
    accounting: 'Accounting posting evidence',
    journal: 'Journal entry',
    receivable: 'Customer receivable account',
    revenue: 'Default sales revenue account',
    history: 'History',
    problem: 'The operation could not be completed.',
    unsaved: 'Discard unsaved invoice changes?',
    orderedPolicy: 'Ordered quantity',
    deliveredPolicy: 'Delivered quantity',
    debit: 'Debit',
    credit: 'Credit',
    unpaid: 'Unpaid',
    partiallySettled: 'Partially settled',
    paidStatus: 'Paid',
    creditedStatus: 'Credited',
    recordPayment: 'Record customer payment',
    createCreditNote: 'Create customer credit note',
    paymentCompleted: 'Payment completed',
  },
  ar: {
    area: 'المبيعات',
    title: 'فواتير العملاء',
    description: 'أنشئ واعتمد ورحّل دليل فاتورة العميل من كميات طلب البيع المؤهلة.',
    search: 'الفاتورة أو الطلب أو العميل',
    status: 'الحالة',
    all: 'الكل',
    empty: 'لا توجد فواتير عملاء.',
    view: 'التفاصيل',
    order: 'طلب البيع',
    customer: 'العميل',
    invoiceDate: 'تاريخ الفاتورة',
    dueDate: 'تاريخ الاستحقاق',
    total: 'الإجمالي',
    outstanding: 'الرصيد المستحق',
    draft: 'مسودة',
    approved: 'معتمدة',
    posted: 'مرحّلة',
    reversed: 'معكوسة',
    cancelled: 'ملغاة',
    create: 'إنشاء فاتورة عميل',
    back: 'رجوع',
    safe: 'إنشاء أو اعتماد الفاتورة لا يغيّر المخزون ولا يرحّل المحاسبة ولا يسجل دفعة.',
    basis: 'أساس كمية الفاتورة',
    ordered: 'المطلوب',
    eligible: 'المؤهل من المصدر',
    committed: 'المفوتر أو الملتزم سابقًا',
    remaining: 'المتبقي المؤهل',
    quantity: 'كمية الفاتورة',
    unitPrice: 'سعر الوحدة',
    notes: 'ملاحظات',
    createDraft: 'إنشاء مسودة الفاتورة',
    snapshot: 'لقطة العميل',
    terms: 'شروط الفاتورة',
    paymentDays: 'أيام السداد',
    lines: 'بنود فاتورة العميل',
    net: 'الصافي',
    tax: 'الضريبة',
    sources: 'التسليمات المصدر',
    approve: 'اعتماد فاتورة العميل',
    post: 'ترحيل فاتورة العميل',
    cancel: 'إلغاء فاتورة العميل',
    reason: 'سبب الإلغاء',
    reverse: 'عكس فاتورة العميل',
    reversalDate: 'تاريخ العكس',
    reversalReason: 'سبب العكس',
    paymentStatus: 'حالة الدفع',
    paid: 'المدفوع',
    credited: 'الرصيد الدائن',
    accounting: 'دليل الترحيل المحاسبي',
    journal: 'القيد اليومي',
    receivable: 'حساب ذمم العميل',
    revenue: 'حساب إيراد المبيعات',
    history: 'السجل',
    problem: 'تعذر إكمال العملية.',
    unsaved: 'هل تريد تجاهل تغييرات الفاتورة غير المحفوظة؟',
    orderedPolicy: 'الكمية المطلوبة',
    deliveredPolicy: 'الكمية المسلّمة',
    debit: 'مدين',
    credit: 'دائن',
    unpaid: 'غير مدفوعة',
    partiallySettled: 'مسددة جزئيًا',
    paidStatus: 'مدفوعة',
    creditedStatus: 'مسددة بإشعار دائن',
    recordPayment: 'تسجيل دفعة عميل',
    createCreditNote: 'إنشاء إشعار دائن للعميل',
    paymentCompleted: 'اكتمل السداد',
  },
} as const;
export type InvoiceKey = keyof typeof m.en;
@Injectable({ providedIn: 'root' })
export class CustomerInvoicesLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: InvoiceKey) {
    return m[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
  status(v: number) {
    return this.text(
      (['', 'draft', 'approved', 'cancelled', 'posted', 'reversed'][v] ?? 'draft') as InvoiceKey,
    );
  }
  basis(v: number) {
    return this.text(v === 1 ? 'deliveredPolicy' : 'orderedPolicy');
  }
  paymentStatus(v: string) {
    const key =
      v === 'Paid'
        ? 'paidStatus'
        : v === 'Credited'
          ? 'creditedStatus'
          : v === 'Partially Settled'
            ? 'partiallySettled'
            : 'unpaid';
    return this.text(key);
  }
}
