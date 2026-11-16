import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { BranchesLocalizationService } from './branches-localization.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { BranchAccessManagement, BranchesApiService, BranchSummary } from './branches-api.service';
import { BranchContextService } from '../../core/branch-context/branch-context.service';

@Component({
  selector: 'app-branches-page',
  imports: [DecimalPipe, ReactiveFormsModule, RouterLink, FormErrorSummaryComponent, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent],
  template: `
    <section class="branches-page">
      <app-page-header eyebrow="ADMIN" [title]="i18n.text('branchesTitle')" [description]="i18n.text('branchesDescription')">@if(session.can('Branches.Create')){<button class="ui-button primary" type="button" (click)="startCreate()">{{ i18n.text('addBranch') }}</button>}</app-page-header>
      @if (formProblem()) { <app-form-error-summary [title]="formProblem()" [errors]="{}" /> }
      @if (showForm()) {<form class="branch-form" [formGroup]="form" (ngSubmit)="saveBranch()"><h2>{{ editingId() ? i18n.text('editBranch') : i18n.text('addBranch') }}</h2><label class="ui-field"><span>{{ i18n.text('branchCode') }}</span><input formControlName="code" maxlength="30" autocomplete="off" /></label><label class="ui-field"><span>{{ i18n.text('branchName') }}</span><input formControlName="name" maxlength="100" /></label><div class="actions"><button class="ui-button secondary" type="button" (click)="cancelBranchForm()">{{ i18n.text('cancel') }}</button><button class="ui-button primary" [disabled]="savingBranch()">{{ savingBranch() ? i18n.text('saving') : i18n.text('save') }}</button></div></form>}
      @if (loading()) { <app-feedback-state kind="loading" [title]="i18n.text('loading')" /> }
      @else if (problem(); as failure) { <app-api-recovery [problem]="failure" (retry)="load()" /> }
      @else if (branches().length === 0) { <app-feedback-state kind="empty" [title]="i18n.text('branchWorkspaceNoBranches')" [description]="i18n.text('branchWorkspaceNoBranchesDescription')" /> }
      @else {
        @if (company(); as total) {
          <section class="operations company"><h2>{{ i18n.text('branchCompanyDashboard') }}</h2><div class="metric-grid">
            <span><strong>{{ total.salesAmount | number:'1.2-2' }}</strong>{{ i18n.text('branchPeriodSales') }}</span><span><strong>{{ total.stockValue | number:'1.2-2' }}</strong>{{ i18n.text('branchStockValue') }}</span><span><strong>{{ total.openTransfers }}</strong>{{ i18n.text('branchOpenTransfers') }}</span><span [class.alert]="total.transferExceptions"><strong>{{ total.transferExceptions }}</strong>{{ i18n.text('branchTransferExceptions') }}</span><span><strong>{{ total.openCashSessions }}</strong>{{ i18n.text('branchOpenCashSessions') }}</span><span [class.alert]="total.cashSessionsAwaitingApproval"><strong>{{ total.cashSessionsAwaitingApproval }}</strong>{{ i18n.text('branchCashApprovals') }}</span>
          </div></section>
        }
        <div class="summary">
          <strong>{{ branches().length }}</strong><span>{{ i18n.text('branchesTitle') }}</span>
          <strong>{{ warehouseCount() }}</strong><span>{{ i18n.text('linkedWarehouses') }}</span>
          <strong>{{ terminalCount() }}</strong><span>{{ i18n.text('posTerminals') }}</span>
        </div>
        <div class="branch-grid">
          @for (branch of branches(); track branch.id) {
            <article class="branch-card" [class.current]="branchContext.selectedId() === branch.id">
              <header><div><small>{{ branch.code }}</small><h2>{{ branch.name }}</h2></div><span class="status" [class.inactive]="!branch.isActive">{{ i18n.text(branch.isActive ? 'active' : 'inactive') }}</span></header>
              @if (branch.operations; as op) {<section class="operations"><div class="metric-grid"><span><strong>{{ op.salesAmount | number:'1.2-2' }}</strong>{{ i18n.text('branchPeriodSales') }}</span><span><strong>{{ op.stockQuantity | number:'1.0-2' }}</strong>{{ i18n.text('branchStockQuantity') }}</span><span><strong>{{ op.openTransfers }}</strong>{{ i18n.text('branchOpenTransfers') }}</span><span [class.alert]="op.transferExceptions + op.cashSessionsAwaitingApproval"><strong>{{ op.transferExceptions + op.cashSessionsAwaitingApproval }}</strong>{{ i18n.text('branchReconciliationExceptions') }}</span></div></section>}
              <div class="branch-admin-actions">@if(session.can('Branches.Edit')){<button type="button" (click)="startEdit(branch)">{{ i18n.text('edit') }}</button>}@if(session.can('Branches.SetActive')){<button type="button" (click)="setBranchActive(branch)">{{ i18n.text(branch.isActive ? 'deactivateBranch' : 'activateBranch') }}</button>}</div>
              @if (branchContext.selectedId() === branch.id) { <p class="current-label">{{ i18n.text('currentOperationalBranch') }}</p> }
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

              @if (branchContext.selectedId() === branch.id) {
                <nav class="branch-actions" [attr.aria-label]="i18n.text('branchOperationalLinks')">
                  @if (session.can('ProductStock.View')) { <a routerLink="/inventory/stocks">{{ i18n.text('openBranchStock') }}</a> }
                  @if (session.can('StockTransactions.View')) { <a routerLink="/inventory/transactions">{{ i18n.text('openBranchMovements') }}</a> }
                  @if (session.can('Sales.View') && branch.posTerminals.length) { <a routerLink="/sales/pos">{{ i18n.text('openBranchPos') }}</a> }
                  @if (session.can('GoodsReceipts.View')) { <a routerLink="/purchases/receipts">{{ i18n.text('openBranchReceiving') }}</a> }
                </nav>
              }
            </article>
          }
        </div>
        @if (management(); as page) {
          <section class="access-card">
            <h2>{{ i18n.text('branchAccessManagement') }}</h2>
            <p class="muted">{{ i18n.text('branchAccessManagementDescription') }}</p>
            <label class="ui-field"><span>{{ i18n.text('user') }}</span>
              <select [value]="selectedUserId()" (change)="selectUser($event)">
                <option value="">{{ i18n.text('selectOption') }}</option>
                @for (user of page.users; track user.id) { <option [value]="user.id">{{ user.displayName }}@if(user.email){ — {{ user.email }} }</option> }
              </select>
            </label>
            @if (selectedUserId()) {
              <div class="grant-grid">
                @for (branch of page.branches; track branch.id) {
                  <label class="grant-row"><input type="checkbox" [checked]="selectedBranchIds().has(branch.id)" (change)="toggleBranch(branch.id, $event)" /><span><strong>{{ branch.code }}</strong> — {{ branch.name }}</span></label>
                }
              </div>
              <button class="ui-button primary" type="button" [disabled]="savingAccess()" (click)="saveAccess()">{{ savingAccess() ? i18n.text('saving') : i18n.text('saveBranchAccess') }}</button>
            }
          </section>
        }
        @if (session.can('Administration.Access')) { <footer><a class="ui-button secondary" routerLink="/settings/inventory-access">{{ i18n.text('manageInventoryAccess') }}</a><a class="ui-button secondary" routerLink="/settings/branch-sales-accounts">{{ i18n.text('branchSalesAccounts') }}</a></footer> }
      }
    </section>`,
  styleUrl: './branches-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BranchesPageComponent {
  protected readonly i18n = inject(BranchesLocalizationService);
  protected readonly session = inject(ApiSessionService);
  protected readonly branchContext = inject(BranchContextService);
  private readonly api = inject(BranchesApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly fb = inject(FormBuilder);
  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly focus = inject(FormFocusService);
  protected readonly loading = signal(true);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly company = signal<import('./branches-api.service').BranchOperations | null>(null);
  protected readonly management = signal<BranchAccessManagement | null>(null);
  protected readonly selectedUserId = signal('');
  protected readonly selectedBranchIds = signal(new Set<number>());
  protected readonly savingAccess = signal(false);
  protected readonly showForm = signal(false);
  protected readonly editingId = signal<number | null>(null);
  protected readonly savingBranch = signal(false);
  protected readonly formProblem = signal('');
  protected readonly form = this.fb.nonNullable.group({ code: ['', [Validators.required, Validators.maxLength(30), Validators.pattern(/^[A-Za-z0-9][A-Za-z0-9_-]*$/)]], name: ['', [Validators.required, Validators.maxLength(100)]] });
  constructor() { this.load(); }
  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    this.api.getWorkspace().subscribe({ next: page => { this.branches.set(page.branches); this.company.set(page.company); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } });
    if (this.session.can('Branches.ManageAccess')) this.api.getAccessManagement().subscribe({ next: page => this.management.set(page), error: error => this.problem.set(this.errors.normalize(error)) });
  }
  protected startCreate(): void { this.editingId.set(null); this.form.reset({ code:'', name:'' }); this.formProblem.set(''); this.showForm.set(true); }
  protected startEdit(branch: BranchSummary): void { this.editingId.set(branch.id); this.form.reset({ code:branch.code, name:branch.name }); this.formProblem.set(''); this.showForm.set(true); }
  protected cancelBranchForm(): void { this.showForm.set(false); this.editingId.set(null); this.formProblem.set(''); this.form.reset({ code:'', name:'' }); }
  protected saveBranch(): void { if(this.form.invalid || this.savingBranch()){this.form.markAllAsTouched();this.formProblem.set(this.i18n.text('correctBranchFields'));this.focus.focusFirstInvalid(this.host.nativeElement);return;}const id=this.editingId();const command=this.form.getRawValue();this.savingBranch.set(true);this.formProblem.set('');const request=id?this.api.update(id,command):this.api.create(command);request.subscribe({next:()=>{this.savingBranch.set(false);this.cancelBranchForm();this.load();},error:error=>{const failure=this.errors.normalize(error);this.formProblem.set(failure.detail??failure.title??this.i18n.text('errorDescription'));this.savingBranch.set(false);}}); }
  protected setBranchActive(branch: BranchSummary): void { if(this.savingBranch())return;this.savingBranch.set(true);this.formProblem.set('');this.api.setActive(branch.id,!branch.isActive).subscribe({next:()=>{this.savingBranch.set(false);this.load();},error:error=>{const failure=this.errors.normalize(error);this.formProblem.set(failure.detail??failure.title??this.i18n.text('errorDescription'));this.savingBranch.set(false);}}); }  protected selectUser(event: Event): void { const id = (event.target as HTMLSelectElement).value; this.selectedUserId.set(id); this.selectedBranchIds.set(new Set(this.management()?.users.find(x => x.id === id)?.branchIds ?? [])); }
  protected toggleBranch(branchId: number, event: Event): void { const next = new Set(this.selectedBranchIds()); (event.target as HTMLInputElement).checked ? next.add(branchId) : next.delete(branchId); this.selectedBranchIds.set(next); }
  protected saveAccess(): void { const userId = this.selectedUserId(); if (!userId || this.savingAccess()) return; this.savingAccess.set(true); this.problem.set(null); this.api.replaceUserGrants(userId, [...this.selectedBranchIds()]).subscribe({ next: page => { this.management.set(page); this.savingAccess.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.savingAccess.set(false); } }); }
  protected warehouseCount(): number { return this.branches().reduce((total, branch) => total + branch.warehouses.length, 0); }
  protected terminalCount(): number { return this.branches().reduce((total, branch) => total + branch.posTerminals.length, 0); }
}
