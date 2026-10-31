import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentDetails } from './data/inventory-adjustment.models';
import { InventoryLocalizationService, InventoryMessageKey } from './inventory-localization.service';

@Component({ selector: 'app-inventory-adjustment-details-page', imports: [DatePipe, DecimalPipe, ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, ApiRecoveryComponent, FormErrorSummaryComponent], templateUrl: './inventory-adjustment-details-page.html', styleUrl: './inventory-adjustments.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class InventoryAdjustmentDetailsPageComponent {
  protected readonly i18n = inject(InventoryLocalizationService); protected readonly session = inject(ApiSessionService); private readonly api = inject(InventoryAdjustmentApiService); private readonly errors = inject(ApiErrorService); private readonly workspace = inject(WorkspaceService); private readonly destroyRef = inject(DestroyRef); private readonly fb = inject(FormBuilder); protected readonly id = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
  protected readonly details = signal<InventoryAdjustmentDetails | null>(null); protected readonly loading = signal(true); protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null); protected readonly problem = signal(''); protected readonly success = signal(''); protected readonly saving = signal(false); protected readonly form = this.fb.group({ lines: this.fb.array([]) });
  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty)); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected get lines(): FormArray { return this.form.controls.lines; }
  protected statusName(status: number): string { return this.i18n.text(({ 1: 'adjustmentDraft', 2: 'adjustmentCounted', 3: 'adjustmentApproved', 4: 'adjustmentPosted', 5: 'adjustmentCancelled' } as Record<number, InventoryMessageKey>)[status] || 'adjustmentDraft'); }
  protected recordCount(): void { if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('countEveryLine')); return; } const lines = this.lines.getRawValue() as { productId: number; countedQuantity: number }[]; this.execute(() => this.api.count(this.id, { lines }), 'confirmCompleteCount', 'countSaved'); }
  protected approve(): void { this.execute(() => this.api.approve(this.id), 'confirmApproveAdjustment', 'adjustmentApprovedSuccess'); }
  protected post(): void { this.execute(() => this.api.post(this.id), 'confirmPostAdjustment', 'adjustmentPostedSuccess'); }
  protected cancel(): void { this.execute(() => this.api.cancel(this.id), 'confirmCancelAdjustment', 'adjustmentCancelledSuccess'); }
  protected load(): void { this.loading.set(true); this.loadingProblem.set(null); this.api.getDetails(this.id).subscribe({ next: details => { this.details.set(details); this.lines.clear({ emitEvent: false }); for (const line of details.lines) this.lines.push(this.fb.nonNullable.group({ productId: line.productId, countedQuantity: [line.countedQuantity ?? 0, Validators.min(0)] }), { emitEvent: false }); this.form.markAsPristine(); this.setDirty(false); this.loading.set(false); }, error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  private execute(request: () => Observable<void>, confirmation: InventoryMessageKey, success: InventoryMessageKey): void { if (this.saving() || !globalThis.confirm(this.i18n.text(confirmation))) return; this.problem.set(''); this.success.set(''); this.saving.set(true); request().subscribe({ next: () => { this.saving.set(false); this.success.set(this.i18n.text(success)); this.load(); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.saving.set(false); } }); }
  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
