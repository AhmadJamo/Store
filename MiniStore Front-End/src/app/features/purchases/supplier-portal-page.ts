import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import {
  SupplierPortalApiService,
  SupplierPortalPage,
  SupplierPortalQuotationCommand,
  SupplierPortalResult,
} from './supplier-portal-api.service';
import { SupplierPortalLocalizationService } from './supplier-portal-localization.service';
@Component({
  selector: 'app-supplier-portal-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './supplier-portal-page.html',
  styleUrl: './supplier-portal-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierPortalPageComponent {
  protected readonly i18n = inject(SupplierPortalLocalizationService);
  private readonly api = inject(SupplierPortalApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<SupplierPortalPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly token = this.route.snapshot.paramMap.get('token') ?? '';
  protected readonly quote = this.fb.nonNullable.group({
    currencyCode: ['', Validators.required],
    supplierReference: [''],
    quotationDate: [this.today()],
    validUntilDate: [this.days(14)],
    leadTimeDays: [0],
    paymentTerms: [''],
    notes: [''],
    lines: this.fb.array<any>([]),
  });
  protected readonly confirmation = this.fb.nonNullable.group({
    confirmedExpectedDate: ['', Validators.required],
    confirmationReference: ['', Validators.required],
    note: [''],
  });
  protected readonly notice = this.fb.nonNullable.group({
    deliveryDate: [this.today(), Validators.required],
    reference: ['', Validators.required],
    carrier: [''],
    trackingNumber: [''],
    note: [''],
  });
  constructor() {
    this.load();
  }
  get quoteLines() {
    return this.quote.controls.lines as FormArray;
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.api.get(this.token).subscribe({
      next: (x) => {
        this.page.set(x);
        this.quote.patchValue({ currencyCode: x.currencyCode });
        this.confirmation.patchValue({ confirmedExpectedDate: x.expectedDate ?? '' });
        this.quoteLines.clear();
        for (const line of x.lines)
          this.quoteLines.push(
            this.fb.nonNullable.group({
              documentLineId: [line.id],
              quantity: [line.quantity, Validators.min(0.000001)],
              unitPrice: [0, Validators.min(0)],
              discountPercent: [0],
              taxPercent: [0],
            }),
          );
        this.loading.set(false);
      },
      error: (e) => {
        this.failure.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  protected submitQuote() {
    if (this.quote.invalid) return;
    this.send(
      this.api.quotation({
        token: this.token,
        idempotencyKey: crypto.randomUUID().replaceAll('-', ''),
        ...this.quote.getRawValue(),
        lines: this.quoteLines.getRawValue() as SupplierPortalQuotationCommand['lines'],
      }),
    );
  }
  protected confirm() {
    if (this.confirmation.invalid) return;
    this.send(
      this.api.confirm({
        token: this.token,
        idempotencyKey: crypto.randomUUID().replaceAll('-', ''),
        ...this.confirmation.getRawValue(),
      }),
    );
  }
  protected delivery() {
    if (this.notice.invalid) return;
    this.send(
      this.api.delivery({
        token: this.token,
        idempotencyKey: crypto.randomUUID().replaceAll('-', ''),
        ...this.notice.getRawValue(),
      }),
    );
  }
  private send(request: Observable<SupplierPortalResult>) {
    if (this.saving()) return;
    this.saving.set(true);
    request.subscribe({
      next: () => {
        this.success.set(this.i18n.text('success'));
        this.saving.set(false);
        this.load();
      },
      error: (e: unknown) => {
        this.failure.set(this.errors.normalize(e));
        this.saving.set(false);
      },
    });
  }
  private today() {
    return new Date().toISOString().slice(0, 10);
  }
  private days(n: number) {
    const d = new Date();
    d.setDate(d.getDate() + n);
    return d.toISOString().slice(0, 10);
  }
}
