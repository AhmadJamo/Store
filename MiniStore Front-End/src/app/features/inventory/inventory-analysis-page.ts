import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { FilterBarComponent } from '../../shared/ui/filter-bar/filter-bar';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { InventoryAnalysisApiService } from './data/inventory-analysis-api.service';
import {
  InventoryActivityState,
  InventoryInsightsPage,
  InventoryReconciliationIssue,
  InventoryReconciliationPage,
} from './data/inventory-analysis.models';
import {
  InventoryAnalysisLocalizationService,
  InventoryAnalysisMessageKey,
} from './inventory-analysis-localization.service';

@Component({
  selector: 'app-inventory-analysis-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    PageHeaderComponent,
    FilterBarComponent,
    SummaryStripComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
    DataTableComponent,
    DataTableCellDirective,
  ],
  templateUrl: './inventory-analysis-page.html',
  styleUrl: './inventory-analysis-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryAnalysisPageComponent {
  protected readonly i18n = inject(InventoryAnalysisLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(InventoryAnalysisApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  protected readonly isReconciliation = this.route.snapshot.routeConfig?.path?.endsWith('reconciliation') ?? false;
  protected readonly insights = signal<InventoryInsightsPage | null>(null);
  protected readonly reconciliation = signal<InventoryReconciliationPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly insightsColumns = computed<readonly DataTableColumn[]>(() => [
    { key: 'productName', header: this.i18n.text('product'), width: '24%' },
    { key: 'warehouseName', header: this.i18n.text('warehouse'), width: '17%' },
    { key: 'onHand', header: this.i18n.text('onHand'), kind: 'number', width: '9rem' },
    { key: 'inventoryValue', header: this.i18n.text('inventoryValue'), kind: 'number', width: '10rem' },
    { key: 'lastOutboundAt', header: this.i18n.text('lastOutbound'), kind: 'date', width: '10rem' },
    { key: 'daysSinceActivity', header: this.i18n.text('daysSinceActivity'), kind: 'number', width: '9rem' },
    { key: 'state', header: this.i18n.text('status'), kind: 'status', width: '10rem' },
  ]);

  protected readonly insightsForm = new FormGroup({
    warehouseId: new FormControl<number | null>(null),
    search: new FormControl('', { nonNullable: true }),
    state: new FormControl<InventoryActivityState | null>(null),
    attentionOnly: new FormControl(false, { nonNullable: true }),
    slowDays: new FormControl(30, { nonNullable: true, validators: [Validators.min(1), Validators.max(3649)] }),
    deadDays: new FormControl(90, { nonNullable: true, validators: [Validators.min(2), Validators.max(3650)] }),
  });
  protected readonly reconciliationForm = new FormGroup({
    warehouseId: new FormControl<number | null>(null),
    search: new FormControl('', { nonNullable: true }),
    exceptionsOnly: new FormControl(true, { nonNullable: true }),
    pageSize: new FormControl(50, { nonNullable: true }),
  });

  constructor() { this.load(); }

  protected insightsHasFilters(): boolean {
    const value = this.insightsForm.getRawValue();
    return !!value.search.trim() || value.warehouseId !== null || value.state !== null || value.attentionOnly;
  }

  protected insightsEmptyTitle(): string { return this.i18n.text(this.insightsHasFilters() ? 'filteredInsightsEmpty' : 'noInsights'); }
  protected insightsEmptyDescription(): string { return this.i18n.text(this.insightsHasFilters() ? 'filteredInsightsEmptyDescription' : 'noInsightsDescription'); }

  protected load(page = 1): void {
    this.loading.set(true);
    this.problem.set(null);
    if (this.isReconciliation) {
      const query = { ...this.reconciliationForm.getRawValue(), page };
      this.api.getReconciliation(query).subscribe({
        next: result => {
          this.reconciliation.set(result);
          this.reconciliationForm.setValue({
            warehouseId: result.warehouseId ?? null,
            search: result.search,
            exceptionsOnly: result.exceptionsOnly,
            pageSize: result.pageSize,
          }, { emitEvent: false });
          this.loading.set(false);
        },
        error: error => this.failed(error),
      });
      return;
    }
    if (this.insightsForm.invalid) {
      this.insightsForm.markAllAsTouched();
      this.loading.set(false);
      return;
    }
    this.api.getInsights(this.insightsForm.getRawValue()).subscribe({
      next: result => {
        this.insights.set(result);
        this.insightsForm.setValue({
          warehouseId: result.query.warehouseId ?? null,
          search: result.query.search,
          state: result.query.state ?? null,
          attentionOnly: result.query.attentionOnly,
          slowDays: result.query.slowDays,
          deadDays: result.query.deadDays,
        }, { emitEvent: false });
        this.loading.set(false);
      },
      error: error => this.failed(error),
    });
  }

  protected stateName(state: number): string {
    return this.i18n.text(({
      0: 'active',
      1: 'slow',
      2: 'dead',
      3: 'neverIssued',
      4: 'noMovement',
    } as Record<number, InventoryAnalysisMessageKey>)[state]);
  }

  protected issueName(issue: InventoryReconciliationIssue): string {
    return this.i18n.text(({
      0: 'issueMissingWarehouseStock',
      1: 'issueOverAllocated',
      2: 'issueNegativeLocation',
      3: 'issueSimpleLocation',
      4: 'issueMissingTransaction',
      5: 'issueQuantityMismatch',
      6: 'issueValueMismatch',
    } as Record<number, InventoryAnalysisMessageKey>)[issue]);
  }

  private failed(error: unknown): void {
    this.problem.set(this.errors.normalize(error));
    this.loading.set(false);
  }
}
