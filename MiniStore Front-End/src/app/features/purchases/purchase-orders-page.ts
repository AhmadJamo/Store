import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PurchaseOrderApiService } from './purchase-order-api.service';
import { PurchaseOrder } from './purchase-order.models';
import {
  PurchaseOrderKey,
  PurchaseOrderLocalizationService,
} from './purchase-order-localization.service';
@Component({
  selector: 'app-purchase-orders-page',
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
  templateUrl: './purchase-orders-page.html',
  styleUrl: './purchase-orders-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseOrdersPageComponent {
  protected readonly i18n = inject(PurchaseOrderLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseOrderApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly rows = signal<PurchaseOrder[]>([]);
  protected readonly detail = signal<PurchaseOrder | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly filter = this.fb.nonNullable.group({ status: [0], search: [''] });
  protected readonly dropship = this.fb.nonNullable.group({
    recipientName: ['', [Validators.required, Validators.maxLength(200)]],
    address: ['', [Validators.required, Validators.maxLength(1000)]],
    customerReference: ['', Validators.maxLength(100)],
  });
  protected readonly confirmation = this.fb.nonNullable.group({
    confirmedExpectedDate: ['', Validators.required],
    confirmationReference: ['', [Validators.required, Validators.maxLength(250)]],
    note: ['', Validators.maxLength(1000)],
  });
  protected readonly delivery = this.fb.nonNullable.group({
    purchaseOrderLineId: [0, Validators.min(1)],
    quantity: [0, Validators.min(0.000001)],
    deliveryDate: [new Date().toISOString().slice(0, 10), Validators.required],
    proofReference: ['', [Validators.required, Validators.maxLength(250)]],
    carrier: ['', Validators.maxLength(150)],
    trackingNumber: ['', Validators.maxLength(150)],
  });
  constructor() {
    this.dropship.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.confirmation.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.delivery.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.resolve());
  }
  canDeactivate() {
    return (
      !(this.dropship.dirty || this.confirmation.dirty || this.delivery.dirty) ||
      globalThis.confirm(this.i18n.text('unsaved'))
    );
  }
  protected loadList() {
    const v = this.filter.getRawValue();
    this.start();
    this.api.getAll(v.status || undefined, v.search).subscribe({
      next: (x) => {
        this.rows.set(x);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  protected command(action: string, reason?: string) {
    const x = this.detail();
    if (!x || (['cancel', 'close-remaining', 'reopen'].includes(action) && !reason?.trim())) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    this.run(this.api.command(x.id, action, x.rowVersion, reason?.trim()), 'success');
  }
  protected configureDropship() {
    const x = this.detail();
    if (!x || this.dropship.invalid) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    const v = this.dropship.getRawValue();
    this.run(
      this.api.configureDropship(x.id, {
        purchaseOrderId: x.id,
        rowVersion: x.rowVersion,
        ...v,
        customerReference: v.customerReference.trim() || undefined,
      }),
      'success',
      this.dropship,
    );
  }
  protected recordConfirmation() {
    const x = this.detail();
    if (!x || this.confirmation.invalid) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    const v = this.confirmation.getRawValue();
    this.run(
      this.api.recordSupplierConfirmation(x.id, {
        purchaseOrderId: x.id,
        rowVersion: x.rowVersion,
        ...v,
        note: v.note.trim() || undefined,
      }),
      'success',
      this.confirmation,
    );
  }
  protected recordDelivery() {
    const x = this.detail();
    if (!x || this.delivery.invalid) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    const v = this.delivery.getRawValue();
    this.run(
      this.api.recordDirectDelivery(x.id, {
        purchaseOrderId: x.id,
        rowVersion: x.rowVersion,
        ...v,
        carrier: v.carrier.trim() || undefined,
        trackingNumber: v.trackingNumber.trim() || undefined,
      }),
      'success',
      this.delivery,
    );
  }
  protected statusName(v: number) {
    return this.i18n.text(
      (
        { 1: 'draft', 2: 'approved', 3: 'confirmed', 4: 'cancelled', 5: 'closed' } as Record<
          number,
          PurchaseOrderKey
        >
      )[v] ?? 'draft',
    );
  }
  protected retry() {
    this.resolve();
  }
  private resolve() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) this.loadDetail(id);
    else this.loadList();
  }
  private loadDetail(id: number) {
    this.start();
    this.api.get(id).subscribe({
      next: (x) => {
        this.detail.set(x);
        this.confirmation.patchValue({ confirmedExpectedDate: x.expectedDate });
        this.confirmation.markAsPristine();
        this.markDirty();
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private run(request: any, message: PurchaseOrderKey, form?: any) {
    if (this.saving()) return;
    this.saving.set(true);
    this.problem.set('');
    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(this.i18n.text(message));
        form?.markAsPristine();
        this.markDirty();
        this.loadDetail(this.detail()!.id);
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
    const t = this.workspace.activeTab();
    if (t)
      this.workspace.markDirty(
        t.id,
        this.dropship.dirty || this.confirmation.dirty || this.delivery.dirty,
      );
  }
}
