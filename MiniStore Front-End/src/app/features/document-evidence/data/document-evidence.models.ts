export type DocumentKind = 'sales-order' | 'customer-invoice';
export interface DocumentEvidenceListItem { kind: DocumentKind; id: number; number: string; party: string; status: string; documentDate: string; currencyCode: string; totalAmount: number; }
export interface DocumentJourneyStep { eventKey: string; fromStatus: string; toStatus: string; actorUserId: string; occurredAtUtc: string; reason?: string; }
export interface RelatedDocument { kind: string; id: number; number: string; relationshipKey: string; canOpen: boolean; }
export interface ExplainNumberPart { partKey: string; amount: number; operation: string; sourceKey: string; }
export interface ExplainNumber { numberKey: string; value: number; currencyCode: string; decimalPlaces: number; formulaKey: string; parts: readonly ExplainNumberPart[]; }
export interface DocumentEvidence extends DocumentEvidenceListItem { journey: readonly DocumentJourneyStep[]; relatedDocuments: readonly RelatedDocument[]; explainedNumbers: readonly ExplainNumber[]; }
