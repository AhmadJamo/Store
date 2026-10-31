import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SupplierCommunicationsApiService } from './supplier-communications-api.service';
import {
  CommunicationKey,
  SupplierCommunicationsLocalizationService,
} from './supplier-communications-localization.service';

@Component({
  selector: 'app-supplier-communications-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './supplier-communications-page.html',
  styleUrl: './supplier-communications-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierCommunicationsPageComponent {
  protected readonly i18n = inject(SupplierCommunicationsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SupplierCommunicationsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  protected readonly communication = signal<any>(null);
  protected readonly history = signal<any[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<any>(null);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id') ?? 0);
  protected readonly documentType = Number(
    this.route.snapshot.queryParamMap.get('documentType') ?? 0,
  );
  protected readonly documentId = Number(this.route.snapshot.queryParamMap.get('documentId') ?? 0);
  protected readonly supplierId = Number(this.route.snapshot.queryParamMap.get('supplierId') ?? 0);
  protected readonly supplierName = this.route.snapshot.queryParamMap.get('supplierName') ?? '';
  protected readonly documentNumber = this.route.snapshot.queryParamMap.get('documentNumber') ?? '';
  protected readonly activity = this.fb.nonNullable.group({
    status: [2, Validators.min(2)],
    occurredAtUtc: [this.localNow(), Validators.required],
    evidenceReference: ['', [Validators.maxLength(500)]],
    note: ['', [Validators.maxLength(2000)]],
  });
  constructor() {
    this.load();
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    const request = this.id
      ? this.api.get(this.id)
      : this.api.byDocument(this.documentType, this.documentId);
    request.subscribe({
      next: (value) => {
        if (this.id) this.communication.set(value);
        else this.history.set(value as any[]);
        this.loading.set(false);
      },
      error: (error) => {
        this.failure.set(this.errors.normalize(error));
        this.loading.set(false);
      },
    });
  }
  protected prepare(language: string) {
    if (this.saving()) return;
    this.saving.set(true);
    this.api
      .prepare({
        documentType: this.documentType,
        documentId: this.documentId,
        supplierId: this.supplierId,
        preferredLanguage: language,
      })
      .subscribe({
        next: (value) => this.router.navigate(['/purchases/communications', value.id]),
        error: (error) => {
          this.failure.set(this.errors.normalize(error));
          this.saving.set(false);
        },
      });
  }
  protected record() {
    if (this.activity.invalid || this.saving()) return;
    this.saving.set(true);
    this.api
      .record(this.id, { communicationId: this.id, ...this.activity.getRawValue() })
      .subscribe({
        next: (value) => {
          this.communication.set(value);
          this.saving.set(false);
          this.activity.patchValue({
            occurredAtUtc: this.localNow(),
            evidenceReference: '',
            note: '',
          });
        },
        error: (error) => {
          this.failure.set(this.errors.normalize(error));
          this.saving.set(false);
        },
      });
  }
  protected status(value: number): CommunicationKey {
    return (
      (
        {
          1: 'prepared',
          2: 'sent',
          3: 'delivered',
          4: 'replied',
          5: 'failed',
          6: 'followUp',
        } as const
      )[value as 1] ?? 'prepared'
    );
  }
  protected sourceRoute(x: any = this.communication()) {
    return x?.documentType === 1
      ? ['/purchases/sourcing', x.documentId]
      : ['/purchases/orders', x?.documentId];
  }
  protected attachmentUrl(attachmentId: number) {
    return this.api.attachmentUrl(this.id, attachmentId);
  }
  private localNow() {
    const date = new Date();
    date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
    return date.toISOString().slice(0, 16);
  }
}
