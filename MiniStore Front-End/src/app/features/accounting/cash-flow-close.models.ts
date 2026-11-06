export type CashFlowCloseView='cash-flow'|'close-readiness';
export interface CashFlowCloseQuery{fromDate:string;toDate:string;view:CashFlowCloseView}
export interface CashFlowSummary{openingCash:number;operating:number;investing:number;financing:number;unclassified:number;netChange:number;closingCash:number}
export interface CashFlowLine{entryId:number;entryNumber:string;date:string;description:string;sourceType:string;sourceReference:string;classification:string;amount:number;evidenceRoute:string}
export interface CloseReadinessCheck{code:string;status:string;count:number;difference:number;evidenceRoute:string}
export interface CashFlowClosePage{fromDate:string;toDate:string;companyCurrencyCode:string;cashAccountCount:number;summary:CashFlowSummary;movements:CashFlowLine[];readiness:CloseReadinessCheck[]}
