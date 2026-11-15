import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { BranchesApiService, BranchSummary } from './branches-api.service';

@Component({
  selector: 'app-branches-page',
  imports: [RouterLink, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent],
  template: `
    <section class="branches-page">
      <app-page-header eyebrow="ADMIN" [title]="i18n.text('branchesTitle')" [description]="i18n.text('branchesDescription')" />
      @if (loading()) { <app-feedback-state kind="loading" [title]="i18n.text('loading')" /> }
      @else if (problem(); as failure) { <app-api-recovery [problem]="failure" (retry)="load()" /> }
      @else if (branches().length === 0) { <app-feedback-state kind="empty" [title]="i18n.text('branchWorkspaceNoBranches')" [description]="i18n.text('branchWorkspaceNoBranchesDescription')" /> }
      @else {
        <div class="summary">
          <strong>{{ branches().length }}</strong><span>{{ i18n.text('branchesTitle') }}</span>
          <strong>{{ warehouseCount() }}</strong><span>{{ i18n.text('linkedWarehouses') }}</span>
          <strong>{{ terminalCount() }}</strong><span>{{ i18n.text('posTerminals') }}</span>
        </div>
        <div class="branch-grid">
          @for (branch of branches(); track branch.id) {
            <article class="branch-card">
              <header><div><small>{{ branch.code }}</small><h2>{{ branch.name }}</h2></div><span class="status" [class.inactive]="!branch.isActive">{{ i18n.text(branch.isActive ? 'active' : 'inactive') }}</span></header>
              <section><h3>{{ i18n.text('linkedWarehouses') }}</h3>
                @if (branch.warehouses.length === 0) { <p class="muted">{{ i18n.text('noLinkedWarehouses') }}</p> }
                @for (warehouse of branch.warehouses; track warehouse.id) {
                  <div class="row"><strong>{{ warehouse.name }}</strong><span>@if (warehouse.isDefaultForPos) { {{ i18n.text('defaultForPos') }} }</span></div>
                  <div class="chips"><span [class.on]="warehouse.allowPosSales">POS</span><span [class.on]="warehouse.allowPurchases">{{ i18n.text('branchWorkspacePurchases') }}</span><span [class.on]="warehouse.allowTransferOut">{{ i18n.text('branchWorkspaceTransferOut') }}</span><span [class.on]="warehouse.allowTransferIn">{{ i18n.text('branchWorkspaceTransferIn') }}</span></div>
                }
              </section>
              <section><h3>{{ i18n.text('posTerminals') }}</h3>
                @if (branch.posTerminals.length === 0) { <p class="muted">{{ i18n.text('branchWorkspaceNoPosTerminals') }}</p> }
                @for (terminal of branch.posTerminals; track terminal.id) { <div class="row"><strong>{{ terminal.name }}</strong><span>{{ terminal.defaultWarehouseName }}</span></div> }
              </section>
            </article>
          }
        </div>
        <footer><a class="ui-button secondary" routerLink="/settings/inventory-access">{{ i18n.text('manageInventoryAccess') }}</a><a class="ui-button secondary" routerLink="/settings/branch-sales-accounts">{{ i18n.text('branchSalesAccounts') }}</a></footer>
      }
    </section>`,
  styles: [`
    .branches-page{display:grid;gap:1rem}.summary{display:grid;grid-template-columns:repeat(3,minmax(5rem,auto) 1fr);gap:.5rem 1rem;align-items:baseline;padding:1rem;border:1px solid var(--ui-border);border-radius:var(--ui-radius);background:var(--ui-surface)}
    .summary strong{font-size:1.5rem}.branch-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(19rem,1fr));gap:1rem}.branch-card{border:1px solid var(--ui-border);border-radius:var(--ui-radius);background:var(--ui-surface);padding:1rem;display:grid;gap:1rem}.branch-card header,.row,footer{display:flex;justify-content:space-between;gap:1rem;align-items:center}.branch-card h2,.branch-card h3{margin:.15rem 0}.branch-card h3{font-size:1rem}.status,.chips span{border-radius:999px;padding:.2rem .55rem;background:var(--ui-success-soft)}.status.inactive,.chips span:not(.on){background:var(--ui-muted-surface);color:var(--ui-muted)}.chips{display:flex;flex-wrap:wrap;gap:.35rem;margin-block:.45rem}.row span,.muted{color:var(--ui-muted);font-size:.85rem}footer{justify-content:flex-start;flex-wrap:wrap}@media(max-width:700px){.summary{grid-template-columns:auto 1fr}}
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BranchesPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(BranchesApiService);
  private readonly errors = inject(ApiErrorService);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly branches = signal<BranchSummary[]>([]);
  constructor() { this.load(); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getWorkspace().subscribe({ next: page => { this.branches.set(page.branches); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected warehouseCount(): number { return this.branches().reduce((total, branch) => total + branch.warehouses.length, 0); }
  protected terminalCount(): number { return this.branches().reduce((total, branch) => total + branch.posTerminals.length, 0); }
}
