using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SupplierQuotationService(
    ISupplierQuotationRepository quotations,
    IPurchaseSourcingRepository sourcing,
    ISupplierRepository suppliers,
    IPurchaseOrderRepository orders,
    IPurchaseAgreementRepository agreements,
    DocumentNumberService numbers,
    CurrencyExchangeRateService exchangeRates,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<SupplierQuotationCreatePageDto> GetCreatePageAsync(
        int sourcingEventId, CreateSupplierQuotationDto? form = null)
    {
        var source = await sourcing.GetByIdAsync(sourcingEventId);
        var currencyPage = await exchangeRates.GetPageAsync();
        var currency = currencyPage.CompanyCurrencyCode;
        var supplierNames = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return new SupplierQuotationCreatePageDto
        {
            Form = form ?? new CreateSupplierQuotationDto
            {
                PurchaseSourcingEventId = sourcingEventId,
                CurrencyCode = currency,
                Lines = source?.Lines.Select(x => new CreateSupplierQuotationLineDto
                    { PurchaseSourcingLineId = x.Id, QuotedQuantity = x.RequestedQuantity }).ToList() ?? []
            },
            SourcingEvent = source is null ? null : MapSource(source),
            Suppliers = source?.Invitations.Select(i => i.SupplierId).Distinct().Select(id =>
                new PurchaseRequestOptionDto(id, supplierNames.GetValueOrDefault(id, $"#{id}"))).ToList() ?? [],
            CompanyCurrencyCode = currency,
            AvailableCurrencyCodes = currencyPage.Rates.Select(x => x.CurrencyCode).Append(currency)
                .Distinct().OrderBy(x => x).ToList()
        };
    }

    public async Task<int> CreateAndSubmitAsync(CreateSupplierQuotationDto dto)
    {
        var source = await sourcing.GetByIdAsync(dto.PurchaseSourcingEventId)
            ?? throw new InvalidOperationException("Sourcing event was not found.");
        if (source.Status != PurchaseSourcingStatus.Sent)
            throw new InvalidOperationException("Quotations can be entered only for sent sourcing events.");
        if (!source.Invitations.Any(x => x.SupplierId == dto.SupplierId))
            throw new InvalidOperationException("Supplier was not invited to this sourcing event.");
        if (await quotations.GetBySourcingAndSupplierAsync(source.Id, dto.SupplierId) is not null)
            throw new InvalidOperationException("This supplier already has a quotation for this sourcing event.");
        var supplier = await suppliers.GetByIdAsync(dto.SupplierId)
            ?? throw new ArgumentException("Supplier was not found.");
        var inputLines = dto.Lines.Where(x => x.PurchaseSourcingLineId > 0).ToList();
        if (inputLines.Count != source.Lines.Count ||
            inputLines.Select(x => x.PurchaseSourcingLineId).Distinct().Count() != inputLines.Count ||
            inputLines.Any(x => !source.Lines.Any(s => s.Id == x.PurchaseSourcingLineId)))
            throw new ArgumentException("Quotation lines must match every sourcing line exactly once.");
        var currency = (await exchangeRates.ResolveAsync(dto.CurrencyCode, dto.QuotationDate)).CurrencyCode;
        SupplierQuotation? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = new SupplierQuotation(
                await numbers.GenerateAsync(DocumentNumberType.SupplierQuotation, DateTime.Today), source.Id,
                supplier.Id, dto.SupplierReference, dto.QuotationDate, dto.ValidUntilDate, dto.LeadTimeDays,
                currency, dto.PaymentTerms, dto.Notes, currentUser.UserId);
            foreach (var input in inputLines)
            {
                var sourceLine = source.Lines.Single(x => x.Id == input.PurchaseSourcingLineId);
                created.AddLine(new SupplierQuotationLine(sourceLine.Id, input.QuotedQuantity, input.UnitPrice,
                    input.DiscountPercent, input.TaxPercent, sourceLine.ProductCodeSnapshot,
                    sourceLine.ProductNameSnapshot, sourceLine.UnitNameSnapshot));
            }
            created.Submit();
            await quotations.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<SupplierQuotationComparisonPageDto> GetComparisonAsync(
        int sourcingEventId, CreatePurchaseQuotationAwardDto? form = null)
    {
        var source = await sourcing.GetByIdAsync(sourcingEventId)
            ?? throw new InvalidOperationException("Sourcing event was not found.");
        var supplierNames = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var rows = await quotations.GetBySourcingEventAsync(sourcingEventId);
        var award = await quotations.GetAwardAsync(sourcingEventId);
        var purchaseOrders = await orders.GetBySourcingEventIdAsync(sourcingEventId);
        var agreement = award is null ? null : await agreements.GetByQuotationIdAsync(award.SupplierQuotationId);
        var mapped = new List<SupplierQuotationDto>();
        foreach (var quotation in rows)
        {
            var rate = await exchangeRates.ResolveAsync(quotation.CurrencyCode, quotation.QuotationDate);
            mapped.Add(Map(quotation,
                supplierNames.GetValueOrDefault(quotation.SupplierId, $"#{quotation.SupplierId}"), rate));
        }

        var allocations = award?.Allocations.Select(x => new PurchaseQuotationAwardAllocationDto
        {
            PurchaseSourcingLineId = x.PurchaseSourcingLineId,
            SupplierQuotationId = x.SupplierQuotationId,
            SupplierQuotationLineId = x.SupplierQuotationLineId,
            SupplierName = supplierNames.GetValueOrDefault(x.SupplierId, $"#{x.SupplierId}"),
            AwardedQuantity = x.AwardedQuantity,
            UnitPrice = x.UnitPriceSnapshot,
            GrossAmount = x.GrossAmount,
            CurrencyCode = x.CurrencyCodeSnapshot,
            LeadTimeDays = x.LeadTimeDaysSnapshot,
            Reason = x.Reason
        }).ToList() ?? [];
        var canCreateAgreement = award is not null && allocations.Count > 0 &&
            allocations.Select(x => x.SupplierQuotationId).Distinct().Count() == 1 &&
            source.Lines.All(line => allocations.Count(x => x.PurchaseSourcingLineId == line.Id) == 1);

        return new SupplierQuotationComparisonPageDto
        {
            SourcingEventId = sourcingEventId,
            AwardedQuotationId = award?.SupplierQuotationId,
            PurchaseOrderId = purchaseOrders.Count == 1 ? purchaseOrders[0].Id : null,
            PurchaseAgreementId = agreement?.Id,
            Form = form ?? new CreatePurchaseQuotationAwardDto
            {
                SourcingEventId = sourcingEventId,
                Allocations = mapped.SelectMany(x => x.Lines).Select(x =>
                    new CreatePurchaseQuotationAwardAllocationDto { SupplierQuotationLineId = x.Id }).ToList()
            },
            SourcingLines = source.Lines.Select(x => new PurchaseSourcingLineDto
            {
                Id = x.Id, ProductCode = x.ProductCodeSnapshot, ProductName = x.ProductNameSnapshot,
                UnitName = x.UnitNameSnapshot, RequestedQuantity = x.RequestedQuantity,
                StockQuantity = x.StockQuantity, Notes = x.Notes
            }).ToList(),
            AwardAllocations = allocations,
            PurchaseOrders = purchaseOrders.Select(x => new PurchaseRequestOptionDto(x.Id, x.OrderNumber)).ToList(),
            CompanyCurrencyCode = mapped.FirstOrDefault()?.CompanyCurrencyCode ??
                (await exchangeRates.GetPageAsync()).CompanyCurrencyCode,
            Quotations = mapped,
            CanCreatePurchaseAgreement = canCreateAgreement
        };
    }

    public async Task AwardAsync(CreatePurchaseQuotationAwardDto dto)
    {
        var source = await sourcing.GetByIdAsync(dto.SourcingEventId)
            ?? throw new InvalidOperationException("Sourcing event was not found.");
        if (source.Status != PurchaseSourcingStatus.Sent)
            throw new InvalidOperationException("Only sent sourcing events can be awarded.");
        if (await quotations.GetAwardAsync(dto.SourcingEventId) is not null)
            throw new InvalidOperationException("This sourcing event award is already complete.");
        var submitted = (await quotations.GetBySourcingEventAsync(dto.SourcingEventId))
            .Where(x => x.Status == SupplierQuotationStatus.Submitted).ToList();
        var quotationLines = submitted.SelectMany(quotation => quotation.Lines.Select(line =>
            new { Quotation = quotation, Line = line })).ToDictionary(x => x.Line.Id);
        var selected = dto.Allocations.Where(x => x.AwardedQuantity > 0).ToList();
        if (selected.Count == 0) throw new ArgumentException("Select at least one positive award allocation.");
        if (selected.Select(x => x.SupplierQuotationLineId).Distinct().Count() != selected.Count)
            throw new ArgumentException("Each supplier quotation line can be allocated only once.");
        if (selected.Any(x => !quotationLines.ContainsKey(x.SupplierQuotationLineId)))
            throw new ArgumentException("One or more award lines do not belong to a submitted quotation for this sourcing event.");
        foreach (var input in selected)
        {
            var candidate = quotationLines[input.SupplierQuotationLineId];
            if (input.AwardedQuantity > candidate.Line.QuotedQuantity)
                throw new InvalidOperationException("Awarded quantity cannot exceed the supplier quoted quantity.");
            if (string.IsNullOrWhiteSpace(input.Reason) || input.Reason.Trim().Length > 1000)
                throw new ArgumentException("Each positive award allocation requires a reason up to 1000 characters.");
        }
        foreach (var sourceLine in source.Lines)
        {
            var allocated = selected.Where(input =>
                    quotationLines[input.SupplierQuotationLineId].Line.PurchaseSourcingLineId == sourceLine.Id)
                .Sum(x => x.AwardedQuantity);
            if (allocated != sourceLine.RequestedQuantity)
                throw new InvalidOperationException("Award allocations for every sourcing line must equal its requested quantity exactly.");
        }

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await quotations.GetAwardAsync(dto.SourcingEventId) is not null)
                throw new InvalidOperationException("This sourcing event award is already complete.");
            var primary = quotationLines[selected[0].SupplierQuotationLineId].Quotation;
            var award = new PurchaseQuotationAward(source.Id, primary.Id,
                "Line-level allocation decision", currentUser.UserId);
            foreach (var input in selected)
            {
                var candidate = quotationLines[input.SupplierQuotationLineId];
                award.AddAllocation(new PurchaseQuotationAwardAllocation(
                    candidate.Line.PurchaseSourcingLineId, candidate.Quotation.Id, candidate.Line.Id,
                    candidate.Quotation.SupplierId, input.AwardedQuantity, candidate.Line.UnitPrice,
                    candidate.Line.DiscountPercent, candidate.Line.TaxPercent, candidate.Quotation.CurrencyCode,
                    candidate.Quotation.LeadTimeDays, candidate.Quotation.PaymentTermsSnapshot, input.Reason!,
                    candidate.Line.ProductCodeSnapshot, candidate.Line.ProductNameSnapshot,
                    candidate.Line.UnitNameSnapshot));
            }
            await quotations.AddAwardAsync(award);
            source.CompleteAward();
        });
    }

    private static SupplierQuotationDto Map(SupplierQuotation quotation, string supplierName,
        ResolvedExchangeRate rate) => new()
    {
        Id = quotation.Id,
        QuotationNumber = quotation.QuotationNumber,
        SupplierId = quotation.SupplierId,
        SupplierName = supplierName,
        Status = quotation.Status,
        CurrencyCode = quotation.CurrencyCode,
        CompanyCurrencyCode = rate.CompanyCurrencyCode,
        ComparisonExchangeRate = rate.Rate,
        CompanyGrossAmount = rate.Convert(quotation.GrossAmount),
        ValidUntilDate = quotation.ValidUntilDate,
        LeadTimeDays = quotation.LeadTimeDays,
        NetAmount = quotation.NetAmount,
        TaxAmount = quotation.TaxAmount,
        GrossAmount = quotation.GrossAmount,
        Lines = quotation.Lines.Select(line => new SupplierQuotationLineDto
        {
            Id = line.Id,
            PurchaseSourcingLineId = line.PurchaseSourcingLineId,
            ProductCode = line.ProductCodeSnapshot,
            ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot,
            QuotedQuantity = line.QuotedQuantity,
            UnitPrice = line.UnitPrice,
            DiscountPercent = line.DiscountPercent,
            TaxPercent = line.TaxPercent,
            GrossAmount = line.GrossAmount,
            CompanyGrossAmount = rate.Convert(line.GrossAmount)
        }).ToList()
    };

    private static PurchaseSourcingDto MapSource(PurchaseSourcingEvent source) => new()
    {
        Id = source.Id,
        SourcingNumber = source.SourcingNumber,
        Status = source.Status,
        Lines = source.Lines.Select(line => new PurchaseSourcingLineDto
        {
            Id = line.Id,
            ProductCode = line.ProductCodeSnapshot,
            ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot,
            RequestedQuantity = line.RequestedQuantity,
            StockQuantity = line.StockQuantity
        }).ToList()
    };
}
