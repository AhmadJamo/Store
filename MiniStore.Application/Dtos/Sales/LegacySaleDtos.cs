using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;

namespace MiniStore.Application.DTOs.Sales;

public sealed record LegacySaleOptionDto(int Id, string Label);

public sealed record LegacySaleProductOptionDto(
    int Id,
    string Code,
    string Name,
    string? Barcode,
    decimal WholesalePrice,
    ProductTrackingPolicy TrackingPolicy);

public sealed record LegacySaleTaxOptionDto(
    int Id,
    string Label,
    decimal Rate,
    bool IsPriceInclusive);

public sealed class LegacySaleSummaryDto
{
    public int Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public SaleChannel Channel { get; init; }
    public DateTime Date { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string? CustomerName { get; init; }
    public decimal TotalAmount { get; init; }
}

public sealed class LegacySaleDetailsPageDto
{
    public SaleDetailsDto Sale { get; init; } = new();
    public SaleChannel Channel { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string? CustomerName { get; init; }
    public string PaymentMethodName { get; init; } = string.Empty;
    public bool IsPosted { get; init; }
}

public sealed class LegacySaleCreatePageDto
{
    public CreateSaleDto Form { get; init; } = new() { Date = DateTime.Today };
    public List<LegacySaleProductOptionDto> Products { get; init; } = [];
    public List<LegacySaleOptionDto> Warehouses { get; init; } = [];
    public List<LegacySaleOptionDto> Customers { get; init; } = [];
    public List<LegacySaleOptionDto> PaymentMethods { get; init; } = [];
    public List<LegacySaleTaxOptionDto> TaxRates { get; init; } = [];
}
