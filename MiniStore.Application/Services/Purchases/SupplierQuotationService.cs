using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SupplierQuotationService(
    ISupplierQuotationRepository quotations, IPurchaseSourcingRepository sourcing,
    ISupplierRepository suppliers, IGeneralSettingsRepository settings,
    DocumentNumberService numbers, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<SupplierQuotationCreatePageDto> GetCreatePageAsync(int sourcingEventId, CreateSupplierQuotationDto? form = null)
    {
        var source = await sourcing.GetByIdAsync(sourcingEventId);
        var currency = (await settings.GetAsync())?.Currency ?? "JOD";
        var supplierNames = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return new SupplierQuotationCreatePageDto
        {
            Form = form ?? new CreateSupplierQuotationDto { PurchaseSourcingEventId = sourcingEventId,
                Lines = source?.Lines.Select(x => new CreateSupplierQuotationLineDto { PurchaseSourcingLineId=x.Id, QuotedQuantity=x.RequestedQuantity }).ToList() ?? [] },
            SourcingEvent = source is null ? null : MapSource(source),
            Suppliers = source?.Invitations.Select(i => i.SupplierId).Distinct().Select(id =>
                new PurchaseRequestOptionDto(id, supplierNames.GetValueOrDefault(id, $"#{id}"))).ToList() ?? [],
            CurrencyCode = currency
        };
    }

    public async Task<int> CreateAndSubmitAsync(CreateSupplierQuotationDto dto)
    {
        var source = await sourcing.GetByIdAsync(dto.PurchaseSourcingEventId) ?? throw new InvalidOperationException("Sourcing event was not found.");
        if (source.Status != PurchaseSourcingStatus.Sent) throw new InvalidOperationException("Quotations can be entered only for sent sourcing events.");
        if (!source.Invitations.Any(x => x.SupplierId == dto.SupplierId)) throw new InvalidOperationException("Supplier was not invited to this sourcing event.");
        if (await quotations.GetBySourcingAndSupplierAsync(source.Id, dto.SupplierId) is not null) throw new InvalidOperationException("This supplier already has a quotation for this sourcing event.");
        var supplier = await suppliers.GetByIdAsync(dto.SupplierId) ?? throw new ArgumentException("Supplier was not found.");
        var inputLines = dto.Lines.Where(x => x.PurchaseSourcingLineId > 0).ToList();
        if (inputLines.Count != source.Lines.Count || inputLines.Select(x => x.PurchaseSourcingLineId).Distinct().Count() != inputLines.Count || inputLines.Any(x => !source.Lines.Any(s => s.Id == x.PurchaseSourcingLineId)))
            throw new ArgumentException("Quotation lines must match every sourcing line exactly once.");
        var currency = (await settings.GetAsync())?.Currency ?? "JOD";
        SupplierQuotation? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = new SupplierQuotation(await numbers.GenerateAsync(DocumentNumberType.SupplierQuotation, DateTime.Today), source.Id, supplier.Id, dto.SupplierReference, dto.QuotationDate, dto.ValidUntilDate, dto.LeadTimeDays, currency, dto.PaymentTerms, dto.Notes, currentUser.UserId);
            foreach(var input in inputLines)
            {
                var sourceLine=source.Lines.Single(x=>x.Id==input.PurchaseSourcingLineId);
                created.AddLine(new SupplierQuotationLine(sourceLine.Id,input.QuotedQuantity,input.UnitPrice,input.DiscountPercent,input.TaxPercent,sourceLine.ProductCodeSnapshot,sourceLine.ProductNameSnapshot,sourceLine.UnitNameSnapshot));
            }
            created.Submit();
            await quotations.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<List<SupplierQuotationDto>> GetComparisonAsync(int sourcingEventId)
    {
        var names=(await suppliers.GetAllAsync()).ToDictionary(x=>x.Id,x=>x.Name);
        return (await quotations.GetBySourcingEventAsync(sourcingEventId)).Select(x=>Map(x,names.GetValueOrDefault(x.SupplierId,$"#{x.SupplierId}"))).ToList();
    }
    public async Task AwardAsync(int sourcingEventId,int quotationId,string reason)
    {
        var source=await sourcing.GetByIdAsync(sourcingEventId)??throw new InvalidOperationException("Sourcing event was not found.");
        if(source.Status!=PurchaseSourcingStatus.Sent)throw new InvalidOperationException("Only sent sourcing events can be awarded.");
        if(await quotations.GetAwardAsync(sourcingEventId)is not null)throw new InvalidOperationException("A quotation has already been awarded for this sourcing event.");
        var quotation=await quotations.GetByIdAsync(quotationId)??throw new InvalidOperationException("Supplier quotation was not found.");
        if(quotation.PurchaseSourcingEventId!=sourcingEventId||quotation.Status!=SupplierQuotationStatus.Submitted)throw new InvalidOperationException("Selected quotation does not belong to this sent sourcing event.");
        await unitOfWork.ExecuteInTransactionAsync(()=>quotations.AddAwardAsync(new PurchaseQuotationAward(sourcingEventId,quotationId,reason,currentUser.UserId)));
    }
    private static SupplierQuotationDto Map(SupplierQuotation x,string name)=>new(){Id=x.Id,QuotationNumber=x.QuotationNumber,SupplierId=x.SupplierId,SupplierName=name,Status=x.Status,CurrencyCode=x.CurrencyCode,ValidUntilDate=x.ValidUntilDate,LeadTimeDays=x.LeadTimeDays,NetAmount=x.NetAmount,TaxAmount=x.TaxAmount,GrossAmount=x.GrossAmount,Lines=x.Lines.Select(l=>new SupplierQuotationLineDto{PurchaseSourcingLineId=l.PurchaseSourcingLineId,ProductCode=l.ProductCodeSnapshot,ProductName=l.ProductNameSnapshot,UnitName=l.UnitNameSnapshot,QuotedQuantity=l.QuotedQuantity,UnitPrice=l.UnitPrice,DiscountPercent=l.DiscountPercent,TaxPercent=l.TaxPercent,GrossAmount=l.GrossAmount}).ToList()};
    private static PurchaseSourcingDto MapSource(PurchaseSourcingEvent x)=>new(){Id=x.Id,SourcingNumber=x.SourcingNumber,Status=x.Status,Lines=x.Lines.Select(l=>new PurchaseSourcingLineDto{Id=l.Id,ProductCode=l.ProductCodeSnapshot,ProductName=l.ProductNameSnapshot,UnitName=l.UnitNameSnapshot,RequestedQuantity=l.RequestedQuantity,StockQuantity=l.StockQuantity}).ToList()};
}
