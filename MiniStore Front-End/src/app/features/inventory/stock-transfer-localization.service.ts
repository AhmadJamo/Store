import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const messages = {
  'ar-JO': {
    inventoryNavigation: 'المخزون', title: 'تحويلات المخزون', description: 'أنشئ التحويلات وراجعها واعتمدها ورحّلها مع إبقاء حركات المخزون تحت سلطة الخادم.',
    newTransfer: 'تحويل جديد', loading: 'جارٍ تحميل تحويلات المخزون', empty: 'لا توجد تحويلات مخزون', emptyDescription: 'أنشئ أول تحويل أو غيّر مرشحات البحث.',
    search: 'بحث', searchPlaceholder: 'رقم التحويل أو المرجع', status: 'الحالة', allStatuses: 'كل الحالات', apply: 'تطبيق', number: 'رقم التحويل',
    sourceWarehouse: 'مستودع المصدر', destinationWarehouse: 'مستودع الوجهة', items: 'الأصناف', createdAt: 'تاريخ الإنشاء', actions: 'الإجراءات', details: 'التفاصيل',
    draft: 'مسودة', submitted: 'مرسل للاعتماد', approved: 'معتمد', rejected: 'مرفوض', posted: 'مرحّل قديم', cancelled: 'ملغي', inTransit: 'قيد النقل', partiallyReceived: 'مستلم جزئيًا', receiptException: 'فروقات استلام مفتوحة', received: 'مستلم بالكامل',
    formDescription: 'اختر المستودعات بالاسم ثم أضف المنتجات والكميات والمواقع المطلوبة.', back: 'العودة للتحويلات', reference: 'المرجع', notes: 'ملاحظات',
    product: 'المنتج', barcode: 'الباركود', quantity: 'الكمية', sourceLocation: 'موقع المصدر', destinationLocation: 'موقع الوجهة', automatic: 'تلقائي حسب سياسة المستودع',
    tracking: 'تخصيص الدفعات أو الأرقام التسلسلية', trackingHelp: 'أدخل دفعة وكمية أو رقماً تسلسلياً في كل سطر، واتركه فارغاً لاستخدام استراتيجية المستودع.',
    addItem: 'إضافة صنف', remove: 'حذف', save: 'حفظ التحويل', saving: 'جارٍ الحفظ', correctFields: 'تحقق من المستودعات المختلفة ومن كل صنف وكمية والمواقع المطلوبة.',
    detailDescription: 'راجع الأصناف ومسار الحالة ثم نفّذ الإجراء المسموح فقط.', createdBy: 'أنشأه', history: 'سجل الحالة', fromStatus: 'من', toStatus: 'إلى', user: 'المستخدم', reason: 'السبب',
    edit: 'تعديل', submit: 'إرسال للاعتماد', approve: 'اعتماد', reject: 'رفض', returnToDraft: 'إعادة لمسودة', post: 'إرسال من المصدر', cancel: 'إلغاء وترجيع الحركة', receive: 'تسجيل الاستلام', receivedQuantity: 'المستلم', remainingQuantity: 'المتبقي', shortQuantity: 'ناقص', damagedQuantity: 'تالف', receiptEvidence: 'توزيع الاستلام والفروقات',
    reasonPrompt: 'أدخل السبب', confirmAction: 'هل تريد تنفيذ هذا الإجراء؟', saved: 'تم حفظ التحويل.', actionCompleted: 'تم تنفيذ الإجراء بنجاح.',
    error: 'تعذر إكمال العملية.', unsaved: 'لديك تغييرات غير محفوظة. هل تريد المغادرة؟', noHistory: 'لا يوجد سجل حالة بعد.', workflowActions: 'إجراءات سير العمل',
    transferDetails: 'بيانات التحويل', transferDetailsDescription: 'حدد مسار التحويل ومرجعه قبل إضافة البنود.', transferItemsDescription: 'أضف المنتجات والكميات وحدد المواقع المطلوبة بحسب سياسة كل مستودع.', requiredField: 'مطلوب', optionalField: 'اختياري',
    workflowProgress: 'تقدم سير العمل', nextSubmit: 'الخطوة التالية: إرسال التحويل للاعتماد.', nextApprove: 'يتطلب الإرسال اعتماد التحويل أولًا.', nextPost: 'الخطوة التالية: إرسال التحويل من فرع المصدر.', rejectedGuidance: 'التحويل مرفوض؛ أعده إلى المسودة قبل إرساله مجددًا.', postedGuidance: 'تحويل قديم مرحّل بالكامل.', cancelledGuidance: 'تم إلغاء التحويل وعكس حركاته.', inTransitGuidance: 'سجّل الكميات المقبولة والفروقات من فرع الوجهة.', partialGuidance: 'بقيت كميات قيد الاستلام.', exceptionGuidance: 'هناك نقص أو تلف مبلغ عنه؛ يبقى مفتوحًا دون خسارة تلقائية.', receivedGuidance: 'اكتمل الاستلام ونقلت الكميات المقبولة.',
  },
  'en-US': {
    inventoryNavigation: 'Inventory', title: 'Stock transfers', description: 'Create, review, approve, and post transfers while the server remains authoritative for stock movements.',
    newTransfer: 'New transfer', loading: 'Loading stock transfers', empty: 'No stock transfers', emptyDescription: 'Create the first transfer or change the search filters.',
    search: 'Search', searchPlaceholder: 'Transfer number or reference', status: 'Status', allStatuses: 'All statuses', apply: 'Apply', number: 'Transfer number',
    sourceWarehouse: 'Source warehouse', destinationWarehouse: 'Destination warehouse', items: 'Items', createdAt: 'Created at', actions: 'Actions', details: 'Details',
    draft: 'Draft', submitted: 'Submitted', approved: 'Approved', rejected: 'Rejected', posted: 'Legacy posted', cancelled: 'Cancelled', inTransit: 'In transit', partiallyReceived: 'Partially received', receiptException: 'Open receipt discrepancy', received: 'Fully received',
    formDescription: 'Choose warehouses by name, then add products, quantities, and required positions.', back: 'Back to transfers', reference: 'Reference', notes: 'Notes',
    product: 'Product', barcode: 'Barcode', quantity: 'Quantity', sourceLocation: 'Source position', destinationLocation: 'Destination position', automatic: 'Automatic by warehouse policy',
    tracking: 'Lot or serial allocations', trackingHelp: 'Enter a lot and quantity or one serial per line; leave empty to use the warehouse strategy.',
    addItem: 'Add item', remove: 'Remove', save: 'Save transfer', saving: 'Saving', correctFields: 'Check distinct warehouses, every product and quantity, and required positions.',
    detailDescription: 'Review items and status evidence, then perform only an available action.', createdBy: 'Created by', history: 'Status history', fromStatus: 'From', toStatus: 'To', user: 'User', reason: 'Reason',
    edit: 'Edit', submit: 'Submit', approve: 'Approve', reject: 'Reject', returnToDraft: 'Return to draft', post: 'Dispatch from source', cancel: 'Cancel and reverse', receive: 'Record receipt', receivedQuantity: 'Received', remainingQuantity: 'Remaining', shortQuantity: 'Short', damagedQuantity: 'Damaged', receiptEvidence: 'Receipt allocation and discrepancies',
    reasonPrompt: 'Enter the reason', confirmAction: 'Perform this action?', saved: 'The transfer was saved.', actionCompleted: 'The action completed successfully.',
    error: 'The operation could not be completed.', unsaved: 'You have unsaved changes. Leave this page?', noHistory: 'No status history yet.', workflowActions: 'Workflow actions',
    transferDetails: 'Transfer details', transferDetailsDescription: 'Define the transfer route and reference before adding line items.', transferItemsDescription: 'Add products and quantities, then choose positions required by each warehouse policy.', requiredField: 'Required', optionalField: 'Optional',
    workflowProgress: 'Workflow progress', nextSubmit: 'Next: submit the transfer for approval.', nextApprove: 'The transfer must be approved before dispatch.', nextPost: 'Next: dispatch from the source branch.', rejectedGuidance: 'The transfer was rejected; return it to draft before resubmitting.', postedGuidance: 'Legacy transfer posted in one step.', cancelledGuidance: 'The transfer was cancelled and its movements were reversed.', inTransitGuidance: 'Record accepted quantities and discrepancies from the destination branch.', partialGuidance: 'Quantities remain open for receipt.', exceptionGuidance: 'A shortage or damage is reported and remains open without automatic loss posting.', receivedGuidance: 'Receipt is complete and accepted quantities were moved.',
  },
} as const;

export type StockTransferMessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class StockTransferLocalizationService {
  private readonly global = inject(LocalizationService);
  text(key: StockTransferMessageKey): string { return messages[this.global.locale()][key]; }
}
