using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseSourcingService(
    IPurchaseSourcingRepository sourcing,
    IPurchaseRequestRepository requests,
    IProductRepository products,
    IMeasurementUnitRepository units,
    ISupplierRepository suppliers,
    IWarehouseRepository warehouses,
    DocumentNumberService numbers,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<List<PurchaseSourcingDto>> GetAllAsync(PurchaseSourcingStatus? status, string? search)
    {
        var requestsById = (await requests.GetAllAsync(null, null)).ToDictionary(x => x.Id);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await sourcing.GetAllAsync(status, search)).Select(x => new PurchaseSourcingDto
        {
            Id = x.Id, SourcingNumber = x.SourcingNumber, Status = x.Status,
            PurchaseRequestNumber = requestsById.GetValueOrDefault(x.PurchaseRequestId)?.RequestNumber ?? $"#{x.PurchaseRequestId}",
            WarehouseName = warehouseNames.GetValueOrDefault(x.WarehouseId, $"#{x.WarehouseId}"),
            RequestedByDate = x.RequestedByDate, Notes = x.Notes, RowVersion = x.RowVersion,
            Invitations = x.Invitations.Select(i => new PurchaseSourcingInvitationDto { SupplierId = i.SupplierId, CreatedAtUtc = i.CreatedAtUtc }).ToList()
        }).ToList();
    }

    public async Task<PurchaseSourcingCreatePageDto> GetCreatePageAsync(int purchaseRequestId, CreatePurchaseSourcingDto? form = null)
    {
        var request = await GetRequestDtoAsync(purchaseRequestId);
        return new PurchaseSourcingCreatePageDto
        {
            Form = form ?? new CreatePurchaseSourcingDto { PurchaseRequestId = purchaseRequestId },
            Request = request,
            Suppliers = (await suppliers.GetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new PurchaseRequestOptionDto(x.Id, x.Name)).ToList()
        };
    }

    public async Task<int> CreateAndSendAsync(CreatePurchaseSourcingDto dto)
    {
        var request = await requests.GetByIdAsync(dto.PurchaseRequestId)
            ?? throw new InvalidOperationException("Purchase request was not found.");
        if (request.Status != PurchaseRequestStatus.Approved)
            throw new InvalidOperationException("Only approved purchase requests can start sourcing.");
        if (await sourcing.GetByPurchaseRequestIdAsync(request.Id) is not null)
            throw new InvalidOperationException("This purchase request already has a sourcing event.");
        var supplierIds = dto.SupplierIds.Where(x => x > 0).Distinct().ToList();
        if (supplierIds.Count == 0) throw new ArgumentException("At least one supplier must be selected.");
        var selectedSuppliers = new Dictionary<int, Supplier>();
        foreach (var supplierId in supplierIds)
            selectedSuppliers[supplierId] = await suppliers.GetByIdAsync(supplierId)
                ?? throw new ArgumentException("One or more suppliers were not found.");

        PurchaseSourcingEvent? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = new PurchaseSourcingEvent(
                await numbers.GenerateAsync(DocumentNumberType.PurchaseSourcingEvent, DateTime.Today),
                request.Id, request.WarehouseId, request.NeededByDate, dto.Notes, currentUser.UserId);
            foreach (var requestLine in request.Lines)
            {
                var product = await products.GetByIdAsync(requestLine.ProductId)
                    ?? throw new InvalidOperationException("Purchase request product was not found.");
                var unit = await units.GetByIdAsync(requestLine.MeasurementUnitId)
                    ?? throw new InvalidOperationException("Purchase request measurement unit was not found.");
                created.AddLine(new PurchaseSourcingLine(requestLine.Id, requestLine.ProductId, requestLine.MeasurementUnitId,
                    requestLine.RequestedQuantity, requestLine.UnitFactorToBase, requestLine.StockQuantity,
                    product.ProductCode, product.Name, unit.Symbol, requestLine.Notes));
            }
            foreach (var supplierId in selectedSuppliers.Keys) created.InviteSupplier(supplierId, dto.InvitationMessage);
            created.Send(currentUser.UserId);
            request.StartSourcing(currentUser.UserId);
            await sourcing.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<PurchaseSourcingDto?> GetAsync(int id)
    {
        var source = await sourcing.GetByIdAsync(id);
        if (source is null) return null;
        var request = await requests.GetByIdAsync(source.PurchaseRequestId);
        var warehouse = await warehouses.GetByIdAsync(source.WarehouseId);
        var supplierNames = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return new PurchaseSourcingDto
        {
            Id = source.Id, SourcingNumber = source.SourcingNumber, Status = source.Status,
            PurchaseRequestNumber = request?.RequestNumber ?? $"#{source.PurchaseRequestId}",
            WarehouseName = warehouse?.Name ?? $"#{source.WarehouseId}", RequestedByDate = source.RequestedByDate,
            Notes = source.Notes, RowVersion = source.RowVersion,
            Lines = source.Lines.Select(x => new PurchaseSourcingLineDto { Id=x.Id, ProductCode = x.ProductCodeSnapshot,
                ProductName = x.ProductNameSnapshot, UnitName = x.UnitNameSnapshot, RequestedQuantity = x.RequestedQuantity,
                StockQuantity = x.StockQuantity, Notes = x.Notes }).ToList(),
            Invitations = source.Invitations.Select(x => new PurchaseSourcingInvitationDto { SupplierId = x.SupplierId,
                SupplierName = supplierNames.GetValueOrDefault(x.SupplierId, $"#{x.SupplierId}"), Message = x.Message,
                CreatedAtUtc = x.CreatedAtUtc }).ToList()
        };
    }

    private async Task<PurchaseRequestDto?> GetRequestDtoAsync(int id)
    {
        var request = await requests.GetByIdAsync(id);
        if (request is null) return null;
        var productsById = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var unitsById = (await units.GetAllAsync()).ToDictionary(x => x.Id);
        return new PurchaseRequestDto { Id = request.Id, RequestNumber = request.RequestNumber,
            WarehouseId = request.WarehouseId, NeededByDate = request.NeededByDate, Priority = request.Priority,
            Justification = request.Justification, Notes = request.Notes, Status = request.Status,
            Lines = request.Lines.Select(x => new PurchaseRequestLineDto { ProductId = x.ProductId,
                ProductCode = productsById.GetValueOrDefault(x.ProductId)?.ProductCode ?? string.Empty,
                ProductName = productsById.GetValueOrDefault(x.ProductId)?.Name ?? $"#{x.ProductId}",
                MeasurementUnitId = x.MeasurementUnitId, UnitName = unitsById.GetValueOrDefault(x.MeasurementUnitId)?.Symbol ?? string.Empty,
                RequestedQuantity = x.RequestedQuantity, StockQuantity = x.StockQuantity, Notes = x.Notes }).ToList() };
    }
}
