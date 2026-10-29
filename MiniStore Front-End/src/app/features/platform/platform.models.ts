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
