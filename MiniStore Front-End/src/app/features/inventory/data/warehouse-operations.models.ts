export interface NamedOption { id: number; name: string }
export interface LocationOption { id: number; warehouseId: number; label?: string; code?: string }
export interface StorageLocationRow { id:number; warehouseId:number; parentLocationId?:number; code:string; name:string; zone?:string; aisle?:string; rack?:string; level?:string; bin?:string; type:number; status:number; isReceivable:boolean; isPickable:boolean; isReservable:boolean; isShippable:boolean; isCountable:boolean }
export interface WarehouseLocationPage { result:{locations:StorageLocationRow[];locationDepths:Record<number,number>;products:unknown[]};options:{warehouses:NamedOption[];parentLocations:LocationOption[]} }
export interface LocationCommand { warehouseId:number;code:string;name:string;barcode?:string;parentLocationId?:number;sequence:number;zone?:string;aisle?:string;rack?:string;level?:string;bin?:string;type:number;maximumQuantity?:number;isReceivable:boolean;isPickable:boolean;isReservable:boolean;isShippable:boolean;isCountable:boolean }
export interface EditLocationCommand extends Omit<LocationCommand,'zone'|'aisle'|'rack'|'level'|'bin'|'type'|'maximumQuantity'>{id:number}
export interface LocationEditPage {input:EditLocationCommand;parentLocations:LocationOption[]}
export interface UnassignedRow {productId:number;productName:string;barcode:string;warehouseId:number;warehouseName:string;warehouseQuantity:number;assignedQuantity:number;unassignedQuantity:number;suggestedLocationId?:number;suggestedLocationCode?:string}
export interface PutawayRule {id:number;warehouseId:number;warehouseName:string;storageLocationId:number;storageLocationCode:string;target:string;priority:number;isActive:boolean}
export interface UnassignedPage {rows:UnassignedRow[];warehouses:NamedOption[];locations:LocationOption[];products:NamedOption[];categories:NamedOption[];rules:PutawayRule[]}
export interface LocationMovement {id:number;productName:string;barcode:string;warehouseName:string;fromLocationCode:string;toLocationCode:string;quantity:number;type:number;reference?:string;notes?:string;createdByUserId:string;createdAt:string}
export interface SourceStock {productId:number;warehouseId:number;storageLocationId:number;quantity:number}
export interface LocationMovementPage {form:unknown;warehouseFilterId?:number;query?:string;movements:LocationMovement[];warehouses:NamedOption[];products:(NamedOption&{barcode:string})[];locations:LocationOption[];sourceStocks:SourceStock[]}
export interface StockMovement {id:number;locationMovementId?:number;productName:string;warehouseName:string;relatedWarehouseName?:string;fromLocation:string;toLocation:string;quantity:number;type:number;status:number;reference?:string;createdAt:string;postedAt?:string;sourceDocumentType?:string;sourceDocumentId?:number;stageSequence?:number}
export interface StockMovementPage {rows:StockMovement[];legacyMovementCount:number;linkedMovementCount:number;totalMovementCount:number;isReconciled:boolean}
export interface MoveCommand {idempotencyKey:string;warehouseId:number;productId:number;fromStorageLocationId:number;toStorageLocationId:number;quantity:number;reference?:string;notes?:string}
