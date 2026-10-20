using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesQuotationService(ISalesQuotationRepository quotations, ICustomerRepository customers,
    IProductRepository products, IMeasurementUnitRepository units, SalesPriceListService pricing,
    ISalesOrderRepository orders, DocumentNumberService numbers, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<SalesQuotationSummaryDto>> GetAllAsync(SalesQuotationStatus? status, string? search) =>
        (await quotations.GetAllAsync(status, search)).Select(x => new SalesQuotationSummaryDto(x.Id, x.QuotationNumber,
            x.CustomerCode, x.CustomerName, x.QuotationDate, x.ValidUntil, x.CurrencyCode, x.TotalAmount, x.Status)).ToList();

    public async Task<SalesQuotationDetailsDto?> GetAsync(int id)
    { var quotation = await quotations.GetByIdAsync(id); if(quotation is null)return null;var order=await orders.GetBySourceQuotationIdAsync(id);return Map(quotation,order?.Id); }

    public async Task<SalesQuotationCreatePageDto> GetCreatePageAsync(CreateSalesQuotationDto? form = null)
    {
        var pricingPage = await pricing.GetPageAsync();
        var customerRows = await customers.GetAllAsync();
        var addresses = new List<SalesQuotationAddressOptionDto>();
        foreach (var customer in customerRows)
            addresses.AddRange((await customers.GetAddressesAsync(customer.Id)).Select(x => new SalesQuotationAddressOptionDto(x.Id, customer.Id, FormatAddress(x))));
        var productRows = (await products.GetAllAsync(null)).Where(x => x.IsActive && x.IsSellableInSales).OrderBy(x => x.Name).ToList();
        return new SalesQuotationCreatePageDto
        {
            Form = form ?? new(),
            Customers = customerRows.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.CustomerCode} — {x.Name}")).ToList(),
            Addresses = addresses,
            Products = productRows.Select(x => new SalesQuotationProductOptionDto(x.Id, $"{x.ProductCode} — {x.Name}", $"{x.ProductCode} {x.Name} {x.Barcode}".Trim())).ToList(),
            Units = pricingPage.Units.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Name} ({x.Symbol})")).ToList(),
            PriceLists = pricingPage.PriceLists.Where(x => x.IsActive).Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Code} — {x.Name}")).ToList(),
            TaxRates = pricingPage.TaxRates.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Name} ({x.Rate:0.####}%)")).ToList()
        };
    }

    public async Task<int> CreateAsync(CreateSalesQuotationDto dto)
    {
        if (!Guid.TryParse(dto.RequestKey, out _)) throw new ArgumentException("Sales quotation request key is invalid.");
        var existing = await quotations.GetByRequestKeyAsync(dto.RequestKey);
        if (existing is not null) return existing.Id;
        if (dto.Lines.Count == 0) throw new ArgumentException("A sales quotation must contain at least one line.");
        SalesQuotation? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            existing = await quotations.GetByRequestKeyAsync(dto.RequestKey);
            if (existing is not null) { created = existing; return; }
            var customer = await customers.GetByIdAsync(dto.CustomerId) ?? throw new InvalidOperationException("Selected quotation customer was not found.");
            if (!customer.IsActive) throw new InvalidOperationException("Selected quotation customer is archived.");
            CustomerAddress? address = null;
            if (dto.CustomerAddressId.HasValue)
            {
                address = await customers.GetAddressAsync(customer.Id, dto.CustomerAddressId.Value) ?? throw new InvalidOperationException("Selected quotation address was not found for this customer.");
                if (!address.IsActive) throw new InvalidOperationException("Selected quotation address is archived.");
            }
            if (dto.ValidUntil < dto.QuotationDate) throw new ArgumentException("Sales quotation expiry cannot be before its date.");
            var evaluated = new List<(CreateSalesQuotationLineDto Input, Product Product, MeasurementUnit Unit, decimal Factor, SalesPriceEvaluationDto Price)>();
            foreach (var line in dto.Lines)
            {
                var product = await products.GetByIdAsync(line.ProductId) ?? throw new InvalidOperationException("Selected quotation product was not found.");
                var unit = await units.GetByIdAsync(line.MeasurementUnitId) ?? throw new InvalidOperationException("Selected quotation sales unit was not found.");
                if (!product.MeasurementUnitId.HasValue) throw new InvalidOperationException("Configure a managed measurement unit for the product before adding it to a quotation.");
                var stockUnit = await units.GetByIdAsync(product.MeasurementUnitId.Value) ?? throw new InvalidOperationException("Product stock unit was not found.");
                var result = await pricing.EvaluateAsync(new EvaluateSalesPriceDto { CustomerId = customer.Id, PriceListId = dto.PriceListId, ProductId = product.Id, MeasurementUnitId = unit.Id, Quantity = line.Quantity, PricingDate = dto.QuotationDate, Channel = dto.Channel, TaxRateId = line.TaxRateId });
                evaluated.Add((line, product, unit, unit.FactorToBaseUnit / stockUnit.FactorToBaseUnit, result));
            }
            var first = evaluated[0].Price;
            if (evaluated.Any(x => x.Price.TransactionCurrency != first.TransactionCurrency || x.Price.ExchangeRate != first.ExchangeRate || x.Price.ExchangeRateDate != first.ExchangeRateDate))
                throw new InvalidOperationException("Every quotation line must resolve to the same currency and exchange-rate snapshot.");
            created = new SalesQuotation(await numbers.GenerateAsync(DocumentNumberType.SalesQuotation, dto.QuotationDate.ToDateTime(TimeOnly.MinValue)), dto.RequestKey,
                customer, address, dto.QuotationDate, dto.ValidUntil, first.TransactionCurrency, first.ExchangeRate, first.ExchangeRateDate, first.ExchangeRateSource, dto.Notes, currentUser.UserId);
            foreach (var row in evaluated)
            {
                var p = row.Price;
                created.AddLine(new SalesQuotationLine(row.Product, row.Unit, row.Factor,
                    new SalesPriceEvaluationSnapshot(p.Quantity, p.NormalizedStockQuantity, p.TransactionCurrency, p.PriceListId, p.RuleId,
                        p.SourceCode, p.UnitPrice, p.CompanyUnitPrice, p.LineNetAmount, p.TaxRateId, p.TaxName, p.TaxRate,
                        p.TaxInclusive, p.TaxSource, p.TaxExemptionReason, p.TaxAmount, p.LineTotal), row.Input.Notes));
            }
            await quotations.AddAsync(created);
        });
        return created!.Id;
    }

    public Task SendAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Send(currentUser.UserId, DateOnly.FromDateTime(DateTime.Today)));
    public Task AcceptAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Accept(currentUser.UserId, DateOnly.FromDateTime(DateTime.Today)));
    public Task RejectAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Reject(currentUser.UserId, dto.Reason ?? ""));
    public Task ExpireAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Expire(currentUser.UserId, DateOnly.FromDateTime(DateTime.Today)));
    public Task CancelAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Cancel(currentUser.UserId, dto.Reason ?? ""));

    private Task ChangeAsync(SalesQuotationStatusCommandDto dto, Action<SalesQuotation> action) => unitOfWork.ExecuteInTransactionAsync(async () =>
    { var quotation = await quotations.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Sales quotation was not found."); EnsureVersion(quotation.RowVersion, dto.RowVersion); action(quotation); });
    private static void EnsureVersion(byte[] current, byte[] submitted) { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException("Sales quotation was modified by another user. Reload and try again."); }
    private static string FormatAddress(CustomerAddress x) => $"{x.Label}: {x.AddressLine1}{(x.AddressLine2 is null ? "" : ", " + x.AddressLine2)}, {x.City}{(x.StateOrProvince is null ? "" : ", " + x.StateOrProvince)} {x.PostalCode}, {x.CountryCode}";
    private static SalesQuotationDetailsDto Map(SalesQuotation x,int? salesOrderId) => new()
    {
        Id = x.Id, QuotationNumber = x.QuotationNumber, Status = x.Status, CustomerCode = x.CustomerCode, CustomerName = x.CustomerName,
        CustomerLegalName = x.CustomerLegalName, CustomerTaxNumber = x.CustomerTaxNumber,
        Address = x.AddressLine1 is null ? null : $"{x.AddressLabel}: {x.AddressLine1}{(x.AddressLine2 is null ? "" : ", " + x.AddressLine2)}, {x.City}{(x.StateOrProvince is null ? "" : ", " + x.StateOrProvince)} {x.PostalCode}, {x.CountryCode}",
        QuotationDate = x.QuotationDate, ValidUntil = x.ValidUntil, CurrencyCode = x.CurrencyCode, ExchangeRate = x.ExchangeRate,
        ExchangeRateDate = x.ExchangeRateDate, ExchangeRateSource = x.ExchangeRateSource, PaymentTermDays = x.PaymentTermDays, Notes = x.Notes,
        NetAmount = x.NetAmount, TaxAmount = x.TaxAmount, TotalAmount = x.TotalAmount, CreatedByUserId = x.CreatedByUserId, CreatedAtUtc = x.CreatedAtUtc, RowVersion = x.RowVersion, SalesOrderId=salesOrderId,
        Lines = x.Lines.Select(l => new SalesQuotationLineDto(l.Id, l.ProductCode, l.ProductName, l.UnitCode, l.UnitName, l.Quantity, l.StockQuantity,
            l.UnitPrice, l.CompanyUnitPrice, l.NetAmount, l.PriceSourceCode, l.TaxName, l.TaxRatePercent, l.IsTaxInclusive, l.TaxSourceCode, l.TaxAmount, l.TotalAmount, l.Notes)).ToList(),
        History = x.History.OrderBy(h => h.CreatedAtUtc).Select(h => new SalesQuotationHistoryDto(h.FromStatus, h.ToStatus, h.Action, h.UserId, h.Reason, h.CreatedAtUtc)).ToList()
    };
}
