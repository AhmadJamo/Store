using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;

public sealed class PurchaseAgreementService(IPurchaseAgreementRepository agreements, IPurchaseOrderRepository orders, ISupplierQuotationRepository quotations, IPurchaseSourcingRepository sourcing, ISupplierRepository suppliers, IWarehouseRepository warehouses, IProductRepository products, DocumentNumberService numbers, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<PurchaseAgreementCreatePageDto> GetCreatePageAsync(int sourcingEventId)
    {
        var award = await quotations.GetAwardAsync(sourcingEventId)
            ?? throw new InvalidOperationException("A quotation award is required before creating a purchase agreement.");
        var source = await sourcing.GetByIdAsync(sourcingEventId)
            ?? throw new InvalidOperationException("Sourcing event was not found.");
        var awarded = RequireSingleSupplierAgreementAward(award, source);
        var quote = await quotations.GetByIdAsync(award.SupplierQuotationId)
            ?? throw new InvalidOperationException("Awarded supplier quotation was not found.");
        var supplier = await suppliers.GetByIdAsync(quote.SupplierId);
        var warehouse = await warehouses.GetByIdAsync(source.WarehouseId);
        return new()
        {
            SourcingEventId = source.Id, QuotationNumber = quote.QuotationNumber,
            SupplierName = supplier?.Name ?? "-", WarehouseName = warehouse?.Name ?? "-",
            CurrencyCode = quote.CurrencyCode,
            Lines = quote.Lines.Where(x => awarded.ContainsKey(x.Id)).Select(x =>
                new PurchaseAgreementSourceLineDto
                {
                    SupplierQuotationLineId = x.Id,
                    Product = $"{x.ProductCodeSnapshot} — {x.ProductNameSnapshot}",
                    UnitName = x.UnitNameSnapshot,
                    QuotedQuantity = awarded[x.Id].AwardedQuantity
                }).ToList()
        };
    }

    public async Task<int> CreateAsync(CreatePurchaseAgreementDto dto)
    {
        PurchaseAgreement? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var award = await quotations.GetAwardAsync(dto.SourcingEventId)
                ?? throw new InvalidOperationException("A quotation award is required before creating a purchase agreement.");
            var source = await sourcing.GetByIdAsync(dto.SourcingEventId)
                ?? throw new InvalidOperationException("Sourcing event was not found.");
            var awarded = RequireSingleSupplierAgreementAward(award, source);
            var quote = await quotations.GetByIdAsync(award.SupplierQuotationId)
                ?? throw new InvalidOperationException("Awarded supplier quotation was not found.");
            if (await agreements.GetByQuotationIdAsync(quote.Id) is not null)
                throw new InvalidOperationException("A purchase agreement already exists for this quotation.");
            if (await orders.GetByQuotationIdAsync(quote.Id) is not null)
                throw new InvalidOperationException("A direct purchase order already exists for this quotation.");
            var positiveLines = dto.Lines.Where(x => x.MaximumQuantity > 0).ToList();
            if (positiveLines.Count == 0)
                throw new ArgumentException("A purchase agreement must contain at least one positive maximum quantity.");
            if (positiveLines.GroupBy(x => x.SupplierQuotationLineId).Any(x => x.Count() > 1))
                throw new ArgumentException("Each quotation line can appear only once in a purchase agreement.");
            var inputs = positiveLines.ToDictionary(x => x.SupplierQuotationLineId);
            if (inputs.Any(x => !awarded.TryGetValue(x.Key, out var allocation) || x.Value.MaximumQuantity > allocation.AwardedQuantity))
                throw new InvalidOperationException("Purchase agreement quantity cannot exceed the awarded quantity.");
            created = new PurchaseAgreement(await numbers.GenerateAsync(DocumentNumberType.PurchaseAgreement, DateTime.Today),
                source.Id, quote.Id, quote.SupplierId, source.WarehouseId, dto.ValidFrom, dto.ValidUntil,
                quote.CurrencyCode, quote.PaymentTermsSnapshot, currentUser.UserId);
            foreach (var line in quote.Lines.Where(x => inputs.ContainsKey(x.Id)))
            {
                var sourceLine = source.Lines.Single(x => x.Id == line.PurchaseSourcingLineId);
                created.AddLine(new PurchaseAgreementLine(line.Id, sourceLine.ProductId,
                    sourceLine.MeasurementUnitId, inputs[line.Id].MaximumQuantity, sourceLine.UnitFactorToBase,
                    line.UnitPrice, line.DiscountPercent, line.TaxPercent, line.ProductCodeSnapshot,
                    line.ProductNameSnapshot, line.UnitNameSnapshot));
            }
            if (created.Lines.Count != inputs.Count)
                throw new ArgumentException("One or more purchase agreement lines do not belong to the awarded quotation.");
            await agreements.AddAsync(created);
        });
        return created!.Id;
    }
    public async Task<List<PurchaseAgreementDto>> GetAllAsync() { var ss = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name); var ws = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name); var result = new List<PurchaseAgreementDto>(); foreach (var value in await agreements.GetAllAsync()) result.Add(await Map(value, ss.GetValueOrDefault(value.SupplierId, "-"), ws.GetValueOrDefault(value.WarehouseId, "-"))); return result; }
    public async Task<PurchaseAgreementDto?> GetAsync(int id) { var value = await agreements.GetByIdAsync(id); if (value is null) return null; return await Map(value, (await suppliers.GetByIdAsync(value.SupplierId))?.Name ?? "-", (await warehouses.GetByIdAsync(value.WarehouseId))?.Name ?? "-"); }
    public async Task ActivateAsync(int id, byte[] rowVersion) => await Change(id, rowVersion, x => x.Activate(currentUser.UserId));
    public async Task CancelAsync(int id, string reason, byte[] rowVersion) => await Change(id, rowVersion, x => x.Cancel(currentUser.UserId, reason));
    public async Task<int> ReleaseAsync(ReleasePurchaseAgreementDto dto) { PurchaseOrder? created = null; await unitOfWork.ExecuteInTransactionAsync(async () => { var agreement = await agreements.GetByIdAsync(dto.AgreementId) ?? throw new InvalidOperationException("Purchase agreement was not found."); var today = DateOnly.FromDateTime(DateTime.Today); if (agreement.Status != PurchaseAgreementStatus.Active) throw new InvalidOperationException("Only active purchase agreements can be released."); if (today < agreement.ValidFrom || today > agreement.ValidUntil) throw new InvalidOperationException("Purchase agreement is outside its validity period."); if (dto.ExpectedDate < today) throw new ArgumentException("Expected delivery date cannot be earlier than order date."); var positiveLines = dto.Lines.Where(x => x.Quantity > 0).ToList(); if (positiveLines.Count == 0) throw new ArgumentException("Enter at least one positive release quantity."); if (positiveLines.GroupBy(x => x.AgreementLineId).Any(x => x.Count() > 1)) throw new ArgumentException("Each agreement line can appear only once in a release."); var inputs = positiveLines.ToDictionary(x => x.AgreementLineId); if (inputs.Keys.Any(id => agreement.Lines.All(x => x.Id != id))) throw new ArgumentException("One or more release lines do not belong to the purchase agreement."); var prior = (await orders.GetByAgreementIdAsync(agreement.Id)).Where(x => x.Status != PurchaseOrderStatus.Cancelled).SelectMany(x => x.Lines).Where(x => x.PurchaseAgreementLineId.HasValue).GroupBy(x => x.PurchaseAgreementLineId!.Value).ToDictionary(x => x.Key, x => x.Sum(y => y.OrderedQuantity)); created = new PurchaseOrder(await numbers.GenerateAsync(DocumentNumberType.PurchaseOrder, DateTime.Today), agreement.PurchaseSourcingEventId, agreement.SupplierQuotationId, agreement.SupplierId, agreement.WarehouseId, today, dto.ExpectedDate, agreement.CurrencyCode, agreement.PaymentTermsSnapshot, currentUser.UserId, agreement.Id); foreach (var line in agreement.Lines.Where(x => inputs.ContainsKey(x.Id))) { var quantity = inputs[line.Id].Quantity; if (quantity > line.MaximumQuantity - prior.GetValueOrDefault(line.Id)) throw new InvalidOperationException("Agreement release quantity exceeds the remaining contracted quantity."); var product = await products.GetByIdAsync(line.ProductId) ?? throw new InvalidOperationException("Purchase agreement product was not found."); created.AddLine(new PurchaseOrderLine(line.SupplierQuotationLineId, line.ProductId, line.MeasurementUnitId, quantity, line.UnitFactorToBase, quantity * line.UnitFactorToBase, line.UnitPrice, line.DiscountPercent, line.TaxPercent, line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot, line.Id, product.PurchaseBillingPolicy, product.PurchaseExpenseAccountId)); } await orders.AddAsync(created); }); return created!.Id; }
    private static Dictionary<int, PurchaseQuotationAwardAllocation> RequireSingleSupplierAgreementAward(
        PurchaseQuotationAward award, PurchaseSourcingEvent source)
    {
        if (award.Allocations.Count == 0 ||
            award.Allocations.Select(x => x.SupplierQuotationId).Distinct().Count() != 1 ||
            source.Lines.Any(line => award.Allocations.Count(x => x.PurchaseSourcingLineId == line.Id) != 1))
            throw new InvalidOperationException("Purchase agreements require a complete award to one supplier; split awards must create purchase orders.");
        return award.Allocations.ToDictionary(x => x.SupplierQuotationLineId);
    }
    private async Task Change(int id, byte[] rv, Action<PurchaseAgreement> action) { await unitOfWork.ExecuteInTransactionAsync(async () => { var value = await agreements.GetByIdAsync(id) ?? throw new InvalidOperationException("Purchase agreement was not found."); if (!value.RowVersion.SequenceEqual(rv)) throw new InvalidOperationException("Purchase agreement was modified by another user. Reload and try again."); action(value); await Task.CompletedTask; }); }
    private async Task<PurchaseAgreementDto> Map(PurchaseAgreement x, string supplier, string warehouse) { var released = (await orders.GetByAgreementIdAsync(x.Id)).Where(o => o.Status != PurchaseOrderStatus.Cancelled).SelectMany(o => o.Lines).Where(l => l.PurchaseAgreementLineId.HasValue).GroupBy(l => l.PurchaseAgreementLineId!.Value).ToDictionary(g => g.Key, g => g.Sum(v => v.OrderedQuantity)); return new() { Id = x.Id, AgreementNumber = x.AgreementNumber, SupplierName = supplier, WarehouseName = warehouse, CurrencyCode = x.CurrencyCode, ValidFrom = x.ValidFrom, ValidUntil = x.ValidUntil, Status = x.Status, RowVersion = x.RowVersion, Lines = x.Lines.Select(l => new PurchaseAgreementLineDto { Id = l.Id, Product = $"{l.ProductCodeSnapshot} — {l.ProductNameSnapshot}", UnitName = l.UnitNameSnapshot, MaximumQuantity = l.MaximumQuantity, ReleasedQuantity = released.GetValueOrDefault(l.Id), UnitPrice = l.UnitPrice, DiscountPercent = l.DiscountPercent, TaxPercent = l.TaxPercent }).ToList() }; }
}
