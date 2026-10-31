import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkCenterApiService } from './data/work-center-api.service';
import { WorkCenterItem } from './data/work-center.models';

@Component({ selector: 'app-work-center-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent], templateUrl: './work-center-page.html', styleUrl: './work-center-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class WorkCenterPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(WorkCenterApiService); private readonly errors = inject(ApiErrorService);
  protected readonly items = signal<readonly WorkCenterItem[]>([]); protected readonly loading = signal(true); protected readonly problem = signal<NormalizedApiProblem | null>(null); protected readonly filter = signal<'all' | 'critical' | 'attention'>('all');
  protected readonly visible = computed(() => this.filter() === 'all' ? this.items() : this.items().filter(x => x.severity === this.filter()));
  constructor() { this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.get().subscribe({ next: page => { this.items.set(page.items); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected text(key: string): string { return this.i18n.text(key as MessageKey); }
}
