import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductAttributeApiService } from './data/product-attribute-api.service';
import { ProductAttributeValueField, ProductAttributeValueInput, ProductAttributeValuesPage } from './data/product-attribute.models';
import { ProductLocalizationService } from './product-localization.service';

@Component({ selector: 'app-product-attribute-values-page', imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, FormErrorSummaryComponent, SafeFormShortcutsDirective, ApiRecoveryComponent], templateUrl: './product-attribute-values-page.html', styleUrl: './product-attribute-values-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductAttributeValuesPageComponent {
  protected readonly i18n = inject(ProductLocalizationService); private readonly route = inject(ActivatedRoute); private readonly api = inject(ProductAttributeApiService);
  private readonly errors = inject(ApiErrorService); private readonly workspace = inject(WorkspaceService); private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject(ElementRef<HTMLElement>); private readonly focus = inject(FormFocusService);
  protected readonly productId = Number(this.route.snapshot.paramMap.get('id')); protected readonly page = signal<ProductAttributeValuesPage | null>(null);
  protected readonly loading = signal(true); protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null); protected readonly problem = signal('');
  protected readonly fieldErrors = signal<Record<string, readonly string[]>>({}); protected readonly saving = signal(false); protected form = new FormGroup<Record<string, FormControl<string>>>({});
  constructor() { this.load(); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.loadingProblem.set(null); this.api.getValues(this.productId).subscribe({ next: page => { this.page.set(page); this.buildForm(page.fields); this.loading.set(false); }, error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected save(): void {
    if (this.saving()) return; this.problem.set(''); this.fieldErrors.set({});
    if (this.form.invalid) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('correctAttributeValues')); this.fieldErrors.set(this.clientErrors()); this.focus.focusFirstInvalid(this.host.nativeElement); return; }
    const values: ProductAttributeValueInput[] = (this.page()?.fields ?? []).map(field => ({ definitionId: field.definitionId, value: this.form.controls[this.key(field)].value.trim() || null }));
    this.saving.set(true); this.api.saveValues(this.productId, values).subscribe({ next: page => { this.page.set(page); this.buildForm(page.fields); this.saving.set(false); this.setWorkspaceDirty(false); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.fieldErrors.set(failure.errors ?? {}); this.saving.set(false); } });
  }
  protected key(field: ProductAttributeValueField): string { return `attribute_${field.definitionId}`; }
  protected errorFor(field: ProductAttributeValueField): string { const control = this.form.controls[this.key(field)]; return control?.touched && control.invalid ? this.i18n.text('requiredAttributeValue') : ''; }
  protected focusField(name: string): void { this.focus.focusField(this.host.nativeElement, name); }
  private buildForm(fields: readonly ProductAttributeValueField[]): void { const controls: Record<string, FormControl<string>> = {}; for (const field of fields) controls[this.key(field)] = new FormControl(field.value ?? '', { nonNullable: true, validators: field.isRequired ? [Validators.required] : [] }); this.form = new FormGroup(controls); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setWorkspaceDirty(this.form.dirty)); this.form.markAsPristine(); }
  private clientErrors(): Record<string, readonly string[]> { return Object.fromEntries(Object.entries(this.form.controls).filter(([, control]) => control.invalid).map(([name]) => [name, [this.i18n.text('requiredAttributeValue')]])); }
  private setWorkspaceDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
