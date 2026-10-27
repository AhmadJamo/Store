import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const messages = {
  'ar-JO': {
    inventoryLedger: 'سجل حركات المخزون', inventoryLedgerDescription: 'راجع الحركات والكميات والتكلفة والقيمة التي احتسبها الخادم دون تغيير السجل التاريخي.',
    searchLedger: 'البحث في السجل', searchLedgerPlaceholder: 'المنتج أو المستودع أو المرجع أو نوع الحركة', transactionType: 'نوع الحركة', allTypes: 'كل الأنواع',
    movement: 'الاتجاه', allMovements: 'كل الحركات', incoming: 'وارد', outgoing: 'صادر', fromDate: 'من تاريخ', toDate: 'إلى تاريخ', rowsPerPage: 'عدد الصفوف', clearFilters: 'مسح المرشحات',
    loading: 'جارٍ تحميل سجل المخزون', noTransactions: 'لا توجد حركات مخزون', noTransactionsDescription: 'غيّر البحث أو المرشحات الزمنية لعرض نتائج أخرى.',
    id: 'الرقم', product: 'المنتج', warehouse: 'المستودع', quantity: 'الكمية', unitCost: 'تكلفة الوحدة', movementValue: 'قيمة الحركة', costVariance: 'فرق التكلفة', type: 'النوع', reference: 'المرجع', date: 'التاريخ',
    totalRecords: 'إجمالي الحركات', previous: 'السابق', next: 'التالي', ascending: 'تصاعدي', descending: 'تنازلي',
    openingBalance: 'رصيد افتتاحي', purchase: 'شراء', sale: 'بيع', transferIn: 'تحويل وارد', transferOut: 'تحويل صادر', adjustmentIn: 'تسوية بالزيادة', adjustmentOut: 'تسوية بالنقص',
    kitchenVariance: 'فرق مطبخ', recipeConsumption: 'استهلاك وصفة', salesReturn: 'مرتجع مبيعات', purchaseReturn: 'مرتجع مشتريات', goodsReceipt: 'استلام بضاعة', goodsReceiptReturn: 'مرتجع استلام',
    landedCostAdjustment: 'تسوية تكلفة إضافية', supplierCreditNoteReturn: 'مرتجع إشعار دائن مورد', goodsReceiptReversal: 'عكس استلام بضاعة', landedCostReversal: 'عكس تكلفة إضافية', customerReturnDisposition: 'معالجة مرتجع زبون',
    productStocks: 'أرصدة المنتجات', productStocksDescription: 'إدارة الرصيد الافتتاحي والكمية الحالية والتسويات اليدوية مع إبقاء التكلفة والمواقع تحت سلطة الخادم.', backToLedger: 'العودة لسجل الحركات',
    addOpeningStock: 'إضافة رصيد افتتاحي', editCurrentStock: 'تعديل الكمية الحالية', selectProduct: 'اختر المنتج', selectWarehouse: 'اختر المستودع', averageUnitCost: 'متوسط تكلفة الوحدة', inventoryValue: 'قيمة المخزون', actions: 'الإجراءات',
    save: 'حفظ', saving: 'جارٍ الحفظ', cancel: 'إلغاء', edit: 'تعديل', delete: 'حذف', noStockRecords: 'لا توجد سجلات أرصدة.', noStockRecordsDescription: 'أضف رصيدًا افتتاحيًا عند توفر الصلاحية أو راجع المرشحات.',
    manualAdjustment: 'تسوية مخزون يدوية', manualAdjustmentDescription: 'سجّل زيادة أو نقصًا موجب الكمية مع سبب واضح. يحتسب الخادم أثر الرصيد والتكلفة.', adjustmentReason: 'سبب التسوية', adjustmentReasonPlaceholder: 'مثال: تصحيح نتيجة الجرد الفعلي', kitchenVarianceHelp: 'فرق المطبخ قد ينشئ رصيدًا سالبًا مضبوطًا فقط عندما يسمح إعداد المكوّن بذلك.',
    saveAdjustment: 'تسجيل التسوية', stockSaved: 'تم حفظ سجل الرصيد.', adjustmentSaved: 'تم تسجيل التسوية.', correctStockFields: 'تحقق من المنتج والمستودع والكمية.', correctAdjustmentFields: 'تحقق من المنتج والمستودع والنوع والكمية والسبب.', confirmDeleteStock: 'هل تريد محاولة حذف سجل الرصيد؟ قد يطلب النظام تعديل الكمية إلى صفر بدل الحذف.', unsavedDescription: 'لديك تغييرات غير محفوظة. هل تريد مغادرة الصفحة؟', errorDescription: 'تعذر إكمال العملية.',
  },
  'en-US': {
    inventoryLedger: 'Inventory ledger', inventoryLedgerDescription: 'Review server-calculated movements, quantities, costs, and values without changing historical evidence.',
    searchLedger: 'Search ledger', searchLedgerPlaceholder: 'Product, warehouse, reference, or movement type', transactionType: 'Transaction type', allTypes: 'All types',
    movement: 'Direction', allMovements: 'All movements', incoming: 'Incoming', outgoing: 'Outgoing', fromDate: 'From date', toDate: 'To date', rowsPerPage: 'Rows per page', clearFilters: 'Clear filters',
    loading: 'Loading inventory ledger', noTransactions: 'No inventory transactions', noTransactionsDescription: 'Change the search or date filters to see other results.',
    id: 'ID', product: 'Product', warehouse: 'Warehouse', quantity: 'Quantity', unitCost: 'Unit cost', movementValue: 'Movement value', costVariance: 'Cost variance', type: 'Type', reference: 'Reference', date: 'Date',
    totalRecords: 'Total transactions', previous: 'Previous', next: 'Next', ascending: 'Ascending', descending: 'Descending',
    openingBalance: 'Opening balance', purchase: 'Purchase', sale: 'Sale', transferIn: 'Transfer in', transferOut: 'Transfer out', adjustmentIn: 'Adjustment in', adjustmentOut: 'Adjustment out',
    kitchenVariance: 'Kitchen variance', recipeConsumption: 'Recipe consumption', salesReturn: 'Sales return', purchaseReturn: 'Purchase return', goodsReceipt: 'Goods receipt', goodsReceiptReturn: 'Goods receipt return',
    landedCostAdjustment: 'Landed cost adjustment', supplierCreditNoteReturn: 'Supplier credit-note return', goodsReceiptReversal: 'Goods receipt reversal', landedCostReversal: 'Landed cost reversal', customerReturnDisposition: 'Customer return disposition',
    productStocks: 'Product stock balances', productStocksDescription: 'Manage opening balances, current quantities, and manual adjustments while the server remains authoritative for cost and locations.', backToLedger: 'Back to transaction ledger',
    addOpeningStock: 'Add opening stock', editCurrentStock: 'Edit current quantity', selectProduct: 'Select product', selectWarehouse: 'Select warehouse', averageUnitCost: 'Average unit cost', inventoryValue: 'Inventory value', actions: 'Actions',
    save: 'Save', saving: 'Saving', cancel: 'Cancel', edit: 'Edit', delete: 'Delete', noStockRecords: 'No stock records found.', noStockRecordsDescription: 'Add an opening balance when permitted or review the filters.',
    manualAdjustment: 'Manual inventory adjustment', manualAdjustmentDescription: 'Record a positive increase or decrease with a clear reason. The server calculates balance and cost effects.', adjustmentReason: 'Adjustment reason', adjustmentReasonPlaceholder: 'Example: Physical count correction', kitchenVarianceHelp: 'Kitchen variance may create a controlled negative balance only when enabled for the ingredient.',
    saveAdjustment: 'Record adjustment', stockSaved: 'Stock record saved.', adjustmentSaved: 'Adjustment recorded.', correctStockFields: 'Check the product, warehouse, and quantity.', correctAdjustmentFields: 'Check the product, warehouse, type, quantity, and reason.', confirmDeleteStock: 'Try to delete this stock record? The server may require adjusting quantity to zero instead.', unsavedDescription: 'You have unsaved changes. Leave this page?', errorDescription: 'The operation could not be completed.',
  },
} as const;

export type InventoryMessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class InventoryLocalizationService {
  private readonly global = inject(LocalizationService);
  text(key: InventoryMessageKey): string { return messages[this.global.locale()][key]; }
}
