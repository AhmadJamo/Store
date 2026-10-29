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
import { LegacySalesApiService } from './legacy-sales-api.service';
import { LegacySalesLocalizationService } from './legacy-sales-localization.service';
import {
  LegacySaleCreateInput,
  LegacySaleCreatePage,
  LegacySaleDetailsPage,
  LegacySaleProductOption,
  LegacySaleSummary,
} from './legacy-sales.models';

@Component({
  selector: 'app-legacy-sales-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './legacy-sales-page.html',
  styleUrl: './legacy-sales-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LegacySalesPageComponent {
  protected readonly i18n = inject(LegacySalesLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(LegacySalesApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<LegacySaleSummary[]>([]);
  protected readonly detail = signal<LegacySaleDetailsPage | null>(null);
  protected readonly page = signal<LegacySaleCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly productSearch = this.fb.nonNullable.control('');
  protected readonly form = this.fb.group({
    invoiceNumber: this.fb.nonNullable.control(''),
    warehouseId: this.fb.nonNullable.control(0, Validators.min(1)),
    posTerminalId: this.fb.control<number | null>(null),
    customerId: this.fb.control<number | null>(null),
    paymentMethodId: this.fb.nonNullable.control(0, Validators.min(1)),
    taxRateId: this.fb.control<number | null>(null),
    posOrderType: this.fb.control<number | null>(null),
    serviceReference: this.fb.control<string | null>(null),
    guestCount: this.fb.control<number | null>(null),
    date: this.fb.nonNullable.control(new Date().toISOString().slice(0, 10), Validators.required),
    notes: this.fb.control<string | null>(null, Validators.maxLength(500)),
    invoiceDiscountType: this.fb.nonNullable.control(1),
    invoiceDiscountValue: this.fb.nonNullable.control(0, Validators.min(0)),
    items: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew = this.route.snapshot.routeConfig?.path === 'sales/legacy/new';
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.syncDirty());
    this.load();
  }
  get items() {
    return this.form.controls.items as FormArray;
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.problem.set(null);
    this.success.set('');
    if (this.id)
      this.api.get(this.id).subscribe({
        next: (x) => {
          this.detail.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
    else if (this.isNew)
      this.api
        .createForm()
        .subscribe({ next: (p) => this.loadForm(p), error: (e) => this.failLoad(e) });
    else
      this.api.list().subscribe({
        next: (x) => {
          this.rows.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
  }
  protected filteredProducts(page: LegacySaleCreatePage) {
    const term = this.productSearch.value.trim().toLocaleLowerCase();
    return term
      ? page.products.filter((x) =>
          `${x.code} ${x.name} ${x.barcode ?? ''}`.toLocaleLowerCase().includes(term),
        )
      : page.products;
  }
  protected addItem() {
    const p = this.page();
    if (p) this.items.push(this.lineGroup(p.products[0]));
  }
  protected removeItem(index: number) {
    this.items.removeAt(index);
  }
  protected productChanged(index: number) {
    const group = this.items.at(index);
    const product = this.page()?.products.find(
      (x) => x.id === Number(group.get('productId')?.value),
    );
    if (product) group.patchValue({ salePrice: product.wholesalePrice, trackingAllocations: null });
  }
  protected isTracked(index: number) {
    const id = Number(this.items.at(index).get('productId')?.value);
    return (this.page()?.products.find((x) => x.id === id)?.trackingPolicy ?? 0) !== 0;
  }
  protected create() {
    if (this.form.invalid || !this.items.length || this.saving()) {
      this.form.markAllAsTouched();
      this.problem.set({ title: this.i18n.text('required') } as NormalizedApiProblem);
      return;
    }
    this.saving.set(true);
    this.problem.set(null);
    this.api.create(this.form.getRawValue() as LegacySaleCreateInput).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.syncDirty();
        void this.router.navigate(['/sales/legacy', x.id]);
      },
      error: (e) => this.failAction(e),
    });
  }
  protected post() {
    const x = this.detail();
    if (!x || x.isPosted || this.saving()) return;
    this.saving.set(true);
    this.api.post(x.sale.id).subscribe({
      next: () => {
        this.success.set(this.i18n.text('postSuccess'));
        this.loadDetailAfterPost(x.sale.id);
      },
      error: (e) => this.failAction(e),
    });
  }
  private loadForm(p: LegacySaleCreatePage) {
    this.page.set(p);
    this.items.clear({ emitEvent: false });
    this.form.patchValue(
      {
        ...p.form,
        items: undefined,
        warehouseId: p.form.warehouseId || p.warehouses[0]?.id || 0,
        paymentMethodId: p.form.paymentMethodId || p.paymentMethods[0]?.id || 0,
        date: p.form.date.slice(0, 10),
      } as any,
      { emitEvent: false },
    );
    const lines = p.form.items.length ? p.form.items : [this.blankLine(p.products[0])];
    for (const line of lines) this.items.push(this.lineGroup(line), { emitEvent: false });
    this.form.markAsPristine();
    this.done();
    this.syncDirty();
  }
  private lineGroup(product?: LegacySaleProductOption | any) {
    const line = 'productId' in (product ?? {}) ? product : this.blankLine(product);
    return this.fb.group({
      productId: [line.productId, [Validators.required, Validators.min(1)]],
      quantity: [line.quantity ?? 1, [Validators.required, Validators.min(0.000001)]],
      salePrice: [line.salePrice ?? product?.wholesalePrice ?? 0],
      discountType: [line.discountType ?? 1],
      discountValue: [line.discountValue ?? 0, Validators.min(0)],
      notes: [line.notes ?? null, Validators.maxLength(200)],
      trackingAllocations: [line.trackingAllocations ?? null, Validators.maxLength(4000)],
    });
  }
  private blankLine(product?: LegacySaleProductOption) {
    return {
      productId: product?.id ?? 0,
      quantity: 1,
      salePrice: product?.wholesalePrice ?? 0,
      discountType: 1,
      discountValue: 0,
      notes: null,
      trackingAllocations: null,
    };
  }
  private loadDetailAfterPost(id: number) {
    this.api.get(id).subscribe({
      next: (x) => {
        this.detail.set(x);
        this.saving.set(false);
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
  private syncDirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
