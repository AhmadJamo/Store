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
import { ProductCategoryApiService } from './data/product-category-api.service';
import { ProductCategory } from './data/product-category.models';
import { ProductLocalizationService } from './product-localization.service';

@Component({ selector: 'app-product-categories-page', imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, DataTableComponent, FormErrorSummaryComponent, SafeFormShortcutsDirective, ApiRecoveryComponent], templateUrl: './product-categories-page.html', styleUrl: './product-categories-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductCategoriesPageComponent {
  protected readonly i18n = inject(ProductLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(ProductCategoryApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly focus = inject(FormFocusService);
  protected readonly items = signal<readonly ProductCategory[]>([]);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal('');
  protected readonly fieldErrors = signal<Record<string, readonly string[]>>({});
  protected readonly saving = signal(false);
  protected readonly changingId = signal<number | null>(null);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    code: ['', [Validators.required, Validators.maxLength(30), Validators.pattern(/^[A-Za-z0-9][A-Za-z0-9_-]*$/)]],
  });

  constructor() {
    this.load();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setWorkspaceDirty(this.form.dirty));
  }

  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.loadingProblem.set(null); this.api.getAll().subscribe({ next: items => { this.items.set(items); this.loading.set(false); }, error: error => { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected create(): void {
    if (this.saving()) return;
    this.problem.set(''); this.fieldErrors.set({});
    if (this.form.invalid) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('correctCategoryFields')); this.fieldErrors.set(this.clientErrors()); this.focus.focusFirstInvalid(this.host.nativeElement); return; }
    this.saving.set(true);
    const raw = this.form.getRawValue();
    this.api.create({ name: raw.name.trim(), code: raw.code.trim().toUpperCase() }).subscribe({ next: category => { this.items.update(items => [...items, category].sort((left, right) => left.name.localeCompare(right.name))); this.form.reset({ name: '', code: '' }); this.form.markAsPristine(); this.setWorkspaceDirty(false); this.saving.set(false); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.fieldErrors.set(failure.errors ?? {}); this.saving.set(false); } });
  }
  protected setStatus(category: ProductCategory): void {
    if (this.changingId() !== null || !globalThis.confirm(this.i18n.text('confirmCategoryStatus'))) return;
    this.problem.set(''); this.changingId.set(category.id);
    this.api.setStatus(category.id, !category.isActive).subscribe({ next: updated => { this.items.update(items => items.map(item => item.id === updated.id ? updated : item)); this.changingId.set(null); }, error: error => { const failure = this.errors.normalize(error); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.changingId.set(null); } });
  }
  protected errorFor(name: string): string { const server = this.fieldErrors()[name] ?? this.fieldErrors()[name[0].toUpperCase() + name.slice(1)]; if (server?.length) return server[0]; const control = this.form.get(name); return control?.touched && control.invalid ? this.i18n.text('invalidField') : ''; }
  protected focusField(name: string): void { this.focus.focusField(this.host.nativeElement, name); }
  private clientErrors(): Record<string, readonly string[]> { return Object.fromEntries(Object.entries(this.form.controls).filter(([, control]) => control.invalid).map(([name]) => [name, [this.i18n.text('invalidField')]])); }
  private setWorkspaceDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
