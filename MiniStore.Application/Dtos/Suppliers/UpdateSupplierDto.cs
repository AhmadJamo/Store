namespace MiniStore.Application.DTOs.Suppliers;

public class UpdateSupplierDto
{
    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Address { get; set; }
}