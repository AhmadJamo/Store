using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierIntegrationRepository
{
    Task<List<SupplierIntegrationCredential>> GetCredentialsAsync();
    Task<SupplierIntegrationCredential?> GetCredentialAsync(int id);
    Task<(SupplierIntegrationCredential Credential, int TenantId)?> ResolveCredentialHashAsync(string keyHash);
    Task<(SupplierPortalAccess Access, int TenantId)?> ResolvePortalHashAsync(string tokenHash);
    Task<List<SupplierPortalAccess>> GetPortalAccessesAsync(SupplierExternalDocumentType type, int documentId);
    Task<SupplierPortalAccess?> GetPortalAccessAsync(int id);
    Task<SupplierExternalSubmission?> FindSubmissionAsync(SupplierExternalChannel channel, int channelIdentityId, string idempotencyKey);
    Task<List<SupplierExternalSubmission>> GetRecentSubmissionsAsync(int take = 100);
    Task<SupplierExternalSubmission?> GetSubmissionAsync(int id);
    Task<List<SupplierExternalAuditEvent>> GetRecentAuditsAsync(int take = 100);
    Task<List<SupplierIntegrationOutbox>> GetOutboxAsync(int take = 100);
    Task<List<(SupplierIntegrationOutbox Message, int TenantId)>> GetDueOutboxAcrossTenantsAsync(int take, DateTime utcNow);
    Task<SupplierIntegrationOutbox?> GetOutboxMessageAsync(int id);
    Task<bool> TryStartOutboxAsync(int id, DateTime utcNow, DateTime leaseUntilUtc);
    Task<SupplierIntegrationOutbox?> FindOutboxByKeyAsync(string idempotencyKey);
    Task AddCredentialAsync(SupplierIntegrationCredential value);
    Task AddPortalAccessAsync(SupplierPortalAccess value);
    Task AddSubmissionAsync(SupplierExternalSubmission value);
    Task AddOutboxAsync(SupplierIntegrationOutbox value);
    Task AddAuditAsync(SupplierExternalAuditEvent value);
}
