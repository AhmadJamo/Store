import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { AccountOption, BranchSalesAccount, BranchSalesAccountsPage } from './data/settings.models';
import { SettingsApiService } from './data/settings-api.service';

@Component({ selector: 'app-branch-sales-accounts-page', imports: [RouterLink, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './branch-sales-accounts-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class BranchSalesAccountsPageComponent {
  protected readonly i18n = inject(LocalizationService); private readonly api = inject(SettingsApiService); private readonly errors = inject(ApiErrorService); private readonly workspace = inject(WorkspaceService);
  protected readonly loading = signal(true); protected readonly savingId = signal<number | null>(null); protected readonly saved = signal(false); protected readonly problem = signal<NormalizedApiProblem | null>(null); protected readonly branches = signal<BranchSalesAccount[]>([]); protected readonly accounts = signal<AccountOption[]>([]); protected readonly drafts = signal<Record<number, number | null>>({});
  constructor() { this.load(); }
  canDeactivate(): boolean { return Object.keys(this.drafts()).length === 0 || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getBranchSalesAccounts().subscribe({ next: page => { this.apply(page); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected selected(branch: BranchSalesAccount): number | null { return this.drafts()[branch.id] ?? branch.salesRevenueAccountId; }
  protected select(branch: BranchSalesAccount, event: Event): void { const raw = (event.target as HTMLSelectElement).value; const value = raw ? Number(raw) : null; this.drafts.update(current => ({ ...current, [branch.id]: value })); this.syncDirty(); }
  protected save(branch: BranchSalesAccount): void { const accountId = this.selected(branch); if (!accountId || this.savingId() !== null) return; this.savingId.set(branch.id); this.saved.set(false); this.problem.set(null); this.api.updateBranchSalesAccount(branch.id, accountId).subscribe({ next: page => { this.apply(page); this.saved.set(true); this.savingId.set(null); }, error: error => { this.problem.set(this.errors.normalize(error)); this.savingId.set(null); } }); }
  private apply(page: BranchSalesAccountsPage): void { this.branches.set(page.branches); this.accounts.set(page.accounts); this.drafts.set({}); this.syncDirty(); }
  private syncDirty(): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, Object.keys(this.drafts()).length > 0); }
}
