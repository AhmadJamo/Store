import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { StockTransferApiService } from './data/stock-transfer-api.service';
import {
  StockTransferDetails,
  StockTransferFormPage,
  StockTransferInput,
  StockTransferLineInput,
  StockTransferRow,
  StockTransferStatus,
} from './data/stock-transfer.models';
import {
  StockTransferLocalizationService,
  StockTransferMessageKey,
} from './stock-transfer-localization.service';

@Component({
  selector: 'app-stock-transfers-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    PageHeaderComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
    DataTableComponent,
    FormErrorSummaryComponent,
  ],
  templateUrl: './stock-transfers-page.html',
  styleUrl: './stock-transfers-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockTransfersPageComponent {
  protected readonly i18n = inject(StockTransferLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(StockTransferApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly id = Number(this.route.snapshot.paramMap.get('id') || 0);
  protected readonly routePath = this.route.snapshot.routeConfig?.path ?? '';
  protected readonly isForm = this.routePath.endsWith('/new') || this.routePath.endsWith('/edit');
  protected readonly isEdit = this.routePath.endsWith('/edit');
  protected readonly isDetails = !!this.id && !this.isEdit;
  protected readonly rows = signal<StockTransferRow[]>([]);
  protected readonly details = signal<StockTransferDetails | null>(null);
  protected readonly page = signal<StockTransferFormPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly search = new FormControl('', { nonNullable: true });
  protected readonly status = new FormControl<number | null>(null);
  protected readonly form = new FormGroup({
    fromWarehouseId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    toWarehouseId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    reference: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(200)] }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    items: new FormArray<FormGroup>([]),
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.setDirty(this.form.dirty);
    });
    this.form.controls.fromWarehouseId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.clearLocations('sourceLocationId'));
    this.form.controls.toWarehouseId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.clearLocations('destinationLocationId'));
  }

  canDeactivate(): boolean {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }

  protected get items(): FormArray<FormGroup> { return this.form.controls.items; }

  protected statusName(status: number): string {
    return this.i18n.text(({
      1: 'draft',
      2: 'submitted',
      3: 'approved',
      4: 'rejected',
      5: 'posted',
      6: 'cancelled',
    } as Record<number, StockTransferMessageKey>)[status] ?? 'draft');
  }

  protected load(): void {
    this.loading.set(true);
    this.loadingProblem.set(null);
    if (this.isForm) {
      const request = this.isEdit ? this.api.getEditForm(this.id) : this.api.getCreateForm();
      request.subscribe({
        next: page => {
          this.page.set(page);
          this.populate(page.form);
          this.loading.set(false);
        },
        error: error => this.loadFailed(error),
      });
      return;
    }
    if (this.isDetails) {
      this.api.getDetails(this.id).subscribe({
        next: details => {
          this.details.set(details);
          this.loading.set(false);
        },
        error: error => this.loadFailed(error),
      });
      return;
    }
    this.api.getAll(this.search.value, (this.status.value || undefined) as StockTransferStatus | undefined)
      .subscribe({
        next: rows => {
          this.rows.set(rows);
          this.loading.set(false);
        },
        error: error => this.loadFailed(error),
      });
  }

  protected addItem(input?: StockTransferLineInput): void {
    this.items.push(this.lineGroup(input));
    this.form.markAsDirty();
  }

  protected removeItem(index: number): void {
    this.items.removeAt(index);
    this.form.markAsDirty();
  }

  protected sourceLocations() {
    const warehouseId = this.form.controls.fromWarehouseId.value;
    return this.page()?.locations.filter(x => x.warehouseId === warehouseId && x.isPickable) ?? [];
  }

  protected destinationLocations() {
    const warehouseId = this.form.controls.toWarehouseId.value;
    return this.page()?.locations.filter(x => x.warehouseId === warehouseId && x.isReceivable) ?? [];
  }

  protected barcode(productId: number): string {
    return this.page()?.products.find(x => x.id === productId)?.barcode ?? '—';
  }

  protected save(): void {
    if (this.saving()) return;
    this.problem.set('');
    const input = this.toInput();
    if (!this.valid(input)) {
      this.form.markAllAsTouched();
      this.problem.set(this.i18n.text('correctFields'));
      return;
    }
    this.saving.set(true);
    const request: Observable<void | { id: number }> = this.isEdit
      ? this.api.update(this.id, input)
      : this.api.create(input);
    request.subscribe({
      next: (result: void | { id: number }) => {
        this.form.markAsPristine();
        this.setDirty(false);
        this.saving.set(false);
        const targetId = this.isEdit ? this.id : (result as { id: number }).id;
        void this.router.navigate(['/inventory/transfers', targetId]);
      },
      error: (error: unknown) => {
        const failure = this.errors.normalize(error);
        this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('error'));
        this.saving.set(false);
      },
    });
  }

  protected submit(): void { this.execute(() => this.api.submit(this.id)); }
  protected approve(): void { this.execute(() => this.api.approve(this.id)); }
  protected returnToDraft(): void { this.execute(() => this.api.returnToDraft(this.id)); }
  protected post(): void { this.execute(() => this.api.post(this.id)); }

  protected reject(): void {
    const reason = globalThis.prompt(this.i18n.text('reasonPrompt'))?.trim();
    if (reason) this.execute(() => this.api.reject(this.id, reason), false);
  }

  protected cancel(): void {
    const reason = globalThis.prompt(this.i18n.text('reasonPrompt'))?.trim();
    if (reason) this.execute(() => this.api.cancel(this.id, reason), false);
  }

  private execute(request: () => Observable<void>, confirm = true): void {
    if (this.saving() || (confirm && !globalThis.confirm(this.i18n.text('confirmAction')))) return;
    this.problem.set('');
    this.success.set('');
    this.saving.set(true);
    request().subscribe({
      next: () => {
        this.success.set(this.i18n.text('actionCompleted'));
        this.saving.set(false);
        this.load();
      },
      error: error => {
        const failure = this.errors.normalize(error);
        this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('error'));
        this.saving.set(false);
      },
    });
  }

  private populate(input: StockTransferInput): void {
    this.form.patchValue({
      fromWarehouseId: input.fromWarehouseId,
      toWarehouseId: input.toWarehouseId,
      reference: input.reference ?? '',
      notes: input.notes ?? '',
    }, { emitEvent: false });
    this.items.clear({ emitEvent: false });
    for (const item of input.items) this.items.push(this.lineGroup(item), { emitEvent: false });
    if (!input.items.length) this.items.push(this.lineGroup(), { emitEvent: false });
    this.form.markAsPristine();
    this.setDirty(false);
  }

  private lineGroup(input?: StockTransferLineInput): FormGroup {
    return new FormGroup({
      productId: new FormControl(input?.productId ?? 0, { nonNullable: true, validators: [Validators.min(1)] }),
      quantity: new FormControl(input?.quantity ?? 1, { nonNullable: true, validators: [Validators.min(0.001)] }),
      sourceLocationId: new FormControl<number | null>(input?.sourceLocationId ?? null),
      destinationLocationId: new FormControl<number | null>(input?.destinationLocationId ?? null),
      trackingAllocations: new FormControl(input?.trackingAllocations ?? '', { nonNullable: true, validators: [Validators.maxLength(2000)] }),
    });
  }

  private toInput(): StockTransferInput {
    const raw = this.form.getRawValue();
    return {
      fromWarehouseId: raw.fromWarehouseId,
      toWarehouseId: raw.toWarehouseId,
      reference: raw.reference || undefined,
      notes: raw.notes || undefined,
      items: raw.items as StockTransferLineInput[],
    };
  }

  private valid(input: StockTransferInput): boolean {
    if (this.form.invalid || input.fromWarehouseId === input.toWarehouseId || !input.items.length) return false;
    if (new Set(input.items.map(x => x.productId)).size !== input.items.length) return false;
    const source = this.page()?.warehouses.find(x => x.id === input.fromWarehouseId);
    const destination = this.page()?.warehouses.find(x => x.id === input.toWarehouseId);
    return input.items.every(x =>
      x.productId > 0 &&
      x.quantity > 0 &&
      (!source?.requireSourceLocation || !!x.sourceLocationId) &&
      (!destination?.requireDestinationLocation || !!x.destinationLocationId));
  }

  private clearLocations(control: 'sourceLocationId' | 'destinationLocationId'): void {
    for (const item of this.items.controls) item.get(control)?.setValue(null, { emitEvent: false });
  }

  private loadFailed(error: unknown): void {
    this.loadingProblem.set(this.errors.normalize(error));
    this.loading.set(false);
  }

  private setDirty(dirty: boolean): void {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, dirty);
  }
}
