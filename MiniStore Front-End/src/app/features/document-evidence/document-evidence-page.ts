import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { DocumentEvidenceComponent } from '../../shared/ui/document-evidence/document-evidence';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { DocumentEvidenceApiService } from './data/document-evidence-api.service';
import { DocumentEvidence, DocumentEvidenceListItem, DocumentKind } from './data/document-evidence.models';

@Component({ selector: 'app-document-evidence-page', imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent, DocumentEvidenceComponent], templateUrl: './document-evidence-page.html', styleUrl: './document-evidence-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class DocumentEvidencePageComponent {
  protected readonly i18n = inject(LocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(DocumentEvidenceApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly items = signal<readonly DocumentEvidenceListItem[]>([]);
  protected readonly evidence = signal<DocumentEvidence | null>(null);
  private readonly kind = this.validKind(this.route.snapshot.paramMap.get('kind'));
  private readonly id = Number(this.route.snapshot.paramMap.get('id')) || null;
  constructor() { this.load(); }
  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    if (this.kind && this.id) { this.api.get(this.kind, this.id).subscribe({ next: value => { this.evidence.set(value); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); return; }
    const sources = [];
    if (this.session.can('SalesOrders.View')) sources.push(this.api.getList('sales-order').pipe(catchError(() => of([]))));
    if (this.session.can('CustomerInvoices.View')) sources.push(this.api.getList('customer-invoice').pipe(catchError(() => of([]))));
    if (!sources.length) { this.items.set([]); this.loading.set(false); return; }
    forkJoin(sources).subscribe({ next: groups => { this.items.set(groups.flat().sort((a, b) => b.documentDate.localeCompare(a.documentDate))); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } });
  }
  private validKind(value: string | null): DocumentKind | null { return value === 'sales-order' || value === 'customer-invoice' ? value : null; }
}
