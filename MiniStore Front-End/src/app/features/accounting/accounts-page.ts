import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { PageContextService, pageContextFields } from '../../shared/state/page-context.service';
import { AccountsApiService } from './accounts-api.service';
import { AccountsLocalizationService } from './accounts-localization.service';
import { AccountPage, AccountTreeNode } from './accounts.models';
@Component({
  selector: 'app-accounts-page',
  imports: [ReactiveFormsModule, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent],
  templateUrl: './accounts-page.html',
  styleUrl: './accounts-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountsPageComponent {
  protected readonly i18n = inject(AccountsLocalizationService);
  private readonly api = inject(AccountsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly context = inject(PageContextService);
  protected readonly page = signal<AccountPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly saving = signal(false);
  protected readonly showForm = signal(false);
  protected readonly search = signal('');
  protected readonly expanded = signal<ReadonlySet<number>>(new Set());
  protected readonly visibleAccounts = computed(() => {
    const roots = this.page()?.tree ?? [];
    const query = this.search().trim().toLocaleLowerCase();
    const rows: { node: AccountTreeNode; depth: number }[] = [];
    const matches = (node: AccountTreeNode): boolean => !query
      || node.code.toLocaleLowerCase().includes(query)
      || node.name.toLocaleLowerCase().includes(query)
      || node.children.some(matches);
    const visit = (node: AccountTreeNode, depth: number) => {
      if (!matches(node)) return;
      rows.push({ node, depth });
      if (query || this.expanded().has(node.id)) node.children.forEach(child => visit(child, depth + 1));
    };
    roots.forEach(root => visit(root, 0));
    return rows;
  });
  protected readonly form = this.fb.group({
    code: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(30)]),
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(200)]),
    type: this.fb.nonNullable.control(1),
    parentAccountId: this.fb.control<number | null>(null),
  });
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.context.watch('account-tree', this.route, { defaults: { search: '' }, fields: { search: pageContextFields.text() } })
      .pipe(takeUntilDestroyed(this.destroy)).subscribe(value => this.search.set(value.search));
    this.load();
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    this.loading.set(true);
    this.problem.set(null);
    this.api.get().subscribe({
      next: (x) => {
        this.page.set(x);
        this.expanded.set(new Set(x.tree.map(node => node.id)));
        this.loading.set(false);
      },
      error: (e) => {
        this.problem.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  protected setSearch(value: string) { void this.context.replace('account-tree', this.route, { defaults: { search: '' }, fields: { search: pageContextFields.text() } }, { search: value }); }
  protected toggle(node: AccountTreeNode) { const next = new Set(this.expanded()); next.has(node.id) ? next.delete(node.id) : next.add(node.id); this.expanded.set(next); }
  protected expandAll() { this.expanded.set(new Set(this.page()?.accounts.map(x => x.id) ?? [])); }
  protected collapseAll() { this.expanded.set(new Set()); }
  protected create() {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.reset({ code: '', name: '', type: 1, parentAccountId: null });
        this.form.markAsPristine();
        this.showForm.set(false);
        this.saving.set(false);
        this.dirty();
        this.load();
      },
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
