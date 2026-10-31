import { DecimalPipe, KeyValuePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { PlatformApiService } from './platform-api.service';
import { PlatformLocalizationService } from './platform-localization.service';
import { PlatformOverview, PlatformPlan } from './platform.models';

@Component({
  selector: 'app-platform-overview-page',
  imports: [DecimalPipe, KeyValuePipe, RouterLink],
  templateUrl: './platform-overview-page.html',
  styleUrl: './platform-pages.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlatformOverviewPageComponent {
  protected readonly i18n = inject(PlatformLocalizationService);
  private readonly api = inject(PlatformApiService);
  private readonly router = inject(Router);
  protected readonly page = signal<PlatformOverview | null>(null);
  protected readonly loading = signal(true);

  constructor() {
    this.api.overview().subscribe({
      next: (page) => {
        this.page.set(page);
        this.loading.set(false);
      },
      error: () => void this.router.navigateByUrl('/platform-login'),
    });
  }

  protected name(plan: PlatformPlan) {
    return this.i18n.locale.locale() === 'ar-JO' ? plan.nameArabic : plan.nameEnglish;
  }

  protected logout() {
    this.api.logout().subscribe({
      next: () => void this.router.navigateByUrl('/platform-login'),
    });
  }
}
