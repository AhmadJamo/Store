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
    designSystem: 'نظام التصميم', theme: 'المظهر', density: 'كثافة العرض', resetPreferences: 'إعادة الضبط',
    themeSystem: 'حسب النظام', themeLight: 'فاتح', themeDark: 'داكن', densityComfortable: 'مريح', densityCompact: 'مضغوط',
    visualTitle: 'معاينة نظام التصميم', visualDescription: 'مرجع حي للحالات المشتركة قبل بناء شاشات الأعمال.',
    actions: 'الإجراءات', primaryAction: 'إجراء رئيسي', secondaryAction: 'إجراء ثانوي', disabledAction: 'إجراء غير متاح',
    statuses: 'الحالات', statusSuccess: 'مكتمل', statusWarning: 'يحتاج مراجعة', statusDanger: 'متعذر', statusInfo: 'معلومة',
    formExample: 'مثال الحقول', customerName: 'اسم الزبون', customerNamePlaceholder: 'أدخل اسم الزبون', save: 'حفظ',
    emptyTitle: 'لا توجد نتائج', emptyDescription: 'غيّر معايير البحث أو أنشئ سجلًا جديدًا عند توفر الصلاحية.',
    loading: 'جارٍ التحميل', errorTitle: 'تعذر تحميل البيانات', errorDescription: 'يمكن إعادة المحاولة دون فقدان التغييرات المحلية.', retry: 'إعادة المحاولة',
    workspaceTabs: 'تبويبات مساحة العمل', back: 'السابق', forward: 'التالي', close: 'إغلاق', closeOthers: 'إغلاق التبويبات الأخرى',
    reopenClosed: 'إعادة فتح آخر تبويب', unsavedTitle: 'تغييرات غير محفوظة',
    unsavedDescription: 'يحتوي هذا التبويب على تغييرات غير محفوظة. هل تريد المتابعة؟',
    continueAction: 'متابعة', cancel: 'إلغاء', dirty: 'غير محفوظ',
  },
  'en-US': {
    appName: 'MiniStore ERP', workspace: 'Workspace', dashboard: 'Home', search: 'Quick search', language: 'العربية',
    phaseTitle: 'Safe incremental modernization',
    phaseDescription: 'The Angular experience is being built while preserving the existing MVC system and business rules.',
    foundation: 'Technical foundation', api: 'Secure API contract', navigation: 'Workspace engine', customers: 'First real slice: Customers',
    ready: 'Angular foundation is ready', protected: 'MVC, business rules, and the database remain unchanged.',
    noTabs: 'Open pages will appear here when the workspace engine begins.',
    designSystem: 'Design system', theme: 'Theme', density: 'Display density', resetPreferences: 'Reset',
    themeSystem: 'System', themeLight: 'Light', themeDark: 'Dark', densityComfortable: 'Comfortable', densityCompact: 'Compact',
    visualTitle: 'Design system preview', visualDescription: 'A live reference for shared states before business screens are built.',
    actions: 'Actions', primaryAction: 'Primary action', secondaryAction: 'Secondary action', disabledAction: 'Unavailable action',
    statuses: 'Statuses', statusSuccess: 'Complete', statusWarning: 'Needs review', statusDanger: 'Blocked', statusInfo: 'Information',
    formExample: 'Field example', customerName: 'Customer name', customerNamePlaceholder: 'Enter customer name', save: 'Save',
    emptyTitle: 'No results', emptyDescription: 'Change the search criteria or create a new record when permitted.',
    loading: 'Loading', errorTitle: 'Data could not be loaded', errorDescription: 'You can retry without losing local changes.', retry: 'Retry',
    workspaceTabs: 'Workspace tabs', back: 'Back', forward: 'Forward', close: 'Close', closeOthers: 'Close other tabs',
    reopenClosed: 'Reopen last closed tab', unsavedTitle: 'Unsaved changes',
    unsavedDescription: 'This tab contains unsaved changes. Do you want to continue?',
    continueAction: 'Continue', cancel: 'Cancel', dirty: 'Unsaved',
  },
} as const;

export type MessageKey = keyof (typeof messages)['en-US'];

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private readonly document = inject(DOCUMENT);
  readonly locale = signal<UiLocale>(this.restoreLocale());
  readonly direction = computed(() => (this.locale() === 'ar-JO' ? 'rtl' : 'ltr'));

  constructor() { this.applyDocumentLanguage(); }
  text(key: MessageKey): string { return messages[this.locale()][key]; }
  toggle(): void {
    this.locale.update((value) => value === 'ar-JO' ? 'en-US' : 'ar-JO');
    try { globalThis.localStorage?.setItem('ministore.ui.locale', this.locale()); } catch { /* in-memory fallback */ }
    this.applyDocumentLanguage();
  }

  private restoreLocale(): UiLocale {
    try { return globalThis.localStorage?.getItem('ministore.ui.locale') === 'en-US' ? 'en-US' : 'ar-JO'; }
    catch { return 'ar-JO'; }
  }

  private applyDocumentLanguage(): void {
    this.document.documentElement.lang = this.locale();
    this.document.documentElement.dir = this.direction();
  }
}
