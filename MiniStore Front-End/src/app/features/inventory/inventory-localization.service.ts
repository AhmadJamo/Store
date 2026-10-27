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
  },
} as const;

export type InventoryMessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class InventoryLocalizationService {
  private readonly global = inject(LocalizationService);
  text(key: InventoryMessageKey): string { return messages[this.global.locale()][key]; }
}
