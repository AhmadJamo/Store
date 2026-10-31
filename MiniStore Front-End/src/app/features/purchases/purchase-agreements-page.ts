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
import { PurchaseAgreementApiService } from './purchase-agreement-api.service';
import {
  PurchaseAgreementLocalizationService,
  AgreementKey,
} from './purchase-agreement-localization.service';
import { AgreementCreatePage, PurchaseAgreement } from './purchase-agreement.models';
@Component({
  selector: 'app-purchase-agreements-page',
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
  templateUrl: './purchase-agreements-page.html',
  styleUrl: './purchase-agreements-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseAgreementsPageComponent {
  protected readonly i18n = inject(PurchaseAgreementLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseAgreementApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  protected readonly rows = signal<PurchaseAgreement[]>([]);
  protected readonly detail = signal<PurchaseAgreement | null>(null);
  protected readonly page = signal<AgreementCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly form = this.fb.nonNullable.group({
    validFrom: [new Date().toISOString().slice(0, 10), Validators.required],
    validUntil: [this.plusDays(365), Validators.required],
    lines: this.fb.array([]),
  });
  protected readonly releaseForm = this.fb.nonNullable.group({
    expectedDate: [this.plusDays(7), Validators.required],
    lines: this.fb.array([]),
  });
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.releaseForm.valueChanges
      .pipe(takeUntilDestroyed(this.destroy))
      .subscribe(() => this.dirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.resolve());
  }
  canDeactivate() {
    return (
      !(this.form.dirty || this.releaseForm.dirty) || globalThis.confirm(this.i18n.text('unsaved'))
    );
  }
  protected get createLines() {
    return this.form.controls.lines as FormArray;
  }
  protected get releaseLines() {
    return this.releaseForm.controls.lines as FormArray;
  }
  protected statusName(v: number) {
    return this.i18n.text(
      ({ 1: 'draft', 2: 'active', 3: 'cancelled' } as Record<number, AgreementKey>)[v] ?? 'draft',
    );
  }
  protected create() {
    const p = this.page(),
      v = this.form.getRawValue();
    if (
      !p ||
      this.form.invalid ||
      v.validUntil < v.validFrom ||
      !v.lines.some((x: any) => x.maximumQuantity > 0)
    )
      return this.problem.set(this.i18n.text('fix'));
    this.save(this.api.create({ sourcingEventId: p.sourcingEventId, ...v }), (x) => {
      this.form.markAsPristine();
      this.dirty();
      void this.router.navigate(['/purchases/agreements', x.id]);
    });
  }
  protected activate(x: PurchaseAgreement) {
    this.save(this.api.activate(x.id, x.rowVersion), () => this.loadDetail(x.id));
  }
  protected cancel(x: PurchaseAgreement, reason: string) {
    if (!reason.trim()) return this.problem.set(this.i18n.text('fix'));
    this.save(this.api.cancel(x.id, x.rowVersion, reason.trim()), () => this.loadDetail(x.id));
  }
  protected release(x: PurchaseAgreement) {
    const v = this.releaseForm.getRawValue();
    if (this.releaseForm.invalid || !v.lines.some((l: any) => l.quantity > 0))
      return this.problem.set(this.i18n.text('fix'));
    this.save(this.api.release(x.id, { agreementId: x.id, ...v }), (r) => {
      this.releaseForm.markAsPristine();
      this.dirty();
      void this.router.navigate(['/purchases/orders', r.id]);
    });
  }
  protected retry() {
    this.resolve();
  }
  private resolve() {
    const path = this.route.snapshot.routeConfig?.path ?? '',
      id = Number(this.route.snapshot.paramMap.get('id'));
    if (path.endsWith('/new'))
      return this.loadCreate(Number(this.route.snapshot.queryParamMap.get('sourcingEventId')));
    if (id) return this.loadDetail(id);
    this.start();
    this.api.getAll().subscribe({
      next: (x) => {
        this.rows.set(x);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private loadCreate(id: number) {
    this.start();
    this.api.getCreateForm(id).subscribe({
      next: (p) => {
        this.page.set(p);
        this.createLines.clear();
        for (const l of p.lines)
          this.createLines.push(
            this.fb.nonNullable.group({
              supplierQuotationLineId: [l.supplierQuotationLineId],
              maximumQuantity: [
                l.quotedQuantity,
                [Validators.min(0), Validators.max(l.quotedQuantity)],
              ],
            }),
          );
        this.form.markAsPristine();
        this.dirty();
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
        this.releaseLines.clear();
        for (const l of x.lines)
          this.releaseLines.push(
            this.fb.nonNullable.group({
              agreementLineId: [l.id],
              quantity: [0, [Validators.min(0), Validators.max(l.remainingQuantity)]],
            }),
          );
        this.releaseForm.markAsPristine();
        this.dirty();
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private save<T>(request: Observable<T>, next: (x: T) => void) {
    if (this.saving()) return;
    this.saving.set(true);
    this.problem.set('');
    request.subscribe({
      next: (x) => {
        this.saving.set(false);
        next(x);
      },
      error: (e) => {
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
  private dirty() {
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.form.dirty || this.releaseForm.dirty);
  }
  private plusDays(days: number) {
    const d = new Date();
    d.setDate(d.getDate() + days);
    return d.toISOString().slice(0, 10);
  }
}
