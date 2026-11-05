import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const c = {
  en: {
    area: 'Accounting',
    title: 'Journal entry',
    description: 'Immutable posting evidence with named accounts and dimensions.',
    date: 'Posting date', status: 'Status',
    sourceType: 'Source type',
    sourceReference: 'Source reference',
    entryDescription: 'Description',
    account: 'Account',
    branch: 'Branch',
    warehouse: 'Warehouse',
    debit: 'Debit',
    credit: 'Credit',
    lineDescription: 'Line description',
    totalDebit: 'Total debit',
    totalCredit: 'Total credit',
    posted: 'Posted',
    reversed: 'Reversed',
    draft: 'Draft',
    reversalOf: 'Reversal of entry',
    directoryTitle: 'Journal entries', directoryDescription: 'Searchable posting evidence with balanced totals and source references.', search: 'Search entries', searchPlaceholder: 'Entry number, source, reference or description', fromDate: 'From date', toDate: 'To date', allStatuses: 'All statuses', view: 'View entry', noEntries: 'No journal entries match these filters.', totalRecords: 'Total entries', previous: 'Previous', next: 'Next', filters: 'Journal filters', filteredAccount: 'Journal evidence for account',
  },
  ar: {
    area: 'المحاسبة',
    title: 'القيد اليومي',
    description: 'دليل ترحيل ثابت مع أسماء الحسابات والأبعاد.',
    date: 'تاريخ الترحيل', status: 'الحالة',
    sourceType: 'نوع المصدر',
    sourceReference: 'مرجع المصدر',
    entryDescription: 'الوصف',
    account: 'الحساب',
    branch: 'الفرع',
    warehouse: 'المستودع',
    debit: 'مدين',
    credit: 'دائن',
    lineDescription: 'وصف السطر',
    totalDebit: 'إجمالي المدين',
    totalCredit: 'إجمالي الدائن',
    posted: 'مُرحّل',
    reversed: 'معكوس',
    draft: 'مسودة',
    reversalOf: 'عكس للقيد',
    directoryTitle: 'دليل القيود اليومية', directoryDescription: 'دليل ترحيل قابل للبحث يعرض التوازن ومراجع المصادر.', search: 'بحث القيود', searchPlaceholder: 'رقم القيد أو المصدر أو المرجع أو الوصف', fromDate: 'من تاريخ', toDate: 'إلى تاريخ', allStatuses: 'كل الحالات', view: 'فتح القيد', noEntries: 'لا توجد قيود تطابق هذه المرشحات.', totalRecords: 'إجمالي القيود', previous: 'السابق', next: 'التالي', filters: 'مرشحات القيود', filteredAccount: 'دليل القيود للحساب',
  },
} as const;
export type JournalKey = keyof typeof c.en;
@Injectable({ providedIn: 'root' })
export class JournalEntryLocalizationService {
  private readonly s = inject(ApiSessionService);
  text(k: JournalKey) {
    return c[this.s.session()?.culture?.startsWith('ar') ? 'ar' : 'en'][k];
  }
  status(v: number) {
    return this.text((['', 'draft', 'posted', 'reversed'][v] ?? 'draft') as JournalKey);
  }
}
