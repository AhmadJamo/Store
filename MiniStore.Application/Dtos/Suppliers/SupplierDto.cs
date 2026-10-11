namespace MiniStore.Application.DTOs.Suppliers;

public class SupplierDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Website { get; set; }
    public int PaymentTermsDays { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public int? AccountId { get; set; }
    public List<int> ProductIds { get; set; } = [];
    public int ProductCount { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
