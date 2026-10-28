import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PurchaseAgreementApiService } from './purchase-agreement-api.service';
import { PurchaseAgreementLocalizationService } from './purchase-agreement-localization.service';
import { PurchaseTemplate } from './purchase-agreement.models';
import { PurchaseOrderApiService } from './purchase-order-api.service';
import { PurchaseOrder } from './purchase-order.models';
@Component({
  selector: 'app-purchase-templates-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './purchase-templates-page.html',
  styleUrl: './purchase-templates-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseTemplatesPageComponent {
  protected readonly i18n = inject(PurchaseAgreementLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseAgreementApiService);
  private readonly orders = inject(PurchaseOrderApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  protected readonly rows = signal<PurchaseTemplate[]>([]);
  protected readonly order = signal<PurchaseOrder | null>(null);
  protected readonly creating = signal(false);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    neededInDays: [7, [Validators.min(0), Validators.max(3650)]],
    priority: [2, Validators.required],
  });
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.resolve());
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected create() {
    const o = this.order();
    if (!o || this.form.invalid) return this.problem.set(this.i18n.text('fix'));
    this.saving.set(true);
    this.api.createTemplate({ purchaseOrderId: o.id, ...this.form.getRawValue() }).subscribe({
      next: () => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/purchases/templates']);
      },
      error: (e) => this.failSave(e),
    });
  }
  protected generate(x: PurchaseTemplate) {
    this.saving.set(true);
    this.api.generate(x.id).subscribe({
      next: (r) => void this.router.navigate(['/purchases/requests', r.id]),
      error: (e) => this.failSave(e),
    });
  }
  protected setActive(x: PurchaseTemplate) {
    this.saving.set(true);
    this.api
      .setActive(x.id, !x.isActive)
      .subscribe({ next: () => this.loadList(), error: (e) => this.failSave(e) });
  }
  protected priorityName(v: number) {
    return this.i18n.text(v === 1 ? 'low' : v === 3 ? 'high' : v === 4 ? 'urgent' : 'normal');
  }
  protected retry() {
    this.resolve();
  }
  private resolve() {
    this.creating.set((this.route.snapshot.routeConfig?.path ?? '').endsWith('/new'));
    if (this.creating()) {
      const id = Number(this.route.snapshot.queryParamMap.get('purchaseOrderId'));
      this.orders.get(id).subscribe({
        next: (x) => {
          this.order.set(x);
          this.loading.set(false);
        },
        error: (e) => this.fail(e),
      });
    } else this.loadList();
  }
  private loadList() {
    this.loading.set(true);
    this.api.getTemplates().subscribe({
      next: (x) => {
        this.rows.set(x);
        this.saving.set(false);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private failSave(e: unknown) {
    this.saving.set(false);
    const p = this.errors.normalize(e);
    this.problem.set(p.detail ?? p.title ?? this.i18n.text('error'));
  }
  private fail(e: unknown) {
    this.loadingProblem.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private dirty() {
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.form.dirty);
  }
}
