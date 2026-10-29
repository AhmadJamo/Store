import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { JournalEntryApiService } from './journal-entry-api.service';
import { JournalEntryLocalizationService } from './journal-entry-localization.service';
import { JournalEntry } from './journal-entry.models';
@Component({
  selector: 'app-journal-entry-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './journal-entry-page.html',
  styleUrl: './journal-entry-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class JournalEntryPageComponent {
  protected readonly i18n = inject(JournalEntryLocalizationService);
  private readonly api = inject(JournalEntryApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  protected readonly entry = signal<JournalEntry | null>(null);
  protected readonly loading = signal(true);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  constructor() {
    this.load();
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.api.get(Number(this.route.snapshot.paramMap.get('id'))).subscribe({
      next: (x) => {
        this.entry.set(x);
        this.loading.set(false);
      },
      error: (e) => {
        this.failure.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
}
