export interface SupplierListQuery { search?: string; includeInactive?: boolean; page?: number; pageSize?: number; }
export interface SupplierListItem { id: number; name: string; legalName?: string; contactPerson?: string; phone?: string; email?: string; paymentTermsDays: number; isActive: boolean; }
export interface SupplierListPage { items: SupplierListItem[]; query: Required<Omit<SupplierListQuery, 'search'>> & { search?: string }; totalCount: number; totalPages: number; }
export interface SupplierDetails extends SupplierListItem { address?: string; taxNumber?: string; registrationNumber?: string; website?: string; notes?: string; accountId?: number; productIds: number[]; productCount: number; rowVersion: string; }
export interface SupplierAccountOption { id: number; code: string; name: string; }
export interface SupplierProductOption { id: number; code: string; name: string; }
export interface SupplierFormPage { form: SupplierDetails; accounts: SupplierAccountOption[]; products: SupplierProductOption[]; }
export interface SupplierCommand { name: string; legalName?: string; contactPerson?: string; phone?: string; email?: string; address?: string; taxNumber?: string; registrationNumber?: string; website?: string; paymentTermsDays: number; notes?: string; isActive: boolean; accountId: number; productIds: number[]; rowVersion?: string; }
