export interface MasterAccount { id:number; code:string; name:string; type:number; parentAccountId:number|null; }
export interface TaxRateRow { id:number; name:string; rate:number; outputAccountName:string; inputAccountName:string; isPriceInclusive:boolean; }
export interface PaymentMethodRow { id:number; name:string; accountName:string; isActive:boolean; }
export interface BranchRow { id:number; code:string; name:string; isActive:boolean; }
export interface CatalogMastersPage { taxRates:readonly TaxRateRow[]; paymentMethods:readonly PaymentMethodRow[]; branches:readonly BranchRow[]; accounts:readonly MasterAccount[]; }
export interface CreateTaxRateCommand { name:string; rate:number; outputAccountId:number; inputAccountId:number; isPriceInclusive:boolean; }
export interface CreatePaymentMethodCommand { name:string; accountId:number; }
export interface CreateBranchCommand { code:string; name:string; }
