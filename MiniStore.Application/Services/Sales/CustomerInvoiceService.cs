using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class CustomerInvoiceService(ICustomerInvoiceRepository invoices, ICustomerPaymentRepository customerPayments, ICustomerCreditNoteRepository customerCredits, ISalesOrderRepository orders,
    ISalesDeliveryRepository deliveries, IProductRepository products, ICustomerRepository customers,
    IWarehouseRepository warehouses, IBranchRepository branches, IAccountRepository accounts,
    ITaxRateRepository taxRates, IAccountingSettingsRepository accountingSettings,
    IGeneralSettingsRepository generalSettings, IJournalEntryRepository journalEntries, JournalPostingService journalPosting,
    DocumentNumberService numbers, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    private const string JournalSourceType = "CustomerInvoice";
    private const string ReversalJournalSourceType = "CustomerInvoiceReversal";
    public async Task<List<CustomerInvoiceSummaryDto>> GetAllAsync(CustomerInvoiceStatus? status, string? search)
    {
        var rows = await invoices.GetAllAsync(status, null);
        var paid = await customerPayments.GetPaidAmountsAsync(rows.Select(x => x.Id));
        var credited = await customerCredits.GetPostedAmountsAsync(rows.Select(x => x.Id));
        var orderNumbers = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        var result = rows.Select(x => new CustomerInvoiceSummaryDto(x.Id, x.InvoiceNumber, x.SalesOrderId,
            orderNumbers.GetValueOrDefault(x.SalesOrderId, $"#{x.SalesOrderId}"), x.CustomerCode, x.CustomerName,
            x.InvoiceDate, x.DueDate, x.CurrencyCode, x.TotalAmount, x.Status, paid.GetValueOrDefault(x.Id),credited.GetValueOrDefault(x.Id),
            Math.Max(0, RoundMoney(x.TotalAmount - paid.GetValueOrDefault(x.Id)-credited.GetValueOrDefault(x.Id))))).ToList();
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
        var paidAmount = (await customerPayments.GetPaidAmountsAsync([invoice.Id])).GetValueOrDefault(invoice.Id);
        var creditedAmount=(await customerCredits.GetPostedAmountsAsync([invoice.Id])).GetValueOrDefault(invoice.Id);
        var outstandingAmount = Math.Max(0, RoundMoney(invoice.TotalAmount - paidAmount-creditedAmount));
        var order = await orders.GetByIdAsync(invoice.SalesOrderId);
        var deliveryNumbers = (await deliveries.GetBySalesOrderAsync(invoice.SalesOrderId))
            .ToDictionary(x => x.Id, x => x.DeliveryNumber);
        var journal = await journalEntries.GetBySourceAsync(JournalSourceType, invoice.InvoiceNumber);
        var reversalJournal = await journalEntries.GetBySourceAsync(ReversalJournalSourceType, invoice.InvoiceNumber);
        var accountMap = (await accounts.GetAllAsync()).ToDictionary(x => x.Id);
        var branchMap = (await branches.GetAllAsync()).ToDictionary(x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id);
        static string? AccountDisplay(Dictionary<int, Account> map, int? id) =>
            id.HasValue && map.TryGetValue(id.Value, out var account) ? $"{account.Code} — {account.Name}" : null;
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
            PaidAmount = paidAmount,CreditedAmount=creditedAmount, OutstandingAmount = outstandingAmount,
            PaymentStatus = outstandingAmount <= 0 ? (paidAmount > 0 ? "Paid" : "Credited") :
                (paidAmount > 0 || creditedAmount > 0 ? "Partially Settled" : "Unpaid"),
            CompanyCurrencyCode = invoice.CompanyCurrencyCode, CompanyNetAmount = invoice.CompanyNetAmount,
            CompanyTaxAmount = invoice.CompanyTaxAmount, CompanyTotalAmount = invoice.CompanyTotalAmount,
            ReceivableAccountIdSnapshot = invoice.ReceivableAccountIdSnapshot,
            SalesRevenueAccountIdSnapshot = invoice.SalesRevenueAccountIdSnapshot,
            ReceivableAccountDisplay = AccountDisplay(accountMap, invoice.ReceivableAccountIdSnapshot),
            SalesRevenueAccountDisplay = AccountDisplay(accountMap, invoice.SalesRevenueAccountIdSnapshot),
            CreatedByUserId = invoice.CreatedByUserId, CreatedAtUtc = invoice.CreatedAtUtc,
            ApprovedByUserId = invoice.ApprovedByUserId, ApprovedAtUtc = invoice.ApprovedAtUtc,
            PostedByUserId = invoice.PostedByUserId, PostedAtUtc = invoice.PostedAtUtc,
            ReversedByUserId = invoice.ReversedByUserId, ReversedAtUtc = invoice.ReversedAtUtc,
            ReversalDate = invoice.ReversalDate, ReversalReason = invoice.ReversalReason,
            JournalEntryId = journal?.Id, JournalEntryNumber = journal?.EntryNumber,
            ReversalJournalEntryId = reversalJournal?.Id, ReversalJournalEntryNumber = reversalJournal?.EntryNumber,
            JournalLines = journal?.Lines.Select(line => new CustomerInvoiceJournalLineDto(
                AccountDisplay(accountMap, line.AccountId) ?? $"#{line.AccountId}",
                line.BranchId.HasValue && branchMap.TryGetValue(line.BranchId.Value, out var branch) ? branch.Name : null,
                line.WarehouseId.HasValue && warehouseMap.TryGetValue(line.WarehouseId.Value, out var warehouse) ? warehouse.Name : null,
                line.Debit, line.Credit, line.Description)).ToList() ?? [],
            CancellationReason = invoice.CancellationReason, RowVersion = invoice.RowVersion,
            Lines = invoice.Lines.Select(line => new CustomerInvoiceLineDto
            {
                Id = line.Id, SalesOrderLineId = line.SalesOrderLineId, ProductCode = line.ProductCode,
                ProductName = line.ProductName, UnitName = line.UnitName, Quantity = line.Quantity,
                StockQuantity = line.StockQuantity, QuantityBasis = line.QuantityBasis, UnitPrice = line.UnitPrice,
                NetAmount = line.NetAmount, TaxName = line.TaxName, TaxRatePercent = line.TaxRatePercent,
                TaxAmount = line.TaxAmount, TotalAmount = line.TotalAmount,
                TaxOutputAccountIdSnapshot = line.TaxOutputAccountIdSnapshot,
                TaxOutputAccountDisplay = AccountDisplay(accountMap, line.TaxOutputAccountIdSnapshot),
                CompanyNetAmount = line.CompanyNetAmount, CompanyTaxAmount = line.CompanyTaxAmount,
                CompanyTotalAmount = line.CompanyTotalAmount,
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

    public Task PostAsync(CustomerInvoiceStatusCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var invoice = await invoices.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Customer invoice was not found.");
        EnsureVersion(invoice.RowVersion, dto.RowVersion);
        if (invoice.Status != CustomerInvoiceStatus.Approved)
            throw new InvalidOperationException("Only an approved customer invoice can be posted.");
        var customer = await customers.GetByIdAsync(invoice.CustomerId)
            ?? throw new InvalidOperationException("Customer was not found.");
        var receivable = await accounts.GetByIdAsync(customer.AccountId);
        if (receivable is not { IsActive: true, Type: AccountType.Asset })
            throw new InvalidOperationException("Assign an active asset receivable account to the customer before posting the customer invoice.");
        var settings = await accountingSettings.GetAsync()
            ?? throw new InvalidOperationException("Configure accounting posting accounts before posting a customer invoice.");
        if (!settings.SalesRevenueAccountId.HasValue)
            throw new InvalidOperationException("Configure a sales revenue account before posting a customer invoice.");
        var revenue = await accounts.GetByIdAsync(settings.SalesRevenueAccountId.Value);
        if (revenue is not { IsActive: true, Type: AccountType.Revenue })
            throw new InvalidOperationException("Configure an active revenue account before posting a customer invoice.");
        var taxMap = (await taxRates.GetAllAsync()).ToDictionary(x => x.Id);
        var outputAccounts = new Dictionary<int, int>();
        foreach (var line in invoice.Lines.Where(x => x.TaxAmount > 0))
        {
            if (!line.TaxRateId.HasValue || !taxMap.TryGetValue(line.TaxRateId.Value, out var tax))
                throw new InvalidOperationException("A tax rate selected on this customer invoice no longer exists.");
            var account = await accounts.GetByIdAsync(tax.OutputAccountId);
            if (account is not { IsActive: true, Type: AccountType.Liability })
                throw new InvalidOperationException("Configure an active output tax liability account before posting a customer invoice.");
            outputAccounts[line.TaxRateId.Value] = tax.OutputAccountId;
        }
        var companyCurrency = (await generalSettings.GetAsync())?.Currency ?? invoice.CurrencyCode;
        invoice.Post(companyCurrency, receivable.Id, revenue.Id, outputAccounts, currentUser.UserId);

        var slices = await BuildPostingSlicesAsync(invoice, revenue.Id);
        var lines = slices.GroupBy(x => new { x.BranchId, x.WarehouseId })
            .Select(group => new JournalEntryLine(receivable.Id, group.Sum(x => x.CompanyTotalAmount), 0,
                group.Key.BranchId, group.Key.WarehouseId, $"Customer receivable for {invoice.InvoiceNumber}"))
            .ToList();
        lines.AddRange(slices.Where(x => x.CompanyNetAmount > 0)
            .GroupBy(x => new { x.RevenueAccountId, x.BranchId, x.WarehouseId })
            .Select(group => new JournalEntryLine(group.Key.RevenueAccountId, 0, group.Sum(x => x.CompanyNetAmount),
                group.Key.BranchId, group.Key.WarehouseId, $"Sales revenue for {invoice.InvoiceNumber}")));
        lines.AddRange(slices.Where(x => x.CompanyTaxAmount > 0)
            .GroupBy(x => new { TaxAccountId = x.TaxOutputAccountId!.Value, x.BranchId, x.WarehouseId })
            .Select(group => new JournalEntryLine(group.Key.TaxAccountId, 0, group.Sum(x => x.CompanyTaxAmount),
                group.Key.BranchId, group.Key.WarehouseId, $"Output tax for {invoice.InvoiceNumber}")));
        await journalPosting.PostAsync(new JournalPostingRequest(invoice.InvoiceDate.ToDateTime(TimeOnly.MinValue),
            $"Customer invoice {invoice.InvoiceNumber}", JournalSourceType, invoice.InvoiceNumber, lines,
            "This customer invoice has already been posted to accounting."));
    });

    private async Task<List<PostingSlice>> BuildPostingSlicesAsync(CustomerInvoice invoice, int defaultRevenueAccountId)
    {
        var deliveryMap = (await deliveries.GetBySalesOrderAsync(invoice.SalesOrderId)).ToDictionary(x => x.Id);
        var accountMap = (await accounts.GetAllAsync()).ToDictionary(x => x.Id);
        var result = new List<PostingSlice>();
        foreach (var line in invoice.Lines)
        {
            var sources = line.SourceAllocations.GroupBy(x => x.SalesDeliveryId)
                .Select(x => new { DeliveryId = x.Key, StockQuantity = x.Sum(y => y.StockQuantity) }).ToList();
            if (sources.Count == 0)
            {
                result.Add(new PostingSlice(null, null, defaultRevenueAccountId, line.TaxOutputAccountIdSnapshot,
                    line.CompanyNetAmount, line.CompanyTaxAmount));
                continue;
            }

            var remainingNet = line.CompanyNetAmount; var remainingTax = line.CompanyTaxAmount;
            for (var index = 0; index < sources.Count; index++)
            {
                var source = sources[index];
                if (!deliveryMap.TryGetValue(source.DeliveryId, out var delivery))
                    throw new InvalidOperationException("A source delivery selected on this customer invoice no longer exists.");
                var warehouse = await warehouses.GetByIdAsync(delivery.WarehouseId)
                    ?? throw new InvalidOperationException("Customer invoice source warehouse was not found.");
                if (!warehouse.BranchId.HasValue)
                    throw new InvalidOperationException("Assign the customer invoice source warehouse to a branch before posting.");
                var branch = await branches.GetByIdAsync(warehouse.BranchId.Value)
                    ?? throw new InvalidOperationException("Customer invoice source branch was not found.");
                if (!branch.IsActive)
                    throw new InvalidOperationException("The customer invoice source branch must be active before posting.");
                var revenueAccountId = branch.SalesRevenueAccountId ?? defaultRevenueAccountId;
                if (!accountMap.TryGetValue(revenueAccountId, out var branchRevenue) ||
                    !branchRevenue.IsActive || branchRevenue.Type != AccountType.Revenue)
                    throw new InvalidOperationException("Configure an active sales revenue account for the customer invoice source branch.");
                var last = index == sources.Count - 1;
                var net = last ? remainingNet : RoundMoney(line.CompanyNetAmount * source.StockQuantity / line.StockQuantity);
                var tax = last ? remainingTax : RoundMoney(line.CompanyTaxAmount * source.StockQuantity / line.StockQuantity);
                remainingNet -= net; remainingTax -= tax;
                result.Add(new PostingSlice(branch.Id, warehouse.Id, revenueAccountId,
                    line.TaxOutputAccountIdSnapshot, net, tax));
            }
        }
        return result;
    }

    public Task ReverseAsync(ReverseCustomerInvoiceDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var invoice = await invoices.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Customer invoice was not found.");
        EnsureVersion(invoice.RowVersion, dto.RowVersion);
        if (invoice.Status != CustomerInvoiceStatus.Posted)
            throw new InvalidOperationException("Only a posted customer invoice can be reversed.");
        if ((await customerPayments.GetPaidAmountsAsync([invoice.Id])).GetValueOrDefault(invoice.Id) > 0)
            throw new InvalidOperationException("Reverse allocated customer payments before reversing the customer invoice.");
        if ((await customerCredits.GetPostedAmountsAsync([invoice.Id])).GetValueOrDefault(invoice.Id) > 0)
            throw new InvalidOperationException("A customer invoice with posted credit notes cannot be reversed.");
        await journalPosting.ReverseAsync(JournalSourceType, invoice.InvoiceNumber, invoice.InvoiceNumber,
            dto.ReversalDate.ToDateTime(TimeOnly.MinValue), dto.Reason);
        invoice.Reverse(currentUser.UserId, dto.ReversalDate, dto.Reason);
    });

    private async Task<EligibilityContext> BuildEligibilityAsync(SalesOrder order)
    {
        var settings = await accountingSettings.GetAsync();
        var policy = settings?.SalesInvoicePolicy ?? SalesInvoiceQuantityPolicy.DeliveredQuantity;
        var productMap = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var postedDeliveries = (await deliveries.GetBySalesOrderAsync(order.Id))
            .Where(x => x.Status == SalesDeliveryStatus.Posted).OrderBy(x => x.PostedAtUtc).ThenBy(x => x.Id).ToList();
        var activeInvoices = (await invoices.GetBySalesOrderAsync(order.Id))
            .Where(x => x.Status is CustomerInvoiceStatus.Draft or CustomerInvoiceStatus.Approved or CustomerInvoiceStatus.Posted).ToList();
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
    private sealed record PostingSlice(int? BranchId, int? WarehouseId, int RevenueAccountId,
        int? TaxOutputAccountId, decimal CompanyNetAmount, decimal CompanyTaxAmount)
    { public decimal CompanyTotalAmount => CompanyNetAmount + CompanyTaxAmount; }
}
