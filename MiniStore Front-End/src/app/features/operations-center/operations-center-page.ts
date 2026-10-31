import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { OperationsCenterApiService } from './data/operations-center-api.service';
import { OperationsCenter } from './data/operations-center.models';

@Component({
  selector: 'app-operations-center-page',
  imports: [DatePipe, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent],
  templateUrl: './operations-center-page.html',
  styleUrl: './operations-center-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OperationsCenterPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(OperationsCenterApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly page = signal<OperationsCenter | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);

  constructor() { this.load(); }

  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.get().subscribe({
      next: page => { this.page.set(page); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected text(key: string): string { return this.i18n.text(key as MessageKey); }
  protected localizedName(name: string, nameArabic?: string): string {
    return this.i18n.locale() === 'ar-JO' && nameArabic ? nameArabic : name;
  }
}
