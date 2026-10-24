import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { CustomerApiService } from './data/customer-api.service';
import { CustomerForm, CustomerFormPage } from './data/customer.models';

@Component({ selector: 'app-customer-form-page', imports: [ReactiveFormsModule, RouterLink], templateUrl: './customer-form-page.html', styleUrl: './customers.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class CustomerFormPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(CustomerApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id')) || undefined;
  protected readonly page = signal<CustomerFormPage | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly fieldErrors = signal<Record<string, readonly string[]>>({});
  protected readonly form = inject(FormBuilder).nonNullable.group({
    id: 0, customerCode: '', name: ['', [Validators.required, Validators.maxLength(200)]], type: 1,
    legalName: ['', Validators.maxLength(200)], phone: ['', Validators.maxLength(50)], email: ['', [Validators.email, Validators.maxLength(256)]],
    taxNumber: ['', Validators.maxLength(100)], registrationNumber: ['', Validators.maxLength(100)], website: ['', Validators.maxLength(500)],
    preferredLanguage: 'ar-JO', timeZoneId: ['Asia/Amman', [Validators.required, Validators.maxLength(100)]], notes: ['', Validators.maxLength(1000)],
    accountId: null as number | null, paymentTermDays: [0, [Validators.min(0), Validators.max(3650)]], preferredCurrencyCode: '',
    defaultTaxRateId: null as number | null, isTaxExempt: false, taxExemptionReason: ['', Validators.maxLength(500)],
    defaultSalesPriceListId: null as number | null, rowVersion: '',
  });

  constructor() {
    this.api.getForm(this.id).subscribe({
      next: (page) => { this.page.set(page); this.form.reset(this.toControls(page.form)); this.form.markAsPristine(); },
      error: (error) => this.problem.set(this.errors.normalize(error).title ?? this.i18n.text('errorDescription')),
    });
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setWorkspaceDirty(this.form.dirty));
  }

  canDeactivate(): boolean {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription'));
  }

  protected save(): void {
    this.problem.set(''); this.fieldErrors.set({});
    if (this.form.invalid) { this.form.markAllAsTouched(); this.problem.set(this.i18n.text('correctCustomerFields')); return; }
    this.saving.set(true);
    const command = this.toCommand();
    const request = this.id ? this.api.update(command) : this.api.create(command);
    request.subscribe({
      next: (customer) => { this.form.markAsPristine(); this.setWorkspaceDirty(false); this.saving.set(false); void this.router.navigate(['/customers', customer.id]); },
      error: (error) => { const problem = this.errors.normalize(error); this.fieldErrors.set(problem.errors ?? {}); this.problem.set(problem.detail ?? problem.title ?? this.i18n.text('errorDescription')); this.saving.set(false); },
    });
  }

  protected errorFor(name: string): string {
    const server = this.fieldErrors()[name] ?? this.fieldErrors()[`Form.${name}`] ?? this.fieldErrors()[name[0].toUpperCase() + name.slice(1)];
    if (server?.length) return server[0];
    const control = this.form.get(name);
    return control?.touched && control.invalid ? this.i18n.text('invalidField') : '';
  }

  private setWorkspaceDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
  private toControls(form: CustomerForm) { return { ...form, customerCode: form.customerCode ?? '', legalName: form.legalName ?? '', phone: form.phone ?? '', email: form.email ?? '', taxNumber: form.taxNumber ?? '', registrationNumber: form.registrationNumber ?? '', website: form.website ?? '', notes: form.notes ?? '', accountId: form.accountId ?? null, preferredCurrencyCode: form.preferredCurrencyCode ?? '', defaultTaxRateId: form.defaultTaxRateId ?? null, taxExemptionReason: form.taxExemptionReason ?? '', defaultSalesPriceListId: form.defaultSalesPriceListId ?? null, rowVersion: form.rowVersion ?? '' }; }
  private toCommand(): CustomerForm { const raw = this.form.getRawValue(); return { ...raw, customerCode: raw.customerCode || undefined, legalName: raw.legalName || undefined, phone: raw.phone || undefined, email: raw.email || undefined, taxNumber: raw.taxNumber || undefined, registrationNumber: raw.registrationNumber || undefined, website: raw.website || undefined, notes: raw.notes || undefined, accountId: raw.accountId ?? undefined, preferredCurrencyCode: raw.preferredCurrencyCode || undefined, defaultTaxRateId: raw.defaultTaxRateId ?? undefined, taxExemptionReason: raw.taxExemptionReason || undefined, defaultSalesPriceListId: raw.defaultSalesPriceListId ?? undefined }; }
}

export const customerFormCanDeactivate = (component: CustomerFormPageComponent) => component.canDeactivate();
