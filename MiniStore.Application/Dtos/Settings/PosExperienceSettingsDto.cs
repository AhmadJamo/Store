using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Settings;

public class PosExperienceSettingsPageDto
{
    public int? SelectedTerminalId { get; set; }
    public List<PosExperienceTerminalDto> Terminals { get; set; } = [];
    public PosExperienceSettingsDto? Settings { get; set; }
}

public class PosExperienceTerminalDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class PosExperienceSettingsDto
{
    public int PosTerminalId { get; set; }
    public string TerminalName { get; set; } = string.Empty;
    public PosExperienceProfile Profile { get; set; } = PosExperienceProfile.Retail;
    public PosProductLayout ProductLayout { get; set; } = PosProductLayout.Grid;
    public PosTheme Theme { get; set; } = PosTheme.Light;
    public PosCartPosition CartPosition { get; set; } = PosCartPosition.Right;
    public string AccentColor { get; set; } = "#2563A8";
    public string HeaderTitle { get; set; } = "Point of Sale";
    public int ProductColumns { get; set; } = 4;
    public bool CompactProductCards { get; set; }
    public bool ShowBarcode { get; set; } = true;
    public bool ShowPrice { get; set; } = true;
    public bool ShowStock { get; set; } = true;
    public bool AutoFocusSearch { get; set; } = true;
    public bool TouchOptimized { get; set; } = true;
    public bool EnableWalkIn { get; set; } = true;
    public bool EnableDineIn { get; set; }
    public bool EnableTakeaway { get; set; }
    public bool EnableDelivery { get; set; }
    public PosOrderType DefaultOrderType { get; set; } = PosOrderType.WalkIn;
    public bool RequireServiceReference { get; set; }
    public bool EnableGuestCount { get; set; }
    public bool EnableItemNotes { get; set; }
    public bool QuickAddOnBarcodeScan { get; set; }
    public bool ApplyProfileDefaults { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public class PosRuntimeSettingsDto
{
    public int PosTerminalId { get; set; }
    public PosProductLayout ProductLayout { get; set; }
    public PosTheme Theme { get; set; }
    public PosCartPosition CartPosition { get; set; }
    public string AccentColor { get; set; } = "#2563A8";
    public string HeaderTitle { get; set; } = "Point of Sale";
    public int ProductColumns { get; set; }
    public bool CompactProductCards { get; set; }
    public bool ShowBarcode { get; set; }
    public bool ShowPrice { get; set; }
    public bool ShowStock { get; set; }
    public bool AutoFocusSearch { get; set; }
    public bool TouchOptimized { get; set; }
    public PosOrderTypeOptions EnabledOrderTypes { get; set; }
    public PosOrderType DefaultOrderType { get; set; }
    public bool RequireServiceReference { get; set; }
    public bool EnableGuestCount { get; set; }
    public bool EnableItemNotes { get; set; }
    public bool QuickAddOnBarcodeScan { get; set; }
}
