import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FieldCueComponent } from '../../shared/forms/field-cue/field-cue';
import { FormSectionComponent } from '../../shared/forms/form-section/form-section';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableCellDirective, DataTableColumn, DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { FilterBarComponent } from '../../shared/ui/filter-bar/filter-bar';
import { SectionHeaderComponent } from '../../shared/ui/section-header/section-header';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { WorkflowFooterComponent } from '../../shared/ui/workflow-footer/workflow-footer';
import { WorkflowProgressComponent, WorkflowStep } from '../../shared/ui/workflow-progress/workflow-progress';
import { DetailSectionComponent } from '../../shared/ui/detail-section/detail-section';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PageContextService, pageContextFields } from '../../shared/state/page-context.service';
import { PurchaseRequestApiService } from './purchase-request-api.service';
import {
  ApprovalRulesPage,
  CreateRequestLine,
  CreateRequestPage,
  PurchaseRequest,
} from './purchase-request.models';
import {
  PurchaseRequestKey,
  PurchaseRequestLocalizationService,
} from './purchase-request-localization.service';

interface PurchaseRequestDraft {
  readonly warehouseId: number;
  readonly neededByDate: string;
  readonly priority: number;
  readonly lines: readonly {
    readonly productId: number;
    readonly measurementUnitId: number;
    readonly requestedQuantity: number;
    readonly suggestedSupplierId: number;
  }[];
}
@Component({
  selector: 'app-purchase-requests-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    PageHeaderComponent,
    FilterBarComponent,
    SectionHeaderComponent,
    SummaryStripComponent,
    WorkflowFooterComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
    DataTableComponent,
    DataTableCellDirective,
    FieldCueComponent,
    FormSectionComponent,
    DetailSectionComponent,
    WorkflowProgressComponent,
  ],
  templateUrl: './purchase-requests-page.html',
  styleUrl: './purchase-requests-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseRequestsPageComponent {
  protected readonly i18n = inject(PurchaseRequestLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(PurchaseRequestApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  private readonly context = inject(PageContextService);
  private readonly listContextSchema = {
    defaults: { status: 0, search: '' },
    fields: { status: pageContextFields.integer(0, 6), search: pageContextFields.text() },
  };
  protected readonly mode = signal<'list' | 'create' | 'details' | 'rules'>('list');
  protected readonly rows = signal<PurchaseRequest[]>([]);
  protected readonly listColumns = computed<readonly DataTableColumn[]>(() => [
    { key: 'requestNumber', header: this.i18n.text('number'), kind: 'code' },
    { key: 'warehouseName', header: this.i18n.text('warehouse') },
    { key: 'neededByDate', header: this.i18n.text('needed'), kind: 'date' },
    { key: 'priority', header: this.i18n.text('priority'), kind: 'status' },
    { key: 'status', header: this.i18n.text('status'), kind: 'status' },
    { key: 'lines', header: this.i18n.text('lines'), kind: 'number' },
  ]);
  protected readonly detail = signal<PurchaseRequest | null>(null);
  protected readonly options = signal<CreateRequestPage | null>(null);
  protected readonly rulesPage = signal<ApprovalRulesPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly filter = this.fb.nonNullable.group({ status: [0], search: [''] });
  protected readonly form = this.fb.nonNullable.group({
    warehouseId: [0, Validators.min(1)],
    neededByDate: ['', Validators.required],
    priority: [2],
    justification: ['', [Validators.required, Validators.maxLength(500)]],
    notes: ['', Validators.maxLength(1000)],
    lines: this.fb.array([]),
  });
  protected readonly ruleForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    warehouseId: [0],
    minimumPriority: [1],
    maximumPriority: [4],
    role1: [''],
    role2: [''],
    role3: [''],
  });
  constructor() {
    this.form.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        if (this.mode() === 'create' && this.form.dirty) this.saveDraft();
        this.markDirty();
      });
    this.ruleForm.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.markDirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.resolve());
    this.context.watch('purchase-requests-list', this.route, this.listContextSchema)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(value => {
        if (this.mode() !== 'list') return;
        this.filter.setValue(value, { emitEvent: false });
        this.loadList();
      });
  }
  canDeactivate() {
    return (
      !(this.form.dirty || this.ruleForm.dirty) || globalThis.confirm(this.i18n.text('unsaved'))
    );
  }
  protected get lines(): FormArray {
    return this.form.controls.lines;
  }
  protected addLine() {
    this.lines.push(this.lineGroup());
    this.form.markAsDirty();
    this.saveDraft();
    this.markDirty();
  }
  protected removeLine(i: number) {
    if (this.lines.length > 1) {
      this.lines.removeAt(i);
      this.form.markAsDirty();
      this.saveDraft();
      this.markDirty();
    }
  }
  protected loadList() {
    const f = this.filter.getRawValue();
    this.start();
    this.api.getAll(f.status || undefined, f.search).subscribe({
      next: (r) => {
        this.rows.set(r);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  protected applyListFilters(): void {
    void this.context.replace('purchase-requests-list', this.route, this.listContextSchema, this.filter.getRawValue())
      .then(changed => { if (!changed) this.loadList(); });
  }
  protected create() {
    if (this.form.invalid || !this.lines.length) {
      this.form.markAllAsTouched();
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    const v = this.form.getRawValue();
    const lines = (
      v.lines as {
        productId: number;
        measurementUnitId: number;
        requestedQuantity: number;
        suggestedSupplierId: number;
        notes: string;
      }[]
    ).map((x) => ({
      productId: x.productId,
      measurementUnitId: x.measurementUnitId,
      requestedQuantity: x.requestedQuantity,
      suggestedSupplierId: x.suggestedSupplierId || undefined,
      notes: x.notes.trim() || undefined,
    })) as CreateRequestLine[];
    this.saving.set(true);
    this.problem.set('');
    this.api
      .create({
        ...v,
        notes: v.notes.trim() || undefined,
        justification: v.justification.trim(),
        lines,
      })
      .subscribe({
        next: (r) => {
          this.context.clearDraft('purchase-request-create');
          this.saving.set(false);
          this.form.markAsPristine();
          this.markDirty();
          void this.router.navigate(['/purchases/requests', r.id]);
        },
        error: (e) => {
          this.saving.set(false);
          this.showError(e);
        },
      });
  }
  protected submit() {
    const x = this.detail();
    if (!x) return;
    this.command(() => this.api.submit(x.id, x.rowVersion), 'submitted');
  }
  protected cancel(reason: string) {
    const x = this.detail();
    if (!x || !reason.trim()) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    this.command(() => this.api.cancel(x.id, x.rowVersion, reason.trim()), 'cancelled');
  }
  protected decide(approve: boolean, note: string) {
    const x = this.detail();
    if (!x || (!approve && !note.trim())) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    this.command(
      () =>
        approve
          ? this.api.approve(x.id, note.trim() || undefined)
          : this.api.reject(x.id, note.trim()),
      approve ? 'approvedMessage' : 'rejectedMessage',
    );
  }
  protected createRule() {
    if (this.ruleForm.invalid) {
      this.ruleForm.markAllAsTouched();
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    const v = this.ruleForm.getRawValue();
    const roles = [v.role1, v.role2, v.role3].map((x) => x.trim()).filter(Boolean);
    if (!roles.length || v.minimumPriority > v.maximumPriority) {
      this.problem.set(this.i18n.text('fix'));
      return;
    }
    this.saving.set(true);
    this.api
      .createApprovalRule({
        name: v.name.trim(),
        warehouseId: v.warehouseId || undefined,
        minimumPriority: v.minimumPriority,
        maximumPriority: v.maximumPriority,
        approverRoles: roles,
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.success.set(this.i18n.text('ruleCreated'));
          this.ruleForm.reset({
            name: '',
            warehouseId: 0,
            minimumPriority: 1,
            maximumPriority: 4,
            role1: '',
            role2: '',
            role3: '',
          });
          this.ruleForm.markAsPristine();
          this.markDirty();
          this.loadRules();
        },
        error: (e) => {
          this.saving.set(false);
          this.showError(e);
        },
      });
  }
  protected toggleRule(id: number, active: boolean, rowVersion: string) {
    this.saving.set(true);
    this.api.setApprovalRuleActive(id, active, rowVersion).subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(this.i18n.text('ruleChanged'));
        this.loadRules();
      },
      error: (e) => {
        this.saving.set(false);
        this.showError(e);
      },
    });
  }
  protected statusName(v: number) {
    return this.i18n.text(
      (
        {
          1: 'draft',
          2: 'submittedStatus',
          3: 'cancelledStatus',
          4: 'approved',
          5: 'rejected',
          6: 'sourced',
        } as Record<number, PurchaseRequestKey>
      )[v] ?? 'draft',
    );
  }
  protected priorityName(v: number) {
    return this.i18n.text(
      ({ 1: 'low', 2: 'normal', 3: 'high', 4: 'urgent' } as Record<number, PurchaseRequestKey>)[
        v
      ] ?? 'normal',
    );
  }
  protected workflowSteps(status: number): readonly WorkflowStep[] {
    const primary = [
      this.i18n.text('draft'), this.i18n.text('submittedStatus'),
      this.i18n.text('approved'), this.i18n.text('sourced'),
    ];
    if (status === 3 || status === 5) return [
      { label: primary[0], state: 'complete' },
      ...(status === 5 ? [{ label: primary[1], state: 'complete' as const }] : []),
      { label: this.statusName(status), state: 'stopped' },
    ];
    const current = ({ 1: 0, 2: 1, 4: 2, 6: 3 } as Record<number, number>)[status] ?? 0;
    return primary.map((label, index) => ({
      label,
      state: index < current ? 'complete' : index === current ? 'current' : 'upcoming',
    }));
  }
  protected workflowGuidance(status: number): string {
    return this.i18n.text(({
      1: 'nextSubmitGuidance', 2: 'approvalGuidance', 3: 'cancelledGuidance',
      4: 'sourcingGuidance', 5: 'rejectedGuidance', 6: 'sourcedGuidance',
    } as Record<number, PurchaseRequestKey>)[status] ?? 'nextSubmitGuidance');
  }
  protected actionName(v: number) {
    return this.i18n.text(
      (
        {
          1: 'createdAction',
          2: 'submittedAction',
          3: 'cancelledAction',
          4: 'approvedAction',
          5: 'rejectedAction',
          6: 'sourcedAction',
        } as Record<number, PurchaseRequestKey>
      )[v] ?? 'createdAction',
    );
  }
  protected retry() {
    this.mode() === 'list' ? this.loadList() : this.resolve();
  }
  protected approvalStatusName(v: number) {
    return this.i18n.text(
      (
        { 1: 'pendingApproval', 2: 'approvedApproval', 3: 'rejectedApproval' } as Record<
          number,
          PurchaseRequestKey
        >
      )[v] ?? 'pendingApproval',
    );
  }
  private resolve() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.route.snapshot.routeConfig?.path === 'purchases/approval-rules') {
      this.mode.set('rules');
      this.loadRules();
      return;
    }
    if (this.route.snapshot.routeConfig?.path?.endsWith('new')) {
      this.mode.set('create');
      this.start();
      this.api.getCreateForm().subscribe({
        next: (p) => {
          this.options.set(p);
          this.form.patchValue({
            warehouseId: p.form.warehouseId,
            neededByDate: p.form.neededByDate,
            priority: p.form.priority,
            justification: p.form.justification,
            notes: p.form.notes ?? '',
          });
          this.lines.clear({ emitEvent: false });
          this.lines.push(this.lineGroup(), { emitEvent: false });
          const restored = this.restoreDraft();
          restored ? this.form.markAsDirty() : this.form.markAsPristine();
          this.markDirty();
          this.loading.set(false);
        },
        error: (e) => this.fail(e),
      });
      return;
    }
    if (id) {
      this.mode.set('details');
      this.loadDetail(id);
      return;
    }
    this.mode.set('list');
  }
  private loadRules() {
    this.start();
    this.api.getApprovalRules().subscribe({
      next: (p) => {
        this.rulesPage.set(p);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private loadDetail(id: number) {
    this.start();
    this.api.get(id).subscribe({
      next: (r) => {
        this.detail.set(r);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private command(
    request: () => ReturnType<PurchaseRequestApiService['submit']>,
    message: PurchaseRequestKey,
  ) {
    if (this.saving()) return;
    this.saving.set(true);
    this.problem.set('');
    request().subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(this.i18n.text(message));
        this.loadDetail(this.detail()!.id);
      },
      error: (e) => {
        this.saving.set(false);
        this.showError(e);
      },
    });
  }
  private start() {
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.problem.set('');
  }
  private fail(e: unknown) {
    this.loadingProblem.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private showError(e: unknown) {
    const f = this.errors.normalize(e);
    this.problem.set(f.detail ?? f.title ?? this.i18n.text('error'));
  }
  private markDirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty || this.ruleForm.dirty);
  }
  private lineGroup(value?: Partial<PurchaseRequestDraft['lines'][number]>) {
    return this.fb.nonNullable.group({
      productId: [value?.productId ?? 0, Validators.min(1)],
      measurementUnitId: [value?.measurementUnitId ?? 0, Validators.min(1)],
      requestedQuantity: [value?.requestedQuantity ?? 1, Validators.min(0.000001)],
      suggestedSupplierId: [value?.suggestedSupplierId ?? 0],
      notes: ['', Validators.maxLength(500)],
    });
  }
  private saveDraft(): void {
    const value = this.form.getRawValue();
    const lines = value.lines as {
      productId: number;
      measurementUnitId: number;
      requestedQuantity: number;
      suggestedSupplierId: number;
    }[];
    this.context.saveDraft<PurchaseRequestDraft>('purchase-request-create', {
      warehouseId: value.warehouseId,
      neededByDate: value.neededByDate,
      priority: value.priority,
      lines: lines.slice(0, 100).map(line => ({
        productId: Number(line.productId) || 0,
        measurementUnitId: Number(line.measurementUnitId) || 0,
        requestedQuantity: Math.max(Number(line.requestedQuantity) || 1, 0.000001),
        suggestedSupplierId: Number(line.suggestedSupplierId) || 0,
      })),
    });
  }
  private restoreDraft(): boolean {
    const draft = this.context.loadDraft<PurchaseRequestDraft>('purchase-request-create');
    if (!draft?.lines?.length) return false;
    this.form.patchValue({
      warehouseId: Number(draft.warehouseId) || 0,
      neededByDate: /^\d{4}-\d{2}-\d{2}$/.test(draft.neededByDate) ? draft.neededByDate : this.form.controls.neededByDate.value,
      priority: [1, 2, 3, 4].includes(draft.priority) ? draft.priority : 2,
    }, { emitEvent: false });
    this.lines.clear({ emitEvent: false });
    for (const line of draft.lines.slice(0, 100)) this.lines.push(this.lineGroup(line), { emitEvent: false });
    return true;
  }
}
