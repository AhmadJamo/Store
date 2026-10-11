namespace MiniStore.Application.DTOs.Suppliers;

public class CreateSupplierDto
{
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
    public bool IsActive { get; set; } = true;
    public int AccountId { get; set; }
    public List<int> ProductIds { get; set; } = [];
}
