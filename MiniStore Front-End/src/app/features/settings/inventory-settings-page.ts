import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
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
import { InventorySettings } from './data/settings.models';

@Component({ selector: 'app-inventory-settings-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './inventory-settings-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class InventorySettingsPageComponent {
  protected readonly i18n = inject(LocalizationService); private readonly api = inject(SettingsApiService); private readonly errors = inject(ApiErrorService); private readonly workspace = inject(WorkspaceService); private readonly destroyRef = inject(DestroyRef);
  protected readonly warehouseTypes = [{ value: 1, label: 'warehouseType1' }, { value: 2, label: 'warehouseType2' }, { value: 3, label: 'warehouseType3' }, { value: 4, label: 'warehouseType4' }, { value: 5, label: 'warehouseType5' }, { value: 6, label: 'warehouseType6' }, { value: 7, label: 'warehouseType7' }, { value: 8, label: 'warehouseType8' }, { value: 9, label: 'warehouseType9' }] as const;
  protected readonly controlModes = [{ value: 1, label: 'controlMode1' }, { value: 2, label: 'controlMode2' }, { value: 3, label: 'controlMode3' }] as const;
  protected readonly pickingStrategies = [{ value: 1, label: 'pickingStrategy1' }, { value: 2, label: 'pickingStrategy2' }, { value: 3, label: 'pickingStrategy3' }, { value: 4, label: 'pickingStrategy4' }, { value: 5, label: 'pickingStrategy5' }] as const;
  protected readonly loading = signal(true); protected readonly saving = signal(false); protected readonly saved = signal(false); protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly form = inject(FormBuilder).nonNullable.group({ defaultWarehouseType: 1, defaultControlMode: 3, defaultPickingStrategy: 4, defaultAllowPosSales: false, defaultEnforceLocationCapacity: true, defaultRequireSourceLocationForTransfers: false, defaultRequireDestinationLocationForTransfers: false, rowVersion: '' });
  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => { if (this.form.controls.defaultControlMode.value === 1) { this.form.controls.defaultRequireSourceLocationForTransfers.setValue(false, { emitEvent: false }); this.form.controls.defaultRequireDestinationLocationForTransfers.setValue(false, { emitEvent: false }); } this.setDirty(this.form.dirty); }); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getInventory().subscribe({ next: value => { this.apply(value); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected save(): void { if (this.saving() || this.form.invalid || !this.form.dirty) return; this.saving.set(true); this.saved.set(false); this.problem.set(null); this.api.updateInventory(this.form.getRawValue()).subscribe({ next: value => { this.apply(value); this.saved.set(true); this.saving.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); } }); }
  protected locationManaged(): boolean { return this.form.controls.defaultControlMode.value !== 1; }
  private apply(value: InventorySettings): void { this.form.reset(value); this.form.markAsPristine(); this.setDirty(false); }
  private setDirty(value: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, value); }
}
