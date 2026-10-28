export interface AgreementSourceLine {
  supplierQuotationLineId: number;
  product: string;
  unitName: string;
  quotedQuantity: number;
}
export interface AgreementCreatePage {
  sourcingEventId: number;
  quotationNumber: string;
  supplierName: string;
  warehouseName: string;
  currencyCode: string;
  lines: AgreementSourceLine[];
}
export interface AgreementLine {
  id: number;
  product: string;
  unitName: string;
  maximumQuantity: number;
  releasedQuantity: number;
  remainingQuantity: number;
  unitPrice: number;
  discountPercent: number;
  taxPercent: number;
}
export interface PurchaseAgreement {
  id: number;
  agreementNumber: string;
  supplierName: string;
  warehouseName: string;
  currencyCode: string;
  validFrom: string;
  validUntil: string;
  status: number;
  rowVersion: string;
  lines: AgreementLine[];
}
export interface PurchaseTemplateLine {
  product: string;
  unitName: string;
  quantity: number;
}
export interface PurchaseTemplate {
  id: number;
  templateNumber: string;
  name: string;
  supplierName: string;
  warehouseName: string;
  neededInDays: number;
  priority: number;
  isActive: boolean;
  generationCount: number;
  lines: PurchaseTemplateLine[];
}
