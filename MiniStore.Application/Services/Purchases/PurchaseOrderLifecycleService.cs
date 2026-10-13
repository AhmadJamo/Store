using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseOrderLifecycleService(
    IPurchaseOrderRepository orders,
    IGoodsReceiptRepository receipts,
    IGoodsReceiptReturnRepository returns,
    IPurchaseServiceAcceptanceRepository serviceAcceptances,
    IVendorBillRepository bills,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    private const decimal QuantityTolerance = 0.000001m;

    public async Task<PurchaseOrderProgressDto> GetProgressAsync(PurchaseOrder order)
    {
        var postedReceipts = (await receipts.GetByPurchaseOrderIdAsync(order.Id))
            .Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
        var postedBills = (await bills.GetByPurchaseOrderIdAsync(order.Id))
            .Where(x => x.Status == VendorBillStatus.Posted).ToList();
        var completedServiceLines = (await serviceAcceptances.GetByPurchaseOrderIdAsync(order.Id))
            .SelectMany(x => x.Lines).ToList();
        var postedReturns = new List<GoodsReceiptReturn>();
        foreach (var receipt in postedReceipts)
            postedReturns.AddRange((await returns.GetByGoodsReceiptIdAsync(receipt.Id))
                .Where(x => x.Status == GoodsReceiptReturnStatus.Posted));

        var receiptLines = postedReceipts.SelectMany(x => x.Lines).ToList();
        var returnedByReceiptLine = postedReturns.SelectMany(x => x.Lines)
            .GroupBy(x => x.GoodsReceiptLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.ReturnQuantity));
        var billedByReceiptLine = postedBills.SelectMany(x => x.Lines)
            .GroupBy(x => x.GoodsReceiptLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.ReceiptClearingQuantity));

        var lines = order.Lines.Select(orderLine =>
        {
            var relatedReceipts = receiptLines.Where(x => x.PurchaseOrderLineId == orderLine.Id).ToList();
            var received = relatedReceipts.Sum(x => x.ReceivedQuantity);
            var returned = relatedReceipts.Sum(x => returnedByReceiptLine.GetValueOrDefault(x.Id));
            var billed = relatedReceipts.Sum(x => billedByReceiptLine.GetValueOrDefault(x.Id));
            var serviceCompleted = completedServiceLines
                .Where(x => x.PurchaseOrderLineId == orderLine.Id).Sum(x => x.CompletedQuantity);
            var remainingToReceive = orderLine.BillingPolicySnapshot == PurchaseBillingPolicy.ReceivedQuantity
                ? Positive(orderLine.OrderedQuantity - received) : 0m;
            var remainingToComplete = orderLine.BillingPolicySnapshot == PurchaseBillingPolicy.ServiceCompletion
                ? Positive(orderLine.OrderedQuantity - serviceCompleted) : 0m;
            var eligibleToBill = orderLine.BillingPolicySnapshot switch
            {
                PurchaseBillingPolicy.OrderedQuantity => orderLine.OrderedQuantity,
                PurchaseBillingPolicy.ServiceCompletion => serviceCompleted,
                _ => Positive(received - returned)
            };
            return new PurchaseOrderLineProgressDto
            {
                PurchaseOrderLineId = orderLine.Id,
                BillingPolicy = orderLine.BillingPolicySnapshot,
                ReceivedQuantity = received,
                ServiceCompletedQuantity = serviceCompleted,
                ReturnedQuantity = returned,
                BilledQuantity = billed,
                RemainingToReceive = remainingToReceive,
                RemainingToComplete = remainingToComplete,
                AvailableToBill = Positive(eligibleToBill - billed),
                UnsettledReceivedQuantity = Positive(eligibleToBill - billed)
            };
        }).ToList();

        var fullyReceived = lines.All(x =>
            x.RemainingToReceive <= QuantityTolerance && x.RemainingToComplete <= QuantityTolerance);
        var fullySettled = lines.All(x => x.UnsettledReceivedQuantity <= QuantityTolerance);
        return new PurchaseOrderProgressDto
        {
            Lines = lines,
            IsFullyReceived = fullyReceived,
            IsFullySettled = fullySettled,
            CanCloseRemaining = order.Status == PurchaseOrderStatus.Confirmed && !fullyReceived && fullySettled,
            CanReopen = order.Status == PurchaseOrderStatus.Closed &&
                        order.ClosureType == PurchaseOrderClosureType.RemainingQuantity
        };
    }

    public async Task<bool> TryCloseAutomaticallyAsync(int purchaseOrderId)
    {
        var order = await orders.GetByIdAsync(purchaseOrderId)
            ?? throw new InvalidOperationException("Purchase order was not found.");
        if (order.Status != PurchaseOrderStatus.Confirmed) return false;
        var progress = await GetProgressAsync(order);
        if (!progress.IsFullyReceived || !progress.IsFullySettled) return false;
        order.CloseAutomatically(currentUser.UserId);
        return true;
    }

    public Task CloseRemainingAsync(int purchaseOrderId, string reason, byte[] rowVersion) =>
        unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await GetTrackedAsync(purchaseOrderId, rowVersion);
            var progress = await GetProgressAsync(order);
            if (!progress.IsFullySettled)
                throw new InvalidOperationException("Post or return every received quantity before closing the remaining purchase order quantity.");
            if (progress.IsFullyReceived)
                throw new InvalidOperationException("This purchase order is fully received and closes automatically after all received quantities are settled.");
            order.CloseRemaining(currentUser.UserId, reason);
        });

    public Task ReopenAsync(int purchaseOrderId, string reason, byte[] rowVersion) =>
        unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await GetTrackedAsync(purchaseOrderId, rowVersion);
            order.Reopen(currentUser.UserId, reason);
        });

    private async Task<PurchaseOrder> GetTrackedAsync(int id, byte[] rowVersion)
    {
        var order = await orders.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Purchase order was not found.");
        if (!order.RowVersion.SequenceEqual(rowVersion))
            throw new InvalidOperationException("Purchase order was modified by another user. Reload and try again.");
        return order;
    }

    private static decimal Positive(decimal value) => value > 0 ? value : 0m;
}
