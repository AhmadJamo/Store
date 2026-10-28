using System.ComponentModel.DataAnnotations;using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class CreatePurchaseApprovalRuleDto
{
    [Required,StringLength(150)]public string Name { get; set; } = string.Empty;
    public int? WarehouseId { get; set; }
    public PurchaseRequestPriority MinimumPriority { get; set; } = PurchaseRequestPriority.Low;
    public PurchaseRequestPriority MaximumPriority { get; set; } = PurchaseRequestPriority.Urgent;
    [MinLength(1),MaxLength(3)]public List<string> ApproverRoles { get; set; } = [];
}

public sealed class PurchaseApprovalRuleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public PurchaseRequestPriority MinimumPriority { get; init; }
    public PurchaseRequestPriority MaximumPriority { get; init; }
    public bool IsActive { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public List<string> ApproverRoles { get; init; } = [];
}

public sealed class PurchaseApprovalRulesPageDto
{
    public CreatePurchaseApprovalRuleDto Form { get; init; } = new();
    public List<PurchaseApprovalRuleDto> Rules { get; init; } = [];
    public List<PurchaseRequestOptionDto> Warehouses { get; init; } = [];
    public List<string> Roles { get; init; } = [];
}

public sealed class PurchaseApprovalInstanceDto
{
    public PurchaseApprovalStatus Status { get; init; }
    public string RuleName { get; init; } = string.Empty;
    public string? CurrentRole { get; init; }
    public bool CanCurrentUserDecide { get; init; }
    public List<PurchaseApprovalStepDto> Steps { get; init; } = [];
}

public sealed class PurchaseApprovalStepDto
{
    public int Sequence { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public PurchaseApprovalStepStatus Status { get; init; }
    public string? DecidedByUserId { get; init; }
    public DateTime? DecidedAtUtc { get; init; }
    public string? Note { get; init; }
}
public sealed class SetPurchaseApprovalRuleActiveDto{[Range(1,int.MaxValue)]public int Id{get;set;}public bool Active{get;set;}[Required]public byte[] RowVersion{get;set;}=[];}
public sealed class PurchaseRequestDecisionDto{[Range(1,int.MaxValue)]public int Id{get;set;}[StringLength(500)]public string? Note{get;set;}}
