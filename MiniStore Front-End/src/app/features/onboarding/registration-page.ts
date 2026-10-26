import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { OnboardingLocalizationService } from '../../core/localization/onboarding-localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { OnboardingApiService } from './onboarding-api.service';
import { PublicPlan } from './onboarding.models';

@Component({
  selector: 'app-registration-page',
  imports: [ReactiveFormsModule, RouterLink, ApiRecoveryComponent, FeedbackStateComponent],
  templateUrl: './registration-page.html',
  styleUrl: './onboarding-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegistrationPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly copy = inject(OnboardingLocalizationService);
  private readonly api = inject(OnboardingApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly session = inject(ApiSessionService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly plans = signal<readonly PublicPlan[]>([]);
  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly showRules = signal(false);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    companyName: ['', [Validators.required, Validators.maxLength(200)]],
    companySlug: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(63), Validators.pattern(/^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$/)]],
    ownerUsername: ['', [Validators.required, Validators.maxLength(100)]],
    ownerEmail: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(1024)]],
    planId: [0, [Validators.required, Validators.min(1)]],
  });

  constructor() { this.load(); }

  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getRegistrationPage().subscribe({
      next: page => {
        this.plans.set(page.plans);
        const requested = Number(this.route.snapshot.queryParamMap.get('planId'));
        const selected = page.plans.some(plan => plan.id === requested) ? requested : page.plans[0]?.id ?? 0;
        this.form.controls.planId.setValue(selected);
        this.form.markAsPristine();
        this.loading.set(false);
      },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected submit(): void {
    if (this.form.invalid || this.submitting()) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();
    this.submitting.set(true);
    this.problem.set(null);
    this.api.register({
      ...value,
      companyName: value.companyName.trim(),
      companySlug: value.companySlug.trim().toLowerCase(),
      ownerUsername: value.ownerUsername.trim(),
      ownerEmail: value.ownerEmail.trim(),
    }).subscribe({
      next: () => void this.finishRegistration(),
      error: error => {
        this.problem.set(this.errors.normalize(error));
        this.form.controls.password.reset('');
        this.submitting.set(false);
      },
    });
  }

  protected planName(plan: PublicPlan): string {
    return this.i18n.locale() === 'ar-JO' ? plan.nameArabic : plan.nameEnglish;
  }

  protected planDescription(plan: PublicPlan): string {
    return this.i18n.locale() === 'ar-JO' ? plan.descriptionArabic : plan.descriptionEnglish;
  }

  protected selectedPlan(): PublicPlan | undefined {
    return this.plans().find(plan => plan.id === Number(this.form.controls.planId.value));
  }

  private async finishRegistration(): Promise<void> {
    try {
      await this.session.load();
      this.form.controls.password.reset('');
      this.form.markAsPristine();
      await this.router.navigateByUrl('/onboarding');
    } catch (error) {
      this.problem.set(this.errors.normalize(error));
      this.form.controls.password.reset('');
      this.submitting.set(false);
    }
  }
}
