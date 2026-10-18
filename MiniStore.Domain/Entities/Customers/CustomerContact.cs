namespace MiniStore.Domain.Entities;

public class CustomerContact
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Role { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? ArchivedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private CustomerContact() { }
    public CustomerContact(int customerId, string name, string? role, string? phone, string? email, bool isPrimary)
    { if (customerId <= 0) throw new ArgumentException("Customer is required."); CustomerId = customerId; Update(name, role, phone, email, isPrimary); }
    public void Update(string name, string? role, string? phone, string? email, bool isPrimary)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200) throw new ArgumentException("Contact name is required and cannot exceed 200 characters.");
        if (role?.Trim().Length > 100) throw new ArgumentException("Contact role cannot exceed 100 characters.");
        if (phone?.Trim().Length > 50) throw new ArgumentException("Contact phone cannot exceed 50 characters.");
        if (email?.Trim().Length > 256 || (!string.IsNullOrWhiteSpace(email) && !System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _))) throw new ArgumentException("Contact email address is invalid.");
        Name=name.Trim(); Role=Normalize(role); Phone=Normalize(phone); Email=Normalize(email); IsPrimary=isPrimary;
    }
    public void RemovePrimary() => IsPrimary = false;
    public void Archive(DateTime utcNow) { IsActive=false; IsPrimary=false; ArchivedAtUtc=utcNow; }
    public void Restore() { IsActive=true; ArchivedAtUtc=null; }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public enum CustomerAddressType { General = 1, Billing = 2, Shipping = 3 }

public class CustomerAddress
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public CustomerAddressType Type { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public string AddressLine1 { get; private set; } = string.Empty;
    public string? AddressLine2 { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string? StateOrProvince { get; private set; }
    public string? PostalCode { get; private set; }
    public string CountryCode { get; private set; } = "JO";
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? ArchivedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private CustomerAddress() { }
    public CustomerAddress(int customerId, CustomerAddressType type, string label, string line1, string? line2, string city, string? state, string? postalCode, string countryCode, bool isDefault)
    { if(customerId<=0) throw new ArgumentException("Customer is required."); CustomerId=customerId; Update(type,label,line1,line2,city,state,postalCode,countryCode,isDefault); }
    public void Update(CustomerAddressType type, string label, string line1, string? line2, string city, string? state, string? postalCode, string countryCode, bool isDefault)
    {
        if(!Enum.IsDefined(type)) throw new ArgumentException("Address type is invalid.");
        if(string.IsNullOrWhiteSpace(label)||label.Trim().Length>100) throw new ArgumentException("Address label is required and cannot exceed 100 characters.");
        if(string.IsNullOrWhiteSpace(line1)||line1.Trim().Length>250) throw new ArgumentException("Address line is required and cannot exceed 250 characters.");
        if(string.IsNullOrWhiteSpace(city)||city.Trim().Length>100) throw new ArgumentException("City is required and cannot exceed 100 characters.");
        countryCode=countryCode?.Trim().ToUpperInvariant()??""; if(countryCode.Length!=2||!countryCode.All(char.IsLetter)) throw new ArgumentException("Country code must contain two letters.");
        if(line2?.Trim().Length>250||state?.Trim().Length>100||postalCode?.Trim().Length>30) throw new ArgumentException("Address details exceed the allowed length.");
        Type=type; Label=label.Trim(); AddressLine1=line1.Trim(); AddressLine2=Normalize(line2); City=city.Trim(); StateOrProvince=Normalize(state); PostalCode=Normalize(postalCode); CountryCode=countryCode; IsDefault=isDefault;
    }
    public void RemoveDefault()=>IsDefault=false;
    public void Archive(DateTime utcNow){IsActive=false;IsDefault=false;ArchivedAtUtc=utcNow;}
    public void Restore(){IsActive=true;ArchivedAtUtc=null;}
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
}
