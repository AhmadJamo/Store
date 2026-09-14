namespace MiniStore.Domain.Entities;

public class Plan
{
    private readonly List<PlanFeature> _features = [];
    private readonly List<PlanLimit> _limits = [];
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NameEnglish { get; private set; } = string.Empty;
    public string NameArabic { get; private set; } = string.Empty;
    public string DescriptionEnglish { get; private set; } = string.Empty;
    public string DescriptionArabic { get; private set; } = string.Empty;
    public decimal MonthlyPrice { get; private set; }
    public decimal AnnualPrice { get; private set; }
    public string Currency { get; private set; } = "USD";
    public bool IsActive { get; private set; } = true;
    public bool IsPublic { get; private set; } = true;
    public int SortOrder { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<PlanFeature> Features => _features;
    public IReadOnlyCollection<PlanLimit> Limits => _limits;
    private Plan() { }
    public Plan(string code, string nameEnglish, string nameArabic, decimal monthlyPrice, decimal annualPrice, string currency, int sortOrder)
    {
        Update(code, nameEnglish, nameArabic, monthlyPrice, annualPrice, currency, sortOrder, true, true, "", "");
    }
    public void Update(string code, string en, string ar, decimal monthly, decimal annual, string currency, int order, bool active, bool visible, string descriptionEn, string descriptionAr)
    {
        code = code.Trim().ToLowerInvariant();
        if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[a-z0-9][a-z0-9-]{1,49}$")) throw new ArgumentException("Invalid plan code.");
        if (string.IsNullOrWhiteSpace(en) || string.IsNullOrWhiteSpace(ar)) throw new ArgumentException("Plan names are required in both languages.");
        if (monthly < 0 || annual < 0) throw new ArgumentException("Plan prices cannot be negative.");
        Code = code; NameEnglish = en.Trim(); NameArabic = ar.Trim(); MonthlyPrice = monthly; AnnualPrice = annual;
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant(); SortOrder = order;
        IsActive = active; IsPublic = visible; DescriptionEnglish = descriptionEn?.Trim() ?? ""; DescriptionArabic = descriptionAr?.Trim() ?? "";
    }
    public void SetFeature(string key, bool enabled) { var x = _features.FirstOrDefault(f => f.Key == key); if (x is null) _features.Add(new PlanFeature(Id, key, enabled)); else x.SetEnabled(enabled); }
    public void SetLimit(string key, int? value) { var x = _limits.FirstOrDefault(l => l.Key == key); if (x is null) _limits.Add(new PlanLimit(Id, key, value)); else x.SetValue(value); }
}

public class PlanFeature
{
    public int PlanId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    private PlanFeature() { }
    public PlanFeature(int planId, string key, bool enabled) { if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Feature key is required."); PlanId = planId; Key = key.Trim(); IsEnabled = enabled; }
    public void SetEnabled(bool enabled) => IsEnabled = enabled;
}

public class PlanLimit
{
    public int PlanId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public int? Value { get; private set; }
    private PlanLimit() { }
    public PlanLimit(int planId, string key, int? value) { if (string.IsNullOrWhiteSpace(key) || value < 0) throw new ArgumentException("Invalid plan limit."); PlanId = planId; Key = key.Trim(); Value = value; }
    public void SetValue(int? value) { if (value < 0) throw new ArgumentException("Limit cannot be negative."); Value = value; }
}
