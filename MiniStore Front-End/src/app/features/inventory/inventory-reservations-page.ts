import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PagerComponent } from '../../shared/ui/pager/pager';
import { InventoryAvailabilityApiService } from './data/inventory-availability-api.service';
import { InventoryReservationPage, InventoryReservationQuery, InventoryReservationStatus } from './data/inventory-availability.models';
import { InventoryLocalizationService, InventoryMessageKey } from './inventory-localization.service';

@Component({ selector: 'app-inventory-reservations-page', imports: [DatePipe, DecimalPipe, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent, PagerComponent], templateUrl: './inventory-reservations-page.html', styleUrl: './inventory-read-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class InventoryReservationsPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService); private readonly api = inject(InventoryAvailabilityApiService); private readonly errors = inject(ApiErrorService); private readonly destroyRef = inject(DestroyRef); private readonly searchChanges = new Subject<string>();
  protected readonly result = signal<InventoryReservationPage | null>(null); protected readonly loading = signal(true); protected readonly problem = signal<NormalizedApiProblem | null>(null); protected query: InventoryReservationQuery = { page: 1, pageSize: 50 }; protected readonly statuses: InventoryReservationStatus[] = [1, 2, 3];
  constructor() { this.searchChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef)).subscribe(search => { this.query = { ...this.query, search, page: 1 }; this.load(); }); this.load(); }
  protected search(value: string): void { this.searchChanges.next(value.trim()); }
  protected filterStatus(value: string): void { this.query = { ...this.query, status: value ? Number(value) as InventoryReservationStatus : undefined, page: 1 }; this.load(); }
  protected resize(value: string): void { this.query = { ...this.query, pageSize: Number(value) as InventoryReservationQuery['pageSize'], page: 1 }; this.load(); }
  protected clearFilters(): void { this.query = { page: 1, pageSize: 50 }; this.load(); }
  protected move(page: number): void { if (page >= 1 && page <= (this.result()?.totalPages || 1)) { this.query = { ...this.query, page }; this.load(); } }
  protected statusName(status: number): string { return this.i18n.text(({ 1: 'reservationActive', 2: 'reservationConsumed', 3: 'reservationReleased' } as Record<number, InventoryMessageKey>)[status] || 'reservationActive'); }
  protected sourceName(sourceType: number): string { return this.i18n.text(sourceType === 2 ? 'saleOrder' : 'stockTransfer'); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getReservations(this.query).subscribe({ next: result => { this.result.set(result); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
}
