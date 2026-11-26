import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentRow } from './data/inventory-adjustment.models';
import { InventoryLocalizationService, InventoryMessageKey } from './inventory-localization.service';

@Component({
  selector: 'app-inventory-adjustments-page',
  imports: [DatePipe, RouterLink, PageHeaderComponent, SummaryStripComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent, DataTableCellDirective],
  templateUrl: './inventory-adjustments-page.html',
  styleUrl: './inventory-adjustments.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryAdjustmentsPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(InventoryAdjustmentApiService);
  private readonly errors = inject(ApiErrorService);

  protected readonly items = signal<InventoryAdjustmentRow[]>([]);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly tableColumns = computed<readonly DataTableColumn[]>(() => [
    { key: 'number', header: this.i18n.text('documentNumber'), kind: 'code', width: '12rem' },
    { key: 'warehouse', header: this.i18n.text('warehouse'), width: '15%' },
    { key: 'location', header: this.i18n.text('location'), width: '18%' },
    { key: 'status', header: this.i18n.text('status'), kind: 'status', width: '10rem' },
    { key: 'isBlindCount', header: this.i18n.text('blindCount'), width: '8rem' },
    { key: 'lineCount', header: this.i18n.text('lines'), kind: 'number', width: '7rem' },
    { key: 'createdAt', header: this.i18n.text('createdAt'), kind: 'date', width: '11rem' },
    { key: 'actions', header: this.i18n.text('actions'), kind: 'actions', width: '7rem' },
  ]);
  protected readonly openCount = computed(() => this.items().filter(item => item.status >= 1 && item.status <= 3).length);
  protected readonly postedCount = computed(() => this.items().filter(item => item.status === 4).length);

  constructor() { this.load(); }

  protected statusName(status: number): string {
    return this.i18n.text(({ 1: 'adjustmentDraft', 2: 'adjustmentCounted', 3: 'adjustmentApproved', 4: 'adjustmentPosted', 5: 'adjustmentCancelled' } as Record<number, InventoryMessageKey>)[status] || 'adjustmentDraft');
  }

  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getAll().subscribe({
      next: items => { this.items.set(items); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
}
