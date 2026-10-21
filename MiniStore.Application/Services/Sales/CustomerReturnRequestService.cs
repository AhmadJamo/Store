using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class CustomerReturnRequestService(ICustomerReturnRequestRepository requests, ISalesDeliveryRepository deliveries,
    ISalesOrderRepository orders, IWarehouseRepository warehouses, IStorageLocationRepository locations,
    DocumentNumberService numbers, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<CustomerReturnRequestSummaryDto>> GetAllAsync(string? search) => (await requests.GetAllAsync(search)).Select(x => new CustomerReturnRequestSummaryDto(x.Id, x.ReturnNumber, x.OrderNumberSnapshot, x.CustomerNameSnapshot, x.RequestDate, x.Status, x.TotalRequestedQuantity)).ToList();
    public async Task<CustomerReturnRequestDetailsDto?> GetAsync(int id)
    {
        var request = await requests.GetByIdAsync(id); if (request is null) return null;
        var delivery = await deliveries.GetByIdAsync(request.SalesDeliveryId); var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name); var locationMap = (await locations.SearchAsync(null, null)).ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}"); var deliveryLines = delivery?.Lines.ToDictionary(x => x.Id) ?? [];
        return new() { Id = request.Id, ReturnNumber = request.ReturnNumber, SalesDeliveryId = request.SalesDeliveryId, DeliveryNumber = delivery?.DeliveryNumber ?? $"#{request.SalesDeliveryId}", SalesOrderId = request.SalesOrderId, OrderNumber = request.OrderNumberSnapshot, CustomerName = request.CustomerNameSnapshot, RequestDate = request.RequestDate, Reason = request.Reason, Notes = request.Notes, Status = request.Status, DecisionReason = request.DecisionReason, CancellationReason = request.CancellationReason, RowVersion = request.RowVersion,
            Lines = request.Lines.Select(x => new CustomerReturnRequestLineDto(x.Id, x.SalesDeliveryLineId, x.ProductCodeSnapshot, x.ProductNameSnapshot, warehouseMap.GetValueOrDefault(x.WarehouseId, "—"), x.StorageLocationId.HasValue ? locationMap.GetValueOrDefault(x.StorageLocationId.Value, "—") : "—", deliveryLines.GetValueOrDefault(x.SalesDeliveryLineId)?.StockQuantity ?? 0, x.StockQuantity, x.TrackingAllocationsSnapshot)).ToList(),
            History = request.History.OrderBy(x => x.CreatedAtUtc).Select(x => new CustomerReturnRequestHistoryDto(x.FromStatus, x.ToStatus, x.Action, x.UserId, x.Reason, x.CreatedAtUtc)).ToList() };
    }
    public async Task<CustomerReturnRequestCreatePageDto?> GetCreatePageAsync(int deliveryId, CreateCustomerReturnRequestDto? form = null)
    {
        var delivery = await deliveries.GetByIdAsync(deliveryId); if (delivery is null || delivery.Status != SalesDeliveryStatus.Posted) return null;
        var order = await orders.GetByIdAsync(delivery.SalesOrderId); if (order is null) return null;
        var committed = await requests.GetActiveRequestedQuantitiesAsync(delivery.Id); var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name); var locationMap = (await locations.SearchAsync(null, null)).ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}");
        return new() { SalesDeliveryId = delivery.Id, DeliveryNumber = delivery.DeliveryNumber, DeliveryDate = delivery.DeliveryDate, OrderNumber = order.OrderNumber, CustomerName = order.CustomerName, Form = form ?? new() { SalesDeliveryId = delivery.Id },
            Lines = delivery.Lines.Select(x => new CustomerReturnEligibleLineDto(x.Id, x.ProductCodeSnapshot, x.ProductNameSnapshot, warehouseMap.GetValueOrDefault(x.WarehouseId, "—"), x.StorageLocationId.HasValue ? locationMap.GetValueOrDefault(x.StorageLocationId.Value, "—") : "—", x.StockQuantity, committed.GetValueOrDefault(x.Id), Math.Max(0, x.StockQuantity - committed.GetValueOrDefault(x.Id)))).Where(x => x.RemainingQuantity > 0).ToList() };
    }
    public async Task<int> CreateAsync(CreateCustomerReturnRequestDto dto)
    {
        CustomerReturnRequest? created = null; await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var delivery = await deliveries.GetByIdAsync(dto.SalesDeliveryId) ?? throw new InvalidOperationException("Sales delivery was not found.");
            if (delivery.Status != SalesDeliveryStatus.Posted) throw new InvalidOperationException("Only a posted sales delivery can be returned.");
            if (dto.RequestDate < delivery.DeliveryDate) throw new ArgumentException("Return request date cannot be earlier than the delivery date.");
            var selected = dto.Lines?.Where(x => x.StockQuantity > 0).ToList() ?? []; if (selected.Count == 0) throw new ArgumentException("Enter at least one positive return quantity."); if (selected.GroupBy(x => x.SalesDeliveryLineId).Any(x => x.Count() > 1)) throw new ArgumentException("A delivery line can appear only once per return request.");
            var order = await orders.GetByIdAsync(delivery.SalesOrderId) ?? throw new InvalidOperationException("Sales order was not found."); var committed = await requests.GetActiveRequestedQuantitiesAsync(delivery.Id); var lineMap = delivery.Lines.ToDictionary(x => x.Id);
            created = new(await numbers.GenerateAsync(DocumentNumberType.CustomerReturnRequest, dto.RequestDate.ToDateTime(TimeOnly.MinValue)), delivery.Id, order.Id, order.CustomerId, order.OrderNumber, order.CustomerName, dto.RequestDate, dto.Reason, dto.Notes, currentUser.UserId);
            foreach (var input in selected) { if (!lineMap.TryGetValue(input.SalesDeliveryLineId, out var source)) throw new InvalidOperationException("A selected line does not belong to this delivery."); if (input.StockQuantity + committed.GetValueOrDefault(source.Id) > source.StockQuantity) throw new InvalidOperationException("Return quantity cannot exceed the delivery line remaining returnable quantity."); var proportionalCost = source.StockQuantity == 0 ? 0 : decimal.Round(source.CostOfGoodsSold * input.StockQuantity / source.StockQuantity, 8, MidpointRounding.AwayFromZero); created.AddLine(new(source.Id, source.SalesOrderLineId, source.ProductId, source.WarehouseId, source.StorageLocationId, input.StockQuantity, source.UnitCost, proportionalCost, source.ProductCodeSnapshot, source.ProductNameSnapshot, source.TrackingAllocations)); }
            await requests.AddAsync(created);
        }); return created!.Id;
    }
    public Task SubmitAsync(CustomerReturnRequestCommandDto dto) => Change(dto, x => x.Submit(currentUser.UserId));
    public Task ApproveAsync(CustomerReturnRequestCommandDto dto) => Change(dto, x => x.Approve(currentUser.UserId));
    public Task RejectAsync(CustomerReturnRequestCommandDto dto) => Change(dto, x => x.Reject(currentUser.UserId, dto.Reason ?? string.Empty));
    public Task CancelAsync(CustomerReturnRequestCommandDto dto) => Change(dto, x => x.Cancel(currentUser.UserId, dto.Reason ?? string.Empty));
    private Task Change(CustomerReturnRequestCommandDto dto, Action<CustomerReturnRequest> action) => unitOfWork.ExecuteInTransactionAsync(async () => { var request = await requests.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Customer return request was not found."); EnsureVersion(request.RowVersion, dto.RowVersion); action(request); });
    private static void EnsureVersion(byte[] current, byte[] submitted) { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException("Customer return request was modified by another user. Reload and try again."); }
}
