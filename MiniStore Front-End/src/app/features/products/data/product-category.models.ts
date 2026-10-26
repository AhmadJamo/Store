export interface ProductCategory {
  id: number;
  name: string;
  code: string;
  isActive: boolean;
}

export interface CreateProductCategoryCommand {
  name: string;
  code: string;
}
