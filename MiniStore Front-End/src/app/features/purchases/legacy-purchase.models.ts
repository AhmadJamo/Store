export interface LegacyPurchaseItem { id:number; productId:number; warehouseId:number|null; warehouseName:string; productName:string; quantity:number; purchasePrice:number; discountAmount:number; taxRateId:number|null; total:number }
export interface LegacyPurchase { id:number; supplierId:number; supplierName:string; warehouseId:number; warehouseName:string; invoiceNumber:string; date:string; notes:string|null; totalAmount:number; items:LegacyPurchaseItem[] }
export interface LegacyPurchaseDetailsPage { purchase:LegacyPurchase; isPosted:boolean }
export interface LegacyPurchaseReturnItem { purchaseItemId:number; productId:number; productName:string; warehouseName:string; quantity:number; payableAmount:number; removedInventoryCost:number }
export interface LegacyPurchaseReturn { id:number; returnNumber:string; purchaseId:number; purchaseInvoiceNumber:string; supplierName:string; date:string; reason:string; payableAmount:number; taxAmount:number; removedInventoryCost:number; items:LegacyPurchaseReturnItem[] }
export interface LegacyPurchaseReturnCreateLine { purchaseItemId:number; productName:string; warehouseName:string; purchasedQuantity:number; alreadyReturnedQuantity:number; remainingQuantity:number }
export interface LegacyPurchaseReturnCreatePage { purchase:LegacyPurchase; date:string; lines:LegacyPurchaseReturnCreateLine[] }
export interface CreateLegacyPurchaseReturn { purchaseId:number; date:string; reason:string; items:{purchaseItemId:number;quantity:number;trackingAllocations:string}[] }
