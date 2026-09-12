namespace MiniStore.Web.Navigation;

public class NavigationGroup
{
    public string Title { get; }

    public string Icon { get; }

    public List<NavigationItem> Items { get; }

    public int Order { get; }

    public NavigationGroup(
        string title,
        string icon,
        int order = 0,
        params NavigationItem[] items)
    {
        Title = title;
        Icon = icon;
        Order = order;

        Items = items
            .OrderBy(x => x.Order)
            .ToList();
    }
}