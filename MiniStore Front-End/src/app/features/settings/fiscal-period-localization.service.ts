import { inject, Injectable } from '@angular/core';
import { UiCultureStateService } from '../../core/localization/ui-culture-state.service';

const copy = {
  en: {
    description: 'Close periods through controlled steps after readiness review, with a reason and audit evidence for every transition.',
    reviewReadiness: 'Review close readiness',
    actionReason: 'Action reason (required)',
    softClose: 'Soft close',
    close: 'Final close',
    reopen: 'Reopen one step',
  },
  ar: {
    description: 'إقفال الفترات بخطوات مضبوطة بعد مراجعة الجاهزية، مع سبب وسجل تدقيق لكل انتقال.',
    reviewReadiness: 'مراجعة جاهزية الإقفال',
    actionReason: 'سبب الإجراء (إلزامي)',
    softClose: 'إقفال جزئي',
    close: 'إقفال نهائي',
    reopen: 'إعادة فتح خطوة واحدة',
  },
} as const;

export type FiscalPeriodCopyKey = keyof typeof copy.en;

@Injectable({ providedIn: 'root' })
export class FiscalPeriodLocalizationService {
  private readonly culture = inject(UiCultureStateService);
  text(key: FiscalPeriodCopyKey): string { return copy[this.culture.locale() === 'ar-JO' ? 'ar' : 'en'][key]; }
}
