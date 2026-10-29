export interface SubscriptionSummary {
  planId: number;
  planNameEnglish: string;
  planNameArabic: string;
  status: number;
  billingCycle: number;
  currentPeriodStart: string;
  currentPeriodEnd: string;
  gracePeriodEnd: string | null;
  cancelAtPeriodEnd: boolean;
  allowsUse: boolean;
}

export interface SubscriptionPlan {
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

export interface SubscriptionPage {
  currentSubscription: SubscriptionSummary | null;
  plans: SubscriptionPlan[];
}

export interface CreateSubscriptionCheckout {
  planId: number;
  cycle: number;
  promotionCode: string | null;
}

export interface SubscriptionCheckoutQuote {
  sessionId: string;
  planId: number;
  planNameEnglish: string;
  planNameArabic: string;
  cycle: number;
  subtotal: number;
  discountAmount: number;
  total: number;
  currency: string;
  promotionCode: string | null;
  expiresAt: string;
}
