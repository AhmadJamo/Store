using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class CustomerInvoiceService(ICustomerInvoiceRepository invoices, ISalesOrderRepository orders,
    ISalesDeliveryRepository deliveries, IProductRepository products, IAccountingSettingsRepository accountingSettings,
    DocumentNumberService numbers, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<CustomerInvoiceSummaryDto>> GetAllAsync(CustomerInvoiceStatus? status, string? search)
    {
        var rows = await invoices.GetAllAsync(status, null);
        var orderNumbers = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        var result = rows.Select(x => new CustomerInvoiceSummaryDto(x.Id, x.InvoiceNumber, x.SalesOrderId,
            orderNumbers.GetValueOrDefault(x.SalesOrderId, $"#{x.SalesOrderId}"), x.CustomerCode, x.CustomerName,
            x.InvoiceDate, x.DueDate, x.CurrencyCode, x.TotalAmount, x.Status)).ToList();
        if (string.IsNullOrWhiteSpace(search)) return result;
        var term = search.Trim();
        return result.Where(x => x.InvoiceNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            x.OrderNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            x.CustomerCode.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            x.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<CustomerInvoiceDetailsDto?> GetAsync(int id)
    {
        var invoice = await invoices.GetByIdAsync(id); if (invoice is null) return null;
        var order = await orders.GetByIdAsync(invoice.SalesOrderId);
        var deliveryNumbers = (await deliveries.GetBySalesOrderAsync(invoice.SalesOrderId))
            .ToDictionary(x => x.Id, x => x.DeliveryNumber);
        return new CustomerInvoiceDetailsDto
        {
            Id = invoice.Id, InvoiceNumber = invoice.InvoiceNumber, SalesOrderId = invoice.SalesOrderId,
            OrderNumber = order?.OrderNumber ?? $"#{invoice.SalesOrderId}", QuantityPolicy = invoice.QuantityPolicy,
            CustomerCode = invoice.CustomerCode, CustomerName = invoice.CustomerName,
            CustomerLegalName = invoice.CustomerLegalName, CustomerTaxNumber = invoice.CustomerTaxNumber,
            Address = FormatAddress(invoice), InvoiceDate = invoice.InvoiceDate, DueDate = invoice.DueDate,
            CurrencyCode = invoice.CurrencyCode, ExchangeRate = invoice.ExchangeRate,
            ExchangeRateDate = invoice.ExchangeRateDate, ExchangeRateSource = invoice.ExchangeRateSource,
            PaymentTermDays = invoice.PaymentTermDays, Notes = invoice.Notes, Status = invoice.Status,
            NetAmount = invoice.NetAmount, TaxAmount = invoice.TaxAmount, TotalAmount = invoice.TotalAmount,
            CreatedByUserId = invoice.CreatedByUserId, CreatedAtUtc = invoice.CreatedAtUtc,
            ApprovedByUserId = invoice.ApprovedByUserId, ApprovedAtUtc = invoice.ApprovedAtUtc,
            CancellationReason = invoice.CancellationReason, RowVersion = invoice.RowVersion,
            Lines = invoice.Lines.Select(line => new CustomerInvoiceLineDto
            {
                Id = line.Id, SalesOrderLineId = line.SalesOrderLineId, ProductCode = line.ProductCode,
                ProductName = line.ProductName, UnitName = line.UnitName, Quantity = line.Quantity,
                StockQuantity = line.StockQuantity, QuantityBasis = line.QuantityBasis, UnitPrice = line.UnitPrice,
                NetAmount = line.NetAmount, TaxName = line.TaxName, TaxRatePercent = line.TaxRatePercent,
                TaxAmount = line.TaxAmount, TotalAmount = line.TotalAmount,
                Sources = line.SourceAllocations.Select(source => new CustomerInvoiceSourceDto(source.SalesDeliveryId,
                    source.SalesDeliveryLineId, deliveryNumbers.GetValueOrDefault(source.SalesDeliveryId, $"#{source.SalesDeliveryId}"),
                    source.StockQuantity)).ToList()
            }).ToList(),
            History = invoice.History.OrderBy(x => x.CreatedAtUtc).Select(x => new CustomerInvoiceHistoryDto(
                x.FromStatus, x.ToStatus, x.Action, x.UserId, x.Reason, x.CreatedAtUtc)).ToList()
        };
    }

    public async Task<CustomerInvoiceCreatePageDto?> GetCreatePageAsync(int salesOrderId)
    {
        var order = await orders.GetByIdAsync(salesOrderId); if (order is null) return null;
        var context = await BuildEligibilityAsync(order);
        return new CustomerInvoiceCreatePageDto
        {
            SalesOrderId = order.Id, OrderNumber = order.OrderNumber, CustomerCode = order.CustomerCode,
            CustomerName = order.CustomerName, CurrencyCode = order.CurrencyCode, QuantityPolicy = context.Policy,
            Lines = context.Lines.Select(x => new CustomerInvoiceEligibleLineDto
            {
                SalesOrderLineId = x.OrderLine.Id, ProductCode = x.OrderLine.ProductCode,
                ProductName = x.OrderLine.ProductName, UnitName = x.OrderLine.UnitName, QuantityBasis = x.Basis,
                OrderedQuantity = x.OrderLine.Quantity, SourceEligibleQuantity = x.SourceEligibleQuantity,
                CommittedQuantity = x.CommittedQuantity, RemainingQuantity = x.RemainingQuantity,
                UnitPrice = x.OrderLine.UnitPrice
            }).ToList(),
            Form = new CreateCustomerInvoiceDto
            {
                SalesOrderId = order.Id, RequestKey = Guid.NewGuid().ToString("N"),
                Lines = context.Lines.Select(x => new CreateCustomerInvoiceLineDto
                    { SalesOrderLineId = x.OrderLine.Id, Quantity = x.RemainingQuantity }).ToList()
            }
        };
    }

    public async Task<CustomerInvoiceProgressDto> GetProgressAsync(int salesOrderId)
    {
        var order = await orders.GetByIdAsync(salesOrderId)
            ?? throw new InvalidOperationException("Sales order was not found.");
        var context = await BuildEligibilityAsync(order);
        return new CustomerInvoiceProgressDto
        {
            TotalOrderLineCount = order.Lines.Count,
            FullyInvoicedLineCount = context.Lines.Count(x => x.CommittedQuantity >= x.OrderLine.Quantity),
            DraftInvoiceCount = context.ActiveInvoices.Count(x => x.Status == CustomerInvoiceStatus.Draft),
            ApprovedInvoiceCount = context.ActiveInvoices.Count(x => x.Status == CustomerInvoiceStatus.Approved),
            HasEligibleRemaining = context.Lines.Any(x => x.RemainingQuantity > 0),
            IsFullyInvoiced = order.Lines.Count > 0 && context.Lines.All(x => x.CommittedQuantity >= x.OrderLine.Quantity),
            CommittedAmount = context.ActiveInvoices.Sum(x => x.TotalAmount)
        };
    }

    public async Task<int> CreateDraftAsync(CreateCustomerInvoiceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RequestKey) || dto.RequestKey.Trim().Length > 100)
            throw new ArgumentException("Customer invoice request key is required and cannot exceed 100 characters.");
        var prior = await invoices.GetByRequestKeyAsync(dto.RequestKey); if (prior is not null) return prior.Id;
        CustomerInvoice? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            prior = await invoices.GetByRequestKeyAsync(dto.RequestKey); if (prior is not null) { created = prior; return; }
            var order = await orders.GetByIdAsync(dto.SalesOrderId)
                ?? throw new InvalidOperationException("Sales order was not found.");
            if (order.Status != SalesOrderStatus.Confirmed)
                throw new InvalidOperationException("Only a confirmed non-cancelled sales order can be invoiced.");
            if (dto.Lines is null || dto.Lines.Count == 0 || dto.Lines.GroupBy(x => x.SalesOrderLineId).Any(x => x.Count() > 1))
                throw new ArgumentException("Customer invoice lines are required and cannot be duplicated.");
            var selected = dto.Lines.Where(x => x.Quantity > 0).ToList();
            if (selected.Count == 0) throw new ArgumentException("Enter at least one positive customer invoice quantity.");

            var context = await BuildEligibilityAsync(order);
            var eligibility = context.Lines.ToDictionary(x => x.OrderLine.Id);
            created = new CustomerInvoice(await numbers.GenerateAsync(DocumentNumberType.CustomerInvoice,
                dto.InvoiceDate.ToDateTime(TimeOnly.MinValue)), dto.RequestKey, order, context.Policy,
                dto.InvoiceDate, dto.Notes, currentUser.UserId);

            var allocatedByDeliveryLine = context.ActiveInvoices.SelectMany(x => x.Lines)
                .SelectMany(x => x.SourceAllocations).GroupBy(x => x.SalesDeliveryLineId)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.StockQuantity));
            foreach (var input in selected)
            {
                if (!eligibility.TryGetValue(input.SalesOrderLineId, out var eligible))
                    throw new InvalidOperationException("A customer invoice line does not belong to this sales order.");
                if (input.Quantity > eligible.RemainingQuantity)
                    throw new InvalidOperationException("Customer invoice quantity cannot exceed the remaining eligible source quantity.");
                var source = eligible.OrderLine; var stockQuantity = input.Quantity * source.UnitFactorToStock;
                var priorLines = context.ActiveInvoices.SelectMany(x => x.Lines)
                    .Where(x => x.SalesOrderLineId == source.Id).ToList();
                var completesOrderLine = priorLines.Sum(x => x.Quantity) + input.Quantity == source.Quantity;
                var net = completesOrderLine ? source.NetAmount - priorLines.Sum(x => x.NetAmount)
                    : RoundMoney(source.NetAmount * input.Quantity / source.Quantity);
                var tax = completesOrderLine ? source.TaxAmount - priorLines.Sum(x => x.TaxAmount)
                    : RoundMoney(source.TaxAmount * input.Quantity / source.Quantity);
                var total = net + tax;
                var allocations = eligible.Basis == SalesInvoiceQuantityBasis.DeliveredQuantity
                    ? AllocateDeliveredStock(context.PostedDeliveries, source.Id, stockQuantity, allocatedByDeliveryLine)
                    : [];
                created.AddLine(new CustomerInvoiceLine(source, input.Quantity, stockQuantity, eligible.Basis,
                    net, tax, total, allocations));
            }
            await invoices.AddAsync(created);
        });
        return created!.Id;
    }

    public Task ApproveAsync(CustomerInvoiceStatusCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var invoice = await invoices.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Customer invoice was not found.");
        EnsureVersion(invoice.RowVersion, dto.RowVersion); invoice.Approve(currentUser.UserId);
    });

    public Task CancelAsync(CustomerInvoiceStatusCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var invoice = await invoices.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Customer invoice was not found.");
        EnsureVersion(invoice.RowVersion, dto.RowVersion); invoice.Cancel(currentUser.UserId, dto.Reason ?? string.Empty);
    });

    private async Task<EligibilityContext> BuildEligibilityAsync(SalesOrder order)
    {
        var settings = await accountingSettings.GetAsync();
        var policy = settings?.SalesInvoicePolicy ?? SalesInvoiceQuantityPolicy.DeliveredQuantity;
        var productMap = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var postedDeliveries = (await deliveries.GetBySalesOrderAsync(order.Id))
            .Where(x => x.Status == SalesDeliveryStatus.Posted).OrderBy(x => x.PostedAtUtc).ThenBy(x => x.Id).ToList();
        var activeInvoices = (await invoices.GetBySalesOrderAsync(order.Id))
            .Where(x => x.Status != CustomerInvoiceStatus.Cancelled).ToList();
        var committed = activeInvoices.SelectMany(x => x.Lines).GroupBy(x => x.SalesOrderLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var deliveredStock = postedDeliveries.SelectMany(x => x.Lines).GroupBy(x => x.SalesOrderLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.StockQuantity));
        var lines = order.Lines.Select(line =>
        {
            if (!productMap.TryGetValue(line.ProductId, out var product))
                throw new InvalidOperationException("Customer invoice product was not found.");
            var basis = product.ProductType == ProductType.Service || policy == SalesInvoiceQuantityPolicy.OrderedQuantity
                ? SalesInvoiceQuantityBasis.OrderedQuantity : SalesInvoiceQuantityBasis.DeliveredQuantity;
            var sourceEligible = basis == SalesInvoiceQuantityBasis.OrderedQuantity ? line.Quantity
                : Math.Min(line.Quantity, deliveredStock.GetValueOrDefault(line.Id) / line.UnitFactorToStock);
            var used = committed.GetValueOrDefault(line.Id);
            return new EligibleLine(line, basis, sourceEligible, used, Math.Max(0, sourceEligible - used));
        }).ToList();
        return new EligibilityContext(policy, lines, postedDeliveries, activeInvoices);
    }

    private static List<CustomerInvoiceSourceAllocation> AllocateDeliveredStock(List<SalesDelivery> deliveries,
        int salesOrderLineId, decimal requiredStockQuantity, Dictionary<int, decimal> allocatedByDeliveryLine)
    {
        var remaining = requiredStockQuantity; var result = new List<CustomerInvoiceSourceAllocation>();
        foreach (var delivery in deliveries)
        foreach (var line in delivery.Lines.Where(x => x.SalesOrderLineId == salesOrderLineId).OrderBy(x => x.Id))
        {
            var available = line.StockQuantity - allocatedByDeliveryLine.GetValueOrDefault(line.Id);
            if (available <= 0) continue;
            var quantity = Math.Min(available, remaining);
            result.Add(new CustomerInvoiceSourceAllocation(delivery.Id, line.Id, quantity));
            allocatedByDeliveryLine[line.Id] = allocatedByDeliveryLine.GetValueOrDefault(line.Id) + quantity;
            remaining -= quantity; if (remaining == 0) return result;
        }
        throw new InvalidOperationException("Customer invoice quantity no longer matches the remaining posted delivery quantity.");
    }

    private static string? FormatAddress(CustomerInvoice invoice) => invoice.AddressLine1 is null ? null
        : $"{invoice.AddressLabel}: {invoice.AddressLine1}{(invoice.AddressLine2 is null ? "" : ", " + invoice.AddressLine2)}, {invoice.City}{(invoice.StateOrProvince is null ? "" : ", " + invoice.StateOrProvince)} {invoice.PostalCode}, {invoice.CountryCode}";
    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    private static void EnsureVersion(byte[] current, byte[] submitted)
    { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException("Customer invoice was modified by another user. Reload and try again."); }

    private sealed record EligibleLine(SalesOrderLine OrderLine, SalesInvoiceQuantityBasis Basis,
        decimal SourceEligibleQuantity, decimal CommittedQuantity, decimal RemainingQuantity);
    private sealed record EligibilityContext(SalesInvoiceQuantityPolicy Policy, List<EligibleLine> Lines,
        List<SalesDelivery> PostedDeliveries, List<CustomerInvoice> ActiveInvoices);
}
