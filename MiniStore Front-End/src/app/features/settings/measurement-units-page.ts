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
import { MeasurementUnit, MeasurementUnitsPage } from './data/settings.models';

@Component({ selector: 'app-measurement-units-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './measurement-units-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class MeasurementUnitsPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly message = signal<'created' | 'deactivated' | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly units = signal<MeasurementUnit[]>([]);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    symbol: ['', [Validators.required, Validators.maxLength(20)]],
    dimension: [2, [Validators.required, Validators.min(1), Validators.max(4)]],
    factorToBaseUnit: [1, [Validators.required, Validators.min(0.000000000001)]],
    decimalPlaces: [3, [Validators.required, Validators.min(0), Validators.max(6)]],
  });

  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty)); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getMeasurementUnits().subscribe({ next: page => { this.apply(page); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected create(): void { if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); return; } const value = this.form.getRawValue(); this.start(); this.api.createMeasurementUnit({ ...value, code: value.code.trim().toUpperCase(), name: value.name.trim(), symbol: value.symbol.trim() }).subscribe({ next: page => { this.apply(page); this.message.set('created'); this.saving.set(false); }, error: error => this.fail(error) }); }
  protected deactivate(unit: MeasurementUnit): void { if (this.saving() || unit.isSystem || !unit.isActive || !globalThis.confirm(this.i18n.text('confirmDeactivateUnit'))) return; this.start(); this.api.deactivateMeasurementUnit(unit.id).subscribe({ next: page => { this.apply(page); this.message.set('deactivated'); this.saving.set(false); }, error: error => this.fail(error) }); }
  protected dimensionName(value: number): string { return this.i18n.text(value === 1 ? 'countDimension' : value === 2 ? 'massDimension' : value === 3 ? 'volumeDimension' : 'lengthDimension'); }
  private start(): void { this.saving.set(true); this.message.set(null); this.problem.set(null); }
  private fail(error: unknown): void { this.problem.set(this.errors.normalize(error)); this.saving.set(false); }
  private apply(page: MeasurementUnitsPage): void { this.units.set(page.units); this.form.reset(page.newUnit); this.form.markAsPristine(); this.setDirty(false); }
  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
