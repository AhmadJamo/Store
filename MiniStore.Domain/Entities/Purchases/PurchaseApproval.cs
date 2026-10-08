namespace MiniStore.Domain.Entities;

public sealed class PurchaseApprovalRule
{
    private PurchaseApprovalRule() { Name = string.Empty; }
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int? WarehouseId { get; private set; }
    public PurchaseRequestPriority MinimumPriority { get; private set; }
    public PurchaseRequestPriority MaximumPriority { get; private set; }
    public bool IsActive { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseApprovalRuleStep> Steps { get; private set; } = [];

    public PurchaseApprovalRule(string name, int? warehouseId, PurchaseRequestPriority minimumPriority,
        PurchaseRequestPriority maximumPriority)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 150)
            throw new ArgumentException("Approval rule name is required and cannot exceed 150 characters.");
        if (warehouseId <= 0) warehouseId = null;
        if (!Enum.IsDefined(minimumPriority) || !Enum.IsDefined(maximumPriority) || minimumPriority > maximumPriority)
            throw new ArgumentException("Approval priority range is invalid.");
        Name = name.Trim();
        WarehouseId = warehouseId;
        MinimumPriority = minimumPriority;
        MaximumPriority = maximumPriority;
        IsActive = true;
    }

    public void AddStep(int sequence, string approverRoleName)
    {
        if (sequence <= 0) throw new ArgumentException("Approval step sequence must be greater than zero.");
        if (Steps.Any(x => x.Sequence == sequence))
            throw new InvalidOperationException("Approval step sequence must be unique.");
        Steps.Add(new PurchaseApprovalRuleStep(sequence, approverRoleName));
    }

    public bool Matches(int warehouseId, PurchaseRequestPriority priority) =>
        IsActive && (!WarehouseId.HasValue || WarehouseId == warehouseId) &&
        priority >= MinimumPriority && priority <= MaximumPriority;

    public void SetActive(bool active) => IsActive = active;
}

public sealed class PurchaseApprovalRuleStep
{
    private PurchaseApprovalRuleStep() { ApproverRoleName = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseApprovalRuleId { get; private set; }
    public int Sequence { get; private set; }
    public string ApproverRoleName { get; private set; }

    public PurchaseApprovalRuleStep(int sequence, string approverRoleName)
    {
        if (sequence <= 0) throw new ArgumentException("Approval step sequence must be greater than zero.");
        if (string.IsNullOrWhiteSpace(approverRoleName) || approverRoleName.Trim().Length > 100)
            throw new ArgumentException("Approver role is required and cannot exceed 100 characters.");
        Sequence = sequence;
        ApproverRoleName = approverRoleName.Trim();
    }
}

public enum PurchaseApprovalStatus { Pending = 1, Approved = 2, Rejected = 3, Cancelled = 4 }
public enum PurchaseApprovalStepStatus { Pending = 1, Approved = 2, Rejected = 3 }

public sealed class PurchaseApprovalInstance
{
    private PurchaseApprovalInstance() { RuleNameSnapshot = RequestedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseRequestId { get; private set; }
    public int PurchaseApprovalRuleId { get; private set; }
    public string RuleNameSnapshot { get; private set; }
    public PurchaseApprovalStatus Status { get; private set; }
    public string RequestedByUserId { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<PurchaseApprovalStep> Steps { get; private set; } = [];

    public PurchaseApprovalInstance(int purchaseRequestId, int approvalRuleId, string ruleName,
        IEnumerable<PurchaseApprovalRuleStep> ruleSteps, string requestedByUserId)
    {
        if (purchaseRequestId <= 0) throw new ArgumentException("Purchase request is required for approval.");
        if (approvalRuleId <= 0) throw new ArgumentException("Approval rule is required.");
        if (string.IsNullOrWhiteSpace(ruleName) || ruleName.Trim().Length > 150)
            throw new ArgumentException("Approval rule name is required and cannot exceed 150 characters.");
        if (string.IsNullOrWhiteSpace(requestedByUserId)) throw new ArgumentException("Approval requester is required.");
        var snapshots = ruleSteps.OrderBy(x => x.Sequence)
            .Select(x => new PurchaseApprovalStep(x.Sequence, x.ApproverRoleName)).ToList();
        if (snapshots.Count == 0) throw new InvalidOperationException("At least one approval step is required.");
        if (snapshots.Select(x => x.Sequence).Distinct().Count() != snapshots.Count)
            throw new InvalidOperationException("Approval step sequence must be unique.");
        PurchaseRequestId = purchaseRequestId;
        PurchaseApprovalRuleId = approvalRuleId;
        RuleNameSnapshot = ruleName.Trim();
        RequestedByUserId = requestedByUserId;
        RequestedAtUtc = DateTime.UtcNow;
        Status = PurchaseApprovalStatus.Pending;
        Steps = snapshots;
    }

    public PurchaseApprovalStep CurrentStep()
    {
        if (Status != PurchaseApprovalStatus.Pending) throw new InvalidOperationException("Approval instance is not pending.");
        return Steps.OrderBy(x => x.Sequence).FirstOrDefault(x => x.Status == PurchaseApprovalStepStatus.Pending)
            ?? throw new InvalidOperationException("Approval instance has no pending step.");
    }

    public void ApproveCurrent(string userId, string? note = null)
    {
        CurrentStep().Approve(userId, note);
        if (!Steps.All(x => x.Status == PurchaseApprovalStepStatus.Approved)) return;
        Status = PurchaseApprovalStatus.Approved;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void RejectCurrent(string userId, string reason)
    {
        CurrentStep().Reject(userId, reason);
        Status = PurchaseApprovalStatus.Rejected;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != PurchaseApprovalStatus.Pending) return;
        Status = PurchaseApprovalStatus.Cancelled;
        CompletedAtUtc = DateTime.UtcNow;
    }
}

public sealed class PurchaseApprovalStep
{
    private PurchaseApprovalStep() { ApproverRoleName = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseApprovalInstanceId { get; private set; }
    public int Sequence { get; private set; }
    public string ApproverRoleName { get; private set; }
    public PurchaseApprovalStepStatus Status { get; private set; }
    public string? DecidedByUserId { get; private set; }
    public DateTime? DecidedAtUtc { get; private set; }
    public string? Note { get; private set; }

    public PurchaseApprovalStep(int sequence, string approverRoleName)
    {
        if (sequence <= 0) throw new ArgumentException("Approval step sequence must be greater than zero.");
        if (string.IsNullOrWhiteSpace(approverRoleName) || approverRoleName.Trim().Length > 100)
            throw new ArgumentException("Approver role is required and cannot exceed 100 characters.");
        Sequence = sequence;
        ApproverRoleName = approverRoleName.Trim();
        Status = PurchaseApprovalStepStatus.Pending;
    }

    public void Approve(string userId, string? note)
    {
        EnsurePending(userId);
        Status = PurchaseApprovalStepStatus.Approved;
        DecidedByUserId = userId;
        DecidedAtUtc = DateTime.UtcNow;
        Note = Normalize(note);
    }

    public void Reject(string userId, string reason)
    {
        EnsurePending(userId);
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Approval rejection reason is required.");
        Status = PurchaseApprovalStepStatus.Rejected;
        DecidedByUserId = userId;
        DecidedAtUtc = DateTime.UtcNow;
        Note = reason.Trim();
    }

    private void EnsurePending(string userId)
    {
        if (Status != PurchaseApprovalStepStatus.Pending) throw new InvalidOperationException("Approval step was already decided.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Approval decision user is required.");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
