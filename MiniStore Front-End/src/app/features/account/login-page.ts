import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountApiService } from '../../core/api/account-api.service';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly account = inject(AccountApiService);
  private readonly session = inject(ApiSessionService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly submitting = signal(false);
  protected readonly problem = signal<string | null>(null);
  protected readonly form = new FormGroup({
    companyCode: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(128)] }),
    userName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(256)] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(1024)] }),
  });

  protected submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitting.set(true);
    this.problem.set(null);
    this.account.login({
      userName: value.userName.trim(),
      password: value.password,
      companyCode: value.companyCode.trim() || null,
    }).subscribe({
      next: result => void this.completeLogin(result.requiresCompanyOnboarding),
      error: error => {
        const normalized = this.errors.normalize(error);
        this.problem.set(normalized.detail ?? normalized.title ?? this.i18n.text('signInFailed'));
        this.form.controls.password.reset('');
        this.submitting.set(false);
      },
    });
  }

  private async completeLogin(requiresCompanyOnboarding: boolean): Promise<void> {
    try {
      await this.session.load();
      this.form.controls.password.reset('');
      await this.router.navigateByUrl(requiresCompanyOnboarding ? '/onboarding' : this.safeReturnUrl());
    } catch (error) {
      const normalized = this.errors.normalize(error);
      this.problem.set(normalized.detail ?? normalized.title ?? this.i18n.text('signInFailed'));
      this.form.controls.password.reset('');
      this.submitting.set(false);
    }
  }

  private safeReturnUrl(): string {
    const value = this.route.snapshot.queryParamMap.get('returnUrl');
    return value?.startsWith('/') && !value.startsWith('//') && !value.startsWith('/login') ? value : '/';
  }
}
