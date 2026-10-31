import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { CustomerReturnReceiptsApiService } from './customer-return-receipts-api.service';
import { CustomerReturnReceiptsLocalizationService } from './customer-return-receipts-localization.service';
import {
  CreateReturnReceipt,
  ReturnReceiptCreatePage,
  ReturnReceiptDetails,
  ReturnReceiptSummary,
} from './customer-return-receipts.models';

@Component({
  selector: 'app-customer-return-receipts-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './customer-return-receipts-page.html',
  styleUrl: './customer-return-receipts-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerReturnReceiptsPageComponent {
  protected readonly i18n = inject(CustomerReturnReceiptsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(CustomerReturnReceiptsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<ReturnReceiptSummary[]>([]);
  protected readonly item = signal<ReturnReceiptDetails | null>(null);
  protected readonly page = signal<ReturnReceiptCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly search = this.fb.nonNullable.control('');
  protected readonly cancelReason = this.fb.nonNullable.control('', [
    Validators.required,
    Validators.maxLength(500),
  ]);
  protected readonly form = this.fb.group({
    customerReturnRequestId: this.fb.nonNullable.control(0),
    receiptDate: this.fb.nonNullable.control(
      new Date().toISOString().slice(0, 10),
      Validators.required,
    ),
    notes: this.fb.control<string | null>(null, Validators.maxLength(1000)),
    lines: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew = this.route.snapshot.routeConfig?.path === 'sales/return-receipts/new';
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  get lines() {
    return this.form.controls.lines as FormArray;
  }
  protected quantityTotal() {
    return this.lines.controls.reduce(
      (sum, x) => sum + Number(x.get('receivedQuantity')?.value || 0),
      0,
    );
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.problem.set(null);
    if (this.id) {
      this.api.get(this.id).subscribe({
        next: (x) => {
          this.item.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
    } else if (this.isNew) {
      const requestId = Number(this.route.snapshot.queryParamMap.get('customerReturnRequestId'));
      this.api.createForm(requestId).subscribe({
        next: (p) => {
          this.page.set(p);
          this.lines.clear({ emitEvent: false });
          for (const line of p.lines) {
            const old = p.form.lines.find(
              (x) => x.customerReturnRequestLineId === line.requestLineId,
            );
            this.lines.push(
              this.fb.nonNullable.group({
                customerReturnRequestLineId: line.requestLineId,
                receivedQuantity: [
                  old?.receivedQuantity ?? 0,
                  [Validators.min(0), Validators.max(line.remainingQuantity)],
                ],
                disposition: old?.disposition ?? 1,
                inspectionNotes: [old?.inspectionNotes ?? '', [Validators.maxLength(500)]],
                trackingAllocations: this.fb.control<string | null>(
                  old?.trackingAllocations ?? null,
                  Validators.maxLength(4000),
                ),
              }),
              { emitEvent: false },
            );
          }
          this.form.patchValue(
            {
              customerReturnRequestId: p.requestId,
              receiptDate: p.form.receiptDate.slice(0, 10),
              notes: p.form.notes,
            },
            { emitEvent: false },
          );
          this.form.markAsPristine();
          this.done();
          this.dirty();
        },
        error: (e) => this.failLoad(e),
      });
    } else {
      this.api.list(this.search.value.trim() || undefined).subscribe({
        next: (x) => {
          this.rows.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
    }
  }
  protected create() {
    if (this.form.invalid || this.quantityTotal() <= 0 || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue() as CreateReturnReceipt).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/return-receipts', x.id]);
      },
      error: (e) => this.failAction(e),
    });
  }
  protected command(action: 'inspect' | 'post' | 'cancel') {
    const item = this.item();
    if (!item || this.saving() || (action === 'cancel' && this.cancelReason.invalid)) {
      if (action === 'cancel') this.cancelReason.markAsTouched();
      return;
    }
    this.saving.set(true);
    this.api
      .command(
        item.id,
        action,
        item.rowVersion,
        action === 'cancel' ? this.cancelReason.value.trim() : null,
      )
      .subscribe({ next: () => this.load(), error: (e) => this.failAction(e) });
  }
  private done() {
    this.loading.set(false);
    this.saving.set(false);
  }
  private failLoad(e: unknown) {
    this.failure.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private failAction(e: unknown) {
    this.problem.set(this.errors.normalize(e));
    this.saving.set(false);
  }
  private dirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
