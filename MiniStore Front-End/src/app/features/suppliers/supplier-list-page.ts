import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
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
import { PageContextService, pageContextFields } from '../../shared/state/page-context.service';

@Component({ selector: 'app-supplier-list-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, DataTableComponent, PagerComponent, ApiRecoveryComponent], templateUrl: './supplier-list-page.html', styleUrl: './supplier-list-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class SupplierListPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SupplierApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly preferences = inject(SafeListPreferencesService);
  private readonly route = inject(ActivatedRoute);
  private readonly context = inject(PageContextService);
  private readonly searchChanges = new Subject<string>();
  protected readonly result = signal<SupplierListPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  private readonly contextSchema = {
    defaults: { page: 1, search: '', ...this.preferences.loadSuppliers() },
    fields: {
      page: pageContextFields.integer(),
      search: pageContextFields.text(),
      includeInactive: pageContextFields.boolean(),
      pageSize: pageContextFields.numberChoice([10, 25, 50, 100] as const),
    },
  };
  protected query = { ...this.contextSchema.defaults };

  constructor() {
    this.searchChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((search) => this.update({ ...this.query, search, page: 1 }));
    this.context.watch('suppliers-list', this.route, this.contextSchema)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(query => { this.query = query; this.load(); });
  }
  protected search(value: string): void { this.searchChanges.next(value.trim()); }
  protected includeInactive(value: boolean): void { this.update({ ...this.query, includeInactive: value, page: 1 }); }
  protected saveView(): void { this.preferences.saveSuppliers(this.query); }
  protected resetView(): void { this.preferences.reset('suppliers'); this.update({ page: 1, search: '', ...this.preferences.loadSuppliers() }); }
  protected move(page: number): void { if (page >= 1 && page <= (this.result()?.totalPages ?? 1)) this.update({ ...this.query, page }); }
  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    this.api.getPage(this.query).subscribe({
      next: (result) => { this.result.set(result); this.loading.set(false); },
      error: (error) => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }
  private update(query: typeof this.query): void { void this.context.replace('suppliers-list', this.route, this.contextSchema, query); }
}
