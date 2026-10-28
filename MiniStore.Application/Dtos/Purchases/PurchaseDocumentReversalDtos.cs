using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class CreatePurchaseDocumentReversalDto
{
    [Range(1, 4)]
    public PurchaseDocumentType DocumentType { get; set; }
    [Range(1, int.MaxValue)]
    public int OriginalDocumentId { get; set; }
    public DateOnly ReversalDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Required, StringLength(1000)]
    public string Reason { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 16)]
    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class PurchaseDocumentReversalCreatePageDto
{
    public CreatePurchaseDocumentReversalDto Form { get; init; } = new();
    public string OriginalDocumentNumber { get; init; } = string.Empty;
    public DateOnly OriginalDocumentDate { get; init; }
    public string OriginalController { get; init; } = string.Empty;
    public int? ExistingReversalId { get; init; }
}

public sealed class PurchaseDocumentReversalDto
{
    public int Id { get; init; }
    public string ReversalNumber { get; init; } = string.Empty;
    public PurchaseDocumentType DocumentType { get; init; }
    public int OriginalDocumentId { get; init; }
    public string OriginalDocumentNumber { get; init; } = string.Empty;
    public DateOnly ReversalDate { get; init; }
    public string Reason { get; init; } = string.Empty;
    public PurchaseDocumentReversalStatus Status { get; init; }
    public string CreatedByUserId { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public string? ReversalJournalEntryNumber { get; init; }
    public string OriginalController { get; init; } = string.Empty;
}
