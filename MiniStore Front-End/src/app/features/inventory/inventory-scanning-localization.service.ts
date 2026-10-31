import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const messages = {
  'ar-JO': {
    title: 'المسح التشغيلي للمخزون', description: 'امسح المنتج والمواقع أو مستند الجرد أولًا؛ لا يتغير المخزون إلا بعد تنفيذ إجراء مصرح.',
    adjustment: 'رقم مستند الجرد', product: 'باركود أو رمز المنتج', source: 'باركود أو رمز موقع المصدر', destination: 'باركود أو رمز موقع الوجهة',
    resolve: 'تحليل الرموز', resolving: 'جارٍ التحليل', waiting: 'بانتظار المسح', resolved: 'تم التعرّف', notFound: 'غير موجود', ambiguous: 'رمز ملتبس',
    countDocument: 'مستند الجرد', productScan: 'المنتج', sourceScan: 'موقع المصدر', destinationScan: 'موقع الوجهة',
    putaway: 'تسكين المخزون الممسوح', putawayHelp: 'ينقل الكمية من غير المعيّن إلى موقع الوجهة الممسوح.', relocate: 'نقل المخزون الممسوح',
    relocateHelp: 'ينقل الكمية بين موقعي المصدر والوجهة الممسوحين داخل المستودع نفسه.', count: 'تسجيل العدّ الممسوح',
    countHelp: 'يحدّث كمية سطر واحد في مستند جرد مسودة ولا يرحّل فرق المخزون.', quantity: 'الكمية', countedQuantity: 'الكمية المعدودة',
    postPutaway: 'تنفيذ التسكين', postRelocation: 'تنفيذ النقل', recordCount: 'تسجيل العدّ', working: 'جارٍ التنفيذ',
    blindCount: 'هذا جرد أعمى؛ تبقى الأرصدة الحالية مخفية حتى اكتمال العدّ.', balances: 'الأرصدة الحالية', warehouse: 'المستودع',
    location: 'الموقع', onHand: 'الفعلي', reserved: 'المحجوز', available: 'المتاح', unassigned: 'غير معيّن (النظام)',
    noBalances: 'لم يُعثر على رصيد مطابق للرموز المحللة.', successPutaway: 'تم تنفيذ التسكين.', successRelocation: 'تم تنفيذ النقل.',
    successCount: 'تم تسجيل العدّ.', error: 'تعذر إكمال العملية.', invalidQuantity: 'أدخل كمية صالحة.', status: 'حالة التحليل',
  },
  'en-US': {
    title: 'Inventory scanning', description: 'Scan the product, positions, or count document first; inventory changes only after an authorized action.',
    adjustment: 'Inventory count number', product: 'Product barcode or code', source: 'Source-position barcode or code', destination: 'Destination-position barcode or code',
    resolve: 'Resolve scans', resolving: 'Resolving', waiting: 'Waiting for scan', resolved: 'Resolved', notFound: 'Not found', ambiguous: 'Ambiguous scan',
    countDocument: 'Count document', productScan: 'Product', sourceScan: 'Source position', destinationScan: 'Destination position',
    putaway: 'Put away scanned stock', putawayHelp: 'Moves the quantity from Unassigned into the scanned destination position.', relocate: 'Relocate scanned stock',
    relocateHelp: 'Moves the quantity between the scanned source and destination positions in the same warehouse.', count: 'Record scanned count',
    countHelp: 'Updates one draft count line and does not post an inventory variance.', quantity: 'Quantity', countedQuantity: 'Counted quantity',
    postPutaway: 'Post putaway', postRelocation: 'Post relocation', recordCount: 'Record count', working: 'Working',
    blindCount: 'This is a blind count; current balances stay hidden until counting is complete.', balances: 'Current balances', warehouse: 'Warehouse',
    location: 'Position', onHand: 'On hand', reserved: 'Reserved', available: 'Available', unassigned: 'Unassigned (system)',
    noBalances: 'No inventory balance matched the resolved scans.', successPutaway: 'Putaway was posted.', successRelocation: 'Relocation was posted.',
    successCount: 'The count was recorded.', error: 'The operation could not be completed.', invalidQuantity: 'Enter a valid quantity.', status: 'Resolution status',
  },
} as const;

export type InventoryScanningMessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class InventoryScanningLocalizationService {
  private readonly global = inject(LocalizationService);
  text(key: InventoryScanningMessageKey): string { return messages[this.global.locale()][key]; }
}
