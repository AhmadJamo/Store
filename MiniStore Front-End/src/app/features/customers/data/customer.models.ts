export interface CustomerListQuery { search?: string; includeArchived?: boolean; sortBy?: string; sortDescending?: boolean; page?: number; pageSize?: number; }
export interface CustomerListItem { id: number; customerCode: string; type: number; name: string; phone?: string; email?: string; accountName: string; isActive: boolean; }
export interface CustomerListPage { query: Required<Omit<CustomerListQuery, 'search'>> & { search?: string }; items: CustomerListItem[]; totalCount: number; totalPages: number; }
export interface CustomerAccountOption { id: number; code: string; name: string; }
export interface CustomerTaxRateOption { id: number; name: string; rate: number; }
export interface CustomerPriceListOption { id: number; code: string; name: string; currencyCode: string; }
export interface CustomerForm {
  id: number; customerCode?: string; name: string; type: number; legalName?: string; phone?: string; email?: string;
  taxNumber?: string; registrationNumber?: string; website?: string; preferredLanguage: string; timeZoneId: string;
  notes?: string; accountId?: number; paymentTermDays: number; preferredCurrencyCode?: string; defaultTaxRateId?: number;
  isTaxExempt: boolean; taxExemptionReason?: string; defaultSalesPriceListId?: number; rowVersion: string;
}
export interface CustomerFormPage { form: CustomerForm; accounts: CustomerAccountOption[]; currencies: string[]; taxRates: CustomerTaxRateOption[]; priceLists: CustomerPriceListOption[]; }
export interface CustomerContact { id: number; name: string; role?: string; phone?: string; email?: string; isPrimary: boolean; isActive: boolean; }
export interface CustomerAddress { id: number; type: number; label: string; snapshot: string; isDefault: boolean; isActive: boolean; }
export interface CustomerDetails extends CustomerListItem {
  legalName?: string; taxNumber?: string; registrationNumber?: string; website?: string; preferredLanguage: string; timeZoneId: string;
  notes?: string; paymentTermDays: number; preferredCurrencyCode?: string; defaultTaxRateName?: string; defaultSalesPriceListName?: string;
  isTaxExempt: boolean; taxExemptionReason?: string; archivedAtUtc?: string; rowVersion: string; contacts: CustomerContact[]; addresses: CustomerAddress[];
}
