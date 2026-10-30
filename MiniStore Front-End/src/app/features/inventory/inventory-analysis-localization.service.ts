import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const messages = {
  'ar-JO': {
    insights: 'رؤى المخزون', insightsDescription: 'تقرير للقراءة فقط يصنف النشاط من حركات الصرف الفعلية دون تغيير المخزون أو المحاسبة.',
    reconciliation: 'مصالحة المخزون', reconciliationDescription: 'مقارنة للقراءة فقط بين رصيد المستودع وتوزيع المواقع وآخر لقطة حركة.',
    openInsights: 'فتح الرؤى', openReconciliation: 'فتح المصالحة', search: 'بحث المنتج', searchPlaceholder: 'الاسم أو الرمز أو الباركود',
    warehouse: 'المستودع', allWarehouses: 'كل المستودعات', status: 'الحالة', allStatuses: 'كل الحالات', slowDays: 'أيام البطء',
    deadDays: 'أيام الركود', attentionOnly: 'بحاجة لانتباه فقط', exceptionsOnly: 'الاستثناءات فقط', apply: 'تطبيق', loading: 'جارٍ تحميل التقرير',
    totalValue: 'قيمة المخزون الحالية', attentionValue: 'قيمة المخزون المحتاج لانتباه', slow: 'بطيء الحركة', dead: 'راكد',
    active: 'نشط', neverIssued: 'لم يُصرف', noMovement: 'لا يوجد سجل حركة', product: 'المنتج', onHand: 'الكمية الفعلية',
    inventoryValue: 'قيمة المخزون', lastOutbound: 'آخر صرف', daysSinceActivity: 'أيام منذ النشاط', never: 'أبدًا',
    noInsights: 'لا توجد نتائج تطابق المرشحات.', exceptions: 'الاستثناءات', displayedRows: 'الصفوف المعروضة', pageSize: 'حجم الصفحة',
    consistent: 'متطابق', warehouseQuantity: 'كمية المستودع', assignedQuantity: 'كمية المواقع', unassignedQuantity: 'غير المعيّن',
    latestQuantity: 'كمية آخر حركة', latestMovement: 'آخر حركة', readOnlyHelp: 'لا يغيّر هذا التقرير المخزون؛ عالج الاستثناء من مستنده الأصلي أو بتسوية معتمدة.',
    noReconciliation: 'لا توجد صفوف مصالحة تطابق المرشحات.', previous: 'السابق', next: 'التالي', page: 'صفحة',
    issueMissingWarehouseStock: 'رصيد موقع بلا رصيد مستودع', issueOverAllocated: 'كمية المواقع تتجاوز المستودع',
    issueNegativeLocation: 'يوجد موقع بكمية سالبة', issueSimpleLocation: 'مستودع بسيط يحتوي أرصدة مواقع',
    issueMissingTransaction: 'رصيد المستودع بلا حركة مخزون', issueQuantityMismatch: 'كمية آخر حركة غير مطابقة',
    issueValueMismatch: 'قيمة آخر حركة غير مطابقة', error: 'تعذر تحميل التقرير.',
  },
  'en-US': {
    insights: 'Inventory insights', insightsDescription: 'A read-only report classifying activity from actual outbound movements without changing inventory or accounting.',
    reconciliation: 'Inventory reconciliation', reconciliationDescription: 'A read-only comparison of warehouse balances, position allocations, and the latest movement snapshot.',
    openInsights: 'Open insights', openReconciliation: 'Open reconciliation', search: 'Product search', searchPlaceholder: 'Name, code, or barcode',
    warehouse: 'Warehouse', allWarehouses: 'All warehouses', status: 'Status', allStatuses: 'All statuses', slowDays: 'Slow days',
    deadDays: 'Dead days', attentionOnly: 'Attention only', exceptionsOnly: 'Exceptions only', apply: 'Apply', loading: 'Loading report',
    totalValue: 'Current inventory value', attentionValue: 'Attention inventory value', slow: 'Slow moving', dead: 'Dead stock',
    active: 'Active', neverIssued: 'Never issued', noMovement: 'No movement history', product: 'Product', onHand: 'On hand',
    inventoryValue: 'Inventory value', lastOutbound: 'Last outbound', daysSinceActivity: 'Days since activity', never: 'Never',
    noInsights: 'No inventory insights match the filters.', exceptions: 'Exceptions', displayedRows: 'Displayed rows', pageSize: 'Page size',
    consistent: 'Consistent', warehouseQuantity: 'Warehouse quantity', assignedQuantity: 'Assigned quantity', unassignedQuantity: 'Unassigned quantity',
    latestQuantity: 'Latest movement quantity', latestMovement: 'Latest movement', readOnlyHelp: 'This report never changes inventory; resolve exceptions through the owning document or an approved adjustment.',
    noReconciliation: 'No reconciliation rows match the filters.', previous: 'Previous', next: 'Next', page: 'Page',
    issueMissingWarehouseStock: 'Position balance has no warehouse balance', issueOverAllocated: 'Assigned quantity exceeds warehouse quantity',
    issueNegativeLocation: 'A position has a negative quantity', issueSimpleLocation: 'Simple warehouse has position balances',
    issueMissingTransaction: 'Warehouse balance has no inventory transaction', issueQuantityMismatch: 'Latest movement quantity does not match',
    issueValueMismatch: 'Latest movement value does not match', error: 'The report could not be loaded.',
  },
} as const;

export type InventoryAnalysisMessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class InventoryAnalysisLocalizationService {
  private readonly global = inject(LocalizationService);
  text(key: InventoryAnalysisMessageKey): string { return messages[this.global.locale()][key]; }
}
