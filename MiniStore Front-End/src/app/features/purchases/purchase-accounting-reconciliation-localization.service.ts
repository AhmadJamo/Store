import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';

const copy = {
  en: {
    title: 'Purchase accounting reconciliation',
    description:
      'Read-only comparison of inventory and GRNI operational values with the general ledger.',
    asOf: 'As of date',
    exceptionsOnly: 'Exceptions and open receipts only',
    apply: 'Apply',
    loading: 'Loading reconciliation evidence…',
    readOnly:
      'This report never changes inventory or accounting. Differences must be corrected through source documents or an approved adjustment workflow.',
    historicalTitle: 'Historical inventory note:',
    historical:
      'Inventory history uses movement time, while accounting uses the posting date. Backdated documents can appear as timing differences.',
    operationalInventory: 'Operational inventory value',
    warehouseSnapshots: 'Warehouse value snapshots',
    inventoryLedger: 'Inventory ledger balance',
    difference: 'Difference',
    operationalGrni: 'Operational GRNI balance',
    openReceipts: 'Open receipts',
    grniLedger: 'GRNI ledger balance',
    configureGrni: 'Configure a goods received not invoiced account to reconcile GRNI.',
    accountingSettings: 'Open accounting settings',
    contactAdmin: 'Contact an administrator to complete this setup.',
    inventoryToLedger: 'Inventory to general ledger',
    inventoryDescription: 'Each warehouse is compared with its assigned inventory account.',
    exceptions: 'Exceptions',
    accountMissing: 'Inventory account is not assigned',
    matched: 'Matched',
    review: 'Needs review',
    operationalValue: 'Operational value',
    ledgerBalance: 'Ledger balance',
    viewStock: 'View warehouse stock',
    assignAccount: 'Assign inventory account',
    noInventoryExceptions: 'No inventory reconciliation exceptions were found.',
    grniDetails: 'GRNI receipt details',
    grniDescription:
      'Receipt value less unbilled returns and vendor bill clearing equals the expected GRNI balance.',
    grniAccount: 'GRNI account',
    open: 'Open',
    cleared: 'Cleared',
    purchaseOrder: 'Purchase order',
    receiptValue: 'Receipt value',
    returned: 'Returned',
    billedClearing: 'Billed clearing',
    outstanding: 'GRNI outstanding',
    settlements: 'Settlement documents',
    noOpenReceipts: 'No open GRNI receipts were found.',
  },
  ar: {
    title: 'مطابقة محاسبة المشتريات',
    description:
      'مقارنة للقراءة فقط بين القيم التشغيلية للمخزون والبضاعة المستلمة غير المفوترة ودفتر الأستاذ العام.',
    asOf: 'كما في تاريخ',
    exceptionsOnly: 'الفروقات والاستلامات المفتوحة فقط',
    apply: 'تطبيق',
    loading: 'جارٍ تحميل أدلة المطابقة…',
    readOnly:
      'هذا التقرير لا يغيّر المخزون أو المحاسبة. يجب معالجة الفروقات من مستنداتها المصدرية أو عبر إجراء تسوية معتمد.',
    historicalTitle: 'ملاحظة المخزون التاريخي:',
    historical:
      'يستخدم سجل المخزون وقت الحركة، بينما تستخدم المحاسبة تاريخ الترحيل. قد تظهر المستندات المؤرخة بأثر رجعي كفروقات توقيت.',
    operationalInventory: 'قيمة المخزون التشغيلية',
    warehouseSnapshots: 'لقطات قيمة المستودعات',
    inventoryLedger: 'رصيد المخزون في دفتر الأستاذ',
    difference: 'الفرق',
    operationalGrni: 'رصيد البضاعة المستلمة غير المفوترة التشغيلي',
    openReceipts: 'الاستلامات المفتوحة',
    grniLedger: 'رصيد حساب البضاعة المستلمة غير المفوترة',
    configureGrni: 'هيّئ حساب البضاعة المستلمة غير المفوترة لإجراء المطابقة.',
    accountingSettings: 'فتح إعدادات المحاسبة',
    contactAdmin: 'تواصل مع المسؤول لإكمال هذا الإعداد.',
    inventoryToLedger: 'المخزون مقابل دفتر الأستاذ',
    inventoryDescription: 'تتم مقارنة كل مستودع مع حساب المخزون المعيّن له.',
    exceptions: 'الفروقات',
    accountMissing: 'لم يتم تعيين حساب مخزون',
    matched: 'متطابق',
    review: 'بحاجة للمراجعة',
    operationalValue: 'القيمة التشغيلية',
    ledgerBalance: 'رصيد دفتر الأستاذ',
    viewStock: 'عرض مخزون المستودع',
    assignAccount: 'تعيين حساب المخزون',
    noInventoryExceptions: 'لم يتم العثور على فروقات في مطابقة المخزون.',
    grniDetails: 'تفاصيل الاستلامات غير المفوترة',
    grniDescription:
      'قيمة الاستلام ناقص المرتجعات غير المفوترة وتسويات فواتير المورد تساوي الرصيد المتوقع.',
    grniAccount: 'حساب البضاعة المستلمة غير المفوترة',
    open: 'مفتوح',
    cleared: 'مسوّى',
    purchaseOrder: 'أمر الشراء',
    receiptValue: 'قيمة الاستلام',
    returned: 'المرتجع',
    billedClearing: 'تسوية الفواتير',
    outstanding: 'الرصيد غير المفوتر',
    settlements: 'مستندات التسوية',
    noOpenReceipts: 'لا توجد استلامات مفتوحة ضمن البضاعة المستلمة غير المفوترة.',
  },
} as const;

export type ReconciliationKey = keyof typeof copy.en;

@Injectable({ providedIn: 'root' })
export class PurchaseAccountingReconciliationLocalizationService {
  private readonly session = inject(ApiSessionService);
  text(key: ReconciliationKey) {
    return copy[this.session.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][key];
  }
}
