import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent, DataTableSort } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { FilterBarComponent } from '../../shared/ui/filter-bar/filter-bar';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PagerComponent } from '../../shared/ui/pager/pager';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { StockTransactionApiService } from './data/stock-transaction-api.service';
import { StockMovementFilter, StockTransactionPage, StockTransactionQuery, StockTransactionSort } from './data/stock-transaction.models';
import { InventoryLocalizationService, InventoryMessageKey } from './inventory-localization.service';

@Component({
  selector: 'app-stock-transactions-page',
  imports: [DatePipe, DecimalPipe, PageHeaderComponent, FilterBarComponent, SummaryStripComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent, DataTableCellDirective, PagerComponent],
  templateUrl: './stock-transactions-page.html',
  styleUrl: './stock-transactions-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockTransactionsPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService);
  private readonly api = inject(StockTransactionApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly searchChanges = new Subject<string>();

  protected readonly page = signal<StockTransactionPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly tableColumns = computed<readonly DataTableColumn[]>(() => [
    { key: 'id', header: this.i18n.text('id'), kind: 'code', sortable: true, width: '6rem' },
    { key: 'productName', header: this.i18n.text('product'), sortable: true, width: '18%' },
    { key: 'warehouseName', header: this.i18n.text('warehouse'), sortable: true, width: '14%' },
    { key: 'quantity', header: this.i18n.text('quantity'), kind: 'number', sortable: true, width: '8rem' },
    { key: 'unitCost', header: this.i18n.text('unitCost'), kind: 'number', width: '9rem' },
    { key: 'transactionValue', header: this.i18n.text('movementValue'), kind: 'number', width: '10rem' },
    { key: 'costVariance', header: this.i18n.text('costVariance'), kind: 'number', width: '9rem' },
    { key: 'type', header: this.i18n.text('type'), kind: 'status', sortable: true, width: '11rem' },
    { key: 'reference', header: this.i18n.text('reference'), sortable: true, width: '11rem' },
    { key: 'createdAt', header: this.i18n.text('date'), kind: 'date', sortable: true, width: '12rem' },
  ]);
  protected tableSort(): DataTableSort { return { key: sortKeys[this.query.sortColumn], direction: this.query.sortDescending ? 'desc' : 'asc' }; }
  protected query: StockTransactionQuery = { page: 1, pageSize: 50, sortColumn: 'date', sortDescending: true, movement: '' };
  protected readonly types = Array.from({ length: 18 }, (_, index) => index + 1);

  constructor() {
    this.searchChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(search => { this.query = { ...this.query, search, page: 1 }; this.load(); });
    this.load();
  }

  protected search(value: string): void { this.searchChanges.next(value.trim()); }
  protected filterType(value: string): void { this.query = { ...this.query, type: value ? Number(value) : undefined, page: 1 }; this.load(); }
  protected filterMovement(value: string): void { this.query = { ...this.query, movement: value as StockMovementFilter, page: 1 }; this.load(); }
  protected filterFrom(value: string): void { this.query = { ...this.query, fromDate: value || undefined, page: 1 }; this.load(); }
  protected filterTo(value: string): void { this.query = { ...this.query, toDate: value || undefined, page: 1 }; this.load(); }
  protected resize(value: string): void { this.query = { ...this.query, pageSize: Number(value) as StockTransactionQuery['pageSize'], page: 1 }; this.load(); }
  protected hasFilters(): boolean { return !!this.query.search || this.query.type !== undefined || !!this.query.movement || !!this.query.fromDate || !!this.query.toDate; }
  protected clearFilters(): void { this.query = { page: 1, pageSize: 50, sortColumn: 'date', sortDescending: true, movement: '' }; this.load(); }
  protected sort(sortColumn: StockTransactionSort): void { this.query = { ...this.query, sortDescending: this.query.sortColumn === sortColumn ? !this.query.sortDescending : false, sortColumn, page: 1 }; this.load(); }
  protected changeSort(sort: DataTableSort): void { this.query = { ...this.query, sortColumn: columnSortKeys[sort.key] ?? 'date', sortDescending: sort.direction === 'desc', page: 1 }; this.load(); }
  protected move(page: number): void { if (page >= 1 && page <= (this.page()?.totalPages || 1)) { this.query = { ...this.query, page }; this.load(); } }
  protected sortLabel(column: StockTransactionSort): string { return this.query.sortColumn === column ? this.i18n.text(this.query.sortDescending ? 'descending' : 'ascending') : ''; }
  protected typeName(type: number): string { return this.i18n.text(typeKeys[type] ?? 'openingBalance'); }
  protected selectedMovementName(): string { return this.i18n.text(this.query.movement === 'incoming' ? 'incoming' : this.query.movement === 'outgoing' ? 'outgoing' : 'allMovements'); }
  protected emptyTitle(): string { return this.i18n.text(this.hasFilters() ? 'filteredTransactionsEmpty' : 'noTransactions'); }
  protected emptyDescription(): string { return this.i18n.text(this.hasFilters() ? 'filteredTransactionsEmptyDescription' : 'noTransactionsDescription'); }

  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getPage(this.query).subscribe({
      next: page => { this.page.set(page); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
}

const sortKeys: Record<StockTransactionSort, string> = {
  id: 'id', product: 'productName', warehouse: 'warehouseName', quantity: 'quantity', type: 'type', reference: 'reference', date: 'createdAt',
};

const columnSortKeys: Record<string, StockTransactionSort> = {
  id: 'id', productName: 'product', warehouseName: 'warehouse', quantity: 'quantity', type: 'type', reference: 'reference', createdAt: 'date',
};

const typeKeys: Record<number, InventoryMessageKey> = {
  1: 'openingBalance', 2: 'purchase', 3: 'sale', 4: 'transferIn', 5: 'transferOut', 6: 'adjustmentIn', 7: 'adjustmentOut',
  8: 'kitchenVariance', 9: 'recipeConsumption', 10: 'salesReturn', 11: 'purchaseReturn', 12: 'goodsReceipt', 13: 'goodsReceiptReturn',
  14: 'landedCostAdjustment', 15: 'supplierCreditNoteReturn', 16: 'goodsReceiptReversal', 17: 'landedCostReversal', 18: 'customerReturnDisposition',
};
