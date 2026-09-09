namespace MiniStore.Domain.Entities;

public class Warehouse
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public Warehouse(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Warehouse name is required.");

        Name = name;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Warehouse name is required.");

        Name = name;
    }
}