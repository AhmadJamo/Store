import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
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
    DataTableComponent,
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
  protected readonly loadingProblem = signal<any>(null);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly editing = signal(false);
  protected readonly editParents = signal<{ id: number; warehouseId: number; label?: string }[]>(
    [],
  );
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
    this.form.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    const f = this.filter.getRawValue();
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.api.getLocations(f.warehouseId || undefined, f.query).subscribe({
      next: (p) => {
        this.page.set(p);
        this.loading.set(false);
      },
      error: (e) => {
        this.loadingProblem.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  protected reset() {
    this.editing.set(false);
    this.editParents.set([]);
    this.form.reset({
      id: 0,
      warehouseId: 0,
      code: '',
      name: '',
      barcode: '',
      parentLocationId: 0,
      sequence: 0,
      zone: '',
      aisle: '',
      rack: '',
      level: '',
      bin: '',
      type: 1,
      maximumQuantity: null,
      isReceivable: false,
      isPickable: true,
      isReservable: true,
      isShippable: false,
      isCountable: true,
    });
    this.form.markAsPristine();
    this.markDirty();
  }
  protected beginEdit(id: number) {
    this.problem.set('');
    this.api.getLocationForm(id).subscribe({
      next: (p) => {
        const x = p.input;
        this.editing.set(true);
        this.editParents.set(p.parentLocations);
        this.form.reset({
          ...x,
          barcode: x.barcode ?? '',
          parentLocationId: x.parentLocationId ?? 0,
          zone: '',
          aisle: '',
          rack: '',
          level: '',
          bin: '',
          type: 1,
          maximumQuantity: null,
        });
        this.form.markAsPristine();
        this.markDirty();
      },
      error: (e) => this.showError(e),
    });
  }
  protected parents() {
    const warehouse = this.form.controls.warehouseId.value;
    return (
      this.editing() ? this.editParents() : (this.page()?.options.parentLocations ?? [])
    ).filter((x) => x.warehouseId === warehouse);
  }
  protected save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.problem.set(this.i18n.text('fixFields'));
      return;
    }
    const v = this.form.getRawValue();
    const common = {
      warehouseId: v.warehouseId,
      code: v.code.trim(),
      name: v.name.trim(),
      barcode: v.barcode.trim() || undefined,
      parentLocationId: v.parentLocationId || undefined,
      sequence: v.sequence,
      isReceivable: v.isReceivable,
      isPickable: v.isPickable,
      isReservable: v.isReservable,
      isShippable: v.isShippable,
      isCountable: v.isCountable,
    };
    const request = this.editing()
      ? this.api.updateLocation({ ...common, id: v.id } as EditLocationCommand)
      : this.api.createLocation({
          ...common,
          zone: v.zone.trim() || undefined,
          aisle: v.aisle.trim() || undefined,
          rack: v.rack.trim() || undefined,
          level: v.level.trim() || undefined,
          bin: v.bin.trim() || undefined,
          type: v.type,
          maximumQuantity: v.maximumQuantity ?? undefined,
        });
    this.saving.set(true);
    this.problem.set('');
    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(this.i18n.text('locationSaved'));
        this.reset();
        this.load();
      },
      error: (e) => {
        this.saving.set(false);
        this.showError(e);
      },
    });
  }
  protected depth(id: number) {
    return this.page()?.result.locationDepths[id] ?? 0;
  }
  protected capabilities(x: {
    isReceivable: boolean;
    isPickable: boolean;
    isReservable: boolean;
    isShippable: boolean;
    isCountable: boolean;
  }) {
    return [
      ['receivable', x.isReceivable],
      ['pickable', x.isPickable],
      ['reservable', x.isReservable],
      ['shippable', x.isShippable],
      ['countable', x.isCountable],
    ]
      .filter(([, on]) => on)
      .map(([key]) => this.i18n.text(key as any))
      .join(' · ');
  }
  private showError(error: unknown) {
    const failure = this.errors.normalize(error);
    this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('error'));
  }
  private markDirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
