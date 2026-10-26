import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { OnboardingLocalizationService } from '../../core/localization/onboarding-localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { OnboardingApiService } from './onboarding-api.service';
import { BusinessTypeOption } from './onboarding.models';

@Component({
  selector: 'app-company-onboarding-page',
  imports: [ReactiveFormsModule, ApiRecoveryComponent, FeedbackStateComponent],
  templateUrl: './company-onboarding-page.html',
  styleUrl: './onboarding-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyOnboardingPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly copy = inject(OnboardingLocalizationService);
  private readonly api = inject(OnboardingApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly session = inject(ApiSessionService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly tenantName = signal('');
  protected readonly businessTypes = signal<readonly BusinessTypeOption[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly skipping = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    businessType: [1, [Validators.required, Validators.min(1)]],
    countryCode: ['JO', Validators.required],
    currency: ['JOD', [Validators.required, Validators.pattern(/^[A-Z]{3}$/)]],
    language: [this.i18n.locale() === 'ar-JO' ? 2 : 1, Validators.required],
    fiscalYearStartMonth: [1, [Validators.required, Validators.min(1), Validators.max(12)]],
    isTaxRegistered: [false],
    defaultTaxRate: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    usePos: [true],
    inventoryControlMode: [3, Validators.required],
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => undefined);
  }

  canDeactivate(): boolean {
    return !this.form.dirty || this.saving() || this.skipping() || globalThis.confirm(this.i18n.text('unsavedDescription'));
  }

  protected load(): void {
    this.loading.set(true);
    this.problem.set(null);
    this.api.getCompanyOnboarding().subscribe({
      next: page => {
        if (page.status !== 1) { void this.router.navigateByUrl('/'); return; }
        this.tenantName.set(page.tenantName);
        this.businessTypes.set(page.businessTypes.filter(option => option.value !== 8));
        this.form.markAsPristine();
        this.loading.set(false);
      },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected chooseBusiness(option: BusinessTypeOption): void {
    this.form.controls.businessType.setValue(option.value);
    this.form.controls.usePos.setValue(option.recommendedPos);
  }

  protected optionName(option: BusinessTypeOption): string {
    return this.i18n.locale() === 'ar-JO' ? option.nameArabic : option.nameEnglish;
  }

  protected optionDescription(option: BusinessTypeOption): string {
    return this.i18n.locale() === 'ar-JO' ? option.descriptionArabic : option.descriptionEnglish;
  }

  protected monthName(month: number): string {
    return new Intl.DateTimeFormat(this.i18n.locale(), { month: 'long', timeZone: 'UTC' }).format(new Date(Date.UTC(2026, month - 1, 1)));
  }

  protected complete(): void {
    if (this.form.invalid || this.saving() || this.skipping()) { this.form.markAllAsTouched(); return; }
    this.saving.set(true);
    this.problem.set(null);
    const request = this.form.getRawValue();
    if (!request.isTaxRegistered) request.defaultTaxRate = 0;
    this.api.complete(request).subscribe({
      next: () => void this.finish(),
      error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); },
    });
  }

  protected skip(): void {
    if (this.saving() || this.skipping() || !globalThis.confirm(this.copy.text('confirmSkipSetup'))) return;
    this.skipping.set(true);
    this.problem.set(null);
    this.api.skip().subscribe({
      next: () => void this.finish(),
      error: error => { this.problem.set(this.errors.normalize(error)); this.skipping.set(false); },
    });
  }

  private async finish(): Promise<void> {
    try {
      this.form.markAsPristine();
      await this.session.load();
      await this.router.navigateByUrl('/');
    } catch (error) {
      this.problem.set(this.errors.normalize(error));
      this.saving.set(false);
      this.skipping.set(false);
    }
  }
}
