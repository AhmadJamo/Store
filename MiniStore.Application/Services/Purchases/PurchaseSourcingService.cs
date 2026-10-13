using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseSourcingService(
    IPurchaseSourcingRepository sourcing,
    IPurchaseRequestRepository requests,
    IPurchaseSourcingPolicyRepository policies,
    IProductRepository products,
    IMeasurementUnitRepository units,
    ISupplierRepository suppliers,
    IWarehouseRepository warehouses,
    ITenantAuthorizationRepository authorization,
    ITenantContext tenantContext,
    GeneralSettingsService generalSettings,
    DocumentNumberService numbers,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<List<PurchaseSourcingDto>> GetAllAsync(PurchaseSourcingStatus? status, string? search)
    {
        var requestsById = (await requests.GetAllAsync(null, null)).ToDictionary(x => x.Id);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var currency = (await generalSettings.GetAsync()).Currency;
        return (await sourcing.GetAllAsync(status, search)).Select(x => new PurchaseSourcingDto
        {
            Id = x.Id,
            SourcingNumber = x.SourcingNumber,
            PurchaseRequestId = x.PurchaseRequestId,
            PurchaseRequestNumber = x.PurchaseRequestId.HasValue
                ? requestsById.GetValueOrDefault(x.PurchaseRequestId.Value)?.RequestNumber ?? $"#{x.PurchaseRequestId}"
                : null,
            Path = x.Path,
            Status = x.Status,
            WarehouseName = warehouseNames.GetValueOrDefault(x.WarehouseId, $"#{x.WarehouseId}"),
            RequestedByDate = x.RequestedByDate,
            SupplierResponseDeadline = x.SupplierResponseDeadline,
            BuyerReference = x.BuyerReference,
            EstimatedAmountCompanyCurrency = x.EstimatedAmountCompanyCurrency,
            CompanyCurrencyCode = currency,
            Notes = x.Notes,
            RowVersion = x.RowVersion,
            Invitations = x.Invitations.Select(i => new PurchaseSourcingInvitationDto
                { SupplierId = i.SupplierId, CreatedAtUtc = i.CreatedAtUtc }).ToList()
        }).ToList();
    }

    public async Task<PurchaseSourcingCreatePageDto> GetCreatePageAsync(
        int purchaseRequestId, CreatePurchaseSourcingDto? form = null)
    {
        var request = await GetRequestDtoAsync(purchaseRequestId);
        var initial = form ?? new CreatePurchaseSourcingDto { PurchaseRequestId = purchaseRequestId };
        if (form is null && request is not null && initial.SupplierResponseDeadline > request.NeededByDate)
            initial.SupplierResponseDeadline = request.NeededByDate;
        return new PurchaseSourcingCreatePageDto
        {
            Form = initial,
            Request = request,
            Suppliers = (await suppliers.GetAllAsync()).Where(x => x.IsActive).OrderBy(x => x.Name)
                .Select(x => new PurchaseRequestOptionDto(x.Id, x.Name)).ToList(),
            CompanyCurrencyCode = (await generalSettings.GetAsync()).Currency
        };
    }

    public async Task<DirectPurchaseSourcingCreatePageDto> GetDirectCreatePageAsync(
        CreateDirectPurchaseSourcingDto? form = null)
    {
        var policy = await GetDirectPolicyDtoAsync();
        return new DirectPurchaseSourcingCreatePageDto
        {
            Form = form ?? new(),
            Policy = policy,
            CompanyCurrencyCode = policy.CompanyCurrencyCode,
            Warehouses = (await warehouses.GetAllAsync()).OrderBy(x => x.Name)
                .Select(x => new PurchaseRequestOptionDto(x.Id, x.Name)).ToList(),
            Products = (await products.GetAllAsync(null))
                .Where(x => x.IsActive && x.ProductType != ProductType.PreparedToOrder)
                .OrderBy(x => x.Name).Select(x => new PurchaseRequestOptionDto(x.Id, $"{x.ProductCode} — {x.Name}"))
                .ToList(),
            Units = (await units.GetActiveAsync()).OrderBy(x => x.Dimension).ThenBy(x => x.Name)
                .Select(x => new PurchaseRequestOptionDto(x.Id, $"{x.Name} ({x.Symbol}) — {x.Dimension}"))
                .ToList(),
            Suppliers = (await suppliers.GetAllAsync()).Where(x => !policy.RequireActiveSuppliers || x.IsActive)
                .OrderBy(x => x.Name).Select(x => new PurchaseRequestOptionDto(x.Id, x.Name)).ToList()
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
        var selectedSuppliers = await ResolveSuppliersAsync(dto.SupplierIds, requireActive: true);

        PurchaseSourcingEvent? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = new PurchaseSourcingEvent(
                await numbers.GenerateAsync(DocumentNumberType.PurchaseSourcingEvent, DateTime.Today),
                request.Id, request.WarehouseId, request.NeededByDate, dto.Notes, currentUser.UserId);
            created.SetCommercialContext(dto.SupplierResponseDeadline, dto.BuyerReference,
                dto.EstimatedAmountCompanyCurrency);
            foreach (var requestLine in request.Lines)
            {
                var product = await products.GetByIdAsync(requestLine.ProductId)
                    ?? throw new InvalidOperationException("Purchase request product was not found.");
                var unit = await units.GetByIdAsync(requestLine.MeasurementUnitId)
                    ?? throw new InvalidOperationException("Purchase request measurement unit was not found.");
                created.AddLine(new PurchaseSourcingLine(requestLine.Id, requestLine.ProductId,
                    requestLine.MeasurementUnitId, requestLine.RequestedQuantity, requestLine.UnitFactorToBase,
                    requestLine.StockQuantity, product.ProductCode, product.Name, unit.Symbol, requestLine.Notes));
            }
            AddInvitationsAndAttachments(created, selectedSuppliers.Keys, dto.InvitationMessage, dto.Attachments);
            created.Send(currentUser.UserId);
            request.StartSourcing(currentUser.UserId);
            await sourcing.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<int> CreateAndSendDirectAsync(CreateDirectPurchaseSourcingDto dto)
    {
        if (!await authorization.HasPermissionAsync(RequireTenant(), currentUser.UserId,
                "PurchaseSourcing.CreateDirect"))
            throw new UnauthorizedAccessException("Direct RFQ permission is required.");
        var policy = await policies.GetAsync()
            ?? throw new InvalidOperationException("Configure the direct RFQ policy before using the fast path.");
        if (!policy.AllowDirectRfq)
            throw new InvalidOperationException("Direct RFQ creation is disabled by company policy. Create a purchase request instead.");
        if (policy.MaximumEstimatedAmountCompanyCurrency <= 0 ||
            dto.EstimatedAmountCompanyCurrency > policy.MaximumEstimatedAmountCompanyCurrency)
            throw new InvalidOperationException("The estimated RFQ amount exceeds the direct path limit. Create a purchase request for approval.");
        if (!string.IsNullOrWhiteSpace(policy.AllowedRoleName) &&
            !await authorization.HasRoleAsync(RequireTenant(), currentUser.UserId, policy.AllowedRoleName))
            throw new UnauthorizedAccessException("The direct RFQ policy requires a different user role.");
        if (await warehouses.GetByIdAsync(dto.WarehouseId) is null)
            throw new ArgumentException("Destination warehouse was not found.");
        if (dto.Lines.Count == 0) throw new ArgumentException("An RFQ must contain at least one line.");
        var selectedSuppliers = await ResolveSuppliersAsync(dto.SupplierIds, policy.RequireActiveSuppliers);

        PurchaseSourcingEvent? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = new PurchaseSourcingEvent(
                await numbers.GenerateAsync(DocumentNumberType.PurchaseSourcingEvent, DateTime.Today),
                dto.WarehouseId, dto.ExpectedDeliveryDate, dto.SupplierResponseDeadline, dto.BuyerReference,
                dto.EstimatedAmountCompanyCurrency, policy.MaximumEstimatedAmountCompanyCurrency,
                policy.AllowedRoleName, dto.Notes, currentUser.UserId);
            foreach (var line in dto.Lines)
            {
                var product = await products.GetByIdAsync(line.ProductId)
                    ?? throw new ArgumentException("Product was not found.");
                if (!product.IsActive || product.ProductType == ProductType.PreparedToOrder)
                    throw new InvalidOperationException("Direct RFQs require an active purchasable product.");
                var unit = await units.GetByIdAsync(line.MeasurementUnitId)
                    ?? throw new ArgumentException("Measurement unit was not found.");
                if (!unit.IsActive || !product.MeasurementUnitId.HasValue)
                    throw new InvalidOperationException("Product and requested unit must use managed active units.");
                var stockUnit = await units.GetByIdAsync(product.MeasurementUnitId.Value)
                    ?? throw new InvalidOperationException("Product stock unit was not found.");
                if (stockUnit.Dimension != unit.Dimension)
                    throw new InvalidOperationException("Requested unit and product stock unit must use the same measurement dimension.");
                var factor = unit.FactorToBaseUnit / stockUnit.FactorToBaseUnit;
                created.AddLine(new PurchaseSourcingLine(product.Id, unit.Id, line.RequestedQuantity,
                    factor, line.RequestedQuantity * factor, product.ProductCode, product.Name, unit.Symbol, line.Notes));
            }
            AddInvitationsAndAttachments(created, selectedSuppliers.Keys, dto.InvitationMessage, dto.Attachments);
            created.Send(currentUser.UserId);
            await sourcing.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task<PurchaseSourcingDto?> GetAsync(int id)
    {
        var source = await sourcing.GetByIdAsync(id);
        if (source is null) return null;
        var request = source.PurchaseRequestId.HasValue
            ? await requests.GetByIdAsync(source.PurchaseRequestId.Value) : null;
        var warehouse = await warehouses.GetByIdAsync(source.WarehouseId);
        var supplierNames = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return new PurchaseSourcingDto
        {
            Id = source.Id,
            SourcingNumber = source.SourcingNumber,
            PurchaseRequestId = source.PurchaseRequestId,
            PurchaseRequestNumber = request?.RequestNumber,
            Path = source.Path,
            Status = source.Status,
            WarehouseName = warehouse?.Name ?? $"#{source.WarehouseId}",
            RequestedByDate = source.RequestedByDate,
            SupplierResponseDeadline = source.SupplierResponseDeadline,
            BuyerReference = source.BuyerReference,
            EstimatedAmountCompanyCurrency = source.EstimatedAmountCompanyCurrency,
            DirectPolicyMaximumAmountSnapshot = source.DirectPolicyMaximumAmountSnapshot,
            DirectPolicyRoleSnapshot = source.DirectPolicyRoleSnapshot,
            CompanyCurrencyCode = (await generalSettings.GetAsync()).Currency,
            Notes = source.Notes,
            RowVersion = source.RowVersion,
            Lines = source.Lines.Select(x => new PurchaseSourcingLineDto
            {
                Id = x.Id, ProductCode = x.ProductCodeSnapshot, ProductName = x.ProductNameSnapshot,
                UnitName = x.UnitNameSnapshot, RequestedQuantity = x.RequestedQuantity,
                StockQuantity = x.StockQuantity, Notes = x.Notes
            }).ToList(),
            Invitations = source.Invitations.Select(x => new PurchaseSourcingInvitationDto
            {
                SupplierId = x.SupplierId,
                SupplierName = supplierNames.GetValueOrDefault(x.SupplierId, $"#{x.SupplierId}"),
                Message = x.Message,
                CreatedAtUtc = x.CreatedAtUtc
            }).ToList(),
            Attachments = source.Attachments.Select(x => new PurchaseSourcingAttachmentDto
                { Id = x.Id, FileName = x.FileName, ContentType = x.ContentType, Size = x.Size }).ToList()
        };
    }

    public Task<PurchaseSourcingAttachment?> GetAttachmentAsync(int sourcingEventId, int attachmentId) =>
        sourcing.GetAttachmentAsync(sourcingEventId, attachmentId);

    private async Task<Dictionary<int, Supplier>> ResolveSuppliersAsync(IEnumerable<int> ids, bool requireActive)
    {
        var supplierIds = ids.Where(x => x > 0).Distinct().ToList();
        if (supplierIds.Count == 0) throw new ArgumentException("At least one supplier must be selected.");
        var selected = new Dictionary<int, Supplier>();
        foreach (var supplierId in supplierIds)
        {
            var supplier = await suppliers.GetByIdAsync(supplierId)
                ?? throw new ArgumentException("One or more suppliers were not found.");
            if (requireActive && !supplier.IsActive)
                throw new InvalidOperationException("Direct RFQs can invite active suppliers only.");
            selected[supplierId] = supplier;
        }
        return selected;
    }

    private void AddInvitationsAndAttachments(PurchaseSourcingEvent created, IEnumerable<int> supplierIds,
        string? invitationMessage, IEnumerable<PurchaseSourcingAttachmentInputDto> attachments)
    {
        foreach (var supplierId in supplierIds) created.InviteSupplier(supplierId, invitationMessage);
        foreach (var file in attachments)
            created.AddAttachment(new PurchaseSourcingAttachment(file.FileName, file.ContentType,
                file.Content, currentUser.UserId));
    }

    private async Task<PurchaseSourcingPolicyDto> GetDirectPolicyDtoAsync()
    {
        var settings = await generalSettings.GetAsync();
        var policy = await policies.GetAsync();
        return new PurchaseSourcingPolicyDto
        {
            AllowDirectRfq = policy?.AllowDirectRfq ?? false,
            MaximumEstimatedAmountCompanyCurrency = policy?.MaximumEstimatedAmountCompanyCurrency ?? 0,
            AllowedRoleName = policy?.AllowedRoleName,
            RequireActiveSuppliers = policy?.RequireActiveSuppliers ?? true,
            CompanyCurrencyCode = settings.Currency
        };
    }

    private async Task<PurchaseRequestDto?> GetRequestDtoAsync(int id)
    {
        var request = await requests.GetByIdAsync(id);
        if (request is null) return null;
        var productsById = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var unitsById = (await units.GetAllAsync()).ToDictionary(x => x.Id);
        return new PurchaseRequestDto
        {
            Id = request.Id, RequestNumber = request.RequestNumber, WarehouseId = request.WarehouseId,
            NeededByDate = request.NeededByDate, Priority = request.Priority, Justification = request.Justification,
            Notes = request.Notes, Status = request.Status,
            Lines = request.Lines.Select(x => new PurchaseRequestLineDto
            {
                ProductId = x.ProductId,
                ProductCode = productsById.GetValueOrDefault(x.ProductId)?.ProductCode ?? string.Empty,
                ProductName = productsById.GetValueOrDefault(x.ProductId)?.Name ?? $"#{x.ProductId}",
                MeasurementUnitId = x.MeasurementUnitId,
                UnitName = unitsById.GetValueOrDefault(x.MeasurementUnitId)?.Symbol ?? string.Empty,
                RequestedQuantity = x.RequestedQuantity, StockQuantity = x.StockQuantity, Notes = x.Notes
            }).ToList()
        };
    }

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");
}
