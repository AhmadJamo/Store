using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesOrderApprovalService(
    ISalesApprovalRepository approvals,
    ISalesOrderRepository orders,
    ICustomerRepository customers,
    IProductRepository products,
    ICustomerInvoiceRepository invoices,
    ICustomerPaymentRepository payments,
    ICustomerCreditNoteRepository credits,
    ITenantRoleRepository roles,
    ITenantAuthorizationRepository authorization,
    ITenantContext tenantContext,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<SalesApprovalRulesPageDto> GetRulesPageAsync(CreateSalesApprovalRuleDto? form = null)
    {
        var customerRows = await customers.GetAllAsync();
        var customerNames = customerRows.ToDictionary(x => x.Id, x => $"{x.CustomerCode} — {x.Name}");
        return new SalesApprovalRulesPageDto
        {
            Form = form ?? new(),
            Customers = customerRows.OrderBy(x => x.Name).Select(x => new SalesApprovalOptionDto(x.Id, customerNames[x.Id])).ToList(),
            Roles = (await roles.GetAllAsync(RequireTenant())).Select(x => x.Name).OrderBy(x => x).ToList(),
            Rules = (await approvals.GetRulesAsync()).Select(rule => new SalesApprovalRuleDto
            {
                Id = rule.Id, Name = rule.Name,
                CustomerName = rule.CustomerId.HasValue ? customerNames.GetValueOrDefault(rule.CustomerId.Value, $"#{rule.CustomerId}") : "All customers",
                Metric = rule.Metric, Comparison = rule.Comparison, Threshold = rule.Threshold,
                Action = rule.Action, Priority = rule.Priority, EffectiveFrom = rule.EffectiveFrom,
                EffectiveTo = rule.EffectiveTo, AllowRequesterApproval = rule.AllowRequesterApproval,
                RequireDistinctApprovers = rule.RequireDistinctApprovers, IsActive = rule.IsActive, RowVersion = rule.RowVersion,
                ApproverRoles = rule.Steps.OrderBy(x => x.Sequence).Select(x => x.ApproverRoleName).ToList()
            }).ToList()
        };
    }

    public async Task CreateRuleAsync(CreateSalesApprovalRuleDto dto)
    {
        if (dto.CustomerId.HasValue && await customers.GetByIdAsync(dto.CustomerId.Value) is null)
            throw new ArgumentException("Selected sales approval customer was not found.");
        var roleNames = dto.ApproverRoles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
        if (dto.Action == SalesControlAction.RequireApproval && roleNames.Count == 0)
            throw new ArgumentException("At least one sales approver role is required for an approval rule.");
        if (dto.Action != SalesControlAction.RequireApproval && roleNames.Count > 0)
            throw new ArgumentException("Approver roles can be selected only when the rule action requires approval.");
        if (roleNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != roleNames.Count)
            throw new ArgumentException("Each sales approver role can appear only once in a rule.");
        var availableRoles = (await roles.GetAllAsync(RequireTenant())).Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (roleNames.Any(x => !availableRoles.Contains(x)))
            throw new ArgumentException("One or more sales approver roles are invalid.");
        var rule = new SalesApprovalRule(dto.Name, dto.CustomerId, dto.Metric, dto.Comparison,
            dto.Threshold, dto.Action, dto.Priority, dto.EffectiveFrom, dto.EffectiveTo,
            dto.AllowRequesterApproval, dto.RequireDistinctApprovers);
        for (var index = 0; index < roleNames.Count; index++) rule.AddStep(index + 1, roleNames[index]);
        await unitOfWork.ExecuteInTransactionAsync(() => approvals.AddRuleAsync(rule));
    }

    public async Task SetActiveAsync(int id, bool active, byte[] rowVersion)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var rule = await approvals.GetRuleAsync(id) ?? throw new InvalidOperationException("Sales approval rule was not found.");
            EnsureVersion(rule.RowVersion, rowVersion, "Sales approval rule was modified by another user. Reload and try again.");
            rule.SetActive(active);
        });
    }

    public async Task<SalesOrderConfirmationOutcome> EvaluateAndConfirmAsync(SalesOrder order)
    {
        if (order.Status != SalesOrderStatus.Draft)
            throw new InvalidOperationException("Only draft sales orders can be confirmed.");
        if (await approvals.GetPendingReviewAsync(order.Id) is not null)
            return SalesOrderConfirmationOutcome.PendingApproval;
        var metrics = await CalculateMetricsAsync(order);
        var rules = (await approvals.GetRulesAsync())
            .Where(x => x.IsEffective(order.CustomerId, order.OrderDate))
            .OrderBy(x => x.Priority).ThenBy(x => x.Id).ToList();
        var triggered = rules.Where(x => x.IsTriggered(metrics[x.Metric]))
            .Select(x => new SalesApprovalTriggeredRule(x, metrics[x.Metric])).ToList();
        if (triggered.Count == 0)
        {
            order.Confirm(currentUser.UserId);
            return SalesOrderConfirmationOutcome.Confirmed;
        }
        var review = new SalesOrderApproval(order.Id, currentUser.UserId, triggered);
        await approvals.AddReviewAsync(review);
        if (review.Status == SalesOrderApprovalStatus.Blocked) return SalesOrderConfirmationOutcome.Blocked;
        if (review.Status == SalesOrderApprovalStatus.Pending) return SalesOrderConfirmationOutcome.PendingApproval;
        order.Confirm(currentUser.UserId);
        return SalesOrderConfirmationOutcome.Confirmed;
    }

    public async Task<SalesOrderApprovalDto?> GetLatestAsync(int salesOrderId)
    {
        var review = await approvals.GetLatestReviewAsync(salesOrderId);
        if (review is null) return null;
        var currentRole = review.Status == SalesOrderApprovalStatus.Pending ? review.CurrentStep().ApproverRoleName : null;
        return new SalesOrderApprovalDto
        {
            Id = review.Id, Status = review.Status, RequestedAtUtc = review.RequestedAtUtc, CurrentRole = currentRole,
            CanCurrentUserDecide = currentRole is not null && await authorization.HasRoleAsync(RequireTenant(), currentUser.UserId, currentRole),
            Findings = review.Findings.Select(x => new SalesOrderApprovalFindingDto(x.RuleNameSnapshot,
                x.Metric, x.Comparison, x.Threshold, x.ActualValue, x.Action)).ToList(),
            Steps = review.Steps.OrderBy(x => x.Sequence).Select(x => new SalesOrderApprovalStepDto(x.Sequence,
                x.ApproverRoleName, x.Status, x.DecidedByUserId, x.DecidedAtUtc, x.Note)).ToList()
        };
    }

    public async Task<SalesOrderApprovalStatus> DecideAsync(SalesOrderApprovalDecisionDto dto, bool approve)
    {
        var status=SalesOrderApprovalStatus.Pending;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.SalesOrderId) ?? throw new InvalidOperationException("Sales order was not found.");
            var review = await approvals.GetPendingReviewAsync(order.Id) ?? throw new InvalidOperationException("Pending sales order approval was not found.");
            var roleName = review.CurrentStep().ApproverRoleName;
            if (!await authorization.HasRoleAsync(RequireTenant(), currentUser.UserId, roleName))
                throw new UnauthorizedAccessException("The current sales approval step requires a different role.");
            if (approve)
            {
                review.ApproveCurrent(currentUser.UserId, dto.Note);
                if (review.Status == SalesOrderApprovalStatus.Approved) order.Confirm(currentUser.UserId);
            }
            else review.RejectCurrent(currentUser.UserId, dto.Note ?? string.Empty);
            status=review.Status;
        });
        return status;
    }

    public async Task CancelPendingAsync(int orderId)
    {
        var review = await approvals.GetPendingReviewAsync(orderId);
        review?.Cancel();
    }

    private async Task<Dictionary<SalesControlMetric, decimal>> CalculateMetricsAsync(SalesOrder order)
    {
        var productMap = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var discount = order.Lines.Select(line =>
        {
            var baseline = productMap.GetValueOrDefault(line.ProductId)?.WholesalePrice ?? 0;
            return baseline <= 0 ? 0 : Math.Max(0, Round((baseline - line.CompanyUnitPrice) * 100 / baseline));
        }).DefaultIfEmpty(0).Max();
        var margin = order.Lines.Select(line =>
        {
            var cost = productMap.GetValueOrDefault(line.ProductId)?.PurchasePrice ?? 0;
            return line.CompanyUnitPrice <= 0 ? 0 : Round((line.CompanyUnitPrice - cost) * 100 / line.CompanyUnitPrice);
        }).DefaultIfEmpty(0).Min();
        var orderCompanyAmount = Round(order.TotalAmount * order.ExchangeRate);
        var customerInvoices = await invoices.GetByCustomerAsync(order.CustomerId);
        var posted = customerInvoices.Where(x => x.Status == CustomerInvoiceStatus.Posted).ToList();
        var invoiceIds = posted.Select(x => x.Id).ToList();
        var paid = await payments.GetPaidAmountsAsync(invoiceIds);
        var paidCompany = await payments.GetReceivableCarryingAmountsAsync(invoiceIds);
        var credited = await credits.GetPostedAmountsAsync(invoiceIds);
        var creditedCompany = await credits.GetPostedCompanyAmountsAsync(invoiceIds);
        var openReceivable = posted.Sum(x => Math.Max(0, x.CompanyTotalAmount - paidCompany.GetValueOrDefault(x.Id) - creditedCompany.GetValueOrDefault(x.Id)));
        var overdueDays = posted.Where(x => x.TotalAmount - paid.GetValueOrDefault(x.Id) - credited.GetValueOrDefault(x.Id) > 0 && x.DueDate < order.OrderDate)
            .Select(x => (decimal)order.OrderDate.DayNumber - x.DueDate.DayNumber).DefaultIfEmpty(0).Max();
        // Only Posted invoices leave the unbilled-order bucket because their open amount is
        // already included in openReceivable. Draft/Approved invoices are not receivables yet.
        var postedInvoiceAmounts = posted.GroupBy(x => x.SalesOrderId)
            .ToDictionary(x => x.Key, x => x.Sum(i => Round(i.TotalAmount * i.ExchangeRate)));
        var unbilledCommitted = (await orders.GetAllAsync(null, null))
            .Where(x => x.Id != order.Id && x.CustomerId == order.CustomerId && x.Status == SalesOrderStatus.Confirmed)
            .Sum(x => Math.Max(0, Round(x.TotalAmount * x.ExchangeRate) - postedInvoiceAmounts.GetValueOrDefault(x.Id)));
        return new Dictionary<SalesControlMetric, decimal>
        {
            [SalesControlMetric.OrderCompanyAmount] = orderCompanyAmount,
            [SalesControlMetric.CreditExposure] = Round(openReceivable + unbilledCommitted + orderCompanyAmount),
            [SalesControlMetric.OverdueDays] = overdueDays,
            [SalesControlMetric.DiscountPercent] = discount,
            [SalesControlMetric.MarginPercent] = margin
        };
    }

    private int RequireTenant() => tenantContext.TenantId ?? throw new InvalidOperationException("An active company is required.");
    private static void EnsureVersion(byte[] current, byte[] submitted, string message)
    { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException(message); }
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
