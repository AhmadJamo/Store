import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { AccountOption, AccountingSettings, AccountingSettingsPage } from './data/settings.models';
import { SettingsApiService } from './data/settings-api.service';

@Component({ selector: 'app-accounting-settings-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './accounting-settings-page.html', styleUrl: './settings-forms.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class AccountingSettingsPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly saved = signal(false);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly accounts = signal<AccountOption[]>([]);
  protected readonly postingFields = [
    ['purchaseDiscountAccountId', 'purchaseDiscountAccount'],
    ['salesDiscountAccountId', 'salesDiscountAccount'],
    ['salesRevenueAccountId', 'salesRevenueAccount'],
    ['costOfSalesAccountId', 'costOfSalesAccount'],
    ['goodsReceivedNotInvoicedAccountId', 'grniAccount'],
    ['purchasePriceVarianceAccountId', 'purchasePriceVarianceAccount'],
    ['landedCostClearingAccountId', 'landedCostClearingAccount'],
    ['realizedExchangeGainAccountId', 'realizedExchangeGainAccount'],
    ['realizedExchangeLossAccountId', 'realizedExchangeLossAccount'],
  ] as const;
  protected readonly form = inject(FormBuilder).group({
    purchaseDiscountAccountId: [null as number | null], salesDiscountAccountId: [null as number | null],
    salesRevenueAccountId: [null as number | null], costOfSalesAccountId: [null as number | null],
    goodsReceivedNotInvoicedAccountId: [null as number | null], purchasePriceVarianceAccountId: [null as number | null],
    landedCostClearingAccountId: [null as number | null], realizedExchangeGainAccountId: [null as number | null],
    realizedExchangeLossAccountId: [null as number | null], customerReceivableAccountMode: [null as number | null],
    customerReceivableControlAccountId: [null as number | null], customerReceivableParentAccountId: [null as number | null],
    customerAccountCodePrefix: ['CUST-', [Validators.required, Validators.maxLength(20), Validators.pattern(/^\S{1,20}$/)]],
    customerPolicyEffectiveFrom: ['', Validators.required],
    salesInvoiceQuantityPolicy: [1, [Validators.required, Validators.min(1), Validators.max(2)]],
  });

  constructor() { this.load(); this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setDirty(this.form.dirty)); }
  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected load(): void { this.loading.set(true); this.problem.set(null); this.api.getAccountingSettings().subscribe({ next: page => { this.apply(page); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } }); }
  protected save(): void { if (this.saving() || this.form.invalid) { this.form.markAllAsTouched(); return; } this.saving.set(true); this.saved.set(false); this.problem.set(null); const value = this.form.getRawValue() as AccountingSettings; value.customerAccountCodePrefix = value.customerAccountCodePrefix.trim().toUpperCase(); this.api.updateAccountingSettings(value).subscribe({ next: page => { this.apply(page); this.saved.set(true); this.saving.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.saving.set(false); } }); }
  private apply(page: AccountingSettingsPage): void { this.accounts.set(page.accounts); this.form.reset({ ...page.settings, customerPolicyEffectiveFrom: page.settings.customerPolicyEffectiveFrom.slice(0, 10) }); this.form.markAsPristine(); this.setDirty(false); }
  private setDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
}
