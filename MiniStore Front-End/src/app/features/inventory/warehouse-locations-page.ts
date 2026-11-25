import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FieldCueComponent } from '../../shared/forms/field-cue/field-cue';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormSectionComponent } from '../../shared/forms/form-section/form-section';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { FilterBarComponent } from '../../shared/ui/filter-bar/filter-bar';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { WorkflowFooterComponent } from '../../shared/ui/workflow-footer/workflow-footer';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { WarehouseOperationsApiService } from './data/warehouse-operations-api.service';
import { EditLocationCommand, WarehouseLocationPage } from './data/warehouse-operations.models';
import { WarehouseOperationsLocalizationService } from './warehouse-operations-localization.service';

@Component({
  selector: 'app-warehouse-locations-page',
  imports: [
    ReactiveFormsModule,
    PageHeaderComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
    FormErrorSummaryComponent,
    FormSectionComponent,
    FieldCueComponent,
    WorkflowFooterComponent,
    FilterBarComponent,
    SummaryStripComponent,
    DataTableComponent,
    DataTableCellDirective,
  ],
  templateUrl: './warehouse-locations-page.html',
  styleUrl: './warehouse-operations.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WarehouseLocationsPageComponent {
  protected readonly i18n = inject(WarehouseOperationsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(WarehouseOperationsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<WarehouseLocationPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly editing = signal(false);
  protected readonly editParents = signal<{ id: number; warehouseId: number; label?: string }[]>([]);
  protected readonly tableColumns = computed<readonly DataTableColumn[]>(() => {
    const columns: DataTableColumn[] = [
      { key: 'code', header: this.i18n.text('code'), kind: 'code', width: '9rem' },
      { key: 'name', header: this.i18n.text('name'), width: '28%' },
      { key: 'warehouse', header: this.i18n.text('warehouse'), width: '18%' },
      { key: 'capabilities', header: this.i18n.text('capabilities') },
      { key: 'status', header: this.i18n.text('status'), kind: 'status', width: '7rem' },
    ];
    if (this.session.can('Warehouses.Edit')) columns.push({ key: 'actions', header: this.i18n.text('editLocation'), kind: 'actions', width: '7rem' });
    return columns;
  });
  protected readonly filter = this.fb.nonNullable.group({ warehouseId: [0], query: [''] });
  protected readonly form = this.fb.nonNullable.group({
    id: [0],
    warehouseId: [0, Validators.min(1)],
    code: ['', [Validators.required, Validators.maxLength(100)]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    barcode: ['', Validators.maxLength(100)],
    parentLocationId: [0],
    sequence: [0, Validators.min(0)],
    zone: ['', Validators.maxLength(100)],
    aisle: ['', Validators.maxLength(100)],
    rack: ['', Validators.maxLength(100)],
    level: ['', Validators.maxLength(100)],
    bin: ['', Validators.maxLength(100)],
    type: [1],
    maximumQuantity: [null as number | null, Validators.min(0)],
    isReceivable: [false],
    isPickable: [true],
    isReservable: [true],
    isShippable: [false],
    isCountable: [true],
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.markDirty());
  }

  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved')); }
  protected load(): void {
    const current = this.filter.getRawValue();
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.api.getLocations(current.warehouseId || undefined, current.query).subscribe({
      next: page => { this.page.set(page); this.loading.set(false); },
      error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
  protected hasFilters(): boolean { const value = this.filter.getRawValue(); return !!value.warehouseId || !!value.query.trim(); }
  protected clearFilters(): void { this.filter.reset({ warehouseId: 0, query: '' }); this.load(); }
  protected emptyTitle(): string { return this.i18n.text(this.hasFilters() ? 'filteredLocationsEmpty' : 'noLocations'); }
  protected emptyDescription(): string { return this.i18n.text(this.hasFilters() ? 'filteredLocationsEmptyDescription' : 'noLocationsDescription'); }
  protected selectedWarehouseName(): string {
    const warehouseId = this.filter.controls.warehouseId.value;
    return this.page()?.options.warehouses.find(item => item.id === warehouseId)?.name ?? this.i18n.text('allWarehouses');
  }
  protected reset(): void {
    this.editing.set(false);
    this.editParents.set([]);
    this.form.reset({
      id: 0, warehouseId: 0, code: '', name: '', barcode: '', parentLocationId: 0, sequence: 0,
      zone: '', aisle: '', rack: '', level: '', bin: '', type: 1, maximumQuantity: null,
      isReceivable: false, isPickable: true, isReservable: true, isShippable: false, isCountable: true,
    });
    this.form.markAsPristine();
    this.markDirty();
  }
  protected beginEdit(id: number): void {
    this.problem.set('');
    this.api.getLocationForm(id).subscribe({
      next: page => {
        const location = page.input;
        this.editing.set(true);
        this.editParents.set(page.parentLocations);
        this.form.reset({
          ...location,
          barcode: location.barcode ?? '',
          parentLocationId: location.parentLocationId ?? 0,
          zone: '', aisle: '', rack: '', level: '', bin: '', type: 1, maximumQuantity: null,
        });
        this.form.markAsPristine();
        this.markDirty();
      },
      error: error => this.showError(error),
    });
  }
  protected parents(): readonly { id: number; warehouseId: number; label?: string }[] {
    const warehouse = this.form.controls.warehouseId.value;
    return (this.editing() ? this.editParents() : (this.page()?.options.parentLocations ?? [])).filter(item => item.warehouseId === warehouse);
  }
  protected save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('fixFields')); return; }
    const value = this.form.getRawValue();
    const common = {
      warehouseId: value.warehouseId,
      code: value.code.trim(),
      name: value.name.trim(),
      barcode: value.barcode.trim() || undefined,
      parentLocationId: value.parentLocationId || undefined,
      sequence: value.sequence,
      isReceivable: value.isReceivable,
      isPickable: value.isPickable,
      isReservable: value.isReservable,
      isShippable: value.isShippable,
      isCountable: value.isCountable,
    };
    const request = this.editing()
      ? this.api.updateLocation({ ...common, id: value.id } as EditLocationCommand)
      : this.api.createLocation({
        ...common,
        zone: value.zone.trim() || undefined,
        aisle: value.aisle.trim() || undefined,
        rack: value.rack.trim() || undefined,
        level: value.level.trim() || undefined,
        bin: value.bin.trim() || undefined,
        type: value.type,
        maximumQuantity: value.maximumQuantity ?? undefined,
      });
    this.saving.set(true);
    this.problem.set('');
    request.subscribe({
      next: () => { this.saving.set(false); this.success.set(this.i18n.text('locationSaved')); this.reset(); this.load(); },
      error: error => { this.saving.set(false); this.showError(error); },
    });
  }
  protected depth(id: number): number { return this.page()?.result.locationDepths[id] ?? 0; }
  protected warehouseName(warehouseId: number): string { return this.page()?.options.warehouses.find(item => item.id === warehouseId)?.name ?? '—'; }
  protected capabilities(location: { isReceivable: boolean; isPickable: boolean; isReservable: boolean; isShippable: boolean; isCountable: boolean }): string {
    return [
      ['receivable', location.isReceivable], ['pickable', location.isPickable], ['reservable', location.isReservable],
      ['shippable', location.isShippable], ['countable', location.isCountable],
    ].filter(([, enabled]) => enabled).map(([key]) => this.i18n.text(key as any)).join(' · ');
  }
  private showError(error: unknown): void { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('error')); }
  private markDirty(): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, this.form.dirty); }
}