import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { CustomerCreditNotesApiService } from './customer-credit-notes-api.service';
import { CustomerCreditNotesLocalizationService } from './customer-credit-notes-localization.service';
import {
  CreateCreditNote,
  CreditNoteCreatePage,
  CreditNoteDetails,
  CreditNoteSummary,
} from './customer-credit-notes.models';

@Component({
  selector: 'app-customer-credit-notes-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './customer-credit-notes-page.html',
  styleUrl: './customer-credit-notes-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerCreditNotesPageComponent {
  protected readonly i18n = inject(CustomerCreditNotesLocalizationService);
  private readonly api = inject(CustomerCreditNotesApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<CreditNoteSummary[]>([]);
  protected readonly item = signal<CreditNoteDetails | null>(null);
  protected readonly page = signal<CreditNoteCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly form = this.fb.group({
    customerInvoiceId: this.fb.nonNullable.control(0),
    requestKey: this.fb.nonNullable.control(''),
    creditDate: this.fb.nonNullable.control(
      new Date().toISOString().slice(0, 10),
      Validators.required,
    ),
    reason: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(1000)]),
    lines: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew = this.route.snapshot.routeConfig?.path === 'sales/credit-notes/new';
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  get lines() {
    return this.form.controls.lines as FormArray;
  }
  protected quantityTotal() {
    return this.lines.controls.reduce((sum, x) => sum + Number(x.get('quantity')?.value || 0), 0);
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
          for (const line of p.lines) {
            const old = p.form.lines.find(
              (x) =>
                x.customerInvoiceLineId === line.customerInvoiceLineId &&
                x.customerReturnReceiptLineId === line.customerReturnReceiptLineId,
            );
            this.lines.push(
              this.fb.nonNullable.group({
                customerInvoiceLineId: line.customerInvoiceLineId,
                customerReturnReceiptLineId: line.customerReturnReceiptLineId,
                quantity: [
                  old?.quantity ?? 0,
                  [Validators.min(0), Validators.max(line.remainingQuantity)],
                ],
              }),
              { emitEvent: false },
            );
          }
          this.form.patchValue(
            {
              customerInvoiceId: p.invoiceId,
              requestKey: p.form.requestKey,
              creditDate: p.form.creditDate.slice(0, 10),
              reason: p.form.reason,
            },
            { emitEvent: false },
          );
          this.form.markAsPristine();
          this.done();
          this.dirty();
        },
        error: (e) => this.failLoad(e),
      });
    } else
      this.api.list().subscribe({
        next: (x) => {
          this.rows.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
  }
  protected create() {
    if (this.form.invalid || this.quantityTotal() <= 0 || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue() as CreateCreditNote).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/credit-notes', x.id]);
      },
      error: (e) => this.failAction(e),
    });
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
