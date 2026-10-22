using System.Globalization;
using MiniStore.Application.Dtos.Analytics;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public sealed class SalesAnalyticsModuleProvider(SalesAnalyticsService sales) : IAnalyticsModuleProvider
{
    public AnalyticsModuleDefinitionDto Definition { get; } = new("sales", "Sales and quote-to-cash", "المبيعات ودورة العرض إلى التحصيل",
        "Conversion, revenue, receivables, collection, customer and product contribution.", "التحويل والإيراد والذمم والتحصيل وأداء العملاء والمنتجات.",
        "AnalyticsCenter.View",
        [new("month", "Month", "الشهر"), new("customer", "Customer", "العميل"), new("product", "Product", "المنتج")],
        [new("customerId", "Customer", "العميل", "select", [])],
        [
            W("quotationValue","Quotation value","قيمة عروض الأسعار",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1,"Quotation-date cohort value.","قيمة مجموعة عروض الأسعار حسب تاريخ العرض."),
            W("acceptanceRate","Quotation acceptance rate","نسبة قبول عروض الأسعار",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Percent,1,"Accepted divided by progressed quotations.","المقبولة مقسومة على العروض التي تجاوزت المسودة."),
            W("invoiceRevenue","Posted invoice net revenue","صافي إيراد الفواتير المرحلة",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1,"Posted company-currency invoice net.","صافي الفواتير المرحلة بعملة الشركة."),
            W("collected","Collected amount","المبلغ المحصل",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1,"Posted payment carrying amount.","القيمة الدفترية للدفعات المرحلة."),
            W("outstanding","Outstanding receivables","الذمم المدينة القائمة",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1,"Posted invoices less payments and credits.","الفواتير المرحلة ناقص الدفعات والإشعارات الدائنة."),
            W("contribution","Gross contribution","المساهمة الإجمالية",AnalyticsVisualizationType.Metric,AnalyticsValueFormat.Money,1,"Invoice net less credits and delivery COGS.","صافي الفواتير ناقص الإشعارات وكلفة التسليم."),
            W("funnel","Quote-to-cash funnel","مسار العرض إلى التحصيل",AnalyticsVisualizationType.Funnel,AnalyticsValueFormat.Number,2,"Cohort progression through linked documents.","تقدم مجموعة العروض عبر المستندات المرتبطة."),
            W("trends","Revenue and collection trends","اتجاهات الإيراد والتحصيل",AnalyticsVisualizationType.Bar,AnalyticsValueFormat.Money,2,"Period activity by the selected dimension.","نشاط الفترة حسب البعد المحدد."),
            W("customers","Customer performance","أداء العملاء",AnalyticsVisualizationType.Table,AnalyticsValueFormat.Money,2,"Revenue, collection, outstanding and overdue evidence.","الإيراد والتحصيل والرصيد والتأخر."),
            W("products","Product contribution","مساهمة المنتجات",AnalyticsVisualizationType.Table,AnalyticsValueFormat.Money,2,"Net revenue and delivery cost by product.","صافي الإيراد وكلفة التسليم حسب المنتج."),
            W("legacyComparison","Legacy and modern comparison","مقارنة القديم والحديث",AnalyticsVisualizationType.Comparison,AnalyticsValueFormat.Money,2,"Separate historical workflows without invented conversion.","مساران تاريخيان منفصلان دون تحويل مصطنع."),
            W("definitions","KPI definitions and sources","تعريفات ومصادر المؤشرات",AnalyticsVisualizationType.Table,AnalyticsValueFormat.Text,2,"Auditable calculation definitions.","تعريفات حساب قابلة للتدقيق.")
        ]);

    public async Task<AnalyticsDatasetDto> GetAsync(AnalyticsCenterQueryDto query)
    {
        var q = new SalesAnalyticsQueryDto { From = query.From!.Value, To = query.To!.Value,
            GroupBy = query.Dimension?.ToLowerInvariant() switch { "customer" => SalesAnalyticsGroupBy.Customer, "product" => SalesAnalyticsGroupBy.Product, _ => SalesAnalyticsGroupBy.Month } };
        if (query.Filters.TryGetValue("customerId", out var customer) && int.TryParse(customer, out var id)) q.CustomerId = id;
        var x = await sales.GetAsync(q);
        return new AnalyticsDatasetDto { ModuleKey = "sales", Title = Definition.Title, TitleArabic = Definition.TitleArabic, Widgets =
        [
            Metric("quotationValue",Definition,x.QuotationValue,$"{x.QuotationCount} quotations",$"{x.QuotationCount} عرض سعر"),
            Metric("acceptanceRate",Definition,x.QuotationAcceptanceRate), Metric("invoiceRevenue",Definition,x.PostedInvoiceRevenue,$"{x.ModernInvoiceCount} posted invoices",$"{x.ModernInvoiceCount} فاتورة مرحلة"),
            Metric("collected",Definition,x.CollectedAmount), Metric("outstanding",Definition,x.OutstandingReceivables,$"{x.OverdueInvoiceCount} overdue invoices",$"{x.OverdueInvoiceCount} فاتورة متأخرة"),
            Metric("contribution",Definition,x.GrossContribution,$"{x.GrossContributionPercent:0.##}% margin",$"هامش {x.GrossContributionPercent:0.##}%"),
            Points("funnel",Definition,x.Funnel.Select(v=>new AnalyticsPointDto(v.Label,v.Count,v.Controller)).ToList()),
            Points("trends",Definition,x.Trends.Select(v=>new AnalyticsPointDto(v.GroupLabel,v.GrossRevenue-v.Credits)).ToList()),
            Table("customers",Definition,["Customer","Quotations","Orders","Invoices","Revenue","Collected","Outstanding","Overdue"],x.Customers.Select(v=>new AnalyticsRowDto([v.CustomerCode+" — "+v.CustomerName,v.QuotationCount.ToString(),v.OrderCount.ToString(),v.InvoiceCount.ToString(),N(v.Revenue),N(v.Collected),N(v.Outstanding),v.OverdueInvoices.ToString()],"Customers",v.CustomerId)).ToList()),
            Table("products",Definition,["Product","Quantity","Net revenue","Delivery cost","Contribution","Margin %"],x.Products.Select(v=>new AnalyticsRowDto([v.ProductCode+" — "+v.ProductName,N(v.InvoicedStockQuantity),N(v.NetRevenue),N(v.DeliveryCost),N(v.GrossContribution),N(v.GrossContributionPercent)],"Products",v.ProductId)).ToList()),
            Table("legacyComparison",Definition,["Workflow","Documents","Value"],[new AnalyticsRowDto(["Legacy immediate sales",x.LegacySaleCount.ToString(),N(x.LegacySaleValue)]),new AnalyticsRowDto(["Modern posted invoices",x.ModernInvoiceCount.ToString(),N(x.ModernInvoiceValue)])]),
            Table("definitions",Definition,["KPI","Value","Source","Period","Explanation"],x.Kpis.Select(v=>new AnalyticsRowDto([v.Label,v.Value,v.Source,v.Period,v.Explanation])).ToList())
        ]};
    }

    private static AnalyticsWidgetDefinitionDto W(string key,string en,string ar,AnalyticsVisualizationType visual,AnalyticsValueFormat format,int span,string desc,string descAr)=>new(key,en,ar,visual,format,span,desc,descAr);
    private static AnalyticsWidgetDataDto Metric(string key,AnalyticsModuleDefinitionDto d,decimal value,string? secondary=null,string? secondaryArabic=null){var x=d.Widgets.Single(w=>w.Key==key);return new(){Key=key,Title=x.Title,TitleArabic=x.TitleArabic,Definition=x.Description,DefinitionArabic=x.DescriptionArabic,Visualization=x.DefaultVisualization,ValueFormat=x.ValueFormat,Value=value,SecondaryText=secondary,SecondaryTextArabic=secondaryArabic};}
    private static AnalyticsWidgetDataDto Points(string key,AnalyticsModuleDefinitionDto d,List<AnalyticsPointDto> points){var x=d.Widgets.Single(w=>w.Key==key);return new(){Key=key,Title=x.Title,TitleArabic=x.TitleArabic,Definition=x.Description,DefinitionArabic=x.DescriptionArabic,Visualization=x.DefaultVisualization,ValueFormat=x.ValueFormat,Points=points};}
    private static AnalyticsWidgetDataDto Table(string key,AnalyticsModuleDefinitionDto d,List<string> columns,List<AnalyticsRowDto> rows){var x=d.Widgets.Single(w=>w.Key==key);return new(){Key=key,Title=x.Title,TitleArabic=x.TitleArabic,Definition=x.Description,DefinitionArabic=x.DescriptionArabic,Visualization=x.DefaultVisualization,ValueFormat=x.ValueFormat,Columns=columns,Rows=rows};}
    private static string N(decimal x)=>x.ToString("0.##",CultureInfo.CurrentCulture);
}
