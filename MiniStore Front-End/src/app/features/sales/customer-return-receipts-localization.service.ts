import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const messages = {
  en: {
    area: 'Sales',
    title: 'Customer return receipts',
    description:
      'Record custody, inspection and controlled inventory disposition for approved returns.',
    search: 'Receipt or return request number',
    apply: 'Apply',
    empty: 'No customer return receipts were found.',
    view: 'Details',
    back: 'Back',
    create: 'Receive and inspect return',
    safe: 'Inspection freezes custody evidence. Only disposition posting can restock goods and reverse cost of sales.',
    request: 'Return request',
    receiptDate: 'Receipt date',
    notes: 'Notes',
    received: 'Received quantity',
    disposition: 'Disposition',
    inspectionNotes: 'Inspection notes',
    approved: 'Approved',
    alreadyReceived: 'Already received',
    remaining: 'Remaining',
    tracking: 'Returned lot or serial allocations',
    trackingHelp: 'Use LOT:QUANTITY or one serial per line from the source delivery.',
    createDraft: 'Create inspection draft',
    draft: 'Draft',
    inspected: 'Inspected',
    cancelled: 'Cancelled',
    posted: 'Posted',
    restock: 'Restock',
    quarantine: 'Quarantine',
    reject: 'Reject',
    destroy: 'Destroy',
    completeInspection: 'Complete inspection',
    postDisposition: 'Post inventory dispositions',
    cancel: 'Cancel receipt draft',
    cancellationReason: 'Cancellation reason',
    completedInspection: 'Inspection completed',
    completedPosting: 'Disposition posting completed',
    history: 'History',
    restockedQuantity: 'Restocked quantity',
    restockedCost: 'Restocked cost',
    journal: 'Journal entry',
    postedBy: 'Posted by',
    unsaved: 'Discard unsaved return receipt changes?',
    quantity: 'Quantity',
  },
  ar: {
    area: 'المبيعات',
    title: 'استلامات مرتجعات العملاء',
    description: 'سجّل الحيازة والفحص والتصرف المخزني المنضبط للمرتجعات المعتمدة.',
    search: 'رقم الاستلام أو طلب المرتجع',
    apply: 'تطبيق',
    empty: 'لا توجد استلامات مرتجعات عملاء.',
    view: 'التفاصيل',
    back: 'رجوع',
    create: 'استلام وفحص المرتجع',
    safe: 'يثبت الفحص دليل الحيازة، وترحيل التصرف وحده يعيد البضاعة للمخزون ويعكس تكلفة المبيعات.',
    request: 'طلب المرتجع',
    receiptDate: 'تاريخ الاستلام',
    notes: 'ملاحظات',
    received: 'الكمية المستلمة',
    disposition: 'التصرف',
    inspectionNotes: 'ملاحظات الفحص',
    approved: 'المعتمد',
    alreadyReceived: 'المستلم سابقاً',
    remaining: 'المتبقي',
    tracking: 'تخصيصات اللوط أو الرقم التسلسلي المرتجعة',
    trackingHelp: 'استخدم LOT:QUANTITY أو رقماً تسلسلياً واحداً بكل سطر من التسليم المصدر.',
    createDraft: 'إنشاء مسودة الفحص',
    draft: 'مسودة',
    inspected: 'مفحوص',
    cancelled: 'ملغي',
    posted: 'مرحّل',
    restock: 'إعادة للمخزون',
    quarantine: 'حجر',
    reject: 'رفض',
    destroy: 'إتلاف',
    completeInspection: 'إكمال الفحص',
    postDisposition: 'ترحيل تصرفات المخزون',
    cancel: 'إلغاء مسودة الاستلام',
    cancellationReason: 'سبب الإلغاء',
    completedInspection: 'اكتمل الفحص',
    completedPosting: 'اكتمل ترحيل التصرف',
    history: 'السجل',
    restockedQuantity: 'الكمية المعادة للمخزون',
    restockedCost: 'كلفة الإعادة للمخزون',
    journal: 'قيد اليومية',
    postedBy: 'رُحّل بواسطة',
    unsaved: 'هل تريد تجاهل تغييرات استلام المرتجع؟',
    quantity: 'الكمية',
  },
} as const;
export type ReturnReceiptKey = keyof typeof messages.en;
@Injectable({ providedIn: 'root' })
export class CustomerReturnReceiptsLocalizationService {
  private readonly session = inject(ApiSessionService);
  text(key: ReturnReceiptKey) {
    return messages[this.session.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][key];
  }
  status(value: number) {
    return this.text(
      (['', 'draft', 'inspected', 'cancelled', 'posted'][value] ?? 'draft') as ReturnReceiptKey,
    );
  }
  disposition(value: number) {
    return this.text(
      (['', 'restock', 'quarantine', 'reject', 'destroy'][value] ?? 'restock') as ReturnReceiptKey,
    );
  }
}
