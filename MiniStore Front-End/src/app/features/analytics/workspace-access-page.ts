import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { AnalyticsApiService } from './data/analytics-api.service';
import { AnalyticsDelegation, AnalyticsDelegationCommand, AnalyticsPrincipalOption, AnalyticsWorkspaceAccess } from './data/analytics.models';

@Component({
  selector: 'app-workspace-access-page',
  imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent],
  templateUrl: './workspace-access-page.html',
  styleUrl: './workspace-access-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkspaceAccessPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(AnalyticsApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly workspaceId = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
  protected readonly access = signal<AnalyticsWorkspaceAccess | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly principals = computed<readonly AnalyticsPrincipalOption[]>(() => this.targetType === 1 ? this.access()?.users ?? [] : this.access()?.roles ?? []);
  protected targetType = 1;
  protected targetId = '';
  protected startsAt = '';
  protected expiresAt = '';

  constructor() { this.load(); }

  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    this.api.getAccess(this.workspaceId).subscribe({
      next: access => { this.access.set(access); this.ensureTarget(); this.loading.set(false); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); },
    });
  }

  protected setTargetType(value: string): void { this.targetType = Number(value); this.ensureTarget(); }
  protected setTarget(value: string): void { this.targetId = value; }

  protected save(): void {
    const access = this.access();
    if (!access || !this.targetId || this.saving()) return;
    const command: AnalyticsDelegationCommand = {
      targetType: this.targetType,
      targetUserId: this.targetType === 1 ? this.targetId : undefined,
      targetRoleId: this.targetType === 2 ? Number(this.targetId) : undefined,
      startsAtUtc: this.toUtc(this.startsAt), expiresAtUtc: this.toUtc(this.expiresAt),
      rowVersion: access.rowVersion,
    };
    this.saving.set(true); this.problem.set(null);
    this.api.createDelegation(this.workspaceId, command).subscribe({
      next: () => { this.startsAt = ''; this.expiresAt = ''; this.saving.set(false); this.load(); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); },
    });
  }

  protected revoke(delegation: AnalyticsDelegation): void {
    const access = this.access();
    if (!access || delegation.revokedAtUtc || this.saving()) return;
    this.saving.set(true); this.problem.set(null);
    this.api.revokeDelegation(this.workspaceId, delegation.id, access.rowVersion).subscribe({
      next: () => { this.saving.set(false); this.load(); },
      error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); },
    });
  }

  protected targetName(delegation: AnalyticsDelegation): string {
    const id = delegation.targetType === 1 ? delegation.targetUserId : String(delegation.targetRoleId);
    const options = delegation.targetType === 1 ? this.access()?.users : this.access()?.roles;
    return options?.find(x => x.id === id)?.displayName ?? id ?? this.i18n.text('notConfigured');
  }

  protected statusKey(delegation: AnalyticsDelegation): MessageKey {
    if (delegation.revokedAtUtc) return 'delegationRevoked';
    if (delegation.isActive) return 'active';
    if (new Date(delegation.startsAtUtc).getTime() > Date.now()) return 'delegationScheduled';
    return 'delegationExpired';
  }

  private ensureTarget(): void { this.targetId = this.principals()[0]?.id ?? ''; }
  private toUtc(value: string): string | undefined { return value ? new Date(value).toISOString() : undefined; }
}
