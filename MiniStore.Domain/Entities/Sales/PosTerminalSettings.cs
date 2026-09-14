using System.Text.RegularExpressions;

namespace MiniStore.Domain.Entities;

public class PosTerminalSettings
{
    public int PosTerminalId { get; private set; }
    public PosExperienceProfile Profile { get; private set; }
    public PosProductLayout ProductLayout { get; private set; }
    public PosTheme Theme { get; private set; }
    public PosCartPosition CartPosition { get; private set; }
    public string AccentColor { get; private set; } = "#2563A8";
    public string HeaderTitle { get; private set; } = "Point of Sale";
    public int ProductColumns { get; private set; }
    public bool CompactProductCards { get; private set; }
    public bool ShowBarcode { get; private set; }
    public bool ShowPrice { get; private set; }
    public bool ShowStock { get; private set; }
    public bool AutoFocusSearch { get; private set; }
    public bool TouchOptimized { get; private set; }
    public PosOrderTypeOptions EnabledOrderTypes { get; private set; }
    public PosOrderType DefaultOrderType { get; private set; }
    public bool RequireServiceReference { get; private set; }
    public bool EnableGuestCount { get; private set; }
    public bool EnableItemNotes { get; private set; }
    public bool QuickAddOnBarcodeScan { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private PosTerminalSettings()
    {
    }

    public PosTerminalSettings(int posTerminalId, PosExperienceProfile profile)
    {
        if (posTerminalId <= 0)
            throw new ArgumentException("POS terminal is required.");
        PosTerminalId = posTerminalId;
        ApplyProfile(profile);
    }

    public void ApplyProfile(PosExperienceProfile profile)
    {
        if (!Enum.IsDefined(profile))
            throw new ArgumentException("Invalid POS profile.");

        Profile = profile;
        Theme = PosTheme.Light;
        CartPosition = PosCartPosition.Right;
        AccentColor = "#2563A8";
        ShowPrice = true;
        ShowStock = true;

        switch (profile)
        {
            case PosExperienceProfile.Grocery:
                ProductLayout = PosProductLayout.BarcodeFocused;
                HeaderTitle = "Fast Checkout";
                ProductColumns = 5;
                CompactProductCards = true;
                ShowBarcode = true;
                AutoFocusSearch = true;
                TouchOptimized = false;
                ConfigureOperations(
                    PosOrderTypeOptions.WalkIn,
                    PosOrderType.WalkIn,
                    false,
                    false,
                    false,
                    true);
                break;
            case PosExperienceProfile.Cafe:
                ProductLayout = PosProductLayout.Grid;
                HeaderTitle = "Cafe Orders";
                ProductColumns = 4;
                CompactProductCards = false;
                ShowBarcode = false;
                AutoFocusSearch = false;
                TouchOptimized = true;
                ConfigureOperations(
                    PosOrderTypeOptions.WalkIn | PosOrderTypeOptions.DineIn | PosOrderTypeOptions.Takeaway,
                    PosOrderType.WalkIn,
                    false,
                    true,
                    true,
                    false);
                break;
            case PosExperienceProfile.Restaurant:
                ProductLayout = PosProductLayout.Grid;
                HeaderTitle = "Restaurant Orders";
                ProductColumns = 3;
                CompactProductCards = false;
                ShowBarcode = false;
                AutoFocusSearch = false;
                TouchOptimized = true;
                ConfigureOperations(
                    PosOrderTypeOptions.DineIn | PosOrderTypeOptions.Takeaway | PosOrderTypeOptions.Delivery,
                    PosOrderType.DineIn,
                    true,
                    true,
                    true,
                    false);
                break;
            case PosExperienceProfile.QuickService:
                ProductLayout = PosProductLayout.Grid;
                HeaderTitle = "Quick Service";
                ProductColumns = 4;
                CompactProductCards = false;
                ShowBarcode = false;
                AutoFocusSearch = true;
                TouchOptimized = true;
                ConfigureOperations(
                    PosOrderTypeOptions.WalkIn | PosOrderTypeOptions.Takeaway | PosOrderTypeOptions.Delivery,
                    PosOrderType.Takeaway,
                    false,
                    false,
                    true,
                    true);
                break;
            default:
                ProductLayout = PosProductLayout.Grid;
                HeaderTitle = "Point of Sale";
                ProductColumns = 4;
                CompactProductCards = false;
                ShowBarcode = true;
                AutoFocusSearch = true;
                TouchOptimized = true;
                ConfigureOperations(
                    PosOrderTypeOptions.WalkIn,
                    PosOrderType.WalkIn,
                    false,
                    false,
                    false,
                    true);
                break;
        }
    }

    public void ConfigureLayout(
        PosExperienceProfile profile,
        PosProductLayout productLayout,
        PosTheme theme,
        PosCartPosition cartPosition,
        string accentColor,
        string headerTitle,
        int productColumns,
        bool compactProductCards,
        bool showBarcode,
        bool showPrice,
        bool showStock,
        bool autoFocusSearch,
        bool touchOptimized)
    {
        if (!Enum.IsDefined(profile) || !Enum.IsDefined(productLayout) ||
            !Enum.IsDefined(theme) || !Enum.IsDefined(cartPosition))
            throw new ArgumentException("Invalid POS layout option.");
        var normalizedAccentColor = accentColor?.Trim() ?? string.Empty;
        if (!Regex.IsMatch(normalizedAccentColor, "^#[0-9A-Fa-f]{6}$"))
            throw new ArgumentException("Accent color must be a six-digit hex color.");
        if (string.IsNullOrWhiteSpace(headerTitle) || headerTitle.Trim().Length > 80)
            throw new ArgumentException("POS header title is required and cannot exceed 80 characters.");
        if (productColumns is < 2 or > 6)
            throw new ArgumentException("Product columns must be between 2 and 6.");

        Profile = profile;
        ProductLayout = productLayout;
        Theme = theme;
        CartPosition = cartPosition;
        AccentColor = normalizedAccentColor.ToUpperInvariant();
        HeaderTitle = headerTitle.Trim();
        ProductColumns = productColumns;
        CompactProductCards = compactProductCards;
        ShowBarcode = showBarcode;
        ShowPrice = showPrice;
        ShowStock = showStock;
        AutoFocusSearch = autoFocusSearch;
        TouchOptimized = touchOptimized;
    }

    public void ConfigureOperations(
        PosOrderTypeOptions enabledOrderTypes,
        PosOrderType defaultOrderType,
        bool requireServiceReference,
        bool enableGuestCount,
        bool enableItemNotes,
        bool quickAddOnBarcodeScan)
    {
        const PosOrderTypeOptions allOptions =
            PosOrderTypeOptions.WalkIn |
            PosOrderTypeOptions.DineIn |
            PosOrderTypeOptions.Takeaway |
            PosOrderTypeOptions.Delivery;

        if (enabledOrderTypes == PosOrderTypeOptions.None ||
            (enabledOrderTypes & ~allOptions) != 0)
            throw new ArgumentException("Select at least one valid POS order type.");
        if (!Enum.IsDefined(defaultOrderType) || !Supports(defaultOrderType, enabledOrderTypes))
            throw new ArgumentException("The default order type must be enabled.");
        if (requireServiceReference && !enabledOrderTypes.HasFlag(PosOrderTypeOptions.DineIn))
            throw new ArgumentException("A required service reference needs Dine In to be enabled.");

        EnabledOrderTypes = enabledOrderTypes;
        DefaultOrderType = defaultOrderType;
        RequireServiceReference = requireServiceReference;
        EnableGuestCount = enableGuestCount;
        EnableItemNotes = enableItemNotes;
        QuickAddOnBarcodeScan = quickAddOnBarcodeScan;
    }

    public bool Supports(PosOrderType orderType) => Supports(orderType, EnabledOrderTypes);

    private static bool Supports(PosOrderType orderType, PosOrderTypeOptions options)
    {
        var option = orderType switch
        {
            PosOrderType.WalkIn => PosOrderTypeOptions.WalkIn,
            PosOrderType.DineIn => PosOrderTypeOptions.DineIn,
            PosOrderType.Takeaway => PosOrderTypeOptions.Takeaway,
            PosOrderType.Delivery => PosOrderTypeOptions.Delivery,
            _ => PosOrderTypeOptions.None
        };

        return option != PosOrderTypeOptions.None && options.HasFlag(option);
    }
}
