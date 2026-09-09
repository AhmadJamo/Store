namespace MiniStore.Domain.Entities;

public class Supplier
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    public Supplier(
        string name,
        string? phone = null,
        string? address = null)
    {
        ValidateName(name);

        Name = name.Trim();
        Phone = phone?.Trim();
        Address = address?.Trim();
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

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Supplier name is required.");
    }
}