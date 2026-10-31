import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductAttributeApiService } from './data/product-attribute-api.service';
import { ProductAttributeCategory, ProductAttributeDataType, ProductAttributeDefinition } from './data/product-attribute.models';
import { ProductLocalizationService, ProductMessageKey } from './product-localization.service';

@Component({ selector: 'app-product-attributes-page', imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, DataTableComponent, FormErrorSummaryComponent, SafeFormShortcutsDirective, ApiRecoveryComponent], templateUrl: './product-attributes-page.html', styleUrl: './product-attributes-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductAttributesPageComponent {
  protected readonly i18n = inject(ProductLocalizationService); protected readonly session = inject(ApiSessionService);
  private readonly api = inject(ProductAttributeApiService); private readonly errors = inject(ApiErrorService); private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef); private readonly host = inject(ElementRef<HTMLElement>); private readonly focus = inject(FormFocusService);
  protected readonly definitions = signal<readonly ProductAttributeDefinition[]>([]); protected readonly categories = signal<readonly ProductAttributeCategory[]>([]);
  protected readonly loading = signal(true); protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null); protected readonly problem = signal('');
  protected readonly fieldErrors = signal<Record<string, readonly string[]>>({}); protected readonly saving = signal(false); protected readonly changingId = signal<number | null>(null);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]], code: ['', [Validators.required, Validators.maxLength(40), Validators.pattern(/^[A-Za-z0-9][A-Za-z0-9_-]*$/)]],
    dataType: [1 as ProductAttributeDataType, Validators.required], isRequired: [false], isVariantDefining: [false], displayOrder: [0, [Validators.required, Validators.min(0)]],
    productCategoryIds: [[] as number[]], optionsText: ['', Validators.maxLength(10000)],
  });
  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setWorkspaceDirty(this.form.dirty)); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.loadingProblem.set(null); this.api.getPage().subscribe({ next: page => { this.definitions.set(page.definitions); this.categories.set(page.categories); this.loading.set(false); }, error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected toggleCategory(id: number, checked: boolean): void { const control = this.form.controls.productCategoryIds; const next = checked ? [...control.value, id] : control.value.filter(value => value !== id); control.setValue([...new Set(next)]); control.markAsDirty(); }
  protected create(): void {
    if (this.saving()) return; this.problem.set(''); this.fieldErrors.set({});
    if (this.form.invalid || (this.form.controls.dataType.value === 4 && !this.form.controls.optionsText.value.trim())) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('correctAttributeFields')); this.fieldErrors.set(this.clientErrors()); this.focus.focusFirstInvalid(this.host.nativeElement); return; }
    this.saving.set(true); const raw = this.form.getRawValue();
    this.api.create({ ...raw, name: raw.name.trim(), code: raw.code.trim().toUpperCase(), optionsText: raw.optionsText.trim() || null }).subscribe({ next: definition => { this.definitions.update(items => [...items, definition].sort((a, b) => a.displayOrder - b.displayOrder || a.name.localeCompare(b.name))); this.form.reset({ name: '', code: '', dataType: 1, isRequired: false, isVariantDefining: false, displayOrder: 0, productCategoryIds: [], optionsText: '' }); this.form.markAsPristine(); this.setWorkspaceDirty(false); this.saving.set(false); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.fieldErrors.set(failure.errors ?? {}); this.saving.set(false); } });
  }
  protected setStatus(item: ProductAttributeDefinition): void { if (this.changingId() !== null || !globalThis.confirm(this.i18n.text('confirmAttributeStatus'))) return; this.changingId.set(item.id); this.problem.set(''); this.api.setStatus(item.id, !item.isActive).subscribe({ next: updated => { this.definitions.update(items => items.map(value => value.id === updated.id ? updated : value)); this.changingId.set(null); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.changingId.set(null); } }); }
  protected typeName(type: ProductAttributeDataType): string { return this.i18n.text(({ 1: 'attributeText', 2: 'attributeNumber', 3: 'attributeBoolean', 4: 'attributeSelection' } as Record<number, ProductMessageKey>)[type]); }
  protected errorFor(name: string): string { const server = this.fieldErrors()[name] ?? this.fieldErrors()[name[0].toUpperCase() + name.slice(1)]; if (server?.length) return server[0]; const control = this.form.get(name); return control?.touched && control.invalid ? this.i18n.text('invalidField') : ''; }
  protected focusField(name: string): void { this.focus.focusField(this.host.nativeElement, name); }
  private clientErrors(): Record<string, readonly string[]> { const errors = Object.fromEntries(Object.entries(this.form.controls).filter(([, control]) => control.invalid).map(([name]) => [name, [this.i18n.text('invalidField')]])); if (this.form.controls.dataType.value === 4 && !this.form.controls.optionsText.value.trim()) errors['optionsText'] = [this.i18n.text('selectionOptionsRequired')]; return errors; }
  private setWorkspaceDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
