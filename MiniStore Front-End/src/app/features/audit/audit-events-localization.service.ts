import { inject, Injectable } from '@angular/core';
import { LocalizationService } from '../../core/localization/localization.service';

const messages = {
  en: {
    title: 'Audit events', description: 'Search tenant audit evidence and export an accountable CSV for an approved purpose.',
    filters: 'Filters', from: 'From', to: 'To', category: 'Category', outcome: 'Outcome', eventType: 'Event type', actor: 'Actor', resourceType: 'Resource type', resourceId: 'Resource ID', correlation: 'Correlation ID', all: 'All', search: 'Search', reset: 'Reset', results: 'Events', occurred: 'Occurred', event: 'Event', resource: 'Resource', details: 'Evidence', empty: 'No matching audit events.', previous: 'Previous', next: 'Next', page: 'Page', of: 'of', pageSize: 'Page size', export: 'Export CSV', purpose: 'Approved purpose', internalReview: 'Internal review', securityInvestigation: 'Security investigation', complianceReview: 'Compliance review', sensitive: 'Sensitive redacted evidence is visible for your permissions.', reasonCode: 'Reason code', path: 'Request path', ip: 'Masked IP', userAgent: 'User-agent hash', changes: 'Redacted changes', metadata: 'Redacted metadata', loading: 'Loading audit events', exporting: 'Preparing export',
  },
  ar: {
    title: 'سجل التدقيق', description: 'ابحث في أدلة تدقيق الشركة وصدّر ملف CSV موثقًا لغرض معتمد.',
    filters: 'عوامل التصفية', from: 'من', to: 'إلى', category: 'الفئة', outcome: 'النتيجة', eventType: 'نوع الحدث', actor: 'المنفذ', resourceType: 'نوع المورد', resourceId: 'معرّف المورد', correlation: 'معرّف التتبع', all: 'الكل', search: 'بحث', reset: 'إعادة ضبط', results: 'الأحداث', occurred: 'وقت الحدوث', event: 'الحدث', resource: 'المورد', details: 'الأدلة', empty: 'لا توجد أحداث تدقيق مطابقة.', previous: 'السابق', next: 'التالي', page: 'صفحة', of: 'من', pageSize: 'حجم الصفحة', export: 'تصدير CSV', purpose: 'الغرض المعتمد', internalReview: 'مراجعة داخلية', securityInvestigation: 'تحقيق أمني', complianceReview: 'مراجعة امتثال', sensitive: 'الأدلة المنقحة الحساسة ظاهرة وفق صلاحياتك.', reasonCode: 'رمز السبب', path: 'مسار الطلب', ip: 'عنوان IP المنقح', userAgent: 'بصمة المتصفح', changes: 'التغييرات المنقحة', metadata: 'البيانات الوصفية المنقحة', loading: 'جارٍ تحميل سجل التدقيق', exporting: 'جارٍ تجهيز التصدير',
  },
} as const;

export type AuditMessageKey = keyof typeof messages.en;

@Injectable({ providedIn: 'root' })
export class AuditEventsLocalizationService {
  readonly locale = inject(LocalizationService);
  text(key: AuditMessageKey): string { return messages[this.locale.locale() === 'ar-JO' ? 'ar' : 'en'][key]; }
  category(value: number): string { const values = this.locale.locale() === 'ar-JO' ? ['', 'الأمن', 'الإدارة', 'الإعدادات', 'العمليات', 'التكامل', 'الوصول للبيانات'] : ['', 'Security', 'Administration', 'Configuration', 'Operations', 'Integration', 'Data access']; return values[value] ?? String(value); }
  outcome(value: number): string { const values = this.locale.locale() === 'ar-JO' ? ['', 'ناجح', 'مرفوض', 'فاشل', 'غير معروف'] : ['', 'Succeeded', 'Denied', 'Failed', 'Unknown']; return values[value] ?? String(value); }
  severity(value: number): string { const values = this.locale.locale() === 'ar-JO' ? ['', 'معلومة', 'تحذير', 'حرج'] : ['', 'Information', 'Warning', 'Critical']; return values[value] ?? String(value); }
  channel(value: number): string { const values = this.locale.locale() === 'ar-JO' ? ['', 'الويب', 'عامل خلفي', 'واجهة API', 'البوابة', 'الاستيراد', 'المنصة'] : ['', 'Web', 'Worker', 'API', 'Portal', 'Import', 'Platform']; return values[value] ?? String(value); }
}
