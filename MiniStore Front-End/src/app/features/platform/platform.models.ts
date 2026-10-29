export interface PlatformSession {
  username: string;
  role: string;
  canManage: boolean;
  canBill: boolean;
  canViewAudit: boolean;
  canExportAudit: boolean;
  canViewSensitiveAudit: boolean;
  canManageRetention: boolean;
}

export interface PlatformPlan {
  id: number;
  code: string;
  nameEnglish: string;
  nameArabic: string;
  descriptionEnglish: string;
  descriptionArabic: string;
  monthlyPrice: number;
  annualPrice: number;
  currency: string;
  limits: Record<string, number | null>;
  features: string[];
}

export interface PlatformOverview {
  session: PlatformSession;
  plans: PlatformPlan[];
}
export interface PlatformCompany { tenantId:number; name:string; slug:string; isActive:boolean; planId:number|null; planName:string; status:number|null; periodEnd:string|null; userCount:number; }
export interface PlatformPromotion { id:number; code:string; discountPercentage:number; startsAt:string; endsAt:string; maximumRedemptions:number|null; redemptionCount:number; planId:number|null; isActive:boolean; }
export interface PendingPlatformPayment { sessionId:string; companyName:string; planNameEnglish:string; planNameArabic:string; total:number; currency:string; expiresAt:string; }
export interface PlatformManagementPage { plans:PlatformPlan[]; companies:PlatformCompany[]; promotions:PlatformPromotion[]; pendingPayments:PendingPlatformPayment[]; }
export interface UpdatePlatformCompany { tenantId:number; planId:number; status:number; periodEnd:string; companyIsActive:boolean; }
export interface CreatePlatformPlan { code:string; nameEnglish:string; nameArabic:string; descriptionEnglish:string; descriptionArabic:string; monthlyPrice:number; annualPrice:number; currency:string; maxUsers:number; maxWarehouses:number; maxBranches:number; maxPosTerminals:number; maxProducts:number; }
export interface CreatePlatformPromotion { code:string; discountPercentage:number; startsAt:string; endsAt:string; maximumRedemptions:number|null; planId:number|null; }
export interface PlatformAuditQuery { from?:string; to?:string; category?:number|null; outcome?:number|null; eventType?:string; actorId?:string; resourceType?:string; resourceId?:string; correlationId?:string; page:number; pageSize:number; }
export interface PlatformAuditEvent { id:number; eventId:string; scope:number; occurredAtUtc:string; category:number; severity:number; outcome:number; eventType:string; action:string; actorType:number; actorId:string; sourceChannel:number; correlationId:string; resourceType:string; resourceId:string; reasonCode:string|null; changesRedactedJson:string|null; metadataRedactedJson:string|null; requestPathTemplate:string|null; sourceIpMasked:string|null; userAgentHash:string|null; }
export interface PlatformAuditPage { query:PlatformAuditQuery; items:PlatformAuditEvent[]; totalCount:number; canViewSensitive:boolean; totalPages:number; }
export interface PlatformAuditPolicy { enabled:boolean; protectedRetentionYears:number; operationalRetentionYears:number; requestEvidenceDays:number; batchSize:number; }
export interface PlatformAuditHold { id:number; holdId:string; scope:number; tenantId:number|null; fromUtc:string; toUtc:string; reasonRedacted:string; createdBy:string; createdAtUtc:string; isActive:boolean; releasedBy:string|null; releaseReasonRedacted:string|null; releasedAtUtc:string|null; }
export interface PlatformAuditRetentionStatus { policy:PlatformAuditPolicy; holds:PlatformAuditHold[]; }
export interface CreatePlatformAuditHold { scope:number; tenantId:number|null; fromUtc:string; toUtc:string; reason:string; }
