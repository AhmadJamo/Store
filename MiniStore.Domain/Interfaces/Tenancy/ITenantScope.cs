namespace MiniStore.Domain.Interfaces;

public interface ITenantScope
{
    IDisposable BeginScope(int tenantId);
}
