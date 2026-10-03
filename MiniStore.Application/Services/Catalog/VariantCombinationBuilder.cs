namespace MiniStore.Application.Services;

public static class VariantCombinationBuilder
{
    public static List<List<T>> Build<T>(IReadOnlyCollection<List<T>> groups, int maximum)
    {
        if (groups.Count == 0 || groups.Any(group => group.Count == 0))
            throw new ArgumentException("Every variant attribute must contain at least one option.");
        var count = groups.Aggregate(1L, (current, group) => current * group.Count);
        if (count > maximum)
            throw new InvalidOperationException($"A maximum of {maximum} variants can be generated at once.");

        var combinations = new List<List<T>> { new() };
        foreach (var group in groups)
            combinations = combinations.SelectMany(current => group.Select(option => current.Append(option).ToList())).ToList();
        return combinations;
    }
}
