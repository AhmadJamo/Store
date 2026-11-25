import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FieldCueComponent } from '../../shared/forms/field-cue/field-cue';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { FormSectionComponent } from '../../shared/forms/form-section/form-section';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkflowFooterComponent } from '../../shared/ui/workflow-footer/workflow-footer';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { WarehouseApiService } from './data/warehouse-api.service';
import { Warehouse, WarehouseFormPage } from './data/warehouse.models';
import { ProductLocalizationService, ProductMessageKey } from './product-localization.service';

@Component({
  selector: 'app-warehouses-page',
  imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, FormErrorSummaryComponent, ApiRecoveryComponent, DataTableComponent, DataTableCellDirective, FieldCueComponent, FormSectionComponent, WorkflowFooterComponent],
  templateUrl: './warehouses-page.html',
  styleUrl: './warehouses-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WarehousesPageComponent {
  protected readonly i18n = inject(ProductLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(WarehouseApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly focus = inject(FormFocusService);
  private readonly fb = inject(FormBuilder);
  protected readonly items = signal<readonly Warehouse[]>([]);
  protected readonly options = signal<WarehouseFormPage | null>(null);
  protected readonly editingId = signal<number | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal('');
  protected readonly saving = signal(false);
  protected readonly tableColumns = computed<readonly DataTableColumn[]>(() => [
    { key: 'name', header: this.i18n.text('warehouseName'), width: '28%' },
    { key: 'branch', header: this.i18n.text('branch'), width: '18%' },
    { key: 'type', header: this.i18n.text('operationalUse'), width: '16%' },
    { key: 'controlMode', header: this.i18n.text('controlMode'), width: '16%' },
    { key: 'actions', header: this.i18n.text('actions'), kind: 'actions', width: '10rem' },
  ]);
  protected readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    branchId: [0, Validators.min(1)],
    inventoryAccountId: [0, Validators.min(1)],
    type: [1],
    controlMode: [3],
    pickingStrategy: [4],
    allowPosSales: [false],
    enforceLocationCapacity: [true],
    requireSourceLocationForTransfers: [false],
    requireDestinationLocationForTransfers: [false],
    purchaseReceivingRoute: [1],
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty));
  }

  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void {
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.api.getAll().subscribe({
      next: items => { this.items.set(items); this.loading.set(false); if (this.session.can('Warehouses.Create')) this.loadForm(); },
      error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
  protected loadForm(id?: number): void {
    this.problem.set('');
    this.api.getForm(id).subscribe({
      next: page => {
        this.options.set(page);
        this.editingId.set(id ?? null);
        this.form.patchValue({ ...page.form, branchId: page.form.branchId ?? 0, inventoryAccountId: page.form.inventoryAccountId ?? 0 });
        this.form.markAsPristine();
      },
      error: error => this.problem.set(this.errors.normalize(error).detail ?? this.i18n.text('errorDescription')),
    });
  }
  protected cancelEdit(): void { this.loadForm(); }
  protected save(): void {
    if (this.saving() || this.form.invalid) {
      this.form.markAllAsTouched();
      this.problem.set(this.i18n.text('correctWarehouseFields'));
      this.focus.focusFirstInvalid(this.host.nativeElement);
      return;
    }
    const command = this.form.getRawValue();
    this.saving.set(true);
    const request = this.editingId() ? this.api.update(this.editingId()!, command) : this.api.create(command);
    request.subscribe({
      next: item => {
        this.items.update(items => this.editingId() ? items.map(x => x.id === item.id ? item : x) : [...items, item]);
        this.saving.set(false);
        this.loadForm();
        this.setDirty(false);
      },
      error: error => {
        const failure = this.errors.normalize(error);
        this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription'));
        this.saving.set(false);
      },
    });
  }
  protected delete(item: Warehouse): void {
    if (!globalThis.confirm(this.i18n.text('confirmDeleteWarehouse'))) return;
    this.api.delete(item.id).subscribe({
      next: () => this.items.update(items => items.filter(x => x.id !== item.id)),
      error: error => this.problem.set(this.errors.normalize(error).detail ?? this.i18n.text('errorDescription')),
    });
  }
  protected typeName(value: number): string {
    return this.i18n.text(({ 1: 'warehouseGeneral', 2: 'warehouseCentral', 3: 'warehouseBackroom', 4: 'warehouseSalesFloor', 5: 'warehouseOutlet', 6: 'warehouseProduction', 7: 'warehouseTransit', 8: 'warehouseReturns', 9: 'warehouseQuarantine' } as Record<number, ProductMessageKey>)[value]);
  }
  protected controlModeName(value: number): string { return this.i18n.text(value === 1 ? 'simpleControl' : value === 2 ? 'locationManaged' : 'hybridControl'); }
  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}