import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiErrorService } from '../../core/api/api-error.service'; import { NormalizedApiProblem } from '../../core/api/api.models'; import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery'; import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state'; import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { AnalyticsApiService } from './data/analytics-api.service'; import { AnalyticsPage, AnalyticsWidget } from './data/analytics.models';
@Component({ selector: 'app-analytics-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent], templateUrl: './analytics-page.html', styleUrl: './analytics-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class AnalyticsPageComponent {
  protected readonly i18n = inject(LocalizationService); private readonly api = inject(AnalyticsApiService); private readonly errors = inject(ApiErrorService);
  protected readonly session = inject(ApiSessionService);
  protected readonly page = signal<AnalyticsPage | null>(null); protected readonly loading = signal(true); protected readonly problem = signal<NormalizedApiProblem | null>(null); protected readonly exportWidget = signal<AnalyticsWidget | null>(null);
  protected query: { workspaceId?: number; moduleKey?: string; from?: string; to?: string; dimension?: string; comparisonMode?: number } = { comparisonMode: 1 };
  constructor() { const workspaceId = Number(inject(ActivatedRoute).snapshot.queryParamMap.get('workspaceId')); if (Number.isInteger(workspaceId) && workspaceId > 0) this.query.workspaceId = workspaceId; this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.get(this.query).subscribe({ next: page => { this.page.set(page); this.query = { ...this.query, moduleKey: page.query.moduleKey, from: page.query.from, to: page.query.to, dimension: page.query.dimension }; this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected set(name: 'moduleKey'|'from'|'to'|'dimension'|'comparisonMode', value: string): void { this.query = { ...this.query, workspaceId: undefined, [name]: name === 'comparisonMode' ? Number(value) : value }; this.load(); }
  protected title(widget: AnalyticsWidget): string { return this.i18n.locale() === 'ar-JO' ? widget.titleArabic : widget.title; }
  protected drillRoute(controller?: string, id?: number): string | null { if (!id) return null; return controller === 'Customers' ? `/customers/${id}` : controller === 'Suppliers' ? `/suppliers/${id}` : null; }
}
