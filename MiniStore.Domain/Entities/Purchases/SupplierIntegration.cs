namespace MiniStore.Domain.Entities;

public enum SupplierExternalDocumentType { Rfq = 1, PurchaseOrder = 2, Invoice = 3 }
public enum SupplierExternalSubmissionType { Quotation = 1, OrderConfirmation = 2, DeliveryNotice = 3, InvoiceUbl = 4 }
public enum SupplierExternalChannel { Portal = 1, Api = 2 }
public enum SupplierOutboxStatus { Pending = 1, Processing = 2, Delivered = 3, Failed = 4 }

public sealed class SupplierIntegrationCredential
{
    private SupplierIntegrationCredential() { Name = KeyPrefix = KeyHash = Scopes = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int SupplierId { get; private set; }
    public string Name { get; private set; }
    public string KeyPrefix { get; private set; }
    public string KeyHash { get; private set; }
    public string Scopes { get; private set; }
    public string? CallbackUrl { get; private set; }
    public string? ProtectedCallbackSecret { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastUsedAtUtc { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public SupplierIntegrationCredential(int supplierId, string name, string keyPrefix, string keyHash, string scopes,
        string? callbackUrl, string? protectedCallbackSecret, DateTime? expiresAtUtc, string userId)
    {
        if (supplierId <= 0) throw new ArgumentException("Supplier is required.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 150) throw new ArgumentException("Integration name is required and cannot exceed 150 characters.");
        if (string.IsNullOrWhiteSpace(keyPrefix) || keyPrefix.Length > 20 || string.IsNullOrWhiteSpace(keyHash) || keyHash.Length != 64) throw new ArgumentException("A valid integration key is required.");
        if (string.IsNullOrWhiteSpace(scopes) || scopes.Length > 500) throw new ArgumentException("Select at least one supplier integration permission.");
        if (callbackUrl?.Length > 1000 || protectedCallbackSecret?.Length > 4000) throw new ArgumentException("Callback configuration is too long.");
        if (expiresAtUtc.HasValue && expiresAtUtc <= DateTime.UtcNow) throw new ArgumentException("Integration expiry must be in the future.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        SupplierId = supplierId; Name = name.Trim(); KeyPrefix = keyPrefix; KeyHash = keyHash; Scopes = scopes;
        CallbackUrl = string.IsNullOrWhiteSpace(callbackUrl) ? null : callbackUrl.Trim();
        ProtectedCallbackSecret = string.IsNullOrWhiteSpace(protectedCallbackSecret) ? null : protectedCallbackSecret;
        ExpiresAtUtc = expiresAtUtc; IsActive = true; CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }

    public bool HasScope(string scope) => Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(scope, StringComparer.Ordinal);
    public bool IsUsable(DateTime utcNow) => IsActive && (!ExpiresAtUtc.HasValue || ExpiresAtUtc > utcNow);
    public void RecordUse() => LastUsedAtUtc = DateTime.UtcNow;
    public void Deactivate() => IsActive = false;
}

public sealed class SupplierPortalAccess
{
    private SupplierPortalAccess() { DocumentNumberSnapshot = TokenHash = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int SupplierId { get; private set; }
    public SupplierExternalDocumentType DocumentType { get; private set; }
    public int DocumentId { get; private set; }
    public string DocumentNumberSnapshot { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? LastAccessedAtUtc { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public SupplierPortalAccess(int supplierId, SupplierExternalDocumentType documentType, int documentId,
        string documentNumber, string tokenHash, DateTime expiresAtUtc, string userId)
    {
        if (supplierId <= 0 || documentId <= 0 || !Enum.IsDefined(documentType)) throw new ArgumentException("A valid supplier portal document is required.");
        if (string.IsNullOrWhiteSpace(documentNumber) || documentNumber.Trim().Length > 50) throw new ArgumentException("Portal document number is required.");
        if (string.IsNullOrWhiteSpace(tokenHash) || tokenHash.Length != 64) throw new ArgumentException("A valid portal token is required.");
        if (expiresAtUtc <= DateTime.UtcNow) throw new ArgumentException("Portal access expiry must be in the future.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        SupplierId = supplierId; DocumentType = documentType; DocumentId = documentId; DocumentNumberSnapshot = documentNumber.Trim();
        TokenHash = tokenHash; ExpiresAtUtc = expiresAtUtc; CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }

    public bool IsUsable(DateTime utcNow) => !IsRevoked && ExpiresAtUtc > utcNow;
    public void RecordAccess() => LastAccessedAtUtc = DateTime.UtcNow;
    public void Revoke() => IsRevoked = true;
}

public sealed class SupplierExternalSubmission
{
    private SupplierExternalSubmission() { IdempotencyKey = RequestHash = Payload = CreatedByActor = string.Empty; }
    public int Id { get; private set; }
    public int SupplierId { get; private set; }
    public SupplierExternalChannel Channel { get; private set; }
    public int ChannelIdentityId { get; private set; }
    public SupplierExternalSubmissionType SubmissionType { get; private set; }
    public SupplierExternalDocumentType DocumentType { get; private set; }
    public int DocumentId { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string RequestHash { get; private set; }
    public string Payload { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? ResultType { get; private set; }
    public int? ResultId { get; private set; }
    public string CreatedByActor { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public SupplierExternalSubmission(int supplierId, SupplierExternalChannel channel, int channelIdentityId,
        SupplierExternalSubmissionType submissionType, SupplierExternalDocumentType documentType, int documentId,
        string idempotencyKey, string requestHash, string payload, string? externalReference,
        string? resultType, int? resultId, string actor)
    {
        if (supplierId <= 0 || channelIdentityId <= 0 || documentId <= 0) throw new ArgumentException("A valid external supplier submission context is required.");
        if (!Enum.IsDefined(channel) || !Enum.IsDefined(submissionType) || !Enum.IsDefined(documentType)) throw new ArgumentException("External supplier submission type is invalid.");
        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Trim().Length > 100) throw new ArgumentException("Idempotency key is required and cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(requestHash) || requestHash.Length != 64) throw new ArgumentException("A valid request hash is required.");
        if (payload?.Length > 2_000_000) throw new ArgumentException("External document cannot exceed 2 MB.");
        if (externalReference?.Trim().Length > 250 || resultType?.Trim().Length > 100 || string.IsNullOrWhiteSpace(actor)) throw new ArgumentException("External submission evidence is invalid.");
        SupplierId = supplierId; Channel = channel; ChannelIdentityId = channelIdentityId; SubmissionType = submissionType;
        DocumentType = documentType; DocumentId = documentId; IdempotencyKey = idempotencyKey.Trim(); RequestHash = requestHash;
        Payload = payload ?? string.Empty; ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim();
        ResultType = string.IsNullOrWhiteSpace(resultType) ? null : resultType.Trim(); ResultId = resultId;
        CreatedByActor = actor.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }
}

public sealed class SupplierIntegrationOutbox
{
    private SupplierIntegrationOutbox() { MessageType = AggregateType = Destination = Payload = IdempotencyKey = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int SupplierIntegrationCredentialId { get; private set; }
    public string MessageType { get; private set; }
    public string AggregateType { get; private set; }
    public int AggregateId { get; private set; }
    public string Destination { get; private set; }
    public string? ProtectedAuthorizationSecret { get; private set; }
    public string Payload { get; private set; }
    public string IdempotencyKey { get; private set; }
    public SupplierOutboxStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime NextAttemptAtUtc { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }
    public string? LastError { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public SupplierIntegrationOutbox(int credentialId, string messageType, string aggregateType, int aggregateId,
        string destination, string? protectedSecret, string payload, string idempotencyKey, string userId)
    {
        if (credentialId <= 0 || aggregateId <= 0) throw new ArgumentException("A valid outbox destination and source are required.");
        if (string.IsNullOrWhiteSpace(messageType) || messageType.Length > 100 || string.IsNullOrWhiteSpace(aggregateType) || aggregateType.Length > 100) throw new ArgumentException("Outbox message identity is required.");
        if (string.IsNullOrWhiteSpace(destination) || destination.Length > 1000 || string.IsNullOrWhiteSpace(payload) || payload.Length > 2_000_000) throw new ArgumentException("Outbox destination and payload are required.");
        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length > 100 || string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Outbox idempotency key and user are required.");
        SupplierIntegrationCredentialId = credentialId; MessageType = messageType.Trim(); AggregateType = aggregateType.Trim(); AggregateId = aggregateId;
        Destination = destination.Trim(); ProtectedAuthorizationSecret = protectedSecret; Payload = payload; IdempotencyKey = idempotencyKey.Trim();
        Status = SupplierOutboxStatus.Pending; NextAttemptAtUtc = DateTime.UtcNow; CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }

    public void StartAttempt() { if (Status == SupplierOutboxStatus.Delivered) throw new InvalidOperationException("Delivered outbox messages cannot be sent again."); Status = SupplierOutboxStatus.Processing; AttemptCount++; }
    public void MarkDelivered() { Status = SupplierOutboxStatus.Delivered; DeliveredAtUtc = DateTime.UtcNow; LastError = null; }
    public void MarkFailed(string error)
    {
        LastError = string.IsNullOrWhiteSpace(error) ? "Delivery failed." : error.Trim()[..Math.Min(error.Trim().Length, 2000)];
        Status = AttemptCount >= 10 ? SupplierOutboxStatus.Failed : SupplierOutboxStatus.Pending;
        var minutes = AttemptCount switch { <= 1 => 1, 2 => 5, 3 => 15, 4 => 60, _ => 360 };
        NextAttemptAtUtc = DateTime.UtcNow.AddMinutes(minutes);
    }
    public void RetryNow() { if (Status == SupplierOutboxStatus.Delivered) throw new InvalidOperationException("Delivered outbox messages cannot be retried."); Status = SupplierOutboxStatus.Pending; NextAttemptAtUtc = DateTime.UtcNow; LastError = null; }
}

public sealed class SupplierExternalAuditEvent
{
    private SupplierExternalAuditEvent() { Actor = Action = Outcome = CorrelationId = string.Empty; }
    public int Id { get; private set; }
    public int? SupplierId { get; private set; }
    public SupplierExternalChannel Channel { get; private set; }
    public int? ChannelIdentityId { get; private set; }
    public string Actor { get; private set; }
    public string Action { get; private set; }
    public string Outcome { get; private set; }
    public string CorrelationId { get; private set; }
    public string? RemoteAddress { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public SupplierExternalAuditEvent(int? supplierId, SupplierExternalChannel channel, int? channelIdentityId,
        string actor, string action, string outcome, string correlationId, string? remoteAddress)
    {
        if (string.IsNullOrWhiteSpace(actor) || string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(outcome) || string.IsNullOrWhiteSpace(correlationId)) throw new ArgumentException("External audit evidence is required.");
        SupplierId = supplierId; Channel = channel; ChannelIdentityId = channelIdentityId; Actor = actor.Trim()[..Math.Min(actor.Trim().Length, 200)];
        Action = action.Trim()[..Math.Min(action.Trim().Length, 150)]; Outcome = outcome.Trim()[..Math.Min(outcome.Trim().Length, 100)];
        CorrelationId = correlationId.Trim()[..Math.Min(correlationId.Trim().Length, 100)]; RemoteAddress = string.IsNullOrWhiteSpace(remoteAddress) ? null : remoteAddress.Trim()[..Math.Min(remoteAddress.Trim().Length, 100)]; CreatedAtUtc = DateTime.UtcNow;
    }
}
