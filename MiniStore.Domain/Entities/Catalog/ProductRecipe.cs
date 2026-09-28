namespace MiniStore.Domain.Entities;

public class ProductRecipe
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int VersionNumber { get; private set; }
    public decimal YieldQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedByUserId { get; private set; }
    public IReadOnlyCollection<RecipeIngredient> Ingredients => _ingredients.AsReadOnly();

    private readonly List<RecipeIngredient> _ingredients = [];

    private ProductRecipe()
    {
        CreatedByUserId = string.Empty;
    }

    public ProductRecipe(
        int productId,
        int versionNumber,
        decimal yieldQuantity,
        string createdByUserId,
        IEnumerable<RecipeIngredient> ingredients)
    {
        if (productId <= 0)
            throw new ArgumentException("Prepared product is required.");
        if (versionNumber <= 0)
            throw new ArgumentException("Recipe version must be greater than zero.");
        if (yieldQuantity <= 0)
            throw new ArgumentException("Recipe yield must be greater than zero.");
        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException("Recipe creator is required.");

        var ingredientList = ingredients?.ToList()
            ?? throw new ArgumentNullException(nameof(ingredients));
        if (ingredientList.Count == 0)
            throw new ArgumentException("A recipe must contain at least one ingredient.");
        if (ingredientList.GroupBy(x => x.IngredientProductId).Any(x => x.Count() > 1))
            throw new ArgumentException("The same ingredient cannot appear more than once.");
        if (ingredientList.Any(x => x.IngredientProductId == productId))
            throw new ArgumentException("A product cannot be an ingredient in its own recipe.");

        ProductId = productId;
        VersionNumber = versionNumber;
        YieldQuantity = yieldQuantity;
        CreatedByUserId = createdByUserId.Trim();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        _ingredients.AddRange(ingredientList);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
