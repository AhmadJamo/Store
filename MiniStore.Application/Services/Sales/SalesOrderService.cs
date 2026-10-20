using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesOrderService(ISalesOrderRepository orders,ISalesQuotationRepository quotations,
    DocumentNumberService numbers,IUnitOfWork unitOfWork,ICurrentUserService currentUser,
    SalesOrderReservationService reservationService)
{
    public async Task<List<SalesOrderSummaryDto>> GetAllAsync(SalesOrderStatus? status,string? search)=>(await orders.GetAllAsync(status,search))
        .Select(x=>new SalesOrderSummaryDto(x.Id,x.OrderNumber,x.SourceQuotationNumber,x.CustomerCode,x.CustomerName,x.OrderDate,x.CurrencyCode,x.TotalAmount,x.Status)).ToList();
    public async Task<SalesOrderDetailsDto?> GetAsync(int id)
    {
        var order=await orders.GetByIdAsync(id);if(order is null)return null;
        var dto=Map(order);dto.Reservation=await reservationService.GetSummaryAsync(id);return dto;
    }

    public async Task<int> CreateFromQuotationAsync(ConvertSalesQuotationDto dto)
    {
        var existing=await orders.GetBySourceQuotationIdAsync(dto.QuotationId);if(existing is not null)return existing.Id;
        SalesOrder? created=null;
        await unitOfWork.ExecuteInTransactionAsync(async()=>
        {
            existing=await orders.GetBySourceQuotationIdAsync(dto.QuotationId);if(existing is not null){created=existing;return;}
            var quotation=await quotations.GetByIdAsync(dto.QuotationId)??throw new InvalidOperationException("Sales quotation was not found.");
            EnsureVersion(quotation.RowVersion,dto.RowVersion,"Sales quotation was modified by another user. Reload and try again.");
            created=new SalesOrder(await numbers.GenerateAsync(DocumentNumberType.SalesOrder,DateTime.Today),quotation,DateOnly.FromDateTime(DateTime.Today),currentUser.UserId);
            await orders.AddAsync(created);
        });
        return created!.Id;
    }

    public Task ConfirmAsync(SalesOrderStatusCommandDto dto)=>ChangeAsync(dto,x=>x.Confirm(currentUser.UserId));
    public Task CancelAsync(SalesOrderStatusCommandDto dto)=>unitOfWork.ExecuteInTransactionAsync(async()=>
    {
        var order=await orders.GetByIdAsync(dto.Id)??throw new InvalidOperationException("Sales order was not found.");
        EnsureVersion(order.RowVersion,dto.RowVersion,"Sales order was modified by another user. Reload and try again.");
        await reservationService.ReleaseAllActiveAsync(order.Id,"Sales order cancelled.");
        order.Cancel(currentUser.UserId,dto.Reason??"");
    });
    private Task ChangeAsync(SalesOrderStatusCommandDto dto,Action<SalesOrder> action)=>unitOfWork.ExecuteInTransactionAsync(async()=>
    {var order=await orders.GetByIdAsync(dto.Id)??throw new InvalidOperationException("Sales order was not found.");EnsureVersion(order.RowVersion,dto.RowVersion,"Sales order was modified by another user. Reload and try again.");action(order);});
    private static void EnsureVersion(byte[] current,byte[] submitted,string message){if(submitted.Length==0||!current.SequenceEqual(submitted))throw new InvalidOperationException(message);}

    private static SalesOrderDetailsDto Map(SalesOrder x)=>new()
    {
        Id=x.Id,OrderNumber=x.OrderNumber,SourceQuotationId=x.SourceQuotationId,SourceQuotationNumber=x.SourceQuotationNumber,Status=x.Status,
        CustomerCode=x.CustomerCode,CustomerName=x.CustomerName,CustomerLegalName=x.CustomerLegalName,CustomerTaxNumber=x.CustomerTaxNumber,
        Address=x.AddressLine1 is null?null:$"{x.AddressLabel}: {x.AddressLine1}{(x.AddressLine2 is null?"":", "+x.AddressLine2)}, {x.City}{(x.StateOrProvince is null?"":", "+x.StateOrProvince)} {x.PostalCode}, {x.CountryCode}",
        OrderDate=x.OrderDate,CurrencyCode=x.CurrencyCode,ExchangeRate=x.ExchangeRate,ExchangeRateDate=x.ExchangeRateDate,ExchangeRateSource=x.ExchangeRateSource,PaymentTermDays=x.PaymentTermDays,Notes=x.Notes,
        NetAmount=x.NetAmount,TaxAmount=x.TaxAmount,TotalAmount=x.TotalAmount,CreatedByUserId=x.CreatedByUserId,CreatedAtUtc=x.CreatedAtUtc,ConfirmedByUserId=x.ConfirmedByUserId,ConfirmedAtUtc=x.ConfirmedAtUtc,RowVersion=x.RowVersion,
        Lines=x.Lines.Select(l=>new SalesOrderLineDto(l.Id,l.SourceQuotationLineId,l.ProductCode,l.ProductName,l.UnitCode,l.UnitName,l.Quantity,l.StockQuantity,l.UnitPrice,l.CompanyUnitPrice,l.NetAmount,l.PriceSourceCode,l.TaxName,l.TaxRatePercent,l.IsTaxInclusive,l.TaxSourceCode,l.TaxAmount,l.TotalAmount,l.Notes)).ToList(),
        History=x.History.OrderBy(h=>h.CreatedAtUtc).Select(h=>new SalesOrderHistoryDto(h.FromStatus,h.ToStatus,h.Action,h.UserId,h.Reason,h.CreatedAtUtc)).ToList()
    };
}
