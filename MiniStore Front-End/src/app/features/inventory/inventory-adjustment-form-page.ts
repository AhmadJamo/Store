import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentCreatePage } from './data/inventory-adjustment.models';
import { InventoryLocalizationService } from './inventory-localization.service';

@Component({ selector: 'app-inventory-adjustment-form-page', imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, FormErrorSummaryComponent], templateUrl: './inventory-adjustment-form-page.html', styleUrl: './inventory-adjustments.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class InventoryAdjustmentFormPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService); private readonly api = inject(InventoryAdjustmentApiService); private readonly errors = inject(ApiErrorService); private readonly router = inject(Router); private readonly workspace = inject(WorkspaceService); private readonly destroyRef = inject(DestroyRef); private readonly host = inject(ElementRef<HTMLElement>); private readonly focus = inject(FormFocusService);
  protected readonly page = signal<InventoryAdjustmentCreatePage | null>(null); protected readonly selectedProducts = signal<Set<number>>(new Set()); protected readonly saving = signal(false); protected readonly problem = signal('');
  protected readonly form = inject(FormBuilder).nonNullable.group({ warehouseId: [0, Validators.min(1)], storageLocationId: [null as number | null], isBlindCount: false, reason: ['', [Validators.required, Validators.maxLength(250)]] });
  protected locations() { return this.page()?.locations.filter(x => x.warehouseId === this.form.controls.warehouseId.value) ?? []; }
  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(true)); this.form.controls.warehouseId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.form.controls.storageLocationId.setValue(null, { emitEvent: false })); }
  canDeactivate(): boolean { return !(this.form.dirty || this.selectedProducts().size) || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected toggleProduct(id: number, checked: boolean): void { this.selectedProducts.update(current => { const next = new Set(current); checked ? next.add(id) : next.delete(id); return next; }); this.form.markAsDirty(); this.setDirty(true); }
  protected save(): void { if (this.saving()) return; this.problem.set(''); if (this.form.invalid || !this.selectedProducts().size) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('correctInventoryCountFields')); this.focus.focusFirstInvalid(this.host.nativeElement); return; } this.saving.set(true); const value = this.form.getRawValue(); this.api.create({ ...value, storageLocationId: value.storageLocationId ?? undefined, productIds: [...this.selectedProducts()] }).subscribe({ next: result => { this.form.markAsPristine(); this.selectedProducts.set(new Set()); this.setDirty(false); this.saving.set(false); void this.router.navigate(['/inventory/adjustments', result.id]); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.saving.set(false); } }); }
  private load(): void { this.api.getCreateForm().subscribe({ next: page => { this.page.set(page); this.form.reset({ warehouseId: page.input.warehouseId, storageLocationId: page.input.storageLocationId ?? null, isBlindCount: page.input.isBlindCount, reason: page.input.reason }); this.selectedProducts.set(new Set(page.input.productIds)); this.form.markAsPristine(); this.setDirty(false); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); } }); }
  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
