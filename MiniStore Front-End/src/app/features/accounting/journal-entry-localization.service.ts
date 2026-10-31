import { inject, Injectable } from '@angular/core';
import { ApiSessionService } from '../../core/api/api-session.service';
const c = {
  en: {
    area: 'Accounting',
    title: 'Journal entry',
    description: 'Immutable posting evidence with named accounts and dimensions.',
    date: 'Posting date',
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
  },
  ar: {
    area: 'المحاسبة',
    title: 'القيد اليومي',
    description: 'دليل ترحيل ثابت مع أسماء الحسابات والأبعاد.',
    date: 'تاريخ الترحيل',
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
