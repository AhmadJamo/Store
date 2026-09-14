namespace MiniStore.Domain.Interfaces;

public interface ITenantContext
{
    int? TenantId { get; }
}
