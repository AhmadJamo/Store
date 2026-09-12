namespace MiniStore.Web.Navigation;

public class NavigationItem
{
    public string Title { get; }

    public string Icon { get; }

    public string Controller { get; }

    public string Action { get; }

    public string Permission { get; }

    public int Order { get; }

    public NavigationItem(
        string title,
        string icon,
        string controller,
        string action,
        string permission,
        int order = 0)
    {
        Title = title;
        Icon = icon;
        Controller = controller;
        Action = action;
        Permission = permission;
        Order = order;
    }
}