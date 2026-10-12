namespace MiniStore.Web.Navigation;

public sealed record DashboardAppDefinition(
    string Title,
    string Description,
    string Icon,
    string Theme,
    int Order,
    IReadOnlyList<NavigationItem> Items);
