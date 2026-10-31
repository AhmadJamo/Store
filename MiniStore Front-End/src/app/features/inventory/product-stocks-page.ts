import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductStockApiService } from './data/product-stock-api.service';
import { ManualStockAdjustmentFormPage, ProductStock, ProductStockFormPage } from './data/product-stock.models';
import { InventoryLocalizationService } from './inventory-localization.service';

@Component({ selector: 'app-product-stocks-page', imports: [DecimalPipe, ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent, FormErrorSummaryComponent], templateUrl: './product-stocks-page.html', styleUrl: './product-stocks-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductStocksPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService); protected readonly session = inject(ApiSessionService);
  private readonly api = inject(ProductStockApiService); private readonly errors = inject(ApiErrorService); private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef); private readonly workspace = inject(WorkspaceService); private readonly host = inject(ElementRef<HTMLElement>); private readonly focus = inject(FormFocusService);
  protected readonly items = signal<readonly ProductStock[]>([]); protected readonly stockOptions = signal<ProductStockFormPage | null>(null); protected readonly adjustmentOptions = signal<ManualStockAdjustmentFormPage | null>(null);
  protected readonly editingId = signal<number | null>(null); protected readonly loading = signal(true); protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null); protected readonly problem = signal(''); protected readonly success = signal(''); protected readonly saving = signal(false);
  protected readonly stockForm = this.fb.nonNullable.group({ productId: [0, Validators.min(1)], warehouseId: [0, Validators.min(1)], quantity: [0, Validators.min(0)] });
  protected readonly adjustmentForm = this.fb.nonNullable.group({ productId: [0, Validators.min(1)], warehouseId: [0, Validators.min(1)], type: [6, Validators.pattern(/^[678]$/)], quantity: [0, Validators.min(0.001)], reference: ['', [Validators.required, Validators.maxLength(100)]] });
  constructor() { this.load(); if (this.session.can('ProductStock.Create')) this.loadStockForm(); if (this.session.can('StockTransactions.Create')) this.loadAdjustmentForm(); this.stockForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty()); this.adjustmentForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty()); }
  canDeactivate() { return !(this.stockForm.dirty || this.adjustmentForm.dirty) || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load() { this.loading.set(true); this.loadingProblem.set(null); this.api.getAll().subscribe({ next: items => { this.items.set(items); this.loading.set(false); }, error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected loadStockForm(id?: number) { this.clearMessages(); this.api.getForm(id).subscribe({ next: page => { this.stockOptions.set(page); this.editingId.set(id ?? null); this.stockForm.patchValue(page.form); this.stockForm.controls.productId[id ? 'disable' : 'enable']({ emitEvent: false }); this.stockForm.controls.warehouseId[id ? 'disable' : 'enable']({ emitEvent: false }); this.stockForm.markAsPristine(); this.syncDirty(); }, error: error => this.problem.set(this.message(error)) }); }
  protected cancelEdit() { this.loadStockForm(); }
  protected saveStock() { if (this.saving() || this.stockForm.invalid) { this.stockForm.markAllAsTouched(); this.problem.set(this.i18n.text('correctStockFields')); this.focus.focusFirstInvalid(this.host.nativeElement); return; } const raw = this.stockForm.getRawValue(); this.saving.set(true); const request = this.editingId() ? this.api.update(this.editingId()!, raw.quantity) : this.api.create(raw); request.subscribe({ next: item => { this.items.update(items => this.editingId() ? items.map(x => x.id === item.id ? item : x) : [...items, item]); this.saving.set(false); this.success.set(this.i18n.text('stockSaved')); this.loadStockForm(); }, error: error => { this.problem.set(this.message(error)); this.saving.set(false); } }); }
  protected delete(item: ProductStock) { if (!globalThis.confirm(this.i18n.text('confirmDeleteStock'))) return; this.clearMessages(); this.api.delete(item.id).subscribe({ next: () => this.items.update(items => items.filter(x => x.id !== item.id)), error: error => this.problem.set(this.message(error)) }); }
  protected saveAdjustment() { if (this.saving() || this.adjustmentForm.invalid) { this.adjustmentForm.markAllAsTouched(); this.problem.set(this.i18n.text('correctAdjustmentFields')); this.focus.focusFirstInvalid(this.host.nativeElement); return; } this.clearMessages(); this.saving.set(true); this.api.createAdjustment(this.adjustmentForm.getRawValue()).subscribe({ next: () => { this.saving.set(false); this.success.set(this.i18n.text('adjustmentSaved')); this.load(); this.resetAdjustment(); }, error: error => { this.problem.set(this.message(error)); this.saving.set(false); } }); }
  private loadAdjustmentForm() { this.api.getAdjustmentForm().subscribe({ next: page => { this.adjustmentOptions.set(page); this.adjustmentForm.patchValue(page.form); this.adjustmentForm.markAsPristine(); }, error: error => this.problem.set(this.message(error)) }); }
  private resetAdjustment() { const page = this.adjustmentOptions(); if (page) this.adjustmentForm.reset(page.form); this.adjustmentForm.markAsPristine(); this.syncDirty(); }
  private clearMessages() { this.problem.set(''); this.success.set(''); }
  private message(error: unknown) { const failure = this.errors.normalize(error); return failure.detail ?? failure.title ?? this.i18n.text('errorDescription'); }
  private syncDirty() { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, this.stockForm.dirty || this.adjustmentForm.dirty); }
}
