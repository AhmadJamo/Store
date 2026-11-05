import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkflowFooterComponent } from '../../shared/ui/workflow-footer/workflow-footer';
import { WorkflowProgressComponent, WorkflowStep } from '../../shared/ui/workflow-progress/workflow-progress';
import { SalesOrdersApiService } from './sales-orders-api.service';
import { SalesOrdersLocalizationService } from './sales-orders-localization.service';
import { SalesOrderDetails, SalesOrderSummary } from './sales-orders.models';

@Component({
  selector: 'app-sales-orders-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
    WorkflowFooterComponent,
    WorkflowProgressComponent,
  ],
  templateUrl: './sales-orders-page.html',
  styleUrl: './sales-orders-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SalesOrdersPageComponent {
  protected readonly i18n = inject(SalesOrdersLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SalesOrdersApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  protected readonly rows = signal<SalesOrderSummary[]>([]);
  protected readonly item = signal<SalesOrderDetails | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly filters = inject(FormBuilder).nonNullable.group({ search: '', status: 0 });
  protected readonly reason = inject(FormBuilder).nonNullable.control('', [Validators.required, Validators.maxLength(500)]);
  protected readonly note = inject(FormBuilder).nonNullable.control('', Validators.maxLength(500));
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));

  constructor() { this.load(); }

  protected load(): void {
    this.loading.set(true);
    this.failure.set(null);
    this.problem.set(null);
    if (this.id) {
      this.api.get(this.id).subscribe({
        next: value => {
          this.item.set(value);
          this.loading.set(false);
          this.saving.set(false);
        },
        error: error => this.failedLoad(error),
      });
      return;
    }
    const filters = this.filters.getRawValue();
    this.api.list(filters.status || undefined, filters.search.trim() || undefined).subscribe({
      next: value => {
        this.rows.set(value);
        this.loading.set(false);
      },
      error: error => this.failedLoad(error),
    });
  }

  protected command(action: 'confirm' | 'cancel'): void {
    const order = this.item();
    if (!order || this.saving() || (action === 'cancel' && this.reason.invalid)) {
      this.reason.markAsTouched();
      return;
    }
    this.saving.set(true);
    this.problem.set(null);
    this.api.command(order.id, action, order.rowVersion, action === 'cancel' ? this.reason.value.trim() : null).subscribe({
      next: () => {
        this.reason.reset();
        this.load();
      },
      error: error => this.failedAction(error),
    });
  }

  protected decide(approve: boolean): void {
    const order = this.item();
    if (!order || this.saving() || (!approve && !this.note.value.trim())) {
      this.note.markAsTouched();
      return;
    }
    this.saving.set(true);
    this.problem.set(null);
    this.api.decide(order.id, approve, this.note.value.trim() || null).subscribe({
      next: () => {
        this.note.reset();
        this.load();
      },
      error: error => this.failedAction(error),
    });
  }

  protected workflowSteps(status: number): readonly WorkflowStep[] {
    if (status === 3) return [
      { label: this.i18n.text('draft'), state: 'complete' },
      { label: this.i18n.text('cancelled'), state: 'stopped' },
    ];
    return [
      { label: this.i18n.text('draft'), state: status === 1 ? 'current' : 'complete' },
      { label: this.i18n.text('confirmed'), state: status === 2 ? 'current' : 'upcoming' },
    ];
  }

  protected workflowGuidance(order: SalesOrderDetails): string {
    if (order.approval?.status === 2) return this.i18n.text('waiting');
    return this.i18n.text(order.status === 1
      ? 'nextConfirmGuidance'
      : order.status === 2 ? 'executionGuidance' : 'cancelledGuidance');
  }

  private failedLoad(error: unknown): void {
    this.failure.set(this.errors.normalize(error));
    this.loading.set(false);
  }

  private failedAction(error: unknown): void {
    this.problem.set(this.errors.normalize(error));
    this.saving.set(false);
  }
}
