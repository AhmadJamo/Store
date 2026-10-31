export interface ProductTemplateCategory { id: number; name: string; code: string; isActive: boolean; }
export interface ProductTemplateProduct { id: number; name: string; productCode: string; productCategoryId: number | null; productTemplateId: number | null; }
export interface ProductVariant { productId: number; productCode: string; productName: string; variantLabel: string | null; }
export interface VariantOption { id: number; name: string; code: string; }
export interface VariantAttribute { definitionId: number; name: string; options: readonly VariantOption[]; }
export interface ProductTemplate { id: number; name: string; code: string; categoryName: string; isActive: boolean; variants: readonly ProductVariant[]; generationAttributes: readonly VariantAttribute[]; }
export interface VariantPreviewRow { label: string; alreadyExists: boolean; }
export interface VariantPreview { productTemplateId: number; sourceProductId: number; optionIds: readonly number[]; rows: readonly VariantPreviewRow[]; existingCount: number; newCount: number; }
export interface ProductTemplatePage { templates: readonly ProductTemplate[]; products: readonly ProductTemplateProduct[]; categories: readonly ProductTemplateCategory[]; preview: VariantPreview | null; }
export interface CreateProductTemplateCommand { name: string; code: string; productCategoryId: number; }
export interface AssignProductTemplateCommand { productId: number; productTemplateId: number | null; }
export interface VariantGenerationCommand { productTemplateId: number; sourceProductId: number; optionIds: readonly number[]; }
export interface VariantGenerationResult { createdCount: number; page: ProductTemplatePage; }
