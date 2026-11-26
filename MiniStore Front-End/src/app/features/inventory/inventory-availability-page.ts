import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { FilterBarComponent } from '../../shared/ui/filter-bar/filter-bar';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PagerComponent } from '../../shared/ui/pager/pager';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { InventoryAvailabilityApiService } from './data/inventory-availability-api.service';
import { InventoryAvailabilityPage, InventoryAvailabilityQuery } from './data/inventory-availability.models';
import { InventoryLocalizationService } from './inventory-localization.service';

@Component({
  selector: 'app-inventory-availability-page',
  imports: [DecimalPipe, PageHeaderComponent, FilterBarComponent, SummaryStripComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent, DataTableCellDirective, PagerComponent],
  templateUrl: './inventory-availability-page.html',
  styleUrl: './inventory-read-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryAvailabilityPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService);
  private readonly api = inject(InventoryAvailabilityApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly searchChanges = new Subject<string>();

  protected readonly result = signal<InventoryAvailabilityPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly tableColumns = computed<readonly DataTableColumn[]>(() => [
    { key: 'productCode', header: this.i18n.text('productCode'), kind: 'code', width: '8rem' },
    { key: 'productName', header: this.i18n.text('product'), width: '24%' },
    { key: 'warehouseName', header: this.i18n.text('warehouse'), width: '16%' },
    { key: 'location', header: this.i18n.text('location'), width: '16%' },
    { key: 'onHand', header: this.i18n.text('onHand'), kind: 'number', width: '8rem' },
    { key: 'reserved', header: this.i18n.text('reserved'), kind: 'number', width: '8rem' },
    { key: 'available', header: this.i18n.text('available'), kind: 'number', width: '8rem' },
  ]);
  protected readonly tableRows = computed(() => (this.result()?.items ?? []).map(row => ({
    ...row,
    rowKey: `${row.productCode}:${row.warehouseId}:${row.locationName}`,
  })));
  protected query: InventoryAvailabilityQuery = { page: 1, pageSize: 50 };

  constructor() {
    this.searchChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(search => { this.query = { ...this.query, search, page: 1 }; this.load(); });
    this.load();
  }

  protected search(value: string): void { this.searchChanges.next(value.trim()); }
  protected filterWarehouse(value: string): void { this.query = { ...this.query, warehouseId: value ? Number(value) : undefined, page: 1 }; this.load(); }
  protected resize(value: string): void { this.query = { ...this.query, pageSize: Number(value) as InventoryAvailabilityQuery['pageSize'], page: 1 }; this.load(); }
  protected hasFilters(): boolean { return !!this.query.search || this.query.warehouseId !== undefined; }
  protected clearFilters(): void { this.query = { page: 1, pageSize: 50 }; this.load(); }
  protected emptyTitle(): string { return this.i18n.text(this.hasFilters() ? 'filteredBalancesEmpty' : 'noBalances'); }
  protected emptyDescription(): string { return this.i18n.text(this.hasFilters() ? 'filteredBalancesEmptyDescription' : 'noBalancesDescription'); }
  protected selectedWarehouseName(): string {
    if (this.query.warehouseId === undefined) return this.i18n.text('allWarehouses');
    return this.result()?.warehouses.find(warehouse => warehouse.id === this.query.warehouseId)?.name ?? this.i18n.text('allWarehouses');
  }
  protected move(page: number): void { if (page >= 1 && page <= (this.result()?.totalPages || 1)) { this.query = { ...this.query, page }; this.load(); } }
  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getBalances(this.query).subscribe({
      next: result => { this.result.set(result); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
}