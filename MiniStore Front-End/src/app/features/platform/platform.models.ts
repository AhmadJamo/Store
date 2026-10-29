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
