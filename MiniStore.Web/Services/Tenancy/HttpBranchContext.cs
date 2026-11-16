using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public sealed class HttpBranchContext(IHttpContextAccessor accessor) : IBranchContext
{
    public const string ItemKey = "MiniStore.VerifiedBranchId";

    public int? BranchId =>
        accessor.HttpContext?.Items.TryGetValue(ItemKey, out var value) == true &&
        value is int branchId ? branchId : null;
}
