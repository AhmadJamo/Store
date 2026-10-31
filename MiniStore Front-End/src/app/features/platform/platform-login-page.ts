import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PlatformApiService } from './platform-api.service';
import { PlatformLocalizationService } from './platform-localization.service';

@Component({
  selector: 'app-platform-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './platform-login-page.html',
  styleUrl: './platform-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlatformLoginPageComponent {
  protected readonly i18n = inject(PlatformLocalizationService);
  private readonly api = inject(PlatformApiService);
  private readonly router = inject(Router);
  protected readonly submitting = signal(false);
  protected readonly rejected = signal(false);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    username: ['', [Validators.required, Validators.maxLength(100)]],
    password: ['', [Validators.required, Validators.maxLength(1024)]],
  });

  constructor() {
    this.api.session().subscribe({
      next: () => void this.router.navigateByUrl('/platform-control'),
      error: () => undefined,
    });
  }

  protected login() {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    this.rejected.set(false);
    this.api.login(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.controls.password.reset('');
        void this.router.navigateByUrl('/platform-control');
      },
      error: () => {
        this.form.controls.password.reset('');
        this.rejected.set(true);
        this.submitting.set(false);
      },
    });
  }
}
