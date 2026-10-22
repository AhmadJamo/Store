using System.Globalization;
using MiniStore.Application.Dtos.Analytics;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public sealed class PurchaseAnalyticsModuleProvider(PurchaseAnalyticsService purchases) : IAnalyticsModuleProvider
{
    public AnalyticsModuleDefinitionDto Definition { get; } = new("purchasing", "Purchasing and procure-to-pay", "المشتريات ودورة الطلب إلى الدفع",
        "Spend, receiving, supplier, buyer and matching analytics.", "تحليل الإنفاق والاستلام والموردين والمشترين والمطابقة.", "AnalyticsCenter.View",
        [new("month","Month","الشهر"),new("supplier","Supplier","المورد"),new("buyer","Buyer","المشتري")], [],
        [
            W("openOrders","Open orders","الطلبات المفتوحة",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Number,1),
            W("spend","Billed spend","الإنفاق المفوتر",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1),
            W("receipts","Receipt value","قيمة الاستلام",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1),
            W("exceptions","Open matching exceptions","استثناءات المطابقة المفتوحة",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Number,1),
            W("trends","Spend and cost trends","اتجاهات الإنفاق والكلفة",AnalyticsVisualizationType.Bar,AnalyticsValueFormat.Money,2),
            W("suppliers","Supplier performance","أداء الموردين",AnalyticsVisualizationType.Table,AnalyticsValueFormat.Money,2),
            W("buyers","Buyer workload","عبء عمل المشترين",AnalyticsVisualizationType.Table,AnalyticsValueFormat.Money,2),
            W("journey","Procure-to-pay journey","مسار الطلب إلى الدفع",AnalyticsVisualizationType.Funnel,AnalyticsValueFormat.Number,2),
            W("legacyComparison","Legacy and modern comparison","مقارنة القديم والحديث",AnalyticsVisualizationType.Comparison,AnalyticsValueFormat.Money,2),
            W("definitions","KPI definitions and sources","تعريفات ومصادر المؤشرات",AnalyticsVisualizationType.Table,AnalyticsValueFormat.Text,2)
        ]);

    public async Task<AnalyticsDatasetDto> GetAsync(AnalyticsCenterQueryDto query)
    {
        var q=new PurchaseAnalyticsQueryDto{From=query.From!.Value,To=query.To!.Value,GroupBy=query.Dimension?.ToLowerInvariant() switch{"supplier"=>PurchaseAnalyticsGroupBy.Supplier,"buyer"=>PurchaseAnalyticsGroupBy.Buyer,_=>PurchaseAnalyticsGroupBy.Month}};
        var x=await purchases.GetAsync(q);
        return new(){ModuleKey="purchasing",Title=Definition.Title,TitleArabic=Definition.TitleArabic,Widgets=
        [
            Metric("openOrders",x.OpenOrders,$"{x.OverdueOrders} overdue",$"{x.OverdueOrders} متأخر"),Metric("spend",x.VendorBillSpend,$"{N(x.OutstandingPayables)} outstanding",$"{N(x.OutstandingPayables)} قائم"),Metric("receipts",x.ReceiptValue,$"{N(x.AverageDeliveryDays)} average days",$"متوسط {N(x.AverageDeliveryDays)} يوم"),Metric("exceptions",x.OpenMatchExceptionCount,$"{x.MatchOverrideCount} overrides",$"{x.MatchOverrideCount} تجاوز"),
            Points("trends",x.Trends.Select(v=>new AnalyticsPointDto(v.GroupLabel,v.Spend)).ToList()),
            Table("suppliers",["Supplier","Orders","Spend","Fill %","On-time %","Outstanding"],x.Suppliers.Select(v=>new AnalyticsRowDto([v.SupplierName,v.OrderCount.ToString(),N(v.BilledSpend),N(v.FillRatePercent),N(v.OnTimeRatePercent),N(v.OutstandingBalance)],"Suppliers",v.SupplierId)).ToList()),
            Table("buyers",["Buyer","Orders","Open","Overdue","Value"],x.BuyerWorkloads.Select(v=>new AnalyticsRowDto([v.BuyerDisplayName,v.OrderCount.ToString(),v.OpenOrders.ToString(),v.OverdueOrders.ToString(),N(v.OrderedValue)])).ToList()),
            Points("journey",x.AcceptanceJourney.Select(v=>new AnalyticsPointDto(v.Label,v.Count,v.Controller)).ToList()),
            Table("legacyComparison",["Workflow","Documents","Value"],[new AnalyticsRowDto(["Legacy direct purchases",x.LegacyPurchaseCount.ToString(),N(x.LegacyPurchaseValue)]),new AnalyticsRowDto(["Modern purchase orders",x.ModernPurchaseOrderCount.ToString(),N(x.ModernPurchaseOrderValue)])]),
            Table("definitions",["KPI","Value","Source","Period","Explanation"],x.Kpis.Select(v=>new AnalyticsRowDto([v.Label,v.Value,v.Source,v.Period,v.Explanation])).ToList())
        ]};
    }
    private static AnalyticsWidgetDefinitionDto W(string key,string en,string ar,AnalyticsVisualizationType v,AnalyticsValueFormat f,int span)=>new(key,en,ar,v,f,span,$"Reusable {en.ToLowerInvariant()} evidence.",$"بيانات قابلة لإعادة الاستخدام: {ar}.");
    private AnalyticsWidgetDataDto Metric(string key,decimal value,string? secondary=null,string? secondaryArabic=null){var d=Definition.Widgets.Single(x=>x.Key==key);return new(){Key=key,Title=d.Title,TitleArabic=d.TitleArabic,Definition=d.Description,DefinitionArabic=d.DescriptionArabic,Visualization=d.DefaultVisualization,ValueFormat=d.ValueFormat,Value=value,SecondaryText=secondary,SecondaryTextArabic=secondaryArabic};}
    private AnalyticsWidgetDataDto Points(string key,List<AnalyticsPointDto> points){var d=Definition.Widgets.Single(x=>x.Key==key);return new(){Key=key,Title=d.Title,TitleArabic=d.TitleArabic,Definition=d.Description,DefinitionArabic=d.DescriptionArabic,Visualization=d.DefaultVisualization,ValueFormat=d.ValueFormat,Points=points};}
    private AnalyticsWidgetDataDto Table(string key,List<string> columns,List<AnalyticsRowDto> rows){var d=Definition.Widgets.Single(x=>x.Key==key);return new(){Key=key,Title=d.Title,TitleArabic=d.TitleArabic,Definition=d.Description,DefinitionArabic=d.DescriptionArabic,Visualization=d.DefaultVisualization,ValueFormat=d.ValueFormat,Columns=columns,Rows=rows};}
    private static string N(decimal x)=>x.ToString("0.##",CultureInfo.CurrentCulture);
}
