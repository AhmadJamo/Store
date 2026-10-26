import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SettingsApiService } from './data/settings-api.service';
import { GeneralPreferences, OrganizationIdentity } from './data/settings.models';

@Component({ selector: 'app-settings-overview-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, PageHeaderComponent, ApiRecoveryComponent, FeedbackStateComponent], templateUrl: './settings-overview-page.html', styleUrl: './settings-overview-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class SettingsOverviewPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly identity = signal<OrganizationIdentity | null>(null);
  protected readonly preferences = signal<GeneralPreferences | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly saved = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly saveProblem = signal<NormalizedApiProblem | null>(null);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    currency: ['JOD', [Validators.required, Validators.maxLength(10)]],
    decimalPlaces: [2, [Validators.required, Validators.min(0), Validators.max(4)]],
    quantityDecimalPlaces: [3, [Validators.required, Validators.min(0), Validators.max(4)]],
    dateFormat: ['dd/MM/yyyy', [Validators.required, Validators.maxLength(50)]],
    timeZone: ['Asia/Amman', [Validators.required, Validators.maxLength(100)]],
    defaultLanguage: 1,
    rowVersion: '',
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty));
  }

  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }

  protected load(): void {
    this.loading.set(true); this.problem.set(null); this.saveProblem.set(null); this.saved.set(false);
    this.api.getOverview().subscribe({
      next: value => { this.identity.set(value.identity); this.applyPreferences(value.preferences); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected save(): void {
    if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving.set(true); this.saved.set(false); this.saveProblem.set(null);
    this.api.updatePreferences(this.form.getRawValue()).subscribe({
      next: value => { this.applyPreferences(value); this.saved.set(true); this.saving.set(false); },
      error: error => { this.saveProblem.set(this.errors.normalize(error)); this.saving.set(false); },
    });
  }

  protected companyName(value: OrganizationIdentity): string { return this.i18n.locale() === 'ar-JO' ? (value.tradeNameArabic || value.tradeName) : value.tradeName; }

  private applyPreferences(value: GeneralPreferences): void { this.preferences.set(value); this.form.reset(value); this.form.markAsPristine(); this.setDirty(false); }
  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
