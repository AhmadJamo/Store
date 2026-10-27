import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PurchaseRequestApiService } from './purchase-request-api.service';
import { CreateRequestLine, CreateRequestPage, PurchaseRequest } from './purchase-request.models';
import {
  PurchaseRequestKey,
  PurchaseRequestLocalizationService,
} from './purchase-request-localization.service';
@Component({
  selector: 'app-purchase-requests-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    PageHeaderComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
    DataTableComponent,
  ],
  templateUrl: './purchase-requests-page.html',
  styleUrl: './purchase-requests-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseRequestsPageComponent {
  protected readonly i18n = inject(PurchaseRequestLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseRequestApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly mode = signal<'list' | 'create' | 'details'>('list');
  protected readonly rows = signal<PurchaseRequest[]>([]);
  protected readonly detail = signal<PurchaseRequest | null>(null);
  protected readonly options = signal<CreateRequestPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly filter = this.fb.nonNullable.group({ status: [0], search: [''] });
  protected readonly form = this.fb.nonNullable.group({
    warehouseId: [0, Validators.min(1)],
    neededByDate: ['', Validators.required],
    priority: [2],
    justification: ['', [Validators.required, Validators.maxLength(500)]],
    notes: ['', Validators.maxLength(1000)],
    lines: this.fb.array([]),
  });
  constructor() {
    this.form.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.resolve());
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected get lines(): FormArray {
    return this.form.controls.lines;
  }
  protected addLine() {
    this.lines.push(
      this.fb.nonNullable.group({
        productId: [0, Validators.min(1)],
        measurementUnitId: [0, Validators.min(1)],
        requestedQuantity: [1, Validators.min(0.000001)],
        suggestedSupplierId: [0],
        notes: ['', Validators.maxLength(500)],
      }),
    );
  }
  protected removeLine(i: number) {
    if (this.lines.length > 1) this.lines.removeAt(i);
  }
  protected loadList() {
    const f = this.filter.getRawValue();
    this.start();
    this.api.getAll(f.status || undefined, f.search).subscribe({
      next: (r) => {
        this.rows.set(r);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  protected create() {
    if (this.form.invalid || !this.lines.length) {
      this.form.markAllAsTouched();
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    const v = this.form.getRawValue();
    const lines = (
      v.lines as {
        productId: number;
        measurementUnitId: number;
        requestedQuantity: number;
        suggestedSupplierId: number;
        notes: string;
      }[]
    ).map((x) => ({
      productId: x.productId,
      measurementUnitId: x.measurementUnitId,
      requestedQuantity: x.requestedQuantity,
      suggestedSupplierId: x.suggestedSupplierId || undefined,
      notes: x.notes.trim() || undefined,
    })) as CreateRequestLine[];
    this.saving.set(true);
    this.problem.set('');
    this.api
      .create({
        ...v,
        notes: v.notes.trim() || undefined,
        justification: v.justification.trim(),
        lines,
      })
      .subscribe({
        next: (r) => {
          this.saving.set(false);
          this.form.markAsPristine();
          this.markDirty();
          void this.router.navigate(['/purchases/requests', r.id]);
        },
        error: (e) => {
          this.saving.set(false);
          this.showError(e);
        },
      });
  }
  protected submit() {
    const x = this.detail();
    if (!x) return;
    this.command(() => this.api.submit(x.id, x.rowVersion), 'submitted');
  }
  protected cancel(reason: string) {
    const x = this.detail();
    if (!x || !reason.trim()) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    this.command(() => this.api.cancel(x.id, x.rowVersion, reason.trim()), 'cancelled');
  }
  protected statusName(v: number) {
    return this.i18n.text(
      (
        {
          1: 'draft',
          2: 'submittedStatus',
          3: 'cancelledStatus',
          4: 'approved',
          5: 'rejected',
          6: 'sourced',
        } as Record<number, PurchaseRequestKey>
      )[v] ?? 'draft',
    );
  }
  protected priorityName(v: number) {
    return this.i18n.text(
      ({ 1: 'low', 2: 'normal', 3: 'high', 4: 'urgent' } as Record<number, PurchaseRequestKey>)[
        v
      ] ?? 'normal',
    );
  }
  protected actionName(v: number) {
    return this.i18n.text(
      (
        {
          1: 'createdAction',
          2: 'submittedAction',
          3: 'cancelledAction',
          4: 'approvedAction',
          5: 'rejectedAction',
          6: 'sourcedAction',
        } as Record<number, PurchaseRequestKey>
      )[v] ?? 'createdAction',
    );
  }
  protected retry() {
    this.resolve();
  }
  protected approvalStatusName(v: number) {
    return this.i18n.text(
      ({ 1: 'pendingApproval', 2: 'approvedApproval', 3: 'rejectedApproval' } as Record<
        number,
        PurchaseRequestKey
      >)[v] ?? 'pendingApproval',
    );
  }
  private resolve() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.route.snapshot.routeConfig?.path?.endsWith('new')) {
      this.mode.set('create');
      this.start();
      this.api.getCreateForm().subscribe({
        next: (p) => {
          this.options.set(p);
          this.form.patchValue({
            warehouseId: p.form.warehouseId,
            neededByDate: p.form.neededByDate,
            priority: p.form.priority,
            justification: p.form.justification,
            notes: p.form.notes ?? '',
          });
          this.lines.clear();
          this.addLine();
          this.form.markAsPristine();
          this.markDirty();
          this.loading.set(false);
        },
        error: (e) => this.fail(e),
      });
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
  private loadDetail(id: number) {
    this.start();
    this.api.get(id).subscribe({
      next: (r) => {
        this.detail.set(r);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private command(
    request: () => ReturnType<PurchaseRequestApiService['submit']>,
    message: PurchaseRequestKey,
  ) {
    if (this.saving()) return;
    this.saving.set(true);
    this.problem.set('');
    request().subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(this.i18n.text(message));
        this.loadDetail(this.detail()!.id);
      },
      error: (e) => {
        this.saving.set(false);
        this.showError(e);
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
  private showError(e: unknown) {
    const f = this.errors.normalize(e);
    this.problem.set(f.detail ?? f.title ?? this.i18n.text('error'));
  }
  private markDirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
