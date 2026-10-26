import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SettingsApiService } from './data/settings-api.service';
import { OrganizationIdentity, OrganizationIdentityField } from './data/settings.models';

@Component({ selector: 'app-organization-identity-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './organization-identity-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class OrganizationIdentityPageComponent {
  protected readonly i18n = inject(LocalizationService); private readonly api = inject(SettingsApiService); private readonly errors = inject(ApiErrorService); private readonly workspace = inject(WorkspaceService); private readonly destroyRef = inject(DestroyRef); private readonly fb = inject(FormBuilder);
  protected readonly loading = signal(true); protected readonly saving = signal(false); protected readonly saved = signal(false); protected readonly problem = signal<NormalizedApiProblem | null>(null); protected readonly identity = signal<OrganizationIdentity | null>(null); protected readonly logoFile = signal<File | null>(null);
  protected readonly form = this.fb.nonNullable.group({
    tradeName: ['', [Validators.required, Validators.maxLength(200)]], tradeNameArabic: ['', Validators.maxLength(200)], legalName: ['', Validators.maxLength(200)], legalNameArabic: ['', Validators.maxLength(200)],
    commercialRegistrationNumber: ['', Validators.maxLength(100)], taxRegistrationNumber: ['', Validators.maxLength(100)], email: ['', [Validators.email, Validators.maxLength(254)]], website: ['', Validators.maxLength(500)],
    phone: ['', Validators.maxLength(50)], secondaryPhone: ['', Validators.maxLength(50)], address: ['', Validators.maxLength(500)], addressArabic: ['', Validators.maxLength(500)], city: ['', Validators.maxLength(100)], cityArabic: ['', Validators.maxLength(100)], stateOrProvince: ['', Validators.maxLength(100)], postalCode: ['', Validators.maxLength(30)], countryCode: ['', [Validators.minLength(2), Validators.maxLength(2)]], documentNameStyle: 1, rowVersion: '', customFields: this.fb.array([]),
  });
  protected get fields(): FormArray { return this.form.controls.customFields; }
  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty)); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getOverview().subscribe({ next: value => { this.apply(value.identity); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected addField(value?: Partial<OrganizationIdentityField>): void { if (this.fields.length >= 20) return; this.fields.push(this.fb.nonNullable.group({ sequence: value?.sequence ?? this.fields.length + 1, section: value?.section ?? 4, label: [value?.label ?? '', [Validators.required, Validators.maxLength(100)]], labelArabic: [value?.labelArabic ?? '', Validators.maxLength(100)], value: [value?.value ?? '', [Validators.required, Validators.maxLength(500)]], valueArabic: [value?.valueArabic ?? '', Validators.maxLength(500)], showOnSalesDocuments: value?.showOnSalesDocuments ?? true, showOnPurchaseDocuments: value?.showOnPurchaseDocuments ?? false, showOnReceipts: value?.showOnReceipts ?? false, showInFooter: value?.showInFooter ?? false })); }
  protected removeField(index: number): void { this.fields.removeAt(index); this.fields.controls.forEach((field, position) => field.get('sequence')?.setValue(position + 1)); this.form.markAsDirty(); }
  protected save(): void { if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); return; } const current = this.identity(); if (!current) return; this.start(); this.api.updateIdentity({ ...current, ...this.form.getRawValue(), customFields: this.fields.getRawValue() as OrganizationIdentityField[] }).subscribe({ next: value => this.success(value), error: error => this.failure(error) }); }
  protected selectLogo(event: Event): void { const file = (event.target as HTMLInputElement).files?.[0] ?? null; this.logoFile.set(file); }
  protected uploadLogo(): void { const file = this.logoFile(); const current = this.identity(); if (!file || !current || this.saving()) return; this.start(); this.api.uploadLogo(file, current.rowVersion).subscribe({ next: value => { this.logoFile.set(null); this.success(value); }, error: error => this.failure(error) }); }
  protected deleteLogo(): void { const current = this.identity(); if (!current || this.saving()) return; this.start(); this.api.deleteLogo(current.rowVersion).subscribe({ next: value => this.success(value), error: error => this.failure(error) }); }
  protected logoUrl(): string { return `/api/v1/settings/organization-logo?v=${encodeURIComponent(this.identity()?.logoVersion ?? '')}`; }
  private start(): void { this.saving.set(true); this.saved.set(false); this.problem.set(null); }
  private success(value: OrganizationIdentity): void { this.apply(value); this.saved.set(true); this.saving.set(false); }
  private failure(error: unknown): void { this.problem.set(this.errors.normalize(error)); this.saving.set(false); }
  private apply(value: OrganizationIdentity): void { this.identity.set(value); this.fields.clear(); value.customFields.forEach(field => this.addField(field)); this.form.patchValue({ ...value, tradeNameArabic: value.tradeNameArabic ?? '', legalName: value.legalName ?? '', legalNameArabic: value.legalNameArabic ?? '', commercialRegistrationNumber: value.commercialRegistrationNumber ?? '', taxRegistrationNumber: value.taxRegistrationNumber ?? '', email: value.email ?? '', website: value.website ?? '', phone: value.phone ?? '', secondaryPhone: value.secondaryPhone ?? '', address: value.address ?? '', addressArabic: value.addressArabic ?? '', city: value.city ?? '', cityArabic: value.cityArabic ?? '', stateOrProvince: value.stateOrProvince ?? '', postalCode: value.postalCode ?? '', countryCode: value.countryCode ?? '' }); this.form.markAsPristine(); this.setDirty(false); }
  private setDirty(value: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, value); }
}
