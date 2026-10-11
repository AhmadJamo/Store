namespace MiniStore.Domain.Entities;

public class Supplier
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public string? LegalName { get; private set; }
    public string? ContactPerson { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? Website { get; private set; }
    public int PaymentTermsDays { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int? AccountId { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public Supplier(
        string name,
        string? phone = null,
        string? address = null)
    {
        ValidateName(name);

        Name = name.Trim();
        Phone = phone?.Trim();
        Address = address?.Trim();
        IsActive = true;
    }

    public void ChangeName(string name)
    {
        ValidateName(name);

        Name = name.Trim();
    }

    public void ChangePhone(string? phone)
    {
        Phone = phone?.Trim();
    }

    public void ChangeAddress(string? address)
    {
        Address = address?.Trim();
    }

    public void UpdateProfile(string? legalName, string? contactPerson, string? phone, string? email,
        string? address, string? taxNumber, string? registrationNumber, string? website,
        int paymentTermsDays, string? notes, bool isActive)
    {
        if (legalName?.Trim().Length > 200) throw new ArgumentException("Supplier legal name cannot exceed 200 characters.");
        if (contactPerson?.Trim().Length > 150) throw new ArgumentException("Supplier contact person cannot exceed 150 characters.");
        if (phone?.Trim().Length > 50) throw new ArgumentException("Supplier phone cannot exceed 50 characters.");
        if (email?.Trim().Length > 256 ||
            (!string.IsNullOrWhiteSpace(email) && !System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _)))
            throw new ArgumentException("Supplier email address is invalid.");
        if (address?.Trim().Length > 500) throw new ArgumentException("Supplier address cannot exceed 500 characters.");
        if (taxNumber?.Trim().Length > 100) throw new ArgumentException("Supplier tax number cannot exceed 100 characters.");
        if (registrationNumber?.Trim().Length > 100) throw new ArgumentException("Supplier registration number cannot exceed 100 characters.");
        if (website?.Trim().Length > 500 || (!string.IsNullOrWhiteSpace(website) &&
            (!Uri.TryCreate(website.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))))
            throw new ArgumentException("Supplier website must be a valid HTTP or HTTPS address.");
        if (paymentTermsDays is < 0 or > 3650) throw new ArgumentException("Supplier payment terms must be between 0 and 3650 days.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Supplier notes cannot exceed 1000 characters.");

        LegalName = Normalize(legalName); ContactPerson = Normalize(contactPerson); Phone = Normalize(phone);
        Email = Normalize(email); Address = Normalize(address); TaxNumber = Normalize(taxNumber);
        RegistrationNumber = Normalize(registrationNumber); Website = Normalize(website);
        PaymentTermsDays = paymentTermsDays; Notes = Normalize(notes); IsActive = isActive;
    }

    public void AssignPayableAccount(int accountId)
    {
        if (accountId <= 0) throw new ArgumentException("A supplier payable account is required.");
        AccountId = accountId;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Supplier name is required.");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
