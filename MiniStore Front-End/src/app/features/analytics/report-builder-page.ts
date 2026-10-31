import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { AnalyticsApiService } from './data/analytics-api.service';
import { AnalyticsWorkspaceBuilder, AnalyticsWorkspaceCommand, AnalyticsWorkspaceWidgetCommand } from './data/analytics.models';

@Component({ selector: 'app-report-builder-page', imports: [RouterLink, PageHeaderComponent, ApiRecoveryComponent, FeedbackStateComponent], templateUrl: './report-builder-page.html', styleUrl: './report-builder-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ReportBuilderPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(AnalyticsApiService); private readonly errors = inject(ApiErrorService); private readonly router = inject(Router);
  protected readonly builder = signal<AnalyticsWorkspaceBuilder | null>(null); protected readonly loading = signal(true); protected readonly saving = signal(false); protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected command: AnalyticsWorkspaceCommand | null = null;
  constructor() { this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getBuilder().subscribe({ next: value => { this.builder.set(value); this.command = structuredClone(value.form); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected moduleChanged(key: string): void { if (this.command) { this.command.moduleKey = key; for (const widget of this.command.widgets) widget.isSelected = widget.moduleKey === key; } }
  protected widgets(): AnalyticsWorkspaceWidgetCommand[] { return this.command?.widgets.filter(widget => widget.moduleKey === this.command?.moduleKey) ?? []; }
  protected save(): void { if (!this.command || !this.command.name.trim() || !this.command.widgets.some(widget => widget.isSelected && widget.moduleKey === this.command?.moduleKey)) return; this.saving.set(true); this.problem.set(null); this.api.createWorkspace(this.command).subscribe({ next: result => void this.router.navigate(['/analytics'], { queryParams: { workspaceId: result.id } }), error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); } }); }
}
