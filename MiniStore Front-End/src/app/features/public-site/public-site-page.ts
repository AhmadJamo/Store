import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PublicPlansApiService } from './public-plans-api.service';
import { PublicSiteLocalizationService } from './public-site-localization.service';
import { PublicPlan } from './public-site.models';

@Component({
  selector: 'app-public-site-page',
  imports: [DecimalPipe, RouterLink, ApiRecoveryComponent, FeedbackStateComponent],
  templateUrl: './public-site-page.html',
  styleUrl: './public-site-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicSitePageComponent {
  protected readonly i18n = inject(PublicSiteLocalizationService);
  private readonly api = inject(PublicPlansApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly pricing = inject(ActivatedRoute).snapshot.data['view'] === 'pricing';
  protected readonly plans = signal<PublicPlan[]>([]);
  protected readonly loading = signal(this.pricing);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);

  constructor() {
    if (this.pricing) this.load();
  }

  protected load() {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getPlans().subscribe({
      next: (plans) => {
        this.plans.set(plans);
        this.loading.set(false);
      },
      error: (error) => {
        this.problem.set(this.errors.normalize(error));
        this.loading.set(false);
      },
    });
  }

  protected name(plan: PublicPlan) {
    return this.i18n.locale.locale() === 'ar-JO' ? plan.nameArabic : plan.nameEnglish;
  }

  protected description(plan: PublicPlan) {
    return this.i18n.locale.locale() === 'ar-JO'
      ? plan.descriptionArabic
      : plan.descriptionEnglish;
  }

  protected limits(plan: PublicPlan) {
    return Object.entries(plan.limits);
  }
}
