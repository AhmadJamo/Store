import { DecimalPipe } from '@angular/common';
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
import { CustomerPaymentsApiService } from './customer-payments-api.service';
import { CustomerPaymentsLocalizationService } from './customer-payments-localization.service';
import {
  CreateCustomerPayment,
  CustomerPayment,
  CustomerPaymentCreatePage,
} from './customer-payments.models';

@Component({
  selector: 'app-customer-payments-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './customer-payments-page.html',
  styleUrl: './customer-payments-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerPaymentsPageComponent {
  protected readonly i18n = inject(CustomerPaymentsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(CustomerPaymentsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<CustomerPayment[]>([]);
  protected readonly item = signal<CustomerPayment | null>(null);
  protected readonly page = signal<CustomerPaymentCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly search = this.fb.nonNullable.control('');
  protected readonly form = this.fb.group({
    sourceCustomerInvoiceId: this.fb.nonNullable.control(0),
    paymentMethodId: this.fb.nonNullable.control(0, [Validators.required, Validators.min(1)]),
    paymentDate: this.fb.nonNullable.control(
      new Date().toISOString().slice(0, 10),
      Validators.required,
    ),
    externalReference: this.fb.control<string | null>(null, Validators.maxLength(100)),
    notes: this.fb.control<string | null>(null, Validators.maxLength(1000)),
    lines: this.fb.array([]),
  });
  protected readonly reverseForm = this.fb.nonNullable.group({
    reversalDate: [new Date().toISOString().slice(0, 10), Validators.required],
    reason: ['', [Validators.required, Validators.maxLength(500)]],
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew = this.route.snapshot.routeConfig?.path === 'sales/payments/new';
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  get lines() {
    return this.form.controls.lines as FormArray;
  }
  protected allocationTotal() {
    return this.lines.controls.reduce((sum, x) => sum + Number(x.get('amount')?.value || 0), 0);
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.problem.set(null);
    if (this.id)
      this.api.get(this.id).subscribe({
        next: (x) => {
          this.item.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
    else if (this.isNew) {
      const invoiceId = Number(this.route.snapshot.queryParamMap.get('customerInvoiceId'));
      this.api.createForm(invoiceId).subscribe({
        next: (p) => {
          this.page.set(p);
          this.lines.clear({ emitEvent: false });
          for (const x of p.form.lines)
            this.lines.push(
              this.fb.nonNullable.group({
                customerInvoiceId: x.customerInvoiceId,
                amount: [x.amount, [Validators.min(0)]],
              }),
              { emitEvent: false },
            );
          this.form.patchValue(
            { ...p.form, paymentDate: p.form.paymentDate.slice(0, 10) },
            { emitEvent: false },
          );
          this.form.markAsPristine();
          this.done();
          this.dirty();
        },
        error: (e) => this.failLoad(e),
      });
    } else
      this.api.list(this.search.value.trim() || undefined).subscribe({
        next: (x) => {
          this.rows.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
  }
  protected create() {
    if (this.form.invalid || this.allocationTotal() <= 0 || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue() as CreateCustomerPayment).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/payments', x.id]);
      },
      error: (e) => this.failAction(e),
    });
  }
  protected reverse() {
    const x = this.item();
    if (!x || this.reverseForm.invalid || this.saving()) {
      this.reverseForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.reverseForm.getRawValue();
    this.api.reverse(x.id, x.rowVersion, value.reversalDate, value.reason.trim()).subscribe({
      next: () => this.load(),
      error: (e) => this.failAction(e),
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
