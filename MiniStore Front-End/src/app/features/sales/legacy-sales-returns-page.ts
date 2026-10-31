import { DatePipe, DecimalPipe } from '@angular/common';
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
import { LegacySalesReturnsApiService } from './legacy-sales-returns-api.service';
import { LegacySalesReturnsLocalizationService } from './legacy-sales-returns-localization.service';
import {
  SalesReturn,
  SalesReturnCreateInput,
  SalesReturnCreatePage,
} from './legacy-sales-returns.models';
@Component({
  selector: 'app-legacy-sales-returns-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './legacy-sales-returns-page.html',
  styleUrl: './legacy-sales-returns-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LegacySalesReturnsPageComponent {
  protected readonly i18n = inject(LegacySalesReturnsLocalizationService);
  private readonly api = inject(LegacySalesReturnsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<SalesReturn[]>([]);
  protected readonly detail = signal<SalesReturn | null>(null);
  protected readonly page = signal<SalesReturnCreatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly saving = signal(false);
  protected readonly form = this.fb.group({
    saleId: this.fb.nonNullable.control(0),
    date: this.fb.nonNullable.control('', Validators.required),
    reason: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(500)]),
    items: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly saleId = Number(this.route.snapshot.paramMap.get('saleId'));
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
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
    if (this.saleId)
      this.api
        .createForm(this.saleId)
        .subscribe({ next: (p) => this.loadForm(p), error: (e) => this.failLoad(e) });
    else if (this.id)
      this.api.get(this.id).subscribe({
        next: (x) => {
          this.detail.set(x);
          this.loading.set(false);
        },
        error: (e) => this.failLoad(e),
      });
    else
      this.api.list().subscribe({
        next: (x) => {
          this.rows.set(x);
          this.loading.set(false);
        },
        error: (e) => this.failLoad(e),
      });
  }
  protected create() {
    const value = this.form.getRawValue();
    if (this.form.invalid || !value.items.some((x: any) => x.quantity > 0) || this.saving()) {
      this.form.markAllAsTouched();
      this.problem.set({ title: this.i18n.text('required') } as NormalizedApiProblem);
      return;
    }
    this.saving.set(true);
    this.api.create(value as SalesReturnCreateInput).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/legacy-returns', x.id]);
      },
      error: (e) => {
        this.problem.set(this.errors.normalize(e));
        this.saving.set(false);
      },
    });
  }
  private loadForm(p: SalesReturnCreatePage) {
    this.page.set(p);
    this.form.patchValue(
      { saleId: p.saleId, date: p.date.slice(0, 10), reason: '' },
      { emitEvent: false },
    );
    this.items.clear({ emitEvent: false });
    for (const x of p.lines)
      this.items.push(
        this.fb.group({
          saleItemId: [x.saleItemId],
          quantity: [0, [Validators.min(0), Validators.max(x.remainingQuantity)]],
          trackingAllocations: [null, Validators.maxLength(4000)],
        }),
        { emitEvent: false },
      );
    this.form.markAsPristine();
    this.loading.set(false);
    this.dirty();
  }
  private failLoad(e: unknown) {
    this.failure.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private dirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
