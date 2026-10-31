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
  limits: Record<string, number | null>;
  features: string[];
}
