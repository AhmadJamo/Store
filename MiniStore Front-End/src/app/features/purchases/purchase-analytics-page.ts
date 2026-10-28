import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PurchaseAnalyticsApiService } from './purchase-analytics-api.service';
import { PurchaseAnalyticsLocalizationService } from './purchase-analytics-localization.service';
@Component({
  selector: 'app-purchase-analytics-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './purchase-analytics-page.html',
  styleUrl: './purchase-analytics-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseAnalyticsPageComponent {
  protected readonly i18n = inject(PurchaseAnalyticsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseAnalyticsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<any>(null);
  protected readonly loading = signal(true);
  protected readonly failure = signal<any>(null);
  protected readonly form = this.fb.nonNullable.group({
    From: [this.daysAgo(89)],
    To: [this.daysAgo(0)],
    SupplierId: [''],
    BuyerUserId: [''],
    OrderStatus: [''],
    GroupBy: [1],
  });
  constructor() {
    this.load();
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.api.get(this.form.getRawValue()).subscribe({
      next: (x) => {
        this.page.set(x);
        this.form.patchValue(x.query, { emitEvent: false });
        this.loading.set(false);
      },
      error: (e) => {
        this.failure.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  protected route(x: any) {
    const map: Record<string, string> = {
      PurchaseRequests: '/purchases/requests',
      PurchaseSourcing: '/purchases/sourcing',
      PurchaseOrders: '/purchases/orders',
      GoodsReceipts: '/purchases/receipts',
      VendorBills: '/purchases/bills',
      SupplierPayments: '/purchases/payments',
    };
    return map[x.controller] ?? '/purchases';
  }
  private daysAgo(n: number) {
    const d = new Date();
    d.setDate(d.getDate() - n);
    return d.toISOString().slice(0, 10);
  }
}
