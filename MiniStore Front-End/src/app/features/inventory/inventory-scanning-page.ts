import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { InventoryScanningApiService } from './data/inventory-scanning-api.service';
import {
  InventoryScanPage,
  InventoryScanQuery,
  InventoryScanStatus,
} from './data/inventory-scanning.models';
import {
  InventoryScanningLocalizationService,
  InventoryScanningMessageKey,
} from './inventory-scanning-localization.service';

@Component({
  selector: 'app-inventory-scanning-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    PageHeaderComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
    DataTableComponent,
  ],
  templateUrl: './inventory-scanning-page.html',
  styleUrl: './inventory-scanning-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryScanningPageComponent {
  protected readonly i18n = inject(InventoryScanningLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(InventoryScanningApiService);
  private readonly errors = inject(ApiErrorService);

  protected readonly page = signal<InventoryScanPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly queryForm = new FormGroup({
    adjustmentScan: new FormControl('', { nonNullable: true }),
    productScan: new FormControl('', { nonNullable: true }),
    sourceLocationScan: new FormControl('', { nonNullable: true }),
    locationScan: new FormControl('', { nonNullable: true }),
  });
  protected readonly quantity = new FormControl<number | null>(null, [Validators.required, Validators.min(0.000001)]);
  protected readonly countedQuantity = new FormControl<number | null>(null, [Validators.required, Validators.min(0)]);

  constructor() { this.resolve(); }

  protected statusName(status: InventoryScanStatus): string {
    return this.i18n.text(({
      0: 'waiting',
      1: 'resolved',
      2: 'notFound',
      3: 'ambiguous',
    } as Record<number, InventoryScanningMessageKey>)[status]);
  }

  protected resolve(): void {
    if (this.saving()) return;
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.problem.set('');
    this.success.set('');
    this.api.resolve(this.queryForm.getRawValue()).subscribe({
      next: page => {
        this.page.set(page);
        this.queryForm.setValue(page.query, { emitEvent: false });
        this.loading.set(false);
      },
      error: error => {
        this.loadingProblem.set(this.errors.normalize(error));
        this.loading.set(false);
      },
    });
  }

  protected canPutaway(page: InventoryScanPage): boolean {
    return this.session.can('ProductStock.Edit') &&
      page.adjustment.status === 0 && page.product.status === 1 &&
      page.sourceLocation.status === 0 && page.location.status === 1;
  }

  protected canRelocate(page: InventoryScanPage): boolean {
    return this.session.can('LocationMovements.Create') &&
      page.adjustment.status === 0 && page.product.status === 1 &&
      page.sourceLocation.status === 1 && page.location.status === 1;
  }

  protected canCount(page: InventoryScanPage): boolean {
    return this.session.can('InventoryAdjustments.Count') && page.canRecordCount;
  }

  protected putaway(page: InventoryScanPage): void {
    const quantity = this.positiveQuantity();
    if (quantity === null) return;
    this.execute(
      () => this.api.putaway({
        productScan: page.query.productScan,
        locationScan: page.query.locationScan,
        quantity,
        idempotencyKey: page.putawayIdempotencyKey,
      }),
      'successPutaway');
  }

  protected relocate(page: InventoryScanPage): void {
    const quantity = this.positiveQuantity();
    if (quantity === null) return;
    this.execute(
      () => this.api.relocate({
        productScan: page.query.productScan,
        sourceLocationScan: page.query.sourceLocationScan,
        destinationLocationScan: page.query.locationScan,
        quantity,
        idempotencyKey: page.relocationIdempotencyKey,
      }),
      'successRelocation');
  }

  protected count(page: InventoryScanPage): void {
    if (this.countedQuantity.invalid || this.countedQuantity.value === null) {
      this.countedQuantity.markAsTouched();
      this.problem.set(this.i18n.text('invalidQuantity'));
      return;
    }
    this.execute(
      () => this.api.count({
        adjustmentNumber: page.query.adjustmentScan,
        productScan: page.query.productScan,
        locationScan: page.query.locationScan,
        countedQuantity: this.countedQuantity.value!,
      }),
      'successCount');
  }

  private positiveQuantity(): number | null {
    if (this.quantity.invalid || this.quantity.value === null) {
      this.quantity.markAsTouched();
      this.problem.set(this.i18n.text('invalidQuantity'));
      return null;
    }
    return this.quantity.value;
  }

  private execute(request: () => Observable<void>, message: InventoryScanningMessageKey): void {
    if (this.saving()) return;
    this.problem.set('');
    this.success.set('');
    this.saving.set(true);
    request().subscribe({
      next: () => {
        this.success.set(this.i18n.text(message));
        this.quantity.reset();
        this.countedQuantity.reset();
        this.saving.set(false);
        this.refreshAfterCommand();
      },
      error: error => {
        const failure = this.errors.normalize(error);
        this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('error'));
        this.saving.set(false);
      },
    });
  }

  private refreshAfterCommand(): void {
    const query: InventoryScanQuery = this.queryForm.getRawValue();
    this.loading.set(true);
    this.api.resolve(query).subscribe({
      next: page => {
        this.page.set(page);
        this.queryForm.setValue(page.query, { emitEvent: false });
        this.loading.set(false);
      },
      error: error => {
        this.loadingProblem.set(this.errors.normalize(error));
        this.loading.set(false);
      },
    });
  }
}
