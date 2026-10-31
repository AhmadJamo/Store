export interface Option {
  id: number;
  label: string;
}
export interface TemplateItem {
  sequence: number;
  sectionTitle: string;
  productId: number;
  productName: string;
  measurementUnitId: number;
  unitName: string;
  defaultQuantity: number;
  minimumQuantity: number;
  maximumQuantity: number;
  allowQuantityChange: boolean;
  taxRateId: number | null;
  itemType: number;
  isDefaultSelected: boolean;
  optionGroupCode: string | null;
  optionGroupName: string | null;
  groupMinimumSelections: number;
  groupMaximumSelections: number;
  notes: string | null;
}
export interface CreateTemplateItem {
  sequence: number;
  sectionTitle: string;
  productId: number;
  measurementUnitId: number;
  defaultQuantity: number;
  minimumQuantity: number;
  maximumQuantity: number;
  allowQuantityChange: boolean;
  taxRateId: number | null;
  itemType: number;
  isDefaultSelected: boolean;
  optionGroupCode: string | null;
  optionGroupName: string | null;
  groupMinimumSelections: number;
  groupMaximumSelections: number;
  notes: string | null;
}
export interface CreateTemplate {
  sourceTemplateId: number | null;
  code: string;
  version: number;
  name: string;
  description: string | null;
  category: string | null;
  tags: string | null;
  validityDays: number;
  channel: number;
  salesPriceListId: number | null;
  documentDesignId: number | null;
  allowAdditionalLines: boolean;
  isDefault: boolean;
  introduction: string | null;
  termsAndConditions: string | null;
  footerNotes: string | null;
  items: CreateTemplateItem[];
}
export interface TemplatePage {
  form: CreateTemplate;
  products: Option[];
  units: Option[];
  priceLists: Option[];
  taxRates: Option[];
  designs: Option[];
}
export interface TemplateSummary {
  id: number;
  code: string;
  version: number;
  name: string;
  category: string | null;
  tags: string | null;
  itemCount: number;
  optionalItemCount: number;
  validityDays: number;
  isDefault: boolean;
  status: number;
  rowVersion: string;
}
export interface TemplateDetails {
  id: number;
  code: string;
  version: number;
  name: string;
  description: string | null;
  category: string | null;
  tags: string | null;
  validityDays: number;
  channel: number;
  priceListName: string | null;
  allowAdditionalLines: boolean;
  isDefault: boolean;
  introduction: string | null;
  termsAndConditions: string | null;
  footerNotes: string | null;
  documentDesignId: number | null;
  designName: string | null;
  status: number;
  createdByUserId: string;
  createdAtUtc: string;
  activatedAtUtc: string | null;
  rowVersion: string;
  items: TemplateItem[];
}
