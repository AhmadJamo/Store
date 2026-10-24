using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public sealed class WorkCenterService(
    SalesOrderService salesOrders,
    CustomerInvoiceService invoices,
    IPermissionService permissions,
    ICurrentUserService currentUser)
{
    public async Task<WorkCenterDto> GetAsync()
    {
        var granted = (await permissions.GetGrantedAsync(currentUser.UserId)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var items = new List<WorkCenterItemDto>();
        if (granted.Contains("SalesOrders.View") && granted.Contains("SalesOrders.Confirm"))
            items.AddRange((await salesOrders.GetAllAsync(SalesOrderStatus.Draft, null)).Take(100).Select(x =>
                new WorkCenterItemDto("confirm-order", "attention", "sales-order", x.Id, x.OrderNumber,
                    x.CustomerName, x.Status.ToString(), null, "workConfirmOrder", "helpConfirmOrder", "SalesOrders.Confirm")));
        if (granted.Contains("CustomerInvoices.View"))
        {
            var rows = (await invoices.GetAllAsync(null, null)).Take(200).ToList();
            if (granted.Contains("CustomerInvoices.Approve"))
                items.AddRange(rows.Where(x => x.Status == CustomerInvoiceStatus.Draft).Select(x =>
                    new WorkCenterItemDto("approve-invoice", "attention", "customer-invoice", x.Id, x.InvoiceNumber,
                        x.CustomerName, x.Status.ToString(), x.DueDate, "workApproveInvoice", "helpApproveInvoice", "CustomerInvoices.Approve")));
            if (granted.Contains("CustomerInvoices.Post"))
                items.AddRange(rows.Where(x => x.Status == CustomerInvoiceStatus.Approved).Select(x =>
                    new WorkCenterItemDto("post-invoice", "attention", "customer-invoice", x.Id, x.InvoiceNumber,
                        x.CustomerName, x.Status.ToString(), x.DueDate, "workPostInvoice", "helpPostInvoice", "CustomerInvoices.Post")));
            if (granted.Contains("CustomerPayments.Create"))
                items.AddRange(rows.Where(x => x.Status == CustomerInvoiceStatus.Posted && x.OutstandingAmount > 0 && x.DueDate < DateOnly.FromDateTime(DateTime.Today)).Select(x =>
                    new WorkCenterItemDto("overdue-invoice", "critical", "customer-invoice", x.Id, x.InvoiceNumber,
                        x.CustomerName, x.Status.ToString(), x.DueDate, "workOverdueInvoice", "helpOverdueInvoice", "CustomerPayments.Create")));
        }
        return new WorkCenterDto(DateTime.UtcNow, items.OrderByDescending(x => x.Severity == "critical").ThenBy(x => x.DueDate).ThenBy(x => x.DocumentNumber).ToList());
    }
}
