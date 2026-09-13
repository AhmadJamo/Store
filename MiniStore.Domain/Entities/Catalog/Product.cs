namespace MiniStore.Domain.Entities;

public class Product
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Barcode { get; private set; }

    public decimal PurchasePrice { get; private set; }

    public decimal SalePrice { get; private set; }

    public decimal WholesalePrice { get; private set; }

    //State
    public Product(
        string name,
        string barcode,
        decimal purchasePrice,
        decimal salePrice,
        decimal wholesalePrice)
    {
        ValidateName(name);
        ValidateBarcode(barcode);
        ValidatePurchasePrice(purchasePrice);
        ValidateSalePrice(salePrice);
        ValidateWholesalePrice(wholesalePrice);

        ValidatePriceOrder(purchasePrice, wholesalePrice, salePrice);

        Name = name;
        Barcode = barcode;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        WholesalePrice = wholesalePrice;
    }

    //Behavior
    public void ChangeName(string name)
    {
        ValidateName(name);

        Name = name;
    }

    public void ChangeBarcode(string barcode)
    {
        ValidateBarcode(barcode);

        Barcode = barcode;
    }

    public void ChangePurchasePrice(decimal price)
    {
        ValidatePurchasePrice(price);

        if (SalePrice < price)
            throw new ArgumentException(
                "Purchase price cannot be higher than sale price.");

        PurchasePrice = price;
    }

    public void ChangeSalePrice(decimal price)
    {
        ValidateSalePrice(price);

        ValidatePriceOrder(PurchasePrice, WholesalePrice, price);

        SalePrice = price;
    }

    public void ChangeWholesalePrice(decimal price)
    {
        ValidateWholesalePrice(price);
        ValidatePriceOrder(PurchasePrice, price, SalePrice);

        WholesalePrice = price;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Product name is required.");
    }

    private static void ValidateBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException(
                "Product barcode is required.");
    }

    private static void ValidatePurchasePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Purchase price cannot be negative.");
    }

    private static void ValidateSalePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Sale price cannot be negative.");
    }

    private static void ValidateWholesalePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Wholesale price cannot be negative.");
    }

    private static void ValidatePriceOrder(
        decimal purchasePrice,
        decimal wholesalePrice,
        decimal salePrice)
    {
        if (wholesalePrice < purchasePrice ||
            salePrice < wholesalePrice)
        {
            throw new ArgumentException(
                "Prices must satisfy purchase price ≤ wholesale price ≤ retail price.");
        }
    }
}
