using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseServiceAcceptanceService(
    IPurchaseServiceAcceptanceRepository acceptances, IPurchaseOrderRepository orders,
    ISupplierRepository suppliers, DocumentNumberService numbers, IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<PurchaseServiceAcceptanceCreatePageDto> GetCreatePageAsync(
        int purchaseOrderId, CreatePurchaseServiceAcceptanceDto? form = null)
    {
        var order = await orders.GetByIdAsync(purchaseOrderId);
        if (order is null) return new() { PurchaseOrderId = purchaseOrderId, Form = form ?? new() { PurchaseOrderId = purchaseOrderId } };
        var supplier = await suppliers.GetByIdAsync(order.SupplierId);
        var completed = (await acceptances.GetByPurchaseOrderIdAsync(order.Id)).SelectMany(x => x.Lines)
            .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.CompletedQuantity));
        var lines = order.Lines.Where(x => x.BillingPolicySnapshot == PurchaseBillingPolicy.ServiceCompletion)
            .Select(x => new PurchaseServiceAcceptanceAvailableLineDto
            {
                PurchaseOrderLineId = x.Id, ProductCode = x.ProductCodeSnapshot, ProductName = x.ProductNameSnapshot,
                UnitName = x.UnitNameSnapshot, OrderedQuantity = x.OrderedQuantity,
                CompletedQuantity = completed.GetValueOrDefault(x.Id),
                RemainingQuantity = Math.Max(0, x.OrderedQuantity - completed.GetValueOrDefault(x.Id))
            }).Where(x => x.RemainingQuantity > 0).ToList();
        return new()
        {
            PurchaseOrderId = order.Id, PurchaseOrderNumber = order.OrderNumber, SupplierName = supplier?.Name ?? "-",
            Lines = lines,
            Form = form ?? new CreatePurchaseServiceAcceptanceDto
            {
                PurchaseOrderId = order.Id,
                Lines = lines.Select(x => new CreatePurchaseServiceAcceptanceLineDto { PurchaseOrderLineId = x.PurchaseOrderLineId }).ToList()
            }
        };
    }

    public async Task<int> CompleteAsync(CreatePurchaseServiceAcceptanceDto dto)
    {
        var submitted = dto.Lines.Where(x => x.CompletedQuantity > 0).ToList();
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive completed service quantity.");
        if (submitted.Select(x => x.PurchaseOrderLineId).Distinct().Count() != submitted.Count)
            throw new ArgumentException("Each purchase order service line can be completed only once per acceptance.");
        PurchaseServiceAcceptance? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status != PurchaseOrderStatus.Confirmed) throw new InvalidOperationException("Only confirmed purchase orders can accept service completion.");
            var completed = (await acceptances.GetByPurchaseOrderIdAsync(order.Id)).SelectMany(x => x.Lines)
                .GroupBy(x => x.PurchaseOrderLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.CompletedQuantity));
            created = new PurchaseServiceAcceptance(
                await numbers.GenerateAsync(DocumentNumberType.PurchaseServiceAcceptance, dto.CompletionDate.ToDateTime(TimeOnly.MinValue)),
                order.Id, dto.CompletionDate, dto.Notes, currentUser.UserId);
            foreach (var input in submitted)
            {
                var line = order.Lines.SingleOrDefault(x => x.Id == input.PurchaseOrderLineId)
                    ?? throw new ArgumentException("One or more completed service lines do not belong to the purchase order.");
                if (line.BillingPolicySnapshot != PurchaseBillingPolicy.ServiceCompletion)
                    throw new InvalidOperationException("Only service-completion billing lines can use service acceptance.");
                if (input.CompletedQuantity > line.OrderedQuantity - completed.GetValueOrDefault(line.Id))
                    throw new InvalidOperationException("Completed service quantity cannot exceed the open ordered quantity.");
                created.AddLine(new PurchaseServiceAcceptanceLine(line.Id, line.ProductId, input.CompletedQuantity,
                    line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot));
            }
            await acceptances.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<PurchaseServiceAcceptanceDto?> GetAsync(int id)
    {
        var value = await acceptances.GetByIdAsync(id); if (value is null) return null;
        var order = await orders.GetByIdAsync(value.PurchaseOrderId);
        return Map(value, order?.OrderNumber ?? "-");
    }

    public async Task<List<PurchaseServiceAcceptanceDto>> GetAllAsync()
    {
        var orderMap = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        return (await acceptances.GetAllAsync()).Select(x => Map(x, orderMap.GetValueOrDefault(x.PurchaseOrderId, "-"))).ToList();
    }

    private static PurchaseServiceAcceptanceDto Map(PurchaseServiceAcceptance value, string orderNumber) => new()
    {
        Id = value.Id, AcceptanceNumber = value.AcceptanceNumber, PurchaseOrderId = value.PurchaseOrderId,
        PurchaseOrderNumber = orderNumber, CompletionDate = value.CompletionDate, Notes = value.Notes,
        CompletedByUserId = value.CompletedByUserId, CompletedAtUtc = value.CompletedAtUtc,
        Lines = value.Lines.Select(x => new PurchaseServiceAcceptanceLineDto
        {
            ProductCode = x.ProductCodeSnapshot, ProductName = x.ProductNameSnapshot,
            UnitName = x.UnitNameSnapshot, CompletedQuantity = x.CompletedQuantity
        }).ToList()
    };
}
