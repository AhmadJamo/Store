import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { ProductApiService } from './data/product-api.service';
import { Product } from './data/product.models';
import { ProductLocalizationService, ProductMessageKey } from './product-localization.service';

@Component({ selector: 'app-product-details-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent], templateUrl: './product-details-page.html', styleUrl: './product-details-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductDetailsPageComponent {
  protected readonly i18n = inject(ProductLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ProductApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id'));
  protected readonly product = signal<Product | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly deleting = signal(false);

  constructor() { this.load(); }
  protected load(): void { this.problem.set(null); this.api.get(this.id).subscribe({ next: product => this.product.set(product), error: error => this.problem.set(this.errors.normalize(error)) }); }
  protected deleteProduct(): void {
    const product = this.product();
    if (!product || this.deleting() || !globalThis.confirm(this.i18n.text('confirmDeleteProduct'))) return;
    this.deleting.set(true); this.problem.set(null);
    this.api.delete(product.id).subscribe({ next: () => { this.deleting.set(false); void this.router.navigate(['/products']); }, error: error => { this.problem.set(this.errors.normalize(error)); this.deleting.set(false); } });
  }
  protected typeName(type: number): string { return this.i18n.text(({ 1: 'rawMaterial', 2: 'directSale', 3: 'preparedToOrder', 4: 'serviceProduct' } as Record<number, ProductMessageKey>)[type] ?? 'none'); }
  protected trackingName(policy: number): string { return this.i18n.text(({ 0: 'noTracking', 1: 'lotTracking', 2: 'serialTracking' } as Record<number, ProductMessageKey>)[policy] ?? 'none'); }
  protected yesNo(value: boolean): string { return this.i18n.text(value ? 'yes' : 'no'); }
}
