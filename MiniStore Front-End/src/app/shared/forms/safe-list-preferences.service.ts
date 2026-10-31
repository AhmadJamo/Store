import { Injectable } from '@angular/core';

export interface CustomerListPreferences { readonly includeArchived: boolean; readonly sortBy: 'code' | 'name' | 'status'; readonly sortDescending: boolean; readonly pageSize: 10 | 25 | 50 | 100; }
export interface SupplierListPreferences { readonly includeInactive: boolean; readonly pageSize: 10 | 25 | 50 | 100; }
type PreferenceScope = 'customers' | 'suppliers';
interface StoredListPreferences { readonly version: 1; readonly customers?: CustomerListPreferences; readonly suppliers?: SupplierListPreferences; }
const storageKey = 'ministore.ui.list-preferences.v1';
const pageSizes = [10, 25, 50, 100] as const;

@Injectable({ providedIn: 'root' })
export class SafeListPreferencesService {
  loadCustomers(): CustomerListPreferences { const value = this.read().customers; return { includeArchived: value?.includeArchived === true, sortBy: ['code', 'name', 'status'].includes(value?.sortBy ?? '') ? value!.sortBy : 'name', sortDescending: value?.sortDescending === true, pageSize: pageSizes.includes(value?.pageSize as never) ? value!.pageSize : 25 }; }
  loadSuppliers(): SupplierListPreferences { const value = this.read().suppliers; return { includeInactive: value?.includeInactive === true, pageSize: pageSizes.includes(value?.pageSize as never) ? value!.pageSize : 25 }; }
  saveCustomers(value: CustomerListPreferences): void { this.write({ ...this.read(), version: 1, customers: this.cleanCustomers(value) }); }
  saveSuppliers(value: SupplierListPreferences): void { this.write({ ...this.read(), version: 1, suppliers: this.cleanSuppliers(value) }); }
  reset(scope: PreferenceScope): void { const current = this.read(); if (scope === 'customers') delete (current as { customers?: CustomerListPreferences }).customers; else delete (current as { suppliers?: SupplierListPreferences }).suppliers; this.write(current); }
  private cleanCustomers(value: CustomerListPreferences): CustomerListPreferences { return { includeArchived: value.includeArchived === true, sortBy: ['code', 'name', 'status'].includes(value.sortBy) ? value.sortBy : 'name', sortDescending: value.sortDescending === true, pageSize: pageSizes.includes(value.pageSize) ? value.pageSize : 25 }; }
  private cleanSuppliers(value: SupplierListPreferences): SupplierListPreferences { return { includeInactive: value.includeInactive === true, pageSize: pageSizes.includes(value.pageSize) ? value.pageSize : 25 }; }
  private read(): StoredListPreferences { try { const parsed = JSON.parse(globalThis.localStorage?.getItem(storageKey) ?? 'null') as Partial<StoredListPreferences> | null; return parsed?.version === 1 ? parsed as StoredListPreferences : { version: 1 }; } catch { try { globalThis.localStorage?.removeItem(storageKey); } catch { /* in-memory fallback */ } return { version: 1 }; } }
  private write(value: StoredListPreferences): void { try { globalThis.localStorage?.setItem(storageKey, JSON.stringify(value)); } catch { /* in-memory fallback */ } }
}
