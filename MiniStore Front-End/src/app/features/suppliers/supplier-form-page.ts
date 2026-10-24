import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SupplierApiService } from './data/supplier-api.service';
import { SupplierCommand, SupplierDetails, SupplierFormPage } from './data/supplier.models';

@Component({ selector: 'app-supplier-form-page', imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, FormErrorSummaryComponent, SafeFormShortcutsDirective], templateUrl: './supplier-form-page.html', styleUrl: './supplier-form-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class SupplierFormPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SupplierApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly focus = inject(FormFocusService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id')) || undefined;
  protected readonly page = signal<SupplierFormPage | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly fieldErrors = signal<Record<string, readonly string[]>>({});
  protected readonly form = inject(FormBuilder).nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]], legalName: ['', Validators.maxLength(200)],
    contactPerson: ['', Validators.maxLength(150)], phone: ['', Validators.maxLength(50)], email: ['', [Validators.email, Validators.maxLength(256)]],
    address: ['', Validators.maxLength(500)], taxNumber: ['', Validators.maxLength(100)], registrationNumber: ['', Validators.maxLength(100)],
    website: ['', Validators.maxLength(500)], paymentTermsDays: [0, [Validators.min(0), Validators.max(3650)]], notes: ['', Validators.maxLength(1000)],
    isActive: true, accountId: [0, Validators.min(1)], productIds: [[] as number[]], rowVersion: '',
  });

  constructor() {
    this.api.getForm(this.id).subscribe({ next: (page) => { this.page.set(page); this.form.reset(this.toControls(page.form)); this.form.markAsPristine(); }, error: (error) => this.problem.set(this.errors.normalize(error).title ?? this.i18n.text('errorDescription')) });
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setWorkspaceDirty(this.form.dirty));
  }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected save(): void {
    if (this.saving()) return;
    this.problem.set(''); this.fieldErrors.set({});
    if (this.form.invalid) { this.form.markAllAsTouched(); this.fieldErrors.set(this.clientErrors()); this.problem.set(this.i18n.text('correctSupplierFields')); this.focus.focusFirstInvalid(this.host.nativeElement); return; }
    this.saving.set(true);
    const command = this.toCommand();
    const request = this.id ? this.api.update(this.id, command) : this.api.create(command);
    request.subscribe({ next: (supplier) => { this.form.markAsPristine(); this.setWorkspaceDirty(false); this.saving.set(false); void this.router.navigate(['/suppliers', supplier.id]); }, error: (error) => { const problem = this.errors.normalize(error); const fieldErrors = problem.errors ?? {}; this.fieldErrors.set(fieldErrors); this.problem.set(problem.detail ?? problem.title ?? this.i18n.text('errorDescription')); this.saving.set(false); const first = Object.keys(fieldErrors)[0]; if (first) this.focus.focusField(this.host.nativeElement, first); } });
  }
  protected errorFor(name: string): string { const server = this.fieldErrors()[name] ?? this.fieldErrors()[name[0].toUpperCase() + name.slice(1)]; if (server?.length) return server[0]; const control = this.form.get(name); return control?.touched && control.invalid ? this.i18n.text('invalidField') : ''; }
  protected focusField(name: string): void { this.focus.focusField(this.host.nativeElement, name); }
  private setWorkspaceDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
  private clientErrors(): Record<string, readonly string[]> { return Object.fromEntries(Object.entries(this.form.controls).filter(([, control]) => control.invalid).map(([name]) => [name, [this.i18n.text('invalidField')]])); }
  private toControls(form: SupplierDetails) { return { name: form.name ?? '', legalName: form.legalName ?? '', contactPerson: form.contactPerson ?? '', phone: form.phone ?? '', email: form.email ?? '', address: form.address ?? '', taxNumber: form.taxNumber ?? '', registrationNumber: form.registrationNumber ?? '', website: form.website ?? '', paymentTermsDays: form.paymentTermsDays ?? 0, notes: form.notes ?? '', isActive: form.isActive ?? true, accountId: form.accountId ?? 0, productIds: form.productIds ?? [], rowVersion: form.rowVersion ?? '' }; }
  private toCommand(): SupplierCommand { const raw = this.form.getRawValue(); return { ...raw, legalName: raw.legalName || undefined, contactPerson: raw.contactPerson || undefined, phone: raw.phone || undefined, email: raw.email || undefined, address: raw.address || undefined, taxNumber: raw.taxNumber || undefined, registrationNumber: raw.registrationNumber || undefined, website: raw.website || undefined, notes: raw.notes || undefined, rowVersion: raw.rowVersion || undefined }; }
}
