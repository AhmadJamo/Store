import { DOCUMENT } from '@angular/common';
import { computed, inject, Injectable, signal } from '@angular/core';

export type UiLocale = 'ar-JO' | 'en-US';

const messages = {
  'ar-JO': {
    appName: 'MiniStore ERP', workspace: 'مساحة العمل', dashboard: 'الرئيسية', search: 'بحث سريع', language: 'English',
    phaseTitle: 'تحديث تدريجي وآمن',
    phaseDescription: 'يتم بناء واجهة Angular مع الحفاظ على نظام MVC الحالي وقواعد الأعمال دون تغيير.',
    foundation: 'الأساس التقني', api: 'عقد API آمن', navigation: 'محرك مساحة العمل', customers: 'أول مسار فعلي: الزبائن',
    ready: 'تم تجهيز Angular foundation', protected: 'MVC وقواعد الأعمال وقاعدة البيانات لم تتغير.',
    noTabs: 'ستظهر الصفحات المفتوحة هنا عند بدء محرك مساحة العمل.',
  },
  'en-US': {
    appName: 'MiniStore ERP', workspace: 'Workspace', dashboard: 'Home', search: 'Quick search', language: 'العربية',
    phaseTitle: 'Safe incremental modernization',
    phaseDescription: 'The Angular experience is being built while preserving the existing MVC system and business rules.',
    foundation: 'Technical foundation', api: 'Secure API contract', navigation: 'Workspace engine', customers: 'First real slice: Customers',
    ready: 'Angular foundation is ready', protected: 'MVC, business rules, and the database remain unchanged.',
    noTabs: 'Open pages will appear here when the workspace engine begins.',
  },
} as const;

export type MessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private readonly document = inject(DOCUMENT);
  readonly locale = signal<UiLocale>('ar-JO');
  readonly direction = computed(() => (this.locale() === 'ar-JO' ? 'rtl' : 'ltr'));

  constructor() { this.applyDocumentLanguage(); }
  text(key: MessageKey): string { return messages[this.locale()][key]; }
  toggle(): void { this.locale.update((value) => value === 'ar-JO' ? 'en-US' : 'ar-JO'); this.applyDocumentLanguage(); }

  private applyDocumentLanguage(): void {
    this.document.documentElement.lang = this.locale();
    this.document.documentElement.dir = this.direction();
  }
}
