import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentRow } from './data/inventory-adjustment.models';
import { InventoryLocalizationService, InventoryMessageKey } from './inventory-localization.service';

@Component({ selector: 'app-inventory-adjustments-page', imports: [DatePipe, RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent], templateUrl: './inventory-adjustments-page.html', styleUrl: './inventory-adjustments.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class InventoryAdjustmentsPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService); protected readonly session = inject(ApiSessionService); private readonly api = inject(InventoryAdjustmentApiService); private readonly errors = inject(ApiErrorService);
  protected readonly items = signal<InventoryAdjustmentRow[]>([]); protected readonly loading = signal(true); protected readonly problem = signal<NormalizedApiProblem | null>(null);
  constructor() { this.load(); }
  protected statusName(status: number): string { return this.i18n.text(({ 1: 'adjustmentDraft', 2: 'adjustmentCounted', 3: 'adjustmentApproved', 4: 'adjustmentPosted', 5: 'adjustmentCancelled' } as Record<number, InventoryMessageKey>)[status] || 'adjustmentDraft'); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getAll().subscribe({ next: items => { this.items.set(items); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
}
