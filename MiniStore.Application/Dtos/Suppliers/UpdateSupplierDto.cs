namespace MiniStore.Application.DTOs.Suppliers;

public class UpdateSupplierDto : CreateSupplierDto
{
    public byte[] RowVersion { get; set; } = [];
}
