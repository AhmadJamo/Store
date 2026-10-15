using Microsoft.AspNetCore.DataProtection;
using MiniStore.Application.Services;

namespace MiniStore.Web.Services;

public sealed class SupplierIntegrationSecretProtector(IDataProtectionProvider provider) : ISupplierIntegrationSecretProtector
{
    private readonly IDataProtector protector=provider.CreateProtector("MiniStore.SupplierIntegration.Callback.v1");
    public string Protect(string value)=>protector.Protect(value);
    public string Unprotect(string value)=>protector.Unprotect(value);
}
