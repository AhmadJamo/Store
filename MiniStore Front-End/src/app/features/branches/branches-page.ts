import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { BranchesLocalizationService } from './branches-localization.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FieldCueComponent } from '../../shared/forms/field-cue/field-cue';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { FormSectionComponent } from '../../shared/forms/form-section/form-section';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DetailSectionComponent } from '../../shared/ui/detail-section/detail-section';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SummaryStripComponent } from '../../shared/ui/summary-strip/summary-strip';
import { WorkflowFooterComponent } from '../../shared/ui/workflow-footer/workflow-footer';
import { BranchAccessManagement, BranchesApiService, BranchSummary } from './branches-api.service';
import { BranchContextService } from '../../core/branch-context/branch-context.service';

@Component({
  selector: 'app-branches-page',
  imports: [DecimalPipe, ReactiveFormsModule, RouterLink, FormErrorSummaryComponent, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent, FieldCueComponent, FormSectionComponent, DetailSectionComponent, SummaryStripComponent, WorkflowFooterComponent],
  templateUrl: './branches-page.html',
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
