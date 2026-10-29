import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SalesApprovalRulesApiService } from './sales-approval-rules-api.service';
import { SalesApprovalRulesLocalizationService } from './sales-approval-rules-localization.service';
import { ApprovalRulesPage, CreateApprovalRule } from './sales-approval-rules.models';
@Component({
  selector: 'app-sales-approval-rules-page',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './sales-approval-rules-page.html',
  styleUrl: './sales-approval-rules-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SalesApprovalRulesPageComponent {
  protected readonly i18n = inject(SalesApprovalRulesLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SalesApprovalRulesApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly page = signal<ApprovalRulesPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly form = this.fb.group({
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(150)]),
    customerId: this.fb.control<number | null>(null),
    metric: this.fb.nonNullable.control(2),
    comparison: this.fb.nonNullable.control(1),
    threshold: this.fb.nonNullable.control(0, [Validators.min(0)]),
    action: this.fb.nonNullable.control(2),
    priority: this.fb.nonNullable.control(100, [Validators.min(1), Validators.max(9999)]),
    effectiveFrom: this.fb.control<string | null>(null),
    effectiveTo: this.fb.control<string | null>(null),
    allowRequesterApproval: this.fb.nonNullable.control(false),
    requireDistinctApprovers: this.fb.nonNullable.control(true),
    approverRoles: this.fb.array([this.fb.nonNullable.control('')]),
  });
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  get roles() {
    return this.form.controls.approverRoles as FormArray;
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.api.page().subscribe({
      next: (p) => {
        this.page.set(p);
        this.loading.set(false);
        this.saving.set(false);
      },
      error: (e) => {
        this.failure.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  protected addRole() {
    if (this.roles.length < 10) this.roles.push(this.fb.nonNullable.control(''));
  }
  protected create() {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    const value = this.form.getRawValue();
    this.api
      .create({
        ...value,
        approverRoles: value.approverRoles.filter((x) => !!x),
      } as CreateApprovalRule)
      .subscribe({
        next: () => {
          this.form.reset({
            name: '',
            customerId: null,
            metric: 2,
            comparison: 1,
            threshold: 0,
            action: 2,
            priority: 100,
            effectiveFrom: null,
            effectiveTo: null,
            allowRequesterApproval: false,
            requireDistinctApprovers: true,
          });
          this.roles.clear();
          this.roles.push(this.fb.nonNullable.control(''));
          this.form.markAsPristine();
          this.dirty();
          this.load();
        },
        error: (e) => {
          this.problem.set(this.errors.normalize(e));
          this.saving.set(false);
        },
      });
  }
  protected toggle(id: number, active: boolean, rowVersion: string) {
    if (this.saving()) return;
    this.saving.set(true);
    this.api.setActive(id, active, rowVersion).subscribe({
      next: () => this.load(),
      error: (e) => {
        this.problem.set(this.errors.normalize(e));
        this.saving.set(false);
      },
    });
  }
  private dirty() {
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.form.dirty);
  }
}
