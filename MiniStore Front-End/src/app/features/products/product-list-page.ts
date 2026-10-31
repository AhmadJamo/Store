import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PagerComponent } from '../../shared/ui/pager/pager';
import { ProductApiService } from './data/product-api.service';
import { ProductPage } from './data/product.models';
import { ProductLocalizationService } from './product-localization.service';

@Component({ selector: 'app-product-list-page', imports: [RouterLink, PageHeaderComponent, ApiRecoveryComponent, DataTableComponent, FeedbackStateComponent, PagerComponent], templateUrl: './product-list-page.html', styleUrl: './product-list-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductListPageComponent {
  protected readonly i18n = inject(ProductLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(ProductApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly changes = new Subject<string>();
  protected readonly page = signal<ProductPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected query = { page: 1, search: '', isActive: undefined as boolean | undefined };

  constructor() {
    this.changes.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef)).subscribe(search => { this.query = { ...this.query, search, page: 1 }; this.load(); });
    this.load();
  }
  protected search(value: string): void { this.changes.next(value.trim()); }
  protected status(value: string): void { this.query = { ...this.query, isActive: value === '' ? undefined : value === 'true', page: 1 }; this.load(); }
  protected move(page: number): void { this.query = { ...this.query, page }; this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getPage(this.query).subscribe({ next: value => { this.page.set(value); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
}
