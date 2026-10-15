using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SupplierIntegrationService(
    ISupplierIntegrationRepository integrations, ISupplierRepository suppliers, IPurchaseSourcingRepository sourcing,
    ISupplierQuotationRepository quotations, IPurchaseOrderRepository orders, DocumentNumberService numbers,
    CurrencyExchangeRateService exchangeRates, SupplierUblService ubl, ISupplierIntegrationSecretProtector secretProtector,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser, ITenantScope tenantScope,
    SemanticAuditEventService audit)
{
    public static readonly string[] AllowedScopes=["rfq.read","quotation.write","po.read","confirmation.write","delivery.write","invoice.write"];

    public async Task<SupplierIntegrationDashboardDto> GetDashboardAsync()
    {
        var supplierRows=await suppliers.GetAllAsync();var names=supplierRows.ToDictionary(x=>x.Id,x=>x.Name);
        var credentials=await integrations.GetCredentialsAsync();
        return new SupplierIntegrationDashboardDto
        {
            Suppliers=supplierRows.Where(x=>x.IsActive).Select(x=>new PurchaseRequestOptionDto(x.Id,x.Name)).ToList(),
            Credentials=credentials.Select(x=>new SupplierIntegrationCredentialDto{Id=x.Id,SupplierId=x.SupplierId,SupplierName=names.GetValueOrDefault(x.SupplierId,$"#{x.SupplierId}"),Name=x.Name,KeyPrefix=x.KeyPrefix,Scopes=x.Scopes,CallbackUrl=x.CallbackUrl,IsActive=x.IsActive,ExpiresAtUtc=x.ExpiresAtUtc,LastUsedAtUtc=x.LastUsedAtUtc,RowVersion=x.RowVersion}).ToList(),
            Outbox=(await integrations.GetOutboxAsync()).Select(x=>new SupplierOutboxDto{Id=x.Id,SupplierName=names.GetValueOrDefault(credentials.FirstOrDefault(c=>c.Id==x.SupplierIntegrationCredentialId)?.SupplierId??0,"—"),MessageType=x.MessageType,AggregateType=x.AggregateType,AggregateId=x.AggregateId,Status=x.Status,AttemptCount=x.AttemptCount,NextAttemptAtUtc=x.NextAttemptAtUtc,DeliveredAtUtc=x.DeliveredAtUtc,LastError=x.LastError}).ToList(),
            Submissions=(await integrations.GetRecentSubmissionsAsync()).Select(x=>new SupplierExternalSubmissionDto{Id=x.Id,SupplierName=names.GetValueOrDefault(x.SupplierId,$"#{x.SupplierId}"),Channel=x.Channel,Type=x.SubmissionType,DocumentReference=$"{x.DocumentType} #{x.DocumentId}",ExternalReference=x.ExternalReference,ResultType=x.ResultType,ResultId=x.ResultId,CreatedAtUtc=x.CreatedAtUtc}).ToList(),
            AuditEvents=(await integrations.GetRecentAuditsAsync()).Select(x=>new SupplierExternalAuditDto{SupplierName=x.SupplierId.HasValue?names.GetValueOrDefault(x.SupplierId.Value,$"#{x.SupplierId}"):"—",Channel=x.Channel,Actor=x.Actor,Action=x.Action,Outcome=x.Outcome,CorrelationId=x.CorrelationId,RemoteAddress=x.RemoteAddress,CreatedAtUtc=x.CreatedAtUtc}).ToList()
        };
    }

    public async Task<CreatedSupplierIntegrationCredentialDto> CreateCredentialAsync(CreateSupplierIntegrationCredentialDto dto)
    {
        var supplier=await suppliers.GetByIdAsync(dto.SupplierId)??throw new ArgumentException("Supplier was not found.");if(!supplier.IsActive)throw new InvalidOperationException("Supplier must be active before integration access is created.");
        var scopes=dto.Scopes.Distinct(StringComparer.Ordinal).ToList();if(scopes.Count==0||scopes.Any(x=>!AllowedScopes.Contains(x,StringComparer.Ordinal)))throw new ArgumentException("Select only supported supplier integration permissions.");
        string? callback=null;if(!string.IsNullOrWhiteSpace(dto.CallbackUrl)){var uri=ValidateCallback(dto.CallbackUrl);callback=uri.AbsoluteUri;}
        var raw="msk_"+Base64Url(RandomNumberGenerator.GetBytes(32));var prefix=raw[..12];
        var value=new SupplierIntegrationCredential(supplier.Id,dto.Name,prefix,Hash(raw),string.Join(' ',scopes.Order()),callback,string.IsNullOrWhiteSpace(dto.CallbackBearerToken)?null:secretProtector.Protect(dto.CallbackBearerToken.Trim()),dto.ExpiresAtUtc?.ToUniversalTime(),currentUser.UserId);
        await unitOfWork.ExecuteInTransactionAsync(async()=>
        {
            await integrations.AddCredentialAsync(value);
            await audit.RecordTenantSuccessAsync(AuditEventCategory.Integration,
                SemanticAuditEventTypes.SupplierCredentialCreated, "Create", "SupplierIntegrationCredential",
                supplier.Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        });return new(value.Id,raw);
    }

    public async Task DeactivateCredentialAsync(int id,byte[] rowVersion)
    {
        var value=await integrations.GetCredentialAsync(id)??throw new InvalidOperationException("Supplier integration credential was not found.");if(!value.RowVersion.SequenceEqual(rowVersion))throw new InvalidOperationException("Supplier integration credential was modified by another user. Reload and try again.");
        await unitOfWork.ExecuteInTransactionAsync(async()=>
        {
            value.Deactivate();
            await audit.RecordTenantSuccessAsync(AuditEventCategory.Integration,
                SemanticAuditEventTypes.SupplierCredentialDeactivated, "Deactivate", "SupplierIntegrationCredential",
                id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        });
    }

    public async Task<CreatedSupplierPortalAccessDto> CreatePortalAccessAsync(CreateSupplierPortalAccessDto dto)
    {
        if(dto.ValidDays is<1 or>90)throw new ArgumentException("Portal access validity must be between 1 and 90 days.");
        var supplier=await suppliers.GetByIdAsync(dto.SupplierId)??throw new ArgumentException("Supplier was not found.");if(!supplier.IsActive)throw new InvalidOperationException("Supplier must be active before portal access is created.");
        string number;
        if(dto.DocumentType==SupplierExternalDocumentType.Rfq){var source=await sourcing.GetByIdAsync(dto.DocumentId)??throw new InvalidOperationException("Sourcing event was not found.");if(!source.Invitations.Any(x=>x.SupplierId==supplier.Id))throw new InvalidOperationException("Supplier is not invited to this sourcing event.");number=source.SourcingNumber;}
        else if(dto.DocumentType==SupplierExternalDocumentType.PurchaseOrder){var order=await orders.GetByIdAsync(dto.DocumentId)??throw new InvalidOperationException("Purchase order was not found.");if(order.SupplierId!=supplier.Id)throw new InvalidOperationException("Supplier does not belong to this purchase order.");number=order.OrderNumber;}
        else throw new ArgumentException("Portal access supports RFQs and purchase orders only.");
        var raw="msp_"+Base64Url(RandomNumberGenerator.GetBytes(32));var expires=DateTime.UtcNow.AddDays(dto.ValidDays);var access=new SupplierPortalAccess(supplier.Id,dto.DocumentType,dto.DocumentId,number,Hash(raw),expires,currentUser.UserId);
        await unitOfWork.ExecuteInTransactionAsync(async()=>
        {
            await integrations.AddPortalAccessAsync(access);
            await audit.RecordTenantSuccessAsync(AuditEventCategory.Integration,
                SemanticAuditEventTypes.SupplierPortalAccessCreated, "Create", "SupplierPortalAccess",
                $"{dto.DocumentType}:{dto.DocumentId}");
        });return new(access.Id,raw,expires);
    }

    public Task<List<SupplierPortalAccessDto>> GetPortalAccessesAsync(SupplierExternalDocumentType type,int documentId)=>MapAccesses(type,documentId);
    private async Task<List<SupplierPortalAccessDto>> MapAccesses(SupplierExternalDocumentType type,int documentId)=>(await integrations.GetPortalAccessesAsync(type,documentId)).Select(x=>new SupplierPortalAccessDto{Id=x.Id,SupplierId=x.SupplierId,DocumentType=x.DocumentType,DocumentId=x.DocumentId,DocumentNumber=x.DocumentNumberSnapshot,ExpiresAtUtc=x.ExpiresAtUtc,IsRevoked=x.IsRevoked,LastAccessedAtUtc=x.LastAccessedAtUtc}).ToList();
    public async Task<List<SupplierIntegrationCredentialDto>> GetCredentialsForSupplierAsync(int supplierId,int? purchaseOrderId=null)
    {
        var names=(await suppliers.GetAllAsync()).ToDictionary(x=>x.Id,x=>x.Name);
        HashSet<int> queuedCredentialIds=purchaseOrderId.HasValue
            ?(await integrations.GetOutboxAsync(1000)).Where(x=>x.AggregateType=="PurchaseOrder"&&x.AggregateId==purchaseOrderId.Value).Select(x=>x.SupplierIntegrationCredentialId).ToHashSet()
            :[];
        return (await integrations.GetCredentialsAsync()).Where(x=>x.SupplierId==supplierId&&x.IsUsable(DateTime.UtcNow)).Select(x=>new SupplierIntegrationCredentialDto{Id=x.Id,SupplierId=x.SupplierId,SupplierName=names.GetValueOrDefault(x.SupplierId,$"#{x.SupplierId}"),Name=x.Name,KeyPrefix=x.KeyPrefix,Scopes=x.Scopes,CallbackUrl=x.CallbackUrl,IsActive=x.IsActive,ExpiresAtUtc=x.ExpiresAtUtc,LastUsedAtUtc=x.LastUsedAtUtc,CanQueuePurchaseOrder=!queuedCredentialIds.Contains(x.Id),RowVersion=x.RowVersion}).ToList();
    }
    public async Task RevokePortalAccessAsync(int id)
    {
        var access=await integrations.GetPortalAccessAsync(id)??throw new InvalidOperationException("Supplier portal access was not found.");
        await unitOfWork.ExecuteInTransactionAsync(async()=>
        {
            access.Revoke();
            await audit.RecordTenantSuccessAsync(AuditEventCategory.Integration,
                SemanticAuditEventTypes.SupplierPortalAccessRevoked, "Revoke", "SupplierPortalAccess",
                id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        });
    }

    public async Task QueuePurchaseOrderAsync(QueueSupplierPurchaseOrderDto dto)
    {
        var credential=await integrations.GetCredentialAsync(dto.CredentialId)??throw new InvalidOperationException("Supplier integration credential was not found.");if(!credential.IsUsable(DateTime.UtcNow))throw new InvalidOperationException("Supplier integration credential is inactive or expired.");if(string.IsNullOrWhiteSpace(credential.CallbackUrl))throw new InvalidOperationException("Configure an HTTPS callback URL before queueing external delivery.");
        var order=await orders.GetByIdAsync(dto.PurchaseOrderId)??throw new InvalidOperationException("Purchase order was not found.");if(order.SupplierId!=credential.SupplierId)throw new InvalidOperationException("Integration credential supplier does not match the purchase order supplier.");if(order.Status is not(PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed))throw new InvalidOperationException("Only approved or confirmed purchase orders can be queued.");
        var supplier=await suppliers.GetByIdAsync(order.SupplierId)??throw new InvalidOperationException("Supplier was not found.");var payload=ubl.CreatePurchaseOrder(order,supplier.Name);var key=$"po:{order.Id}:ubl:{credential.Id}";if(await integrations.FindOutboxByKeyAsync(key) is not null)throw new InvalidOperationException("This purchase order is already queued for this integration destination.");
        var message=new SupplierIntegrationOutbox(credential.Id,"ubl.purchase-order","PurchaseOrder",order.Id,credential.CallbackUrl,credential.ProtectedCallbackSecret,payload,key,currentUser.UserId);
        await unitOfWork.ExecuteInTransactionAsync(()=>integrations.AddOutboxAsync(message));
    }

    public async Task RetryOutboxAsync(int id){var value=await integrations.GetOutboxMessageAsync(id)??throw new InvalidOperationException("Outbox message was not found.");await unitOfWork.ExecuteInTransactionAsync(()=>{value.RetryNow();return Task.CompletedTask;});}
    public async Task<SupplierExternalSubmissionDetailsDto?> GetSubmissionAsync(int id){var value=await integrations.GetSubmissionAsync(id);if(value is null)return null;var supplier=await suppliers.GetByIdAsync(value.SupplierId);return new(){Id=value.Id,SupplierName=supplier?.Name??$"#{value.SupplierId}",Channel=value.Channel,Type=value.SubmissionType,DocumentType=value.DocumentType,DocumentId=value.DocumentId,IdempotencyKey=value.IdempotencyKey,RequestHash=value.RequestHash,Payload=value.Payload,ExternalReference=value.ExternalReference,Actor=value.CreatedByActor,CreatedAtUtc=value.CreatedAtUtc};}
    public async Task<string> ExportPurchaseOrderUblAsync(int id){var order=await orders.GetByIdAsync(id)??throw new InvalidOperationException("Purchase order was not found.");var supplier=await suppliers.GetByIdAsync(order.SupplierId)??throw new InvalidOperationException("Supplier was not found.");return ubl.CreatePurchaseOrder(order,supplier.Name);}

    public async Task<SupplierPortalPageDto?> GetPortalPageAsync(string token,string? remoteAddress,string correlationId)
    {
        var resolved=await integrations.ResolvePortalHashAsync(Hash(token));if(resolved is null)return null;using var scope=tenantScope.BeginScope(resolved.Value.TenantId);var access=resolved.Value.Access;if(!access.IsUsable(DateTime.UtcNow))return null;
        var page=await BuildPortalPage(access,token);if(page is null)return null;await unitOfWork.ExecuteInTransactionAsync(async()=>{access.RecordAccess();await integrations.AddAuditAsync(new SupplierExternalAuditEvent(access.SupplierId,SupplierExternalChannel.Portal,access.Id,$"portal:{access.Id}","document.view","success",correlationId,remoteAddress));});return page;
    }

    public async Task<SupplierExternalResultDto> SubmitPortalQuotationAsync(SupplierPortalQuotationDto dto,string? remoteAddress,string correlationId)
    {
        var resolved=await ResolvePortal(dto.Token,SupplierExternalDocumentType.Rfq);using var scope=tenantScope.BeginScope(resolved.TenantId);var access=resolved.Access;
        var payload=JsonSerializer.Serialize(new{dto.CurrencyCode,dto.SupplierReference,dto.QuotationDate,dto.ValidUntilDate,dto.LeadTimeDays,dto.PaymentTerms,dto.Notes,dto.Lines});
        return await SubmitQuotation(access,SupplierExternalChannel.Portal,access.Id,dto.IdempotencyKey,payload,dto.CurrencyCode,dto.SupplierReference,dto.QuotationDate,dto.ValidUntilDate,dto.LeadTimeDays,dto.PaymentTerms,dto.Notes,dto.Lines,$"portal:{access.Id}",remoteAddress,correlationId);
    }

    public async Task<SupplierExternalResultDto> ConfirmPortalOrderAsync(SupplierExternalOrderConfirmationDto dto,string? remoteAddress,string correlationId)
    {
        var resolved=await ResolvePortal(dto.Token,SupplierExternalDocumentType.PurchaseOrder);using var scope=tenantScope.BeginScope(resolved.TenantId);var access=resolved.Access;var payload=JsonSerializer.Serialize(new{dto.ConfirmedExpectedDate,dto.ConfirmationReference,dto.Note});
        return await ConfirmOrder(access.SupplierId,access.DocumentId,SupplierExternalChannel.Portal,access.Id,dto.IdempotencyKey,payload,dto.ConfirmedExpectedDate,dto.ConfirmationReference,dto.Note,$"portal:{access.Id}",remoteAddress,correlationId);
    }

    public async Task<SupplierExternalResultDto> SubmitPortalDeliveryAsync(SupplierExternalDeliveryNoticeDto dto,string? remoteAddress,string correlationId)
    {
        var resolved=await ResolvePortal(dto.Token,SupplierExternalDocumentType.PurchaseOrder);using var scope=tenantScope.BeginScope(resolved.TenantId);var access=resolved.Access;var payload=JsonSerializer.Serialize(new{dto.DeliveryDate,dto.Reference,dto.Carrier,dto.TrackingNumber,dto.Note});
        return await SaveDeliveryNotice(access.SupplierId,access.DocumentId,SupplierExternalChannel.Portal,access.Id,dto.IdempotencyKey,payload,dto.Reference,$"portal:{access.Id}",remoteAddress,correlationId);
    }

    public async Task<SupplierPortalPageDto> GetApiRfqAsync(string apiKey,int id,string? remoteAddress,string correlationId)=>await GetApiDocument(apiKey,"rfq.read",SupplierExternalDocumentType.Rfq,id,remoteAddress,correlationId);
    public async Task<SupplierPortalPageDto> GetApiOrderAsync(string apiKey,int id,string? remoteAddress,string correlationId)=>await GetApiDocument(apiKey,"po.read",SupplierExternalDocumentType.PurchaseOrder,id,remoteAddress,correlationId);
    public async Task<string> GetApiOrderUblAsync(string apiKey,int id,string? remoteAddress,string correlationId){var resolved=await ResolveApi(apiKey,"po.read");using var scope=tenantScope.BeginScope(resolved.TenantId);var order=await RequireOrder(resolved.Credential.SupplierId,id);var supplier=await suppliers.GetByIdAsync(order.SupplierId)??throw new InvalidOperationException("Supplier was not found.");await Audit(resolved.Credential,"purchase-order.ubl.read","success",correlationId,remoteAddress);return ubl.CreatePurchaseOrder(order,supplier.Name);}
    public async Task<SupplierExternalResultDto> SubmitApiQuotationAsync(string apiKey,string idempotencyKey,SupplierApiQuotationRequest dto,string? remoteAddress,string correlationId){var resolved=await ResolveApi(apiKey,"quotation.write");using var scope=tenantScope.BeginScope(resolved.TenantId);var payload=JsonSerializer.Serialize(dto);return await SubmitQuotation(null,SupplierExternalChannel.Api,resolved.Credential.Id,idempotencyKey,payload,dto.CurrencyCode,dto.SupplierReference,dto.QuotationDate,dto.ValidUntilDate,dto.LeadTimeDays,dto.PaymentTerms,dto.Notes,dto.Lines,$"api:{resolved.Credential.Id}",remoteAddress,correlationId,resolved.Credential,dto.PurchaseSourcingEventId);}
    public async Task<SupplierExternalResultDto> ConfirmApiOrderAsync(string apiKey,string idempotencyKey,int orderId,SupplierApiOrderConfirmationRequest dto,string? remoteAddress,string correlationId){var resolved=await ResolveApi(apiKey,"confirmation.write");using var scope=tenantScope.BeginScope(resolved.TenantId);var payload=JsonSerializer.Serialize(dto);return await ConfirmOrder(resolved.Credential.SupplierId,orderId,SupplierExternalChannel.Api,resolved.Credential.Id,idempotencyKey,payload,dto.ConfirmedExpectedDate,dto.ConfirmationReference,dto.Note,$"api:{resolved.Credential.Id}",remoteAddress,correlationId,resolved.Credential);}
    public async Task<SupplierExternalResultDto> ConfirmApiOrderUblAsync(string apiKey,string idempotencyKey,int orderId,string xml,string? remoteAddress,string correlationId){var resolved=await ResolveApi(apiKey,"confirmation.write");using var scope=tenantScope.BeginScope(resolved.TenantId);var order=await RequireOrder(resolved.Credential.SupplierId,orderId);var parsed=ubl.ParseOrderConfirmation(xml);if(!string.IsNullOrWhiteSpace(parsed.PurchaseOrderReference)&&!string.Equals(parsed.PurchaseOrderReference,order.OrderNumber,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("UBL order confirmation reference does not match the requested order.");return await ConfirmOrder(resolved.Credential.SupplierId,orderId,SupplierExternalChannel.Api,resolved.Credential.Id,idempotencyKey,xml,parsed.ConfirmedDeliveryDate,parsed.ConfirmationReference,parsed.Note,$"api:{resolved.Credential.Id}",remoteAddress,correlationId,resolved.Credential);}
    public async Task<SupplierExternalResultDto> SubmitApiDeliveryAsync(string apiKey,string idempotencyKey,int orderId,SupplierApiDeliveryNoticeRequest dto,string? remoteAddress,string correlationId){var resolved=await ResolveApi(apiKey,"delivery.write");using var scope=tenantScope.BeginScope(resolved.TenantId);var payload=JsonSerializer.Serialize(dto);return await SaveDeliveryNotice(resolved.Credential.SupplierId,orderId,SupplierExternalChannel.Api,resolved.Credential.Id,idempotencyKey,payload,dto.Reference,$"api:{resolved.Credential.Id}",remoteAddress,correlationId,resolved.Credential);}
    public async Task<SupplierExternalResultDto> SubmitApiInvoiceUblAsync(string apiKey,string idempotencyKey,int orderId,string xml,string? remoteAddress,string correlationId)
    {
        var resolved=await ResolveApi(apiKey,"invoice.write");using var scope=tenantScope.BeginScope(resolved.TenantId);var order=await RequireOrder(resolved.Credential.SupplierId,orderId);var parsed=ubl.ParseInvoice(xml);if(!string.IsNullOrWhiteSpace(parsed.PurchaseOrderReference)&&!string.Equals(parsed.PurchaseOrderReference,order.OrderNumber,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("UBL invoice purchase order reference does not match the requested order.");if(!string.Equals(parsed.CurrencyCode,order.CurrencyCode,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("UBL invoice currency does not match the purchase order currency.");
        return await SaveSubmission(resolved.Credential.SupplierId,SupplierExternalChannel.Api,resolved.Credential.Id,SupplierExternalSubmissionType.InvoiceUbl,SupplierExternalDocumentType.Invoice,orderId,idempotencyKey,xml,parsed.InvoiceNumber,null,null,$"api:{resolved.Credential.Id}",resolved.Credential,"invoice.ubl.received",remoteAddress,correlationId);
    }

    private async Task<SupplierExternalResultDto> SubmitQuotation(SupplierPortalAccess? access,SupplierExternalChannel channel,int identityId,string key,string payload,string currency,string? reference,DateOnly quotationDate,DateOnly validUntil,int leadDays,string? paymentTerms,string? notes,List<SupplierPortalQuotationLineDto> lines,string actor,string? remote,string correlation,SupplierIntegrationCredential? credential=null,int? sourceId=null)
    {
        var supplierId=access?.SupplierId??credential!.SupplierId;var documentId=access?.DocumentId??sourceId!.Value;var prior=await Replay(channel,identityId,key,payload);if(prior is not null)return prior;
        var source=await sourcing.GetByIdAsync(documentId)??throw new InvalidOperationException("Sourcing event was not found.");if(source.Status!=PurchaseSourcingStatus.Sent||!source.Invitations.Any(x=>x.SupplierId==supplierId))throw new InvalidOperationException("This RFQ is not open for this supplier.");if(await quotations.GetBySourcingAndSupplierAsync(source.Id,supplierId) is not null)throw new InvalidOperationException("This supplier already has a quotation for this sourcing event.");
        if(lines.Count!=source.Lines.Count||lines.Select(x=>x.DocumentLineId).Distinct().Count()!=lines.Count||lines.Any(x=>!source.Lines.Any(s=>s.Id==x.DocumentLineId)))throw new ArgumentException("Quotation lines must match every sourcing line exactly once.");var resolvedCurrency=(await exchangeRates.ResolveAsync(currency,quotationDate)).CurrencyCode;
        SupplierQuotation? quote=null;SupplierExternalSubmission? submission=null;
        await unitOfWork.ExecuteInTransactionAsync(async()=>{quote=new SupplierQuotation(await numbers.GenerateAsync(DocumentNumberType.SupplierQuotation,DateTime.Today),source.Id,supplierId,reference,quotationDate,validUntil,leadDays,resolvedCurrency,paymentTerms,notes,actor);foreach(var input in lines){var line=source.Lines.Single(x=>x.Id==input.DocumentLineId);quote.AddLine(new SupplierQuotationLine(line.Id,input.Quantity,input.UnitPrice,input.DiscountPercent,input.TaxPercent,line.ProductCodeSnapshot,line.ProductNameSnapshot,line.UnitNameSnapshot));}quote.Submit();await quotations.AddAsync(quote);await unitOfWork.FlushAsync();submission=new SupplierExternalSubmission(supplierId,channel,identityId,SupplierExternalSubmissionType.Quotation,SupplierExternalDocumentType.Rfq,source.Id,key,Hash(payload),payload,reference,"SupplierQuotation",quote.Id,actor);await integrations.AddSubmissionAsync(submission);if(credential is not null)credential.RecordUse();await integrations.AddAuditAsync(new SupplierExternalAuditEvent(supplierId,channel,identityId,actor,"quotation.submit","success",correlation,remote));});return new(false,submission!.Id,"SupplierQuotation",quote!.Id);
    }

    private async Task<SupplierExternalResultDto> ConfirmOrder(int supplierId,int orderId,SupplierExternalChannel channel,int identityId,string key,string payload,DateOnly date,string reference,string? note,string actor,string? remote,string correlation,SupplierIntegrationCredential? credential=null)
    {
        var prior=await Replay(channel,identityId,key,payload);if(prior is not null)return prior;var order=await RequireOrder(supplierId,orderId);SupplierExternalSubmission? submission=null;
        await unitOfWork.ExecuteInTransactionAsync(async()=>{order.RecordSupplierConfirmation(date,reference,note,actor);submission=new SupplierExternalSubmission(supplierId,channel,identityId,SupplierExternalSubmissionType.OrderConfirmation,SupplierExternalDocumentType.PurchaseOrder,order.Id,key,Hash(payload),payload,reference,"PurchaseOrder",order.Id,actor);await integrations.AddSubmissionAsync(submission);if(credential is not null)credential.RecordUse();await integrations.AddAuditAsync(new SupplierExternalAuditEvent(supplierId,channel,identityId,actor,"purchase-order.confirm","success",correlation,remote));});return new(false,submission!.Id,"PurchaseOrder",order.Id);
    }

    private async Task<SupplierExternalResultDto> SaveDeliveryNotice(int supplierId,int orderId,SupplierExternalChannel channel,int identityId,string key,string payload,string reference,string actor,string? remote,string correlation,SupplierIntegrationCredential? credential=null)
    {
        var prior=await Replay(channel,identityId,key,payload);if(prior is not null)return prior;var order=await RequireOrder(supplierId,orderId);if(order.Status is not(PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed))throw new InvalidOperationException("Delivery notices require an approved or confirmed purchase order.");return await SaveSubmission(supplierId,channel,identityId,SupplierExternalSubmissionType.DeliveryNotice,SupplierExternalDocumentType.PurchaseOrder,orderId,key,payload,reference,"PurchaseOrder",orderId,actor,credential,"delivery-notice.submit",remote,correlation);
    }

    private async Task<SupplierExternalResultDto> SaveSubmission(int supplierId,SupplierExternalChannel channel,int identityId,SupplierExternalSubmissionType type,SupplierExternalDocumentType documentType,int documentId,string key,string payload,string? reference,string? resultType,int? resultId,string actor,SupplierIntegrationCredential? credential,string action,string? remote,string correlation)
    {
        var prior=await Replay(channel,identityId,key,payload);if(prior is not null)return prior;SupplierExternalSubmission? submission=null;await unitOfWork.ExecuteInTransactionAsync(async()=>{submission=new SupplierExternalSubmission(supplierId,channel,identityId,type,documentType,documentId,key,Hash(payload),payload,reference,resultType,resultId,actor);await integrations.AddSubmissionAsync(submission);if(credential is not null)credential.RecordUse();await integrations.AddAuditAsync(new SupplierExternalAuditEvent(supplierId,channel,identityId,actor,action,"success",correlation,remote));});return new(false,submission!.Id,resultType,resultId);
    }

    private async Task<SupplierExternalResultDto?> Replay(SupplierExternalChannel channel,int identityId,string key,string payload)
    {
        if(string.IsNullOrWhiteSpace(key)||key.Length>100)throw new ArgumentException("Idempotency-Key is required and cannot exceed 100 characters.");var existing=await integrations.FindSubmissionAsync(channel,identityId,key);if(existing is null)return null;if(!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(existing.RequestHash),Convert.FromHexString(Hash(payload))))throw new InvalidOperationException("Idempotency key was already used with different content.");return new(true,existing.Id,existing.ResultType,existing.ResultId);
    }

    private async Task<SupplierPortalPageDto> GetApiDocument(string key,string requiredScope,SupplierExternalDocumentType type,int id,string? remote,string correlation)
    {
        var resolved=await ResolveApi(key,requiredScope);using var scope=tenantScope.BeginScope(resolved.TenantId);SupplierPortalPageDto? page;
        if(type==SupplierExternalDocumentType.Rfq){var source=await sourcing.GetByIdAsync(id);if(source is null||source.Invitations.All(x=>x.SupplierId!=resolved.Credential.SupplierId))throw new InvalidOperationException("Supplier document was not found.");page=await MapRfq(source,resolved.Credential.SupplierId,string.Empty);}
        else{var order=await RequireOrder(resolved.Credential.SupplierId,id);page=await MapOrder(order,string.Empty);}
        await Audit(resolved.Credential,type==SupplierExternalDocumentType.Rfq?"rfq.read":"purchase-order.read","success",correlation,remote);return page;
    }

    private async Task<SupplierPortalPageDto?> BuildPortalPage(SupplierPortalAccess access,string token)
    {
        if(access.DocumentType==SupplierExternalDocumentType.Rfq){var source=await sourcing.GetByIdAsync(access.DocumentId);return source is null?null:await MapRfq(source,access.SupplierId,token);}
        if(access.DocumentType==SupplierExternalDocumentType.PurchaseOrder){var order=await orders.GetByIdAsync(access.DocumentId);return order is null||order.SupplierId!=access.SupplierId?null:await MapOrder(order,token);}return null;
    }
    private async Task<SupplierPortalPageDto> MapRfq(PurchaseSourcingEvent source,int supplierId,string token){var supplier=await suppliers.GetByIdAsync(supplierId)??throw new InvalidOperationException("Supplier was not found.");return new(){Token=token,DocumentType=SupplierExternalDocumentType.Rfq,DocumentId=source.Id,DocumentNumber=source.SourcingNumber,SupplierName=supplier.Name,CurrencyCode=(await exchangeRates.GetPageAsync()).CompanyCurrencyCode,ResponseDeadline=source.SupplierResponseDeadline,ExpectedDate=source.RequestedByDate,CanSubmitQuotation=source.Status==PurchaseSourcingStatus.Sent&&await quotations.GetBySourcingAndSupplierAsync(source.Id,supplierId) is null,Lines=source.Lines.Select(x=>new SupplierPortalLineDto{Id=x.Id,ProductCode=x.ProductCodeSnapshot,ProductName=x.ProductNameSnapshot,UnitName=x.UnitNameSnapshot,Quantity=x.RequestedQuantity}).ToList()};}
    private async Task<SupplierPortalPageDto> MapOrder(PurchaseOrder order,string token){var supplier=await suppliers.GetByIdAsync(order.SupplierId)??throw new InvalidOperationException("Supplier was not found.");return new(){Token=token,DocumentType=SupplierExternalDocumentType.PurchaseOrder,DocumentId=order.Id,DocumentNumber=order.OrderNumber,SupplierName=supplier.Name,CurrencyCode=order.CurrencyCode,ExpectedDate=order.ExpectedDate,PaymentTerms=order.PaymentTermsSnapshot,CanConfirmOrder=order.Status is PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed,CanSubmitDeliveryNotice=order.Status is PurchaseOrderStatus.Approved or PurchaseOrderStatus.Confirmed,Lines=order.Lines.Select(x=>new SupplierPortalLineDto{Id=x.Id,ProductCode=x.ProductCodeSnapshot,ProductName=x.ProductNameSnapshot,UnitName=x.UnitNameSnapshot,Quantity=x.OrderedQuantity,UnitPrice=x.UnitPrice}).ToList()};}
    private async Task<PurchaseOrder> RequireOrder(int supplierId,int orderId){var order=await orders.GetByIdAsync(orderId)??throw new InvalidOperationException("Supplier document was not found.");if(order.SupplierId!=supplierId)throw new InvalidOperationException("Supplier document was not found.");return order;}
    private async Task Audit(SupplierIntegrationCredential credential,string action,string outcome,string correlation,string? remote){await unitOfWork.ExecuteInTransactionAsync(async()=>{credential.RecordUse();await integrations.AddAuditAsync(new SupplierExternalAuditEvent(credential.SupplierId,SupplierExternalChannel.Api,credential.Id,$"api:{credential.Id}",action,outcome,correlation,remote));});}
    private async Task<(SupplierIntegrationCredential Credential,int TenantId)> ResolveApi(string raw,string scope){if(string.IsNullOrWhiteSpace(raw))throw new UnauthorizedAccessException("Supplier API key is required.");var resolved=await integrations.ResolveCredentialHashAsync(Hash(raw));if(resolved is null||!resolved.Value.Credential.IsUsable(DateTime.UtcNow)||!resolved.Value.Credential.HasScope(scope))throw new UnauthorizedAccessException("Supplier API key is invalid or does not grant this operation.");return resolved.Value;}
    private async Task<(SupplierPortalAccess Access,int TenantId)> ResolvePortal(string raw,SupplierExternalDocumentType type){if(string.IsNullOrWhiteSpace(raw))throw new UnauthorizedAccessException("Supplier portal link is invalid or expired.");var resolved=await integrations.ResolvePortalHashAsync(Hash(raw));if(resolved is null||!resolved.Value.Access.IsUsable(DateTime.UtcNow)||resolved.Value.Access.DocumentType!=type)throw new UnauthorizedAccessException("Supplier portal link is invalid or expired.");return resolved.Value;}
    private static Uri ValidateCallback(string value){if(!Uri.TryCreate(value.Trim(),UriKind.Absolute,out var uri)||uri.Scheme!=Uri.UriSchemeHttps||uri.IsLoopback||System.Net.IPAddress.TryParse(uri.Host,out _))throw new ArgumentException("Callback URL must use HTTPS and a public hostname.");return uri;}
    private static string Hash(string value)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value??string.Empty)));
    private static string Base64Url(byte[] value)=>Convert.ToBase64String(value).TrimEnd('=').Replace('+','-').Replace('/','_');
}
