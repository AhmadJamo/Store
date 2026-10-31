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
import { PosExperienceSettings, PosExperienceSettingsPage, PosExperienceTerminal } from './data/settings.models';

@Component({ selector: 'app-pos-experience-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './pos-experience-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class PosExperiencePageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly saved = signal<'profile' | 'custom' | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly terminals = signal<PosExperienceTerminal[]>([]);
  protected readonly selectedTerminalId = signal<number | null>(null);
  protected readonly hasSettings = signal(false);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    posTerminalId: 0, terminalName: '', profile: 1, productLayout: 1, theme: 1, cartPosition: 1,
    accentColor: ['#2563A8', [Validators.required, Validators.pattern(/^#[0-9A-Fa-f]{6}$/)]],
    headerTitle: ['', [Validators.required, Validators.maxLength(80)]], productColumns: [4, [Validators.min(2), Validators.max(6)]],
    compactProductCards: false, showBarcode: true, showPrice: true, showStock: true, autoFocusSearch: true, touchOptimized: true,
    enableWalkIn: true, enableDineIn: false, enableTakeaway: false, enableDelivery: false, defaultOrderType: 1,
    requireServiceReference: false, enableGuestCount: false, enableItemNotes: false, quickAddOnBarcodeScan: false,
    applyProfileDefaults: false, rowVersion: '',
  });

  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty()); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected selectTerminal(event: Event): void { const value = Number((event.target as HTMLSelectElement).value); if (this.form.dirty && !globalThis.confirm(this.i18n.text('unsavedDescription'))) return; this.load(value || undefined); }
  protected save(applyProfileDefaults = false): void {
    if (this.saving() || this.form.invalid || !this.hasSettings()) { this.form.markAllAsTouched(); return; }
    const value: PosExperienceSettings = { ...this.form.getRawValue(), headerTitle: this.form.controls.headerTitle.value.trim(), applyProfileDefaults };
    this.saving.set(true); this.saved.set(null); this.problem.set(null);
    this.api.updatePosExperience(value).subscribe({ next: page => { this.apply(page); this.saved.set(applyProfileDefaults ? 'profile' : 'custom'); this.saving.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); } });
  }
  protected profileName(value: number): string { return this.i18n.text(['', 'retailProfile', 'groceryProfile', 'cafeProfile', 'restaurantProfile', 'quickServiceProfile'][value] as 'retailProfile'); }
  protected orderTypeName(value: number): string { return this.i18n.text(['', 'walkIn', 'dineIn', 'takeaway', 'delivery'][value] as 'walkIn'); }
  private load(terminalId?: number): void { this.loading.set(true); this.problem.set(null); this.api.getPosExperience(terminalId).subscribe({ next: page => { this.apply(page); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  private apply(page: PosExperienceSettingsPage): void { this.terminals.set(page.terminals); this.selectedTerminalId.set(page.selectedTerminalId); this.hasSettings.set(page.settings !== null); if (page.settings) { this.form.reset(page.settings); this.form.markAsPristine(); } else { this.form.reset(); this.form.markAsPristine(); } this.syncDirty(); }
  private syncDirty(): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, this.form.dirty); }
}
