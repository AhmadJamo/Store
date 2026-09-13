namespace MiniStore.Domain.Entities;

public class Permission
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Group { get; private set; }

    private Permission()
    {
        Name = string.Empty;
        DisplayName = string.Empty;
        Group = string.Empty;
    }

    public Permission(
        string name,
        string displayName,
        string group)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Permission name is required.");

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException(
                "Permission display name is required.");

        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException(
                "Permission group is required.");

        Name = name;
        DisplayName = displayName;
        Group = group;
    }
}