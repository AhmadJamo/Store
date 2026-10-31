import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SubscriptionApiService } from './subscription-api.service';
import { SubscriptionLocalizationService } from './subscription-localization.service';
import {
  SubscriptionCheckoutQuote,
  SubscriptionPage,
  SubscriptionPlan,
} from './subscription.models';

@Component({
  selector: 'app-subscription-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './subscription-page.html',
  styleUrl: './subscription-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SubscriptionPageComponent {
  protected readonly i18n = inject(SubscriptionLocalizationService);
  private readonly api = inject(SubscriptionApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<SubscriptionPage | null>(null);
  protected readonly selected = signal<SubscriptionPlan | null>(null);
  protected readonly quote = signal<SubscriptionCheckoutQuote | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly form = this.fb.group({
    planId: this.fb.nonNullable.control(0, [Validators.min(1)]),
    cycle: this.fb.nonNullable.control(1),
    promotionCode: this.fb.nonNullable.control('', [Validators.maxLength(40)]),
  });

  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }

  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }

  protected load() {
    this.loading.set(true);
    this.problem.set(null);
    this.api.get().subscribe({
      next: (page) => {
        this.page.set(page);
        this.loading.set(false);
      },
      error: (error) => {
        this.problem.set(this.errors.normalize(error));
        this.loading.set(false);
      },
    });
  }

  protected select(plan: SubscriptionPlan) {
    this.selected.set(plan);
    this.quote.set(null);
    this.problem.set(null);
    this.form.reset({ planId: plan.id, cycle: 1, promotionCode: '' });
    this.form.markAsPristine();
    this.dirty();
  }

  protected cancel() {
    this.selected.set(null);
    this.form.reset({ planId: 0, cycle: 1, promotionCode: '' });
    this.form.markAsPristine();
    this.dirty();
  }

  protected checkout() {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.problem.set(null);
    this.api
      .checkout({
        planId: value.planId,
        cycle: value.cycle,
        promotionCode: value.promotionCode.trim() || null,
      })
      .subscribe({
        next: (quote) => {
          this.quote.set(quote);
          this.selected.set(null);
          this.form.reset({ planId: 0, cycle: 1, promotionCode: '' });
          this.form.markAsPristine();
          this.saving.set(false);
          this.dirty();
        },
        error: (error) => {
          this.problem.set(this.errors.normalize(error));
          this.saving.set(false);
        },
      });
  }

  protected planName(plan: SubscriptionPlan) {
    return this.i18n.isArabic() ? plan.nameArabic : plan.nameEnglish;
  }

  protected planDescription(plan: SubscriptionPlan) {
    return this.i18n.isArabic() ? plan.descriptionArabic : plan.descriptionEnglish;
  }

  protected quotePlanName(quote: SubscriptionCheckoutQuote) {
    return this.i18n.isArabic() ? quote.planNameArabic : quote.planNameEnglish;
  }

  private dirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
