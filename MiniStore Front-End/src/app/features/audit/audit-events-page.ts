import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { AuditEventsLocalizationService } from './audit-events-localization.service';
import { AuditEventsApiService } from './data/audit-events-api.service';
import { AuditEventPage, AuditEventQuery } from './data/audit-events.models';

@Component({
  selector: 'app-audit-events-page',
  imports: [DatePipe, ReactiveFormsModule, ApiRecoveryComponent, DataTableComponent, FeedbackStateComponent, PageHeaderComponent],
  templateUrl: './audit-events-page.html',
  styleUrl: './audit-events-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuditEventsPageComponent {
  protected readonly i18n = inject(AuditEventsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(AuditEventsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<AuditEventPage | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly loading = signal(true);
  protected readonly exporting = signal(false);
  protected readonly purpose = signal('InternalReview');
  protected readonly form = this.fb.group({ from: '', to: '', category: this.fb.control<number | null>(null), outcome: this.fb.control<number | null>(null), eventType: '', actorId: '', resourceType: '', resourceId: '', correlationId: '', page: 1, pageSize: 25 });

  constructor() { this.load(); }

  protected load(page?: number): void {
    if (page) this.form.controls.page.setValue(page);
    this.loading.set(true); this.problem.set(null);
    this.api.getPage(this.query()).subscribe({ next: value => { this.page.set(value); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } });
  }
  protected search(): void { this.load(1); }
  protected reset(): void { this.form.reset({ from: '', to: '', category: null, outcome: null, eventType: '', actorId: '', resourceType: '', resourceId: '', correlationId: '', page: 1, pageSize: 25 }); this.load(); }
  protected move(delta: number): void { const current = this.page(); if (current) this.load(Math.min(current.totalPages, Math.max(1, current.query.page + delta))); }
  protected export(): void {
    if (this.exporting()) return;
    this.exporting.set(true); this.problem.set(null);
    this.api.export(this.query(), this.purpose()).subscribe({ next: blob => { const url = URL.createObjectURL(blob); const link = document.createElement('a'); link.href = url; link.download = 'audit-events-tenant.csv'; link.click(); URL.revokeObjectURL(url); this.exporting.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.exporting.set(false); } });
  }
  private query(): AuditEventQuery { const value = this.form.getRawValue(); return { from: value.from || undefined, to: value.to || undefined, category: value.category, outcome: value.outcome, eventType: value.eventType || undefined, actorId: value.actorId || undefined, resourceType: value.resourceType || undefined, resourceId: value.resourceId || undefined, correlationId: value.correlationId || undefined, page: value.page ?? 1, pageSize: value.pageSize ?? 25 }; }
}
