import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin } from 'rxjs';
import { AccountingSettings, AccountingSettingsPage, BranchSalesAccountsPage, ConfigureBranchWarehouse, ConfigurePosTerminalWarehouse, CreateMeasurementUnit, CreatePosTerminal, CurrencyExchangeRatesPage, DiscountSettings, DocumentNumberSettings, FiscalPeriodsPage, FiscalPeriodStatusCommand, GeneralPreferences, InventoryAccessSettings, InventorySettings, MeasurementUnitsPage, OrganizationIdentity, PosExperienceSettings, PosExperienceSettingsPage, PurchaseMatchingSettings, PurchaseSourcingPolicy, SaveCurrencyExchangeRate, WorkflowCatalogPage } from './settings.models';
@Injectable({ providedIn: 'root' })
export class SettingsApiService {
  constructor(private readonly http: HttpClient) {}
  getOverview() { return forkJoin({ identity: this.http.get<OrganizationIdentity>('/api/v1/settings/organization-identity'), preferences: this.http.get<GeneralPreferences>('/api/v1/settings/preferences') }); }
  updatePreferences(value: GeneralPreferences) { return this.http.put<GeneralPreferences>('/api/v1/settings/preferences', value); }
  updateIdentity(value: OrganizationIdentity) { return this.http.put<OrganizationIdentity>('/api/v1/settings/organization-identity', value); }
  uploadLogo(file: File, rowVersion: string) { const body = new FormData(); body.append('logoFile', file); body.append('rowVersion', rowVersion); return this.http.post<OrganizationIdentity>('/api/v1/settings/organization-logo', body); }
  deleteLogo(rowVersion: string) { return this.http.delete<OrganizationIdentity>('/api/v1/settings/organization-logo', { body: { rowVersion } }); }
  getDiscounts() { return this.http.get<DiscountSettings>('/api/v1/settings/discounts'); }
  updateDiscounts(value: DiscountSettings) { return this.http.put<DiscountSettings>('/api/v1/settings/discounts', value); }
  getPurchaseMatching() { return this.http.get<PurchaseMatchingSettings>('/api/v1/settings/purchase-matching'); }
  updatePurchaseMatching(value: PurchaseMatchingSettings) { return this.http.put<PurchaseMatchingSettings>('/api/v1/settings/purchase-matching', value); }
  getInventory() { return this.http.get<InventorySettings>('/api/v1/settings/inventory'); }
  updateInventory(value: InventorySettings) { return this.http.put<InventorySettings>('/api/v1/settings/inventory', value); }
  getPurchaseSourcing() { return this.http.get<PurchaseSourcingPolicy>('/api/v1/settings/purchase-sourcing'); }
  updatePurchaseSourcing(value: PurchaseSourcingPolicy) { return this.http.put<PurchaseSourcingPolicy>('/api/v1/settings/purchase-sourcing', value); }
  getDocumentNumbers() { return this.http.get<DocumentNumberSettings>('/api/v1/settings/document-numbers'); }
  updateDocumentNumbers(value: DocumentNumberSettings) { return this.http.put<DocumentNumberSettings>('/api/v1/settings/document-numbers', value); }
  getFiscalPeriods() { return this.http.get<FiscalPeriodsPage>('/api/v1/settings/fiscal-periods'); }
  createFiscalPeriod(value: FiscalPeriodsPage['newPeriod']) { return this.http.post<FiscalPeriodsPage>('/api/v1/settings/fiscal-periods', value); }
  changeFiscalPeriodStatus(value: FiscalPeriodStatusCommand) { return this.http.put<FiscalPeriodsPage>('/api/v1/settings/fiscal-periods/status', value); }
  getCurrencyExchangeRates() { return this.http.get<CurrencyExchangeRatesPage>('/api/v1/settings/currency-exchange-rates'); }
  saveCurrencyExchangeRate(value: SaveCurrencyExchangeRate) { return this.http.post<CurrencyExchangeRatesPage>('/api/v1/settings/currency-exchange-rates', value); }
  getMeasurementUnits() { return this.http.get<MeasurementUnitsPage>('/api/v1/settings/measurement-units'); }
  createMeasurementUnit(value: CreateMeasurementUnit) { return this.http.post<MeasurementUnitsPage>('/api/v1/settings/measurement-units', value); }
  deactivateMeasurementUnit(id: number) { return this.http.post<MeasurementUnitsPage>(`/api/v1/settings/measurement-units/${id}/deactivate`, {}); }
  getAccountingSettings() { return this.http.get<AccountingSettingsPage>('/api/v1/settings/accounting'); }
  updateAccountingSettings(value: AccountingSettings) { return this.http.put<AccountingSettingsPage>('/api/v1/settings/accounting', value); }
  getBranchSalesAccounts() { return this.http.get<BranchSalesAccountsPage>('/api/v1/settings/branch-sales-accounts'); }
  updateBranchSalesAccount(branchId: number, accountId: number) { return this.http.put<BranchSalesAccountsPage>(`/api/v1/settings/branch-sales-accounts/${branchId}`, { accountId }); }
  getInventoryAccess() { return this.http.get<InventoryAccessSettings>('/api/v1/settings/inventory-access'); }
  configureBranchWarehouse(value: ConfigureBranchWarehouse) { return this.http.put<InventoryAccessSettings>('/api/v1/settings/inventory-access/branch-warehouse', value); }
  createPosTerminal(value: CreatePosTerminal) { return this.http.post<InventoryAccessSettings>('/api/v1/settings/inventory-access/pos-terminals', value); }
  configurePosTerminalWarehouse(value: ConfigurePosTerminalWarehouse) { return this.http.put<InventoryAccessSettings>('/api/v1/settings/inventory-access/pos-terminal-warehouse', value); }
  getPosExperience(terminalId?: number) { const suffix = terminalId ? `?terminalId=${terminalId}` : ''; return this.http.get<PosExperienceSettingsPage>(`/api/v1/settings/pos-experience${suffix}`); }
  updatePosExperience(value: PosExperienceSettings) { return this.http.put<PosExperienceSettingsPage>('/api/v1/settings/pos-experience', value); }
  getWorkflowCatalog() { return this.http.get<WorkflowCatalogPage>('/api/v1/settings/workflow-catalog'); }
}
