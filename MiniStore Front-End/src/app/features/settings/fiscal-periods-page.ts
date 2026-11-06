import { SlicePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SettingsApiService } from './data/settings-api.service';
import { FiscalPeriod, FiscalPeriodsPage, FiscalPeriodTransition } from './data/settings.models';
import { FiscalPeriodLocalizationService } from './fiscal-period-localization.service';

type PeriodAction = 'soft-close' | 'close' | 'reopen';

@Component({
  selector: 'app-fiscal-periods-page',
  imports: [SlicePipe, ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent],
  templateUrl: './fiscal-periods-page.html',
  styleUrl: './settings-forms.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FiscalPeriodsPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly periodI18n = inject(FiscalPeriodLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly saved = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly periods = signal<FiscalPeriod[]>([]);
  protected readonly drafts = signal<Record<number, string>>({});
  protected readonly form = inject(FormBuilder).nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty());
  }

  canDeactivate(): boolean {
    return (!this.form.dirty && !this.hasDraft()) || globalThis.confirm(this.i18n.text('unsavedDescription'));
  }

  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getFiscalPeriods().subscribe({
      next: value => { this.apply(value); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected create(): void {
    if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.start();
    this.api.createFiscalPeriod(this.form.getRawValue()).subscribe({
      next: value => this.success(value),
      error: error => this.failure(error),
    });
  }

  protected setReason(id: number, event: Event): void {
    const reason = (event.target as HTMLInputElement).value;
    this.drafts.update(value => ({ ...value, [id]: reason }));
    this.syncDirty();
  }

  protected transition(period: FiscalPeriod, action: PeriodAction): void {
    const reason = this.drafts()[period.id]?.trim();
    if (!reason || this.saving()) return;
    const command: FiscalPeriodTransition = { reason, rowVersion: period.rowVersion };
    this.start();
    this.api.transitionFiscalPeriod(period.id, action, command).subscribe({
      next: value => this.success(value),
      error: error => this.failure(error),
    });
  }

  protected statusName(status: number): string {
    return status === 1 ? this.i18n.text('open') : status === 2 ? this.i18n.text('softClosed') : this.i18n.text('closed');
  }

  protected canSoftClose(period: FiscalPeriod): boolean { return period.status === 1 && this.session.can('FiscalPeriods.SoftClose'); }
  protected canClose(period: FiscalPeriod): boolean { return period.status === 2 && this.session.can('FiscalPeriods.Close'); }
  protected canReopen(period: FiscalPeriod): boolean { return (period.status === 2 || period.status === 3) && this.session.can('FiscalPeriods.Reopen'); }
  protected hasAction(period: FiscalPeriod): boolean { return this.canSoftClose(period) || this.canClose(period) || this.canReopen(period); }

  private hasDraft(): boolean { return Object.values(this.drafts()).some(reason => reason.length > 0); }
  private start(): void { this.saving.set(true); this.saved.set(false); this.problem.set(null); }
  private success(value: FiscalPeriodsPage): void { this.apply(value); this.saved.set(true); this.saving.set(false); }
  private failure(error: unknown): void { this.problem.set(this.errors.normalize(error)); this.saving.set(false); }
  private apply(value: FiscalPeriodsPage): void {
    this.periods.set(value.periods);
    this.drafts.set({});
    this.form.reset({
      name: value.newPeriod.name ?? '',
      startDate: value.newPeriod.startDate?.slice(0, 10) ?? '',
      endDate: value.newPeriod.endDate?.slice(0, 10) ?? '',
    });
    this.form.markAsPristine();
    this.syncDirty();
  }
  private syncDirty(): void {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty || this.hasDraft());
  }
}
