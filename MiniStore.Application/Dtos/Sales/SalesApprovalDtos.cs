using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed class CreateSalesApprovalRuleDto
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public SalesControlMetric Metric { get; set; } = SalesControlMetric.CreditExposure;
    public SalesControlComparison Comparison { get; set; } = SalesControlComparison.GreaterThanOrEqual;
    [Range(typeof(decimal), "0", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal Threshold { get; set; }
    public SalesControlAction Action { get; set; } = SalesControlAction.RequireApproval;
    [Range(1, 9999)] public int Priority { get; set; } = 100;
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool AllowRequesterApproval { get; set; }
    public bool RequireDistinctApprovers { get; set; } = true;
    public List<string> ApproverRoles { get; set; } = [];
}

public sealed class SalesApprovalRuleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public SalesControlMetric Metric { get; init; }
    public SalesControlComparison Comparison { get; init; }
    public decimal Threshold { get; init; }
    public SalesControlAction Action { get; init; }
    public int Priority { get; init; }
    public DateOnly? EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public bool AllowRequesterApproval { get; init; }
    public bool RequireDistinctApprovers { get; init; }
    public bool IsActive { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public List<string> ApproverRoles { get; init; } = [];
}

public sealed class SalesApprovalRulesPageDto
{
    public CreateSalesApprovalRuleDto Form { get; init; } = new();
    public List<SalesApprovalRuleDto> Rules { get; init; } = [];
    public List<SalesApprovalOptionDto> Customers { get; init; } = [];
    public List<string> Roles { get; init; } = [];
}

public sealed record SalesApprovalOptionDto(int Id, string Label);
public enum SalesOrderConfirmationOutcome { Confirmed = 1, PendingApproval = 2, Blocked = 3 }

public sealed class SalesOrderApprovalDto
{
    public int Id { get; init; }
    public SalesOrderApprovalStatus Status { get; init; }
    public DateTime RequestedAtUtc { get; init; }
    public string? CurrentRole { get; init; }
    public bool CanCurrentUserDecide { get; init; }
    public List<SalesOrderApprovalFindingDto> Findings { get; init; } = [];
    public List<SalesOrderApprovalStepDto> Steps { get; init; } = [];
}

public sealed record SalesOrderApprovalFindingDto(string RuleName, SalesControlMetric Metric,
    SalesControlComparison Comparison, decimal Threshold, decimal ActualValue, SalesControlAction Action);
public sealed record SalesOrderApprovalStepDto(int Sequence, string RoleName,
    SalesOrderApprovalStepStatus Status, string? DecidedByUserId, DateTime? DecidedAtUtc, string? Note);

public sealed class SalesOrderApprovalDecisionDto
{
    [Range(1, int.MaxValue)] public int SalesOrderId { get; set; }
    [StringLength(500)] public string? Note { get; set; }
}
