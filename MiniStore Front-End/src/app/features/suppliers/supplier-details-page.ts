import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SupplierApiService } from './data/supplier-api.service';
import { SupplierDetails } from './data/supplier.models';

@Component({ selector: 'app-supplier-details-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent], templateUrl: './supplier-details-page.html', styleUrl: './supplier-details-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class SupplierDetailsPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(SupplierApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly supplier = signal<SupplierDetails | null>(null);
  protected readonly problem = signal('');
  constructor() { this.api.get(Number(this.route.snapshot.paramMap.get('id'))).subscribe({ next: (supplier) => this.supplier.set(supplier), error: (error) => this.problem.set(this.errors.normalize(error).title ?? this.i18n.text('errorDescription')) }); }
}
