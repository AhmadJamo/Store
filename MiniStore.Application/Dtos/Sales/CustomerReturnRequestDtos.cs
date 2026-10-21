using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed record CustomerReturnRequestSummaryDto(int Id, string ReturnNumber, string OrderNumber, string CustomerName, DateOnly RequestDate, CustomerReturnRequestStatus Status, decimal Quantity);
public sealed record CustomerReturnRequestLineDto(int Id, int SalesDeliveryLineId, string ProductCode, string ProductName, string WarehouseName, string LocationName, decimal DeliveredQuantity, decimal RequestedQuantity, string? TrackingAllocations);
public sealed record CustomerReturnRequestHistoryDto(CustomerReturnRequestStatus FromStatus, CustomerReturnRequestStatus ToStatus, CustomerReturnRequestHistoryAction Action, string UserId, string? Reason, DateTime CreatedAtUtc);
public sealed class CustomerReturnRequestDetailsDto
{
    public int Id { get; init; } public string ReturnNumber { get; init; } = string.Empty; public int SalesDeliveryId { get; init; } public string DeliveryNumber { get; init; } = string.Empty;
    public int SalesOrderId { get; init; } public string OrderNumber { get; init; } = string.Empty; public string CustomerName { get; init; } = string.Empty;
    public DateOnly RequestDate { get; init; } public string Reason { get; init; } = string.Empty; public string? Notes { get; init; } public CustomerReturnRequestStatus Status { get; init; }
    public string? DecisionReason { get; init; } public string? CancellationReason { get; init; } public byte[] RowVersion { get; init; } = [];
    public List<CustomerReturnRequestLineDto> Lines { get; init; } = []; public List<CustomerReturnRequestHistoryDto> History { get; init; } = [];
    public bool HasRemainingReceipt { get; init; } public List<int> ReceiptIds { get; init; } = [];
}
public sealed class CreateCustomerReturnRequestLineDto { [Range(1, int.MaxValue)] public int SalesDeliveryLineId { get; set; } [Range(typeof(decimal), "0", "999999999999.999999")] public decimal StockQuantity { get; set; } }
public sealed class CreateCustomerReturnRequestDto
{
    [Range(1, int.MaxValue)] public int SalesDeliveryId { get; set; } public DateOnly RequestDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty; [StringLength(1000)] public string? Notes { get; set; }
    public List<CreateCustomerReturnRequestLineDto> Lines { get; set; } = [];
}
public sealed record CustomerReturnEligibleLineDto(int SalesDeliveryLineId, string ProductCode, string ProductName, string WarehouseName, string LocationName, decimal DeliveredQuantity, decimal AlreadyRequestedQuantity, decimal RemainingQuantity);
public sealed class CustomerReturnRequestCreatePageDto { public int SalesDeliveryId { get; init; } public string DeliveryNumber { get; init; } = string.Empty; public string OrderNumber { get; init; } = string.Empty; public string CustomerName { get; init; } = string.Empty; public DateOnly DeliveryDate { get; init; } public CreateCustomerReturnRequestDto Form { get; init; } = new(); public List<CustomerReturnEligibleLineDto> Lines { get; init; } = []; }
public sealed class CustomerReturnRequestCommandDto { [Range(1, int.MaxValue)] public int Id { get; set; } public byte[] RowVersion { get; set; } = []; [StringLength(500)] public string? Reason { get; set; } }
