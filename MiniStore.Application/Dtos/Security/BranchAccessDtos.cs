using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.Dtos.Security;

public sealed record BranchAccessOptionDto(int Id, string Code, string Name, bool IsActive);
public sealed record BranchAccessUserDto(string Id, string DisplayName, string? Email, IReadOnlyList<int> BranchIds);
public sealed record BranchAccessManagementDto(
    IReadOnlyList<BranchAccessOptionDto> Branches,
    IReadOnlyList<BranchAccessUserDto> Users);

public sealed class ReplaceUserBranchGrantsDto
{
    [Required, StringLength(450)]
    public string UserId { get; set; } = string.Empty;
    public List<int> BranchIds { get; set; } = [];
}
