import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PurchaseReplenishmentApiService } from './purchase-replenishment-api.service';
import {
  PurchaseReplenishmentLocalizationService,
  ReplenishmentKey,
} from './purchase-replenishment-localization.service';
import { ReplenishmentPage } from './purchase-replenishment.models';
@Component({
  selector: 'app-purchase-replenishment-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './purchase-replenishment-page.html',
  styleUrl: './purchase-replenishment-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseReplenishmentPageComponent {
  protected readonly i18n = inject(PurchaseReplenishmentLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseReplenishmentApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly fb = inject(FormBuilder);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  protected readonly page = signal<ReplenishmentPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly message = signal('');
  protected readonly problem = signal('');
  protected readonly form = this.fb.nonNullable.group({
    isEnabled: [false],
    frequencyHours: [24, [Validators.min(1), Validators.max(168)]],
    forecastLookbackDays: [90, [Validators.min(7), Validators.max(730)]],
    mode: [1, Validators.required],
    supplierResponseDays: [7, [Validators.min(1), Validators.max(90)]],
    rowVersion: [''],
  });
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected save() {
    if (this.form.invalid) return;
    this.command(this.api.save(this.form.getRawValue()), 'success');
  }
  protected run() {
    this.command(this.api.run(), 'ran');
  }
  protected modeName(v: number) {
    return this.i18n.text(
      ({ 1: 'review', 2: 'requests', 3: 'rfq' } as Record<number, ReplenishmentKey>)[v] ?? 'review',
    );
  }
  protected retry() {
    this.load();
  }
  private command(request: any, key: ReplenishmentKey) {
    this.saving.set(true);
    request.subscribe({
      next: () => {
        this.message.set(this.i18n.text(key));
        this.load();
      },
      error: (e: unknown) => {
        this.saving.set(false);
        const p = this.errors.normalize(e);
        this.problem.set(p.detail ?? p.title ?? this.i18n.text('error'));
      },
    });
  }
  private load() {
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.api.get().subscribe({
      next: (p) => {
        this.page.set(p);
        this.form.patchValue(p.settings, { emitEvent: false });
        this.form.markAsPristine();
        this.dirty();
        this.saving.set(false);
        this.loading.set(false);
      },
      error: (e) => {
        this.loadingProblem.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  private dirty() {
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.form.dirty);
  }
}
