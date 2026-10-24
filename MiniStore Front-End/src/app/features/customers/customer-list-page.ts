import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { CustomerApiService } from './data/customer-api.service';
import { CustomerListPage } from './data/customer.models';

@Component({ selector: 'app-customer-list-page', imports: [RouterLink], templateUrl: './customer-list-page.html', styleUrl: './customers.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class CustomerListPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(CustomerApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly searchChanges = new Subject<string>();
  protected readonly page = signal<CustomerListPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal('');
  private query = { page: 1, pageSize: 25, search: '', includeArchived: false, sortBy: 'name', sortDescending: false };
  constructor() {
    this.searchChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((search) => { this.query = { ...this.query, search, page: 1 }; this.load(); });
    this.load();
  }
  protected search(value: string): void { this.searchChanges.next(value.trim()); }
  protected includeArchived(value: boolean): void { this.query = { ...this.query, includeArchived: value, page: 1 }; this.load(); }
  protected sort(sortBy: string): void { this.query = { ...this.query, sortDescending: this.query.sortBy === sortBy ? !this.query.sortDescending : false, sortBy, page: 1 }; this.load(); }
  protected move(page: number): void { if (page >= 1 && page <= (this.page()?.totalPages ?? 1)) { this.query = { ...this.query, page }; this.load(); } }
  protected load(): void {
    this.loading.set(true); this.problem.set('');
    this.api.getPage(this.query).subscribe({
      next: (page) => { this.page.set(page); this.loading.set(false); },
      error: (error) => { const problem = this.errors.normalize(error); this.problem.set(problem.title ?? this.i18n.text('errorDescription')); this.loading.set(false); },
    });
  }
}
