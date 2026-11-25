import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DetailSectionComponent } from '../../shared/ui/detail-section/detail-section';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { CustomerApiService } from './data/customer-api.service';
import { CustomerDetails } from './data/customer.models';

@Component({ selector: 'app-customer-details-page', imports: [RouterLink, PageHeaderComponent, DetailSectionComponent, FeedbackStateComponent, ApiRecoveryComponent], templateUrl: './customer-details-page.html', styleUrl: './customers.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class CustomerDetailsPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(CustomerApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id'));
  protected readonly customer = signal<CustomerDetails | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);

  constructor() { this.load(); }

  protected load(): void {
    this.problem.set(null);
    this.api.get(this.id).subscribe({
      next: customer => this.customer.set(customer),
      error: error => this.problem.set(this.errors.normalize(error)),
    });
  }
}
