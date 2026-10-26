import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SettingsApiService } from './data/settings-api.service';
import { WorkflowCatalogPage, WorkflowCatalogStep } from './data/settings.models';

@Component({ selector: 'app-workflow-catalog-page', imports: [RouterLink, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './workflow-catalog-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class WorkflowCatalogPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly page = signal<WorkflowCatalogPage | null>(null);
  constructor() { this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getWorkflowCatalog().subscribe({ next: page => { this.page.set(page); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected modules(steps: WorkflowCatalogStep[]): string[] { return [...new Set(steps.map(step => step.module))]; }
  protected phases(steps: WorkflowCatalogStep[], module: string): string[] { return [...new Set(steps.filter(step => step.module === module).map(step => step.phase))]; }
  protected phaseSteps(steps: WorkflowCatalogStep[], module: string, phase: string): WorkflowCatalogStep[] { return steps.filter(step => step.module === module && step.phase === phase); }
}
