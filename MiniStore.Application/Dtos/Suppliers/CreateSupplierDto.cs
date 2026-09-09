namespace MiniStore.Application.DTOs.Suppliers;

public class CreateSupplierDto
{
    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Address { get; set; }
}