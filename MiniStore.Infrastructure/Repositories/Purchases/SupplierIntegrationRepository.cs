using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SupplierIntegrationRepository(AppDbContext context) : ISupplierIntegrationRepository
{
    public Task<List<SupplierIntegrationCredential>> GetCredentialsAsync()=>context.SupplierIntegrationCredentials.OrderBy(x=>x.SupplierId).ThenBy(x=>x.Name).ToListAsync();
    public Task<SupplierIntegrationCredential?> GetCredentialAsync(int id)=>context.SupplierIntegrationCredentials.FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<(SupplierIntegrationCredential Credential,int TenantId)?> ResolveCredentialHashAsync(string keyHash){var row=await context.SupplierIntegrationCredentials.IgnoreQueryFilters().Select(x=>new{x,TenantId=EF.Property<int>(x,"TenantId")}).SingleOrDefaultAsync(x=>x.x.KeyHash==keyHash);return row is null?null:(row.x,row.TenantId);}
    public async Task<(SupplierPortalAccess Access,int TenantId)?> ResolvePortalHashAsync(string tokenHash){var row=await context.SupplierPortalAccesses.IgnoreQueryFilters().Select(x=>new{x,TenantId=EF.Property<int>(x,"TenantId")}).SingleOrDefaultAsync(x=>x.x.TokenHash==tokenHash);return row is null?null:(row.x,row.TenantId);}
    public Task<List<SupplierPortalAccess>> GetPortalAccessesAsync(SupplierExternalDocumentType type,int documentId)=>context.SupplierPortalAccesses.Where(x=>x.DocumentType==type&&x.DocumentId==documentId).OrderByDescending(x=>x.CreatedAtUtc).ToListAsync();
    public Task<SupplierPortalAccess?> GetPortalAccessAsync(int id)=>context.SupplierPortalAccesses.FirstOrDefaultAsync(x=>x.Id==id);
    public Task<SupplierExternalSubmission?> FindSubmissionAsync(SupplierExternalChannel channel,int channelIdentityId,string idempotencyKey)=>context.SupplierExternalSubmissions.FirstOrDefaultAsync(x=>x.Channel==channel&&x.ChannelIdentityId==channelIdentityId&&x.IdempotencyKey==idempotencyKey);
    public Task<List<SupplierExternalSubmission>> GetRecentSubmissionsAsync(int take=100)=>context.SupplierExternalSubmissions.OrderByDescending(x=>x.CreatedAtUtc).Take(take).ToListAsync();
    public Task<SupplierExternalSubmission?> GetSubmissionAsync(int id)=>context.SupplierExternalSubmissions.FirstOrDefaultAsync(x=>x.Id==id);
    public Task<List<SupplierExternalAuditEvent>> GetRecentAuditsAsync(int take=100)=>context.SupplierExternalAuditEvents.OrderByDescending(x=>x.CreatedAtUtc).Take(take).ToListAsync();
    public Task<List<SupplierIntegrationOutbox>> GetOutboxAsync(int take=100)=>context.SupplierIntegrationOutboxMessages.OrderByDescending(x=>x.CreatedAtUtc).Take(take).ToListAsync();
    public async Task<List<(SupplierIntegrationOutbox Message,int TenantId)>> GetDueOutboxAcrossTenantsAsync(int take,DateTime utcNow){var rows=await context.SupplierIntegrationOutboxMessages.IgnoreQueryFilters().Where(x=>(x.Status==SupplierOutboxStatus.Pending||x.Status==SupplierOutboxStatus.Processing)&&x.NextAttemptAtUtc<=utcNow).OrderBy(x=>x.NextAttemptAtUtc).Take(take).Select(x=>new{x,TenantId=EF.Property<int>(x,"TenantId")}).ToListAsync();return rows.Select(x=>(x.x,x.TenantId)).ToList();}
    public Task<SupplierIntegrationOutbox?> GetOutboxMessageAsync(int id)=>context.SupplierIntegrationOutboxMessages.FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<bool> TryStartOutboxAsync(int id,DateTime utcNow,DateTime leaseUntilUtc)=>(await context.SupplierIntegrationOutboxMessages.Where(x=>x.Id==id&&x.Status!=SupplierOutboxStatus.Delivered&&x.NextAttemptAtUtc<=utcNow).ExecuteUpdateAsync(setters=>setters.SetProperty(x=>x.Status,SupplierOutboxStatus.Processing).SetProperty(x=>x.AttemptCount,x=>x.AttemptCount+1).SetProperty(x=>x.NextAttemptAtUtc,leaseUntilUtc)))==1;
    public Task<SupplierIntegrationOutbox?> FindOutboxByKeyAsync(string idempotencyKey)=>context.SupplierIntegrationOutboxMessages.FirstOrDefaultAsync(x=>x.IdempotencyKey==idempotencyKey);
    public Task AddCredentialAsync(SupplierIntegrationCredential value)=>context.SupplierIntegrationCredentials.AddAsync(value).AsTask();
    public Task AddPortalAccessAsync(SupplierPortalAccess value)=>context.SupplierPortalAccesses.AddAsync(value).AsTask();
    public Task AddSubmissionAsync(SupplierExternalSubmission value)=>context.SupplierExternalSubmissions.AddAsync(value).AsTask();
    public Task AddOutboxAsync(SupplierIntegrationOutbox value)=>context.SupplierIntegrationOutboxMessages.AddAsync(value).AsTask();
    public Task AddAuditAsync(SupplierExternalAuditEvent value)=>context.SupplierExternalAuditEvents.AddAsync(value).AsTask();
}
