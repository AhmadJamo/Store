import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PurchaseAccountingReconciliationApiService } from './purchase-accounting-reconciliation-api.service';
import { PurchaseAccountingReconciliationLocalizationService } from './purchase-accounting-reconciliation-localization.service';

@Component({
  selector: 'app-purchase-accounting-reconciliation-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './purchase-accounting-reconciliation-page.html',
  styleUrl: './purchase-accounting-reconciliation-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseAccountingReconciliationPageComponent {
  protected readonly Math = Math;
  protected readonly i18n = inject(PurchaseAccountingReconciliationLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseAccountingReconciliationApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<any>(null);
  protected readonly loading = signal(true);
  protected readonly failure = signal<any>(null);
  protected readonly today = new Date().toISOString().slice(0, 10);
  protected readonly form = this.fb.nonNullable.group({
    asOfDate: [this.today],
    exceptionsOnly: [true],
  });

  constructor() {
    this.load();
  }

  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    const value = this.form.getRawValue();
    this.api.get(value.asOfDate, value.exceptionsOnly).subscribe({
      next: (page) => {
        this.page.set(page);
        this.form.patchValue(
          { asOfDate: page.asOfDate, exceptionsOnly: page.exceptionsOnly },
          { emitEvent: false },
        );
        this.loading.set(false);
      },
      error: (error) => {
        this.failure.set(this.errors.normalize(error));
        this.loading.set(false);
      },
    });
  }

  protected settlementRoute(document: any) {
    return document.documentType === 'VendorBill'
      ? ['/purchases/bills', document.documentId]
      : ['/purchases/receipt-returns', document.documentId];
  }
}
