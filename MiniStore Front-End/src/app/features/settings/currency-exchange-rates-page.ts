import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SettingsApiService } from './data/settings-api.service';
import { CurrencyExchangeRate, CurrencyExchangeRatesPage } from './data/settings.models';

@Component({
  selector: 'app-currency-exchange-rates-page',
  imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent],
  templateUrl: './currency-exchange-rates-page.html',
  styleUrl: './settings-forms.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CurrencyExchangeRatesPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly saved = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly companyCurrencyCode = signal('');
  protected readonly rates = signal<CurrencyExchangeRate[]>([]);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    currencyCode: ['', [Validators.required, Validators.pattern(/^[A-Za-z]{3}$/)]],
    effectiveDate: ['', Validators.required],
    companyCurrencyPerUnit: [0, [Validators.required, Validators.min(0.000000000001)]],
    source: ['', [Validators.required, Validators.maxLength(100)]],
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty));
  }

  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }

  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    this.api.getCurrencyExchangeRates().subscribe({
      next: value => { this.apply(value); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected save(): void {
    if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();
    this.saving.set(true); this.saved.set(false); this.problem.set(null);
    this.api.saveCurrencyExchangeRate({ ...value, currencyCode: value.currencyCode.trim().toUpperCase(), source: value.source.trim() }).subscribe({
      next: page => { this.apply(page); this.saved.set(true); this.saving.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); },
    });
  }

  private apply(value: CurrencyExchangeRatesPage): void {
    this.companyCurrencyCode.set(value.companyCurrencyCode);
    this.rates.set(value.rates);
    this.form.reset({ ...value.form, effectiveDate: value.form.effectiveDate.slice(0, 10) });
    this.form.markAsPristine();
    this.setDirty(false);
  }

  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
