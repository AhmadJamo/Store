import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PurchaseSourcingApiService } from './purchase-sourcing-api.service';
import { PurchaseOrderApiService } from './purchase-order-api.service';
import {
  PurchaseSourcing,
  QuotationComparison,
  QuotationCreatePage,
  SourcingCreatePage,
} from './purchase-sourcing.models';
import {
  PurchaseSourcingLocalizationService,
  SourcingKey,
} from './purchase-sourcing-localization.service';

@Component({
  selector: 'app-purchase-sourcing-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    DataTableComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './purchase-sourcing-page.html',
  styleUrl: './purchase-sourcing-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseSourcingPageComponent {
  private readonly orders = inject(PurchaseOrderApiService);
  protected readonly i18n = inject(PurchaseSourcingLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseSourcingApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly mode = signal<'list' | 'create' | 'details' | 'quote' | 'compare'>('list');
  protected readonly rows = signal<PurchaseSourcing[]>([]);
  protected readonly detail = signal<PurchaseSourcing | null>(null);
  protected readonly createPage = signal<SourcingCreatePage | null>(null);
  protected readonly quotePage = signal<QuotationCreatePage | null>(null);
  protected readonly comparison = signal<QuotationComparison | null>(null);
  protected readonly selectedSuppliers = signal<number[]>([]);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly filter = this.fb.nonNullable.group({ status: [0], search: [''] });
  protected readonly sourcingForm = this.fb.nonNullable.group({
    supplierResponseDeadline: ['', Validators.required],
    buyerReference: ['', Validators.maxLength(100)],
    estimatedAmountCompanyCurrency: [0, Validators.min(0)],
    notes: ['', Validators.maxLength(1000)],
    invitationMessage: ['', Validators.maxLength(1000)],
  });
  protected readonly quoteForm = this.fb.nonNullable.group({
    supplierId: [0, Validators.min(1)],
    currencyCode: ['', Validators.pattern(/^[A-Za-z]{3}$/)],
    supplierReference: ['', Validators.maxLength(100)],
    quotationDate: ['', Validators.required],
    validUntilDate: ['', Validators.required],
    leadTimeDays: [0, [Validators.min(0), Validators.max(3650)]],
    paymentTerms: ['', Validators.maxLength(500)],
    notes: ['', Validators.maxLength(1000)],
    lines: this.fb.array([]),
  });
  protected readonly awardForm = this.fb.nonNullable.group({ lines: this.fb.array([]) });
  constructor() {
    this.sourcingForm.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.quoteForm.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.awardLines.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.resolve());
  }
  canDeactivate() {
    return (
      !(this.sourcingForm.dirty || this.quoteForm.dirty || this.awardLines.dirty) ||
      globalThis.confirm(this.i18n.text('unsaved'))
    );
  }
  protected get quotationLines() {
    return this.quoteForm.controls.lines as FormArray;
  }
  protected get awardLines() {
    return this.awardForm.controls.lines as FormArray;
  }
  protected loadList() {
    const x = this.filter.getRawValue();
    this.start();
    this.api.getAll(x.status || undefined, x.search).subscribe({
      next: (r) => {
        this.rows.set(r);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  protected toggleSupplier(id: number, checked: boolean) {
    this.selectedSuppliers.update((ids) => (checked ? [...ids, id] : ids.filter((x) => x !== id)));
    this.sourcingForm.markAsDirty();
  }
  protected create() {
    const page = this.createPage();
    if (!page || this.sourcingForm.invalid || !this.selectedSuppliers().length) {
      this.problem.set(this.i18n.text('fix'));
      this.sourcingForm.markAllAsTouched();
      return;
    }
    const v = this.sourcingForm.getRawValue();
    this.save(
      this.api.create({
        purchaseRequestId: page.request.id,
        ...v,
        buyerReference: v.buyerReference.trim() || undefined,
        notes: v.notes.trim() || undefined,
        invitationMessage: v.invitationMessage.trim() || undefined,
        supplierIds: this.selectedSuppliers(),
      }),
      (r) => {
        this.sourcingForm.markAsPristine();
        void this.router.navigate(['/purchases/sourcing', r.id]);
      },
      'created',
    );
  }
  protected submitQuotation() {
    const page = this.quotePage();
    if (!page || this.quoteForm.invalid) {
      this.problem.set(this.i18n.text('fix'));
      this.quoteForm.markAllAsTouched();
      return;
    }
    const v = this.quoteForm.getRawValue();
    this.save(
      this.api.createQuotation({
        ...v,
        purchaseSourcingEventId: page.sourcingEvent.id,
        currencyCode: v.currencyCode.toUpperCase(),
        supplierReference: v.supplierReference.trim() || undefined,
        paymentTerms: v.paymentTerms.trim() || undefined,
        notes: v.notes.trim() || undefined,
      }),
      () => {
        this.quoteForm.markAsPristine();
        void this.router.navigate(['/purchases/sourcing', page.sourcingEvent.id, 'comparison']);
      },
      'quoteCreated',
    );
  }
  protected award() {
    const page = this.comparison();
    if (!page) return;
    const allocations = (
      this.awardLines.getRawValue() as {
        supplierQuotationLineId: number;
        awardedQuantity: number;
        reason: string;
      }[]
    )
      .filter((x) => x.awardedQuantity > 0)
      .map((x) => ({ ...x, reason: x.reason.trim() }));
    if (!allocations.length || allocations.some((x) => !x.reason)) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    this.save(
      this.api.award({ sourcingEventId: page.sourcingEventId, allocations }),
      () => {
        this.awardLines.markAsPristine();
        this.loadComparison(page.sourcingEventId);
      },
      'awardDone',
    );
  }
  protected createOrders() {
    const page = this.comparison();
    if (!page) return;
    this.save(
      this.orders.createFromAward(page.sourcingEventId),
      (r) =>
        void this.router.navigate(
          r.ids.length === 1 ? ['/purchases/orders', r.ids[0]] : ['/purchases/orders'],
        ),
      'ordersCreated',
    );
  }
  protected canCreateAgreement(page: QuotationComparison) {
    return new Set(page.awardAllocations.map((x) => x.supplierQuotationId)).size === 1;
  }
  protected statusName(v: number) {
    return this.i18n.text(
      ({ 1: 'draft', 2: 'sent', 3: 'closed', 4: 'cancelled' } as Record<number, SourcingKey>)[v] ??
        'draft',
    );
  }
  protected retry() {
    this.resolve();
  }
  private resolve() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    const path = this.route.snapshot.routeConfig?.path ?? '';
    if (path.endsWith('new')) {
      this.mode.set('create');
      this.loadCreate(Number(this.route.snapshot.queryParamMap.get('purchaseRequestId')));
      return;
    }
    if (path.endsWith('quotation')) {
      this.mode.set('quote');
      this.loadQuote(id);
      return;
    }
    if (path.endsWith('comparison')) {
      this.mode.set('compare');
      this.loadComparison(id);
      return;
    }
    if (id) {
      this.mode.set('details');
      this.loadDetail(id);
      return;
    }
    this.mode.set('list');
    this.loadList();
  }
  private loadCreate(requestId: number) {
    this.start();
    this.api.getCreateForm(requestId).subscribe({
      next: (p) => {
        this.createPage.set(p);
        this.sourcingForm.patchValue(p.form);
        this.selectedSuppliers.set(p.form.supplierIds ?? []);
        this.sourcingForm.markAsPristine();
        this.markDirty();
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private loadDetail(id: number) {
    this.start();
    this.api.get(id).subscribe({
      next: (x) => {
        this.detail.set(x);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private loadQuote(id: number) {
    this.start();
    this.api.getQuotationForm(id).subscribe({
      next: (p) => {
        this.quotePage.set(p);
        this.quoteForm.patchValue(p.form);
        this.quotationLines.clear();
        for (const x of p.form.lines)
          this.quotationLines.push(
            this.fb.nonNullable.group({
              purchaseSourcingLineId: [x.purchaseSourcingLineId],
              quotedQuantity: [x.quotedQuantity, Validators.min(0.000001)],
              unitPrice: [x.unitPrice, Validators.min(0)],
              discountPercent: [x.discountPercent, [Validators.min(0), Validators.max(100)]],
              taxPercent: [x.taxPercent, [Validators.min(0), Validators.max(100)]],
            }),
          );
        this.quoteForm.markAsPristine();
        this.markDirty();
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private loadComparison(id: number) {
    this.start();
    this.api.comparison(id).subscribe({
      next: (p) => {
        this.comparison.set(p);
        this.awardLines.clear();
        for (const x of p.form.allocations)
          this.awardLines.push(
            this.fb.nonNullable.group({
              supplierQuotationLineId: [x.supplierQuotationLineId],
              awardedQuantity: [x.awardedQuantity ?? 0, Validators.min(0)],
              reason: [x.reason ?? '', Validators.maxLength(1000)],
            }),
          );
        this.awardLines.markAsPristine();
        this.markDirty();
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private save<T>(request: Observable<T>, next: (x: T) => void, message: SourcingKey) {
    if (this.saving()) return;
    this.saving.set(true);
    this.problem.set('');
    request.subscribe({
      next: (x: T) => {
        this.saving.set(false);
        this.success.set(this.i18n.text(message));
        next(x);
      },
      error: (e: unknown) => {
        this.saving.set(false);
        const p = this.errors.normalize(e);
        this.problem.set(p.detail ?? p.title ?? this.i18n.text('error'));
      },
    });
  }
  private start() {
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.problem.set('');
  }
  private fail(e: unknown) {
    this.loadingProblem.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private markDirty() {
    const tab = this.workspace.activeTab();
    if (tab)
      this.workspace.markDirty(
        tab.id,
        this.sourcingForm.dirty || this.quoteForm.dirty || this.awardLines.dirty,
      );
  }
}
