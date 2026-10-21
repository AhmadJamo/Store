namespace MiniStore.Domain.Entities;

public enum SalesControlMetric
{
    OrderCompanyAmount = 1,
    CreditExposure = 2,
    OverdueDays = 3,
    DiscountPercent = 4,
    MarginPercent = 5
}

public enum SalesControlComparison
{
    GreaterThanOrEqual = 1,
    GreaterThan = 2,
    LessThanOrEqual = 3,
    LessThan = 4
}

public enum SalesControlAction { Warn = 1, RequireApproval = 2, Block = 3 }
public enum SalesOrderApprovalStatus { AutoApproved = 1, Pending = 2, Approved = 3, Rejected = 4, Blocked = 5, Cancelled = 6 }
public enum SalesOrderApprovalStepStatus { Pending = 1, Approved = 2, Rejected = 3 }

public sealed class SalesApprovalRule
{
    private SalesApprovalRule() { Name = string.Empty; }
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int? CustomerId { get; private set; }
    public SalesControlMetric Metric { get; private set; }
    public SalesControlComparison Comparison { get; private set; }
    public decimal Threshold { get; private set; }
    public SalesControlAction Action { get; private set; }
    public int Priority { get; private set; }
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool AllowRequesterApproval { get; private set; }
    public bool RequireDistinctApprovers { get; private set; }
    public bool IsActive { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SalesApprovalRuleStep> Steps { get; private set; } = [];

    public SalesApprovalRule(string name, int? customerId, SalesControlMetric metric,
        SalesControlComparison comparison, decimal threshold, SalesControlAction action,
        int priority, DateOnly? effectiveFrom, DateOnly? effectiveTo,
        bool allowRequesterApproval = false, bool requireDistinctApprovers = true)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 150)
            throw new ArgumentException("Sales approval rule name is required and cannot exceed 150 characters.");
        if (customerId <= 0) customerId = null;
        if (!Enum.IsDefined(metric) || !Enum.IsDefined(comparison) || !Enum.IsDefined(action))
            throw new ArgumentException("Sales approval rule condition is invalid.");
        if (threshold < 0 || threshold > 999999999999.999999m)
            throw new ArgumentException("Sales approval threshold must be between zero and 999999999999.999999.");
        if (priority is < 1 or > 9999)
            throw new ArgumentException("Sales approval rule priority must be between 1 and 9999.");
        if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveTo < effectiveFrom)
            throw new ArgumentException("Sales approval rule effective date range is invalid.");
        Name = name.Trim(); CustomerId = customerId; Metric = metric; Comparison = comparison;
        Threshold = threshold; Action = action; Priority = priority;
        EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo; IsActive = true;
        AllowRequesterApproval = allowRequesterApproval; RequireDistinctApprovers = requireDistinctApprovers;
    }

    public void AddStep(int sequence, string roleName)
    {
        if (Action != SalesControlAction.RequireApproval)
            throw new InvalidOperationException("Approval steps can be added only to rules that require approval.");
        if (sequence <= 0 || Steps.Any(x => x.Sequence == sequence))
            throw new ArgumentException("Sales approval step sequence must be positive and unique.");
        Steps.Add(new SalesApprovalRuleStep(sequence, roleName));
    }

    public bool IsEffective(int customerId, DateOnly date) => IsActive &&
        (!CustomerId.HasValue || CustomerId == customerId) &&
        (!EffectiveFrom.HasValue || EffectiveFrom <= date) &&
        (!EffectiveTo.HasValue || EffectiveTo >= date);

    public bool IsTriggered(decimal value) => Comparison switch
    {
        SalesControlComparison.GreaterThanOrEqual => value >= Threshold,
        SalesControlComparison.GreaterThan => value > Threshold,
        SalesControlComparison.LessThanOrEqual => value <= Threshold,
        SalesControlComparison.LessThan => value < Threshold,
        _ => false
    };

    public void SetActive(bool active) => IsActive = active;
}

public sealed class SalesApprovalRuleStep
{
    private SalesApprovalRuleStep() { ApproverRoleName = string.Empty; }
    public int Id { get; private set; }
    public int SalesApprovalRuleId { get; private set; }
    public int Sequence { get; private set; }
    public string ApproverRoleName { get; private set; }
    public SalesApprovalRuleStep(int sequence, string roleName)
    {
        if (sequence <= 0) throw new ArgumentException("Sales approval step sequence must be greater than zero.");
        if (string.IsNullOrWhiteSpace(roleName) || roleName.Trim().Length > 100)
            throw new ArgumentException("Sales approver role is required and cannot exceed 100 characters.");
        Sequence = sequence; ApproverRoleName = roleName.Trim();
    }
}

public sealed class SalesOrderApproval
{
    private SalesOrderApproval() { RequestedByUserId = string.Empty; }
    public int Id { get; private set; }
    public int SalesOrderId { get; private set; }
    public SalesOrderApprovalStatus Status { get; private set; }
    public string RequestedByUserId { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public bool AllowRequesterApproval { get; private set; }
    public bool RequireDistinctApprovers { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SalesOrderApprovalFinding> Findings { get; private set; } = [];
    public List<SalesOrderApprovalStep> Steps { get; private set; } = [];

    public SalesOrderApproval(int salesOrderId, string userId,
        IEnumerable<SalesApprovalTriggeredRule> triggeredRules)
    {
        if (salesOrderId <= 0) throw new ArgumentException("Sales order is required for approval evaluation.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Approval requester is required.");
        var triggered = triggeredRules.OrderBy(x => x.Rule.Priority).ThenBy(x => x.Rule.Id).ToList();
        if (triggered.Count == 0) throw new ArgumentException("At least one triggered sales control is required.");
        SalesOrderId = salesOrderId; RequestedByUserId = userId.Trim(); RequestedAtUtc = DateTime.UtcNow;
        Findings = triggered.Select(x => new SalesOrderApprovalFinding(x.Rule, x.ActualValue)).ToList();
        var approvalRules = triggered.Where(x => x.Rule.Action == SalesControlAction.RequireApproval).ToList();
        AllowRequesterApproval = approvalRules.Count > 0 && approvalRules.All(x => x.Rule.AllowRequesterApproval);
        RequireDistinctApprovers = approvalRules.Any(x => x.Rule.RequireDistinctApprovers);
        if (triggered.Any(x => x.Rule.Action == SalesControlAction.Block))
        {
            Status = SalesOrderApprovalStatus.Blocked; CompletedAtUtc = DateTime.UtcNow; return;
        }
        if (approvalRules.Count == 0)
        {
            Status = SalesOrderApprovalStatus.AutoApproved; CompletedAtUtc = DateTime.UtcNow; return;
        }
        var roles = approvalRules.SelectMany(x => x.Rule.Steps.OrderBy(s => s.Sequence).Select(s => s.ApproverRoleName))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (roles.Count == 0) throw new InvalidOperationException("A triggered sales approval rule has no approver steps.");
        Steps = roles.Select((role, index) => new SalesOrderApprovalStep(index + 1, role)).ToList();
        Status = SalesOrderApprovalStatus.Pending;
    }

    public SalesOrderApprovalStep CurrentStep()
    {
        if (Status != SalesOrderApprovalStatus.Pending)
            throw new InvalidOperationException("Sales order approval is not pending.");
        return Steps.OrderBy(x => x.Sequence).First(x => x.Status == SalesOrderApprovalStepStatus.Pending);
    }

    public void ApproveCurrent(string userId, string? note)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Approval decision user is required.");
        var normalizedUserId = userId.Trim();
        if (!AllowRequesterApproval && string.Equals(RequestedByUserId, normalizedUserId, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The sales order requester cannot approve this control evaluation.");
        if (RequireDistinctApprovers && Steps.Any(x => x.Status == SalesOrderApprovalStepStatus.Approved && string.Equals(x.DecidedByUserId, normalizedUserId, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Each sales approval step requires a different user.");
        CurrentStep().Approve(normalizedUserId, note);
        if (!Steps.All(x => x.Status == SalesOrderApprovalStepStatus.Approved)) return;
        Status = SalesOrderApprovalStatus.Approved; CompletedAtUtc = DateTime.UtcNow;
    }

    public void RejectCurrent(string userId, string reason)
    {
        CurrentStep().Reject(userId, reason);
        Status = SalesOrderApprovalStatus.Rejected; CompletedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != SalesOrderApprovalStatus.Pending) return;
        Status = SalesOrderApprovalStatus.Cancelled; CompletedAtUtc = DateTime.UtcNow;
    }
}

public sealed record SalesApprovalTriggeredRule(SalesApprovalRule Rule, decimal ActualValue);

public sealed class SalesOrderApprovalFinding
{
    private SalesOrderApprovalFinding() { RuleNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int SalesOrderApprovalId { get; private set; }
    public int SalesApprovalRuleId { get; private set; }
    public string RuleNameSnapshot { get; private set; }
    public SalesControlMetric Metric { get; private set; }
    public SalesControlComparison Comparison { get; private set; }
    public decimal Threshold { get; private set; }
    public decimal ActualValue { get; private set; }
    public SalesControlAction Action { get; private set; }
    public SalesOrderApprovalFinding(SalesApprovalRule rule, decimal actualValue)
    {
        SalesApprovalRuleId = rule.Id; RuleNameSnapshot = rule.Name; Metric = rule.Metric;
        Comparison = rule.Comparison; Threshold = rule.Threshold; ActualValue = actualValue; Action = rule.Action;
    }
}

public sealed class SalesOrderApprovalStep
{
    private SalesOrderApprovalStep() { ApproverRoleName = string.Empty; }
    public int Id { get; private set; }
    public int SalesOrderApprovalId { get; private set; }
    public int Sequence { get; private set; }
    public string ApproverRoleName { get; private set; }
    public SalesOrderApprovalStepStatus Status { get; private set; }
    public string? DecidedByUserId { get; private set; }
    public DateTime? DecidedAtUtc { get; private set; }
    public string? Note { get; private set; }
    public SalesOrderApprovalStep(int sequence, string roleName)
    {
        if (sequence <= 0) throw new ArgumentException("Sales approval step sequence must be greater than zero.");
        if (string.IsNullOrWhiteSpace(roleName) || roleName.Trim().Length > 100)
            throw new ArgumentException("Sales approver role is required and cannot exceed 100 characters.");
        Sequence = sequence; ApproverRoleName = roleName.Trim(); Status = SalesOrderApprovalStepStatus.Pending;
    }
    public void Approve(string userId, string? note)
    {
        EnsurePending(userId); Status = SalesOrderApprovalStepStatus.Approved;
        DecidedByUserId = userId.Trim(); DecidedAtUtc = DateTime.UtcNow; Note = Normalize(note);
    }
    public void Reject(string userId, string reason)
    {
        EnsurePending(userId);
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ArgumentException("Sales approval rejection reason is required and cannot exceed 500 characters.");
        Status = SalesOrderApprovalStepStatus.Rejected; DecidedByUserId = userId.Trim();
        DecidedAtUtc = DateTime.UtcNow; Note = reason.Trim();
    }
    private void EnsurePending(string userId)
    {
        if (Status != SalesOrderApprovalStepStatus.Pending) throw new InvalidOperationException("Sales approval step was already decided.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Approval decision user is required.");
    }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
