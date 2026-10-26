import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { IdentityApiService } from './data/identity-api.service';
import { TenantUser } from './data/identity.models';

@Component({ selector: 'app-user-list-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent, DataTableComponent], templateUrl: './user-list-page.html', styleUrl: './identity-pages.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class UserListPageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(IdentityApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly users = signal<readonly TenantUser[]>([]);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  constructor() { this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getUsers().subscribe({ next: users => { this.users.set(users); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
}
