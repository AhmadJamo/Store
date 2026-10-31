export interface RecipeListItem { productId: number; productName: string; inventoryBehavior: number; activeVersion: number | null; yieldQuantity: number | null; ingredientCount: number; }
export interface RecipeIngredientInput { ingredientProductId: number; quantity: number; measurementUnitId: number; }
export interface RecipeIngredientOption { productId: number; productName: string; stockMeasurementUnitId: number; stockUnitName: string; stockUnitSymbol: string; stockUnitDimension: number; allowNegativeRecipeConsumption: boolean; }
export interface RecipeMeasurementUnit { id: number; code: string; name: string; symbol: string; dimension: number; decimalPlaces: number; isActive: boolean; }
export interface RecipeEditor { productId: number; productName: string; activeVersion: number | null; yieldQuantity: number; ingredients: readonly RecipeIngredientInput[]; ingredientOptions: readonly RecipeIngredientOption[]; measurementUnits: readonly RecipeMeasurementUnit[]; }
