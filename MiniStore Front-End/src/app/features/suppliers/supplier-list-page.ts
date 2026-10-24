import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PagerComponent } from '../../shared/ui/pager/pager';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { SafeListPreferencesService } from '../../shared/forms/safe-list-preferences.service';
import { SupplierApiService } from './data/supplier-api.service';
import { SupplierListPage } from './data/supplier.models';

@Component({ selector: 'app-supplier-list-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, DataTableComponent, PagerComponent, ApiRecoveryComponent], templateUrl: './supplier-list-page.html', styleUrl: './supplier-list-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class SupplierListPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SupplierApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly preferences = inject(SafeListPreferencesService);
  private readonly searchChanges = new Subject<string>();
  protected readonly result = signal<SupplierListPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected query = { page: 1, search: '', ...this.preferences.loadSuppliers() };

  constructor() {
    this.searchChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((search) => { this.query = { ...this.query, search, page: 1 }; this.load(); });
    this.load();
  }
  protected search(value: string): void { this.searchChanges.next(value.trim()); }
  protected includeInactive(value: boolean): void { this.query = { ...this.query, includeInactive: value, page: 1 }; this.load(); }
  protected saveView(): void { this.preferences.saveSuppliers(this.query); }
  protected resetView(): void { this.preferences.reset('suppliers'); this.query = { page: 1, search: '', ...this.preferences.loadSuppliers() }; this.load(); }
  protected move(page: number): void { if (page >= 1 && page <= (this.result()?.totalPages ?? 1)) { this.query = { ...this.query, page }; this.load(); } }
  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    this.api.getPage(this.query).subscribe({
      next: (result) => { this.result.set(result); this.loading.set(false); },
      error: (error) => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
}
