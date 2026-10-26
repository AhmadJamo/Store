export interface PublicPlan {
  id: number;
  code: string;
  nameEnglish: string;
  nameArabic: string;
  descriptionEnglish: string;
  descriptionArabic: string;
  monthlyPrice: number;
  annualPrice: number;
  currency: string;
  limits: Readonly<Record<string, number | null>>;
  features: readonly string[];
}

export interface CompanyRegistrationPage {
  plans: readonly PublicPlan[];
}

export interface RegisterCompanyRequest {
  companyName: string;
  companySlug: string;
  ownerUsername: string;
  ownerEmail: string;
  password: string;
  planId: number;
}

export interface CompanyRegistrationResult {
  requiresCompanyOnboarding: boolean;
}

export interface BusinessTypeOption {
  value: number;
  nameEnglish: string;
  nameArabic: string;
  descriptionEnglish: string;
  descriptionArabic: string;
  recommendedPos: boolean;
}

export interface CompanyOnboardingPage {
  tenantId: number;
  tenantName: string;
  status: number;
  businessTypes: readonly BusinessTypeOption[];
}

export interface CompleteCompanyOnboardingRequest {
  businessType: number;
  countryCode: string;
  currency: string;
  language: number;
  fiscalYearStartMonth: number;
  isTaxRegistered: boolean;
  defaultTaxRate: number;
  usePos: boolean;
  inventoryControlMode: number;
}
