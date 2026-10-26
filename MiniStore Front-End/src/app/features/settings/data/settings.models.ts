export interface OrganizationIdentity {
  tradeName: string; tradeNameArabic?: string; legalName?: string; legalNameArabic?: string;
  commercialRegistrationNumber?: string; taxRegistrationNumber?: string; email?: string; website?: string;
  phone?: string; secondaryPhone?: string; address?: string; addressArabic?: string; city?: string; cityArabic?: string;
  stateOrProvince?: string; postalCode?: string; countryCode?: string; documentNameStyle: number;
  hasLogo: boolean; logoVersion?: string; rowVersion: string; customFields: OrganizationIdentityField[];
}
export interface OrganizationIdentityField {
  sequence: number; section: number; label: string; labelArabic?: string; value: string; valueArabic?: string;
  showOnSalesDocuments: boolean; showOnPurchaseDocuments: boolean; showOnReceipts: boolean; showInFooter: boolean;
}
export interface GeneralPreferences { currency: string; decimalPlaces: number; quantityDecimalPlaces: number; dateFormat: string; timeZone: string; defaultLanguage: number; rowVersion: string; }
export interface DiscountSettings {
  enabled: boolean; allowLineDiscount: boolean; allowInvoiceDiscount: boolean; allowPercentageDiscount: boolean;
  allowFixedAmountDiscount: boolean; defaultDiscountType: number; maxLineDiscountPercent: number;
  maxLineDiscountAmount: number; maxInvoiceDiscountPercent: number; maxInvoiceDiscountAmount: number;
  allowDiscountAboveLimit: boolean; discountOverridePermission: string; rowVersion: string;
}
export interface PurchaseMatchingSettings { quantityTolerancePercent: number; priceTolerancePercent: number; rowVersion: string; }
export interface InventorySettings {
  defaultWarehouseType: number; defaultControlMode: number; defaultPickingStrategy: number;
  defaultAllowPosSales: boolean; defaultEnforceLocationCapacity: boolean;
  defaultRequireSourceLocationForTransfers: boolean; defaultRequireDestinationLocationForTransfers: boolean;
  rowVersion: string;
}
export interface PurchaseSourcingPolicy { allowDirectRfq: boolean; maximumEstimatedAmountCompanyCurrency: number; allowedRoleName?: string; requireActiveSuppliers: boolean; availableRoles: string[]; companyCurrencyCode: string; rowVersion: string; }
export interface DocumentSequence { documentType:number; nameEnglish:string; nameArabic:string; prefix:string; suffix:string; formatTemplate:string; numberLength:number; nextNumber:number; resetStartNumber:number; resetPeriod:number; preview:string; rowVersion:string; }
export interface DocumentNumberSettings { sequences: DocumentSequence[]; }
export interface FiscalPeriod { id:number; name:string; startDate:string; endDate:string; status:number; statusChangeReason?:string; rowVersion:string; }
export interface FiscalPeriodsPage { periods:FiscalPeriod[]; newPeriod:{name:string;startDate:string;endDate:string}; }
export interface FiscalPeriodStatusCommand { id:number;status:number;reason:string;rowVersion:string; }
export interface CurrencyExchangeRate { currencyCode:string; effectiveDate:string; companyCurrencyPerUnit:number; source:string; }
export interface SaveCurrencyExchangeRate { currencyCode:string; effectiveDate:string; companyCurrencyPerUnit:number; source:string; }
export interface CurrencyExchangeRatesPage { companyCurrencyCode:string; form:SaveCurrencyExchangeRate; rates:CurrencyExchangeRate[]; }
export interface MeasurementUnit { id:number; code:string; name:string; symbol:string; dimension:number; factorToBaseUnit:number; decimalPlaces:number; isSystem:boolean; isActive:boolean; }
export interface CreateMeasurementUnit { code:string; name:string; symbol:string; dimension:number; factorToBaseUnit:number; decimalPlaces:number; }
export interface MeasurementUnitsPage { units:MeasurementUnit[]; newUnit:CreateMeasurementUnit; }
export interface AccountOption { id:number; code:string; name:string; type:number; parentAccountId:number|null; }
export interface AccountingSettings { purchaseDiscountAccountId:number|null; salesDiscountAccountId:number|null; salesRevenueAccountId:number|null; costOfSalesAccountId:number|null; goodsReceivedNotInvoicedAccountId:number|null; purchasePriceVarianceAccountId:number|null; landedCostClearingAccountId:number|null; realizedExchangeGainAccountId:number|null; realizedExchangeLossAccountId:number|null; customerReceivableAccountMode:number|null; customerReceivableControlAccountId:number|null; customerReceivableParentAccountId:number|null; customerAccountCodePrefix:string; customerPolicyEffectiveFrom:string; salesInvoiceQuantityPolicy:number; }
export interface AccountingSettingsPage { settings:AccountingSettings; accounts:AccountOption[]; }
export interface BranchSalesAccount { id:number; code:string; name:string; isActive:boolean; salesRevenueAccountId:number|null; }
export interface BranchSalesAccountsPage { branches:BranchSalesAccount[]; accounts:AccountOption[]; }
export interface InventoryAccessBranch { id:number; displayName:string; }
export interface InventoryAccessWarehouse { id:number; name:string; allowPosSales:boolean; }
export interface BranchWarehouseAccess { branchId:number; branchName:string; warehouseId:number; warehouseName:string; priority:number; isDefaultForPos:boolean; allowPosSales:boolean; allowPurchases:boolean; allowTransferOut:boolean; allowTransferIn:boolean; allowReplenishment:boolean; }
export interface PosTerminalWarehouse { warehouseId:number; warehouseName:string; priority:number; }
export interface PosTerminalAccess { id:number; name:string; branchId:number; branchName:string; defaultWarehouseId:number; defaultWarehouseName:string; isActive:boolean; warehouses:PosTerminalWarehouse[]; }
export interface InventoryAccessSettings { branches:InventoryAccessBranch[]; warehouses:InventoryAccessWarehouse[]; branchAccesses:BranchWarehouseAccess[]; posTerminals:PosTerminalAccess[]; }
export interface ConfigureBranchWarehouse { branchId:number; warehouseId:number; priority:number; isDefaultForPos:boolean; allowPosSales:boolean; allowPurchases:boolean; allowTransferOut:boolean; allowTransferIn:boolean; allowReplenishment:boolean; }
export interface CreatePosTerminal { name:string; branchId:number; defaultWarehouseId:number; }
export interface ConfigurePosTerminalWarehouse { posTerminalId:number; warehouseId:number; priority:number; makeDefault:boolean; }
export interface PosExperienceTerminal { id:number; displayName:string; }
export interface PosExperienceSettings { posTerminalId:number; terminalName:string; profile:number; productLayout:number; theme:number; cartPosition:number; accentColor:string; headerTitle:string; productColumns:number; compactProductCards:boolean; showBarcode:boolean; showPrice:boolean; showStock:boolean; autoFocusSearch:boolean; touchOptimized:boolean; enableWalkIn:boolean; enableDineIn:boolean; enableTakeaway:boolean; enableDelivery:boolean; defaultOrderType:number; requireServiceReference:boolean; enableGuestCount:boolean; enableItemNotes:boolean; quickAddOnBarcodeScan:boolean; applyProfileDefaults:boolean; rowVersion:string; }
export interface PosExperienceSettingsPage { selectedTerminalId:number|null; terminals:PosExperienceTerminal[]; settings:PosExperienceSettings|null; }
export interface WorkflowCatalogStep { module:string; number:number; phase:string; title:string; responsibleRole:string; reviewerRole:string; instructions:string; controlPrinciple:string; completionEvidence:string; conditional:boolean; canOpen:boolean; workUrl:string|null; }
export interface WorkflowCatalogPage { moduleName:string; purpose:string; steps:WorkflowCatalogStep[]; }
