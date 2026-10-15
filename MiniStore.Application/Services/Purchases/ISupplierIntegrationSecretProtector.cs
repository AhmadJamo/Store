namespace MiniStore.Application.Services;

public interface ISupplierIntegrationSecretProtector
{
    string Protect(string value);
    string Unprotect(string value);
}
