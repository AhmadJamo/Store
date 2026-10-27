export type ProductAttributeDataType = 1 | 2 | 3 | 4;

export interface ProductAttributeCategory { id: number; name: string; code: string; isActive: boolean; }
export interface ProductAttributeDefinition {
  id: number; name: string; code: string; dataType: ProductAttributeDataType; isRequired: boolean;
  isVariantDefining: boolean; isActive: boolean; displayOrder: number;
  categories: readonly string[]; options: readonly string[];
}
export interface CreateProductAttributeCommand {
  name: string; code: string; dataType: ProductAttributeDataType; isRequired: boolean;
  isVariantDefining: boolean; displayOrder: number; productCategoryIds: readonly number[]; optionsText: string | null;
}
export interface ProductAttributePage {
  form: CreateProductAttributeCommand; definitions: readonly ProductAttributeDefinition[]; categories: readonly ProductAttributeCategory[];
}
export interface ProductAttributeOption { id: number; name: string; code: string; }
export interface ProductAttributeValueField {
  definitionId: number; name: string; code: string; dataType: ProductAttributeDataType;
  isRequired: boolean; isVariantDefining: boolean; value: string | null; options: readonly ProductAttributeOption[];
}
export interface ProductAttributeValuesPage {
  productId: number; productName: string; productCode: string; categoryName: string | null;
  fields: readonly ProductAttributeValueField[];
}
export interface ProductAttributeValueInput { definitionId: number; value: string | null; }
