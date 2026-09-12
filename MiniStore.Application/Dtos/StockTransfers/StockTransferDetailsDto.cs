
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.StockTransfers;

public class StockTransferDetailsDto
{
    public int Id { get; set; }

    public string TransferNumber { get; set; } = string.Empty;

    public int FromWarehouseId { get; set; }

    public string FromWarehouseName { get; set; } = string.Empty;

    public int ToWarehouseId { get; set; }

    public string ToWarehouseName { get; set; } = string.Empty;

    public StockTransferStatus Status { get; set; }

    // ==========================================
    // Created
    // ==========================================

    public string CreatedByUserId { get; set; } = string.Empty;

    public string CreatedByUserName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    // ==========================================
    // General
    // ==========================================

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    // ==========================================
    // Submitted
    // ==========================================

    public string? SubmittedByUserId { get; set; }

    public string? SubmittedByUserName { get; set; }

    public DateTime? SubmittedAt { get; set; }

    // ==========================================
    // Approved
    // ==========================================

    public string? ApprovedByUserId { get; set; }

    public string? ApprovedByUserName { get; set; }

    public DateTime? ApprovedAt { get; set; }

    // ==========================================
    // Rejected
    // ==========================================

    public string? RejectedByUserId { get; set; }

    public string? RejectedByUserName { get; set; }

    public DateTime? RejectedAt { get; set; }

    public string? RejectionReason { get; set; }

    // ==========================================
    // Posted
    // ==========================================

    public string? PostedByUserId { get; set; }

    public string? PostedByUserName { get; set; }

    public DateTime? PostedAt { get; set; }

    // ==========================================
    // Cancelled
    // ==========================================

    public string? CancelledByUserId { get; set; }

    public string? CancelledByUserName { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    // ==========================================
    // Items
    // ==========================================

    public List<StockTransferItemDto> Items { get; set; } = new();

    // ==========================================
    // History
    // ==========================================

    public List<StockTransferHistoryDto> History { get; set; } = new();
}
