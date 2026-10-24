namespace MiniStore.Application.DTOs.Api;

public sealed record PageRequest(int Page = 1, int PageSize = 25, string? Sort = null, string? Search = null)
{
    public const int MaximumPageSize = 200;

    public PageRequest Normalize(IReadOnlySet<string>? allowedSorts = null)
    {
        var normalizedSort = string.IsNullOrWhiteSpace(Sort) ? null : Sort.Trim();
        if (normalizedSort is not null && allowedSorts is not null && !allowedSorts.Contains(normalizedSort))
            throw new ArgumentException("The selected sort field is not supported.", nameof(Sort));

        return this with
        {
            Page = Math.Max(1, Page),
            PageSize = Math.Clamp(PageSize, 1, MaximumPageSize),
            Sort = normalizedSort,
            Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim()
        };
    }
}

public sealed record PageResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
