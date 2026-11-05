import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { PageContextService } from '../../shared/state/page-context.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { JournalEntryListQuery } from './journal-entry.models';
import { ManualJournalApiService } from './manual-journal-api.service';
import { ManualJournalLocalizationService } from './manual-journal-localization.service';
import {
  ManualJournal,
  ManualJournalCreatePage,
  ManualJournalDraft,
  ManualJournalListPage,
} from './manual-journal.models';
@Component({
  selector: 'app-manual-journal-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    PageHeaderComponent,
    DataTableComponent,
    FeedbackStateComponent,
    ApiRecoveryComponent,
  ],
  templateUrl: './manual-journal-page.html',
  styleUrl: './manual-journal-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ManualJournalPageComponent {
  protected readonly i18n = inject(ManualJournalLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(ManualJournalApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly context = inject(PageContextService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly page = signal<ManualJournalListPage | null>(null);
  protected readonly options = signal<ManualJournalCreatePage | null>(null);
  protected readonly item = signal<ManualJournal | null>(null);
  protected readonly success = signal('');
  protected readonly id = Number(this.route.snapshot.paramMap.get('id')) || 0;
  protected readonly form = this.fb.group({
    date: [new Date().toISOString().slice(0, 10), Validators.required],
    description: ['', [Validators.required, Validators.maxLength(500)]],
    lines: this.fb.array([]),
  });
  protected readonly reverseForm = this.fb.nonNullable.group({
    date: [new Date().toISOString().slice(0, 10), Validators.required],
    reason: ['', [Validators.required, Validators.maxLength(500)]],
  });
  private query: JournalEntryListQuery = {
    search: '',
    fromDate: '',
    toDate: '',
    status: 'all',
    accountId: 0,
    page: 1,
    pageSize: 25,
  };
  private requestKey: string = crypto.randomUUID();
  private readonly reversalKey: string = crypto.randomUUID();
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => {
      this.persistDraft();
      this.dirty();
    });
    this.load();
  }
  get lines() {
    return this.form.controls.lines as FormArray;
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected addLine(value?: any) {
    this.lines.push(
      this.fb.group({
        accountId: [value?.accountId ?? 0, Validators.min(1)],
        branchId: value?.branchId ?? null,
        warehouseId: value?.warehouseId ?? null,
        debit: [value?.debit ?? 0, Validators.min(0)],
        credit: [value?.credit ?? 0, Validators.min(0)],
        description: [value?.description ?? null, Validators.maxLength(500)],
      }),
    );
  }
  protected removeLine(index: number) {
    if (this.lines.length > 2) this.lines.removeAt(index);
  }
  protected total(kind: 'debit' | 'credit') {
    return this.lines.controls.reduce((sum, row) => sum + Number(row.get(kind)?.value ?? 0), 0);
  }
  protected balanced() {
    return (
      this.lines.length >= 2 &&
      this.total('debit') > 0 &&
      this.total('debit') === this.total('credit')
    );
  }
  protected save() {
    if (this.form.invalid || !this.balanced() || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.problem.set(null);
    this.api
      .create({ ...this.form.getRawValue(), idempotencyKey: this.requestKey } as any)
      .subscribe({
        next: (value) => {
          this.context.clearDraft('manual-journal-create');
          this.form.markAsPristine();
          this.dirty();
          void this.router.navigate(['/accounting/manual-journals', value.id]);
        },
        error: (error) => this.failed(error),
      });
  }
  protected post() {
    if (!this.item() || this.saving() || !globalThis.confirm(this.i18n.text('confirmPost'))) return;
    this.saving.set(true);
    this.api
      .post(this.id)
      .subscribe({ next: () => this.loadDetail(), error: (error) => this.failed(error) });
  }
  protected reverse() {
    if (
      !this.item() ||
      this.reverseForm.invalid ||
      this.saving() ||
      !globalThis.confirm(this.i18n.text('confirmReverse'))
    ) {
      this.reverseForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api
      .reverse(this.id, { ...this.reverseForm.getRawValue(), idempotencyKey: this.reversalKey })
      .subscribe({
        next: (value) => void this.router.navigate(['/accounting/manual-journals', value.id]),
        error: (error) => this.failed(error),
      });
  }
  protected retry() {
    this.load();
  }
  private load() {
    this.loading.set(true);
    this.failure.set(null);
    if (this.id) this.loadDetail();
    else {
      if (this.session.can('ManualJournals.View'))
        this.api.list(this.query).subscribe({
          next: (value) => {
            this.page.set(value);
            this.loading.set(false);
          },
          error: (error) => this.loadFailed(error),
        });
      else {
        this.page.set({ query: {}, accountDisplay: null, items: [], totalCount: 0, totalPages: 0 });
        this.loading.set(false);
      }
      if (this.session.can('ManualJournals.Create'))
        this.api.createPage().subscribe({
          next: (value) => {
            this.options.set(value);
            this.restoreDraft();
          },
          error: (error) => this.loadFailed(error),
        });
    }
  }
  private loadDetail() {
    this.loading.set(true);
    this.api.get(this.id).subscribe({
      next: (value) => {
        this.item.set(value);
        this.loading.set(false);
        this.saving.set(false);
      },
      error: (error) => this.loadFailed(error),
    });
  }
  private restoreDraft() {
    if (this.lines.length) return;
    const draft = this.context.loadDraft<ManualJournalDraft>('manual-journal-create');
    this.requestKey = draft?.idempotencyKey ?? crypto.randomUUID();
    this.form.patchValue(
      {
        date: draft?.date ?? new Date().toISOString().slice(0, 10),
        description: draft?.description ?? '',
      },
      { emitEvent: false },
    );
    for (const line of draft?.lines ?? [{}, {}]) this.addLine(line);
    this.form.markAsPristine();
    this.dirty();
  }
  private persistDraft() {
    if (!this.options()) return;
    const value = { ...this.form.getRawValue(), idempotencyKey: this.requestKey } as ManualJournalDraft;
    if (this.form.dirty) this.context.saveDraft('manual-journal-create', value);
    else this.context.clearDraft('manual-journal-create');
  }
  private failed(error: unknown) {
    this.problem.set(this.errors.normalize(error));
    this.saving.set(false);
  }
  private loadFailed(error: unknown) {
    this.failure.set(this.errors.normalize(error));
    this.loading.set(false);
  }
  private dirty() {
    const tab = this.workspace.activeTab();
    if (tab) this.workspace.markDirty(tab.id, this.form.dirty);
  }
}
