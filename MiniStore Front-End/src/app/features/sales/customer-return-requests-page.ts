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
import { CustomerReturnRequestsApiService } from './customer-return-requests-api.service';
import { CustomerReturnRequestsLocalizationService } from './customer-return-requests-localization.service';
import {
  CreateReturnRequest,
  ReturnRequestCreatePage,
  ReturnRequestDetails,
  ReturnRequestSummary,
} from './customer-return-requests.models';

@Component({
  selector: 'app-customer-return-requests-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './customer-return-requests-page.html',
  styleUrl: './customer-return-requests-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerReturnRequestsPageComponent {
  protected readonly i18n = inject(CustomerReturnRequestsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(CustomerReturnRequestsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<ReturnRequestSummary[]>([]);
  protected readonly item = signal<ReturnRequestDetails | null>(null);
  protected readonly page = signal<ReturnRequestCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly search = this.fb.nonNullable.control('');
  protected readonly actionReason = this.fb.nonNullable.control('', [
    Validators.required,
    Validators.maxLength(500),
  ]);
  protected readonly form = this.fb.group({
    salesDeliveryId: this.fb.nonNullable.control(0),
    requestDate: this.fb.nonNullable.control(
      new Date().toISOString().slice(0, 10),
      Validators.required,
    ),
    reason: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(500)]),
    notes: this.fb.control<string | null>(null, Validators.maxLength(1000)),
    lines: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew = this.route.snapshot.routeConfig?.path === 'sales/return-requests/new';

  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }

  get lines() {
    return this.form.controls.lines as FormArray;
  }
  protected quantityTotal() {
    return this.lines.controls.reduce(
      (sum, control) => sum + Number(control.get('stockQuantity')?.value || 0),
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
        next: (value) => {
          this.item.set(value);
          this.done();
        },
        error: (error) => this.failLoad(error),
      });
      return;
    }
    if (this.isNew) {
      const deliveryId = Number(this.route.snapshot.queryParamMap.get('salesDeliveryId'));
      this.api.createForm(deliveryId).subscribe({
        next: (page) => {
          this.page.set(page);
          this.lines.clear({ emitEvent: false });
          for (const line of page.lines) {
            const old = page.form.lines.find(
              (value) => value.salesDeliveryLineId === line.salesDeliveryLineId,
            );
            this.lines.push(
              this.fb.nonNullable.group({
                salesDeliveryLineId: line.salesDeliveryLineId,
                stockQuantity: [
                  old?.stockQuantity ?? 0,
                  [Validators.min(0), Validators.max(line.remainingQuantity)],
                ],
              }),
              { emitEvent: false },
            );
          }
          this.form.patchValue(
            {
              salesDeliveryId: page.salesDeliveryId,
              requestDate: page.form.requestDate.slice(0, 10),
              reason: page.form.reason,
              notes: page.form.notes,
            },
            { emitEvent: false },
          );
          this.form.markAsPristine();
          this.done();
          this.dirty();
        },
        error: (error) => this.failLoad(error),
      });
      return;
    }
    this.api.list(this.search.value.trim() || undefined).subscribe({
      next: (rows) => {
        this.rows.set(rows);
        this.done();
      },
      error: (error) => this.failLoad(error),
    });
  }

  protected create() {
    if (this.form.invalid || this.quantityTotal() <= 0 || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue() as CreateReturnRequest).subscribe({
      next: (value) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/return-requests', value.id]);
      },
      error: (error) => this.failAction(error),
    });
  }

  protected command(action: 'submit' | 'approve' | 'reject' | 'cancel') {
    const item = this.item();
    const needsReason = action === 'reject' || action === 'cancel';
    if (!item || this.saving() || (needsReason && this.actionReason.invalid)) {
      if (needsReason) this.actionReason.markAsTouched();
      return;
    }
    this.saving.set(true);
    this.api
      .command(
        item.id,
        action,
        item.rowVersion,
        needsReason ? this.actionReason.value.trim() : null,
      )
      .subscribe({
        next: () => {
          this.actionReason.reset();
          this.load();
        },
        error: (error) => this.failAction(error),
      });
  }

  private done() {
    this.loading.set(false);
    this.saving.set(false);
  }
  private failLoad(error: unknown) {
    this.failure.set(this.errors.normalize(error));
    this.loading.set(false);
  }
  private failAction(error: unknown) {
    this.problem.set(this.errors.normalize(error));
    this.saving.set(false);
  }
  private dirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
