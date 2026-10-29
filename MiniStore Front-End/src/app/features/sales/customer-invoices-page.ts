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
import { CustomerInvoicesApiService } from './customer-invoices-api.service';
import { CustomerInvoicesLocalizationService } from './customer-invoices-localization.service';
import { InvoiceCreatePage, InvoiceDetails, InvoiceSummary } from './customer-invoices.models';
@Component({
  selector: 'app-customer-invoices-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './customer-invoices-page.html',
  styleUrl: './customer-invoices-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerInvoicesPageComponent {
  protected readonly i18n = inject(CustomerInvoicesLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(CustomerInvoicesApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<InvoiceSummary[]>([]);
  protected readonly item = signal<InvoiceDetails | null>(null);
  protected readonly page = signal<InvoiceCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly filters = this.fb.nonNullable.group({ search: '', status: 0 });
  protected readonly reason = this.fb.nonNullable.control('', [
    Validators.required,
    Validators.maxLength(500),
  ]);
  protected readonly reverseForm = this.fb.nonNullable.group({
    reversalDate: new Date().toISOString().slice(0, 10),
    reason: ['', [Validators.required, Validators.maxLength(500)]],
  });
  protected readonly form = this.fb.group({
    salesOrderId: 0,
    requestKey: '',
    invoiceDate: new Date().toISOString().slice(0, 10),
    notes: this.fb.control<string | null>(null, [Validators.maxLength(1000)]),
    lines: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew = this.route.snapshot.routeConfig?.path === 'sales/invoices/new';
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  get lines() {
    return this.form.controls.lines as FormArray;
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
      const orderId = Number(this.route.snapshot.queryParamMap.get('salesOrderId'));
      this.api.createForm(orderId).subscribe({
        next: (p) => {
          this.page.set(p);
          this.lines.clear({ emitEvent: false });
          for (const x of p.form.lines)
            this.lines.push(
              this.fb.group({
                salesOrderLineId: x.salesOrderLineId,
                quantity: [x.quantity, [Validators.min(0)]],
              }),
              { emitEvent: false },
            );
          this.form.patchValue(
            { ...p.form, invoiceDate: p.form.invoiceDate.slice(0, 10) },
            { emitEvent: false },
          );
          this.form.markAsPristine();
          this.done();
          this.dirty();
        },
        error: (e) => this.failLoad(e),
      });
    } else {
      const f = this.filters.getRawValue();
      this.api.list(f.status || undefined, f.search.trim() || undefined).subscribe({
        next: (x) => {
          this.rows.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
    }
  }
  protected create() {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/invoices', x.id]);
      },
      error: (e) => this.failAction(e),
    });
  }
  protected command(action: 'approve' | 'cancel' | 'post') {
    const x = this.item();
    if (!x || this.saving() || (action === 'cancel' && this.reason.invalid)) {
      this.reason.markAsTouched();
      return;
    }
    this.saving.set(true);
    this.api
      .command(x.id, action, x.rowVersion, action === 'cancel' ? this.reason.value.trim() : null)
      .subscribe({ next: () => this.load(), error: (e) => this.failAction(e) });
  }
  protected reverse() {
    const x = this.item();
    if (!x || this.reverseForm.invalid || this.saving()) {
      this.reverseForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const v = this.reverseForm.getRawValue();
    this.api
      .reverse(x.id, x.rowVersion, v.reversalDate, v.reason.trim())
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
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.form.dirty);
  }
}
