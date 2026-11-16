namespace MiniStore.Domain.Entities;

public enum PosCashSessionStatus { Open = 1, CountSubmitted = 2, AwaitingApproval = 3, Closed = 4, Cancelled = 5 }
public enum PosCashMovementType { OpeningFloat = 1, CashSale = 2, CashRefund = 3, PaidIn = 4, PaidOut = 5 }

public sealed class PosCashPolicy
{
    public int Id { get; private set; }
    public int TenantId { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public bool RequireBlindClose { get; private set; } = true;
    public bool RequireApprovalForAnyVariance { get; private set; }
    public decimal VarianceTolerance { get; private set; }
    public decimal MandatoryApprovalThreshold { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private PosCashPolicy() { }
    public PosCashPolicy(int tenantId, string currencyCode, decimal tolerance, decimal approvalThreshold, bool approveAnyVariance = false)
    {
        if (tenantId <= 0 || string.IsNullOrWhiteSpace(currencyCode)) throw new ArgumentException("Tenant and currency are required.");
        if (tolerance < 0 || approvalThreshold < tolerance) throw new ArgumentException("Cash variance thresholds are invalid.");
        TenantId = tenantId; CurrencyCode = currencyCode.Trim().ToUpperInvariant(); VarianceTolerance = tolerance;
        MandatoryApprovalThreshold = approvalThreshold; RequireApprovalForAnyVariance = approveAnyVariance;
    }
}

public sealed class PosCashDrawer
{
    public int Id { get; private set; }
    public int TenantId { get; private set; }
    public int BranchId { get; private set; }
    public int PosTerminalId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];
    private PosCashDrawer() { }
    public PosCashDrawer(int tenantId, int branchId, int terminalId, string code, string name)
    {
        if (tenantId <= 0 || branchId <= 0 || terminalId <= 0 || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Cash drawer scope, code and name are required.");
        TenantId = tenantId; BranchId = branchId; PosTerminalId = terminalId; Code = code.Trim().ToUpperInvariant(); Name = name.Trim();
    }
}

public sealed class PosCashSession
{
    private readonly List<PosCashMovement> _movements = [];
    private readonly List<PosCashCount> _counts = [];
    public long Id { get; private set; }
    public int TenantId { get; private set; }
    public int BranchId { get; private set; }
    public int PosTerminalId { get; private set; }
    public int PosCashDrawerId { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public string OpenedByUserId { get; private set; } = string.Empty;
    public DateTime OpenedAtUtc { get; private set; }
    public decimal OpeningFloat { get; private set; }
    public PosCashSessionStatus Status { get; private set; }
    public decimal? ExpectedCash { get; private set; }
    public decimal? DeclaredCash { get; private set; }
    public decimal? Variance { get; private set; }
    public string? ClosedByUserId { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public string? ApprovedByUserId { get; private set; }
    public string? ApprovalReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<PosCashMovement> Movements => _movements.AsReadOnly();
    public IReadOnlyCollection<PosCashCount> Counts => _counts.AsReadOnly();
    private PosCashSession() { }
    public PosCashSession(int tenantId, int branchId, int terminalId, int drawerId, string currency, string userId, decimal openingFloat, DateTime openedAtUtc)
    {
        if (tenantId <= 0 || branchId <= 0 || terminalId <= 0 || drawerId <= 0 || string.IsNullOrWhiteSpace(currency) || string.IsNullOrWhiteSpace(userId) || openingFloat < 0) throw new ArgumentException("Cash session opening values are invalid.");
        TenantId=tenantId; BranchId=branchId; PosTerminalId=terminalId; PosCashDrawerId=drawerId; CurrencyCode=currency.Trim().ToUpperInvariant(); OpenedByUserId=userId; OpeningFloat=openingFloat; OpenedAtUtc=openedAtUtc; Status=PosCashSessionStatus.Open;
        _movements.Add(new PosCashMovement(tenantId, PosCashMovementType.OpeningFloat, openingFloat, openedAtUtc, userId, null));
    }
    public void RecordMovement(PosCashMovementType type, decimal amount, DateTime occurredAtUtc, string userId, string? reason)
    {
        if (Status != PosCashSessionStatus.Open || type == PosCashMovementType.OpeningFloat || amount <= 0) throw new InvalidOperationException("The cash movement is not allowed.");
        _movements.Add(new PosCashMovement(TenantId, type, amount, occurredAtUtc, userId, reason));
    }
    public void SubmitBlindCount(decimal declaredCash, DateTime countedAtUtc, string userId, PosCashPolicy policy)
    {
        if (Status != PosCashSessionStatus.Open || declaredCash < 0 || policy.TenantId != TenantId || policy.CurrencyCode != CurrencyCode) throw new InvalidOperationException("The cash count cannot be submitted.");
        var expected = _movements.Sum(x => x.ExpectedCashEffect); var variance = declaredCash - expected; var absolute = Math.Abs(variance);
        _counts.Add(new PosCashCount(TenantId, declaredCash, countedAtUtc, userId)); ExpectedCash=expected; DeclaredCash=declaredCash; Variance=variance; ClosedByUserId=userId;
        Status = policy.RequireApprovalForAnyVariance && absolute > 0 || absolute >= policy.MandatoryApprovalThreshold ? PosCashSessionStatus.AwaitingApproval : PosCashSessionStatus.CountSubmitted;
    }
    public void ApproveClose(string approver, string reason, DateTime closedAtUtc)
    {
        if (Status != PosCashSessionStatus.AwaitingApproval || string.IsNullOrWhiteSpace(approver) || approver == ClosedByUserId || string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("Independent approver and reason are required.");
        ApprovedByUserId=approver; ApprovalReason=reason.Trim(); ClosedAtUtc=closedAtUtc; Status=PosCashSessionStatus.Closed;
    }
    public void CompleteClose(DateTime closedAtUtc)
    {
        if (Status != PosCashSessionStatus.CountSubmitted) throw new InvalidOperationException("The cash session is not ready to close."); ClosedAtUtc=closedAtUtc; Status=PosCashSessionStatus.Closed;
    }
}

public sealed class PosCashMovement
{
    public long Id { get; private set; } public int TenantId { get; private set; } public long PosCashSessionId { get; private set; }
    public PosCashMovementType Type { get; private set; } public decimal Amount { get; private set; } public DateTime OccurredAtUtc { get; private set; }
    public string RecordedByUserId { get; private set; } = string.Empty; public string? Reason { get; private set; }
    public decimal ExpectedCashEffect => Type is PosCashMovementType.CashRefund or PosCashMovementType.PaidOut ? -Amount : Amount;
    private PosCashMovement() { }
    internal PosCashMovement(int tenantId, PosCashMovementType type, decimal amount, DateTime occurredAtUtc, string userId, string? reason)
    { if (tenantId<=0 || amount<0 || string.IsNullOrWhiteSpace(userId) || type is PosCashMovementType.PaidIn or PosCashMovementType.PaidOut && string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Cash movement values are invalid.");TenantId=tenantId;Type=type;Amount=amount;OccurredAtUtc=occurredAtUtc;RecordedByUserId=userId;Reason=reason?.Trim(); }
}

public sealed class PosCashCount
{
    public long Id { get; private set; } public int TenantId { get; private set; } public long PosCashSessionId { get; private set; }
    public decimal DeclaredCash { get; private set; } public DateTime CountedAtUtc { get; private set; } public string CountedByUserId { get; private set; } = string.Empty; public bool WasBlind { get; private set; } = true;
    private PosCashCount() { }
    internal PosCashCount(int tenantId, decimal amount, DateTime countedAtUtc, string userId)
    { if (tenantId<=0 || amount<0 || string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("A valid blind cash count is required.");TenantId=tenantId;DeclaredCash=amount;CountedAtUtc=countedAtUtc;CountedByUserId=userId; }
}
