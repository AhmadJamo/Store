using System.Globalization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesAnalyticsService(
    ISalesQuotationRepository quotations, ISalesOrderRepository orders,
    ISalesDeliveryRepository deliveries, ICustomerInvoiceRepository invoices,
    ICustomerPaymentRepository payments, ICustomerCreditNoteRepository creditNotes,
    ISaleRepository legacySales, ICustomerRepository customers)
{
    public async Task<SalesAnalyticsDto> GetAsync(SalesAnalyticsQueryDto query)
    {
        Validate(query);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var allCustomers = await customers.GetAllAsync();
        var allQuotations = await quotations.GetAllAsync(null, null);
        var allOrders = await orders.GetAllAsync(null, null);
        var allDeliveries = await deliveries.GetAllAsync();
        var allInvoices = await invoices.GetAllAsync();
        var allPayments = await payments.GetAllAsync();
        var allCredits = await creditNotes.GetAllAsync();
        var allLegacy = await legacySales.GetAllAsync();

        bool Customer(int id) => !query.CustomerId.HasValue || id == query.CustomerId;
        var quoteCohort = allQuotations.Where(x => InPeriod(x.QuotationDate, query) && Customer(x.CustomerId)).ToList();
        var quoteIds = quoteCohort.Select(x => x.Id).ToHashSet();
        var cohortOrders = allOrders.Where(x => quoteIds.Contains(x.SourceQuotationId) && x.Status != SalesOrderStatus.Cancelled).ToList();
        var cohortOrderIds = cohortOrders.Select(x => x.Id).ToHashSet();
        var cohortDeliveries = allDeliveries.Where(x => cohortOrderIds.Contains(x.SalesOrderId) && x.Status == SalesDeliveryStatus.Posted).ToList();
        var cohortInvoices = allInvoices.Where(x => cohortOrderIds.Contains(x.SalesOrderId) && x.Status == CustomerInvoiceStatus.Posted).ToList();
        var cohortInvoiceIds = cohortInvoices.Select(x => x.Id).ToHashSet();
        var cohortPayments = allPayments.Where(x => x.Status == CustomerPaymentStatus.Posted && x.Lines.Any(l => cohortInvoiceIds.Contains(l.CustomerInvoiceId))).ToList();

        var periodOrders = allOrders.Where(x => InPeriod(x.OrderDate, query) && Customer(x.CustomerId) && x.Status != SalesOrderStatus.Cancelled).ToList();
        var ordersById = allOrders.ToDictionary(x => x.Id);
        var periodDeliveries = allDeliveries.Where(x => InPeriod(x.DeliveryDate, query) && x.Status == SalesDeliveryStatus.Posted &&
            ordersById.TryGetValue(x.SalesOrderId, out var order) && Customer(order.CustomerId)).ToList();
        var periodInvoices = allInvoices.Where(x => InPeriod(x.InvoiceDate, query) && Customer(x.CustomerId) && x.Status == CustomerInvoiceStatus.Posted).ToList();
        var periodInvoiceIds = periodInvoices.Select(x => x.Id).ToHashSet();
        var periodPayments = allPayments.Where(x => InPeriod(x.PaymentDate, query) && x.Status == CustomerPaymentStatus.Posted && Customer(x.CustomerId)).ToList();
        var periodCredits = allCredits.Where(x => InPeriod(x.CreditDate, query) && x.Status == CustomerCreditNoteStatus.Posted && Customer(x.CustomerId)).ToList();
        var allPostedInvoices = allInvoices.Where(x => x.Status == CustomerInvoiceStatus.Posted && Customer(x.CustomerId)).ToList();
        var postedIds = allPostedInvoices.Select(x => x.Id).ToArray();
        var paidByInvoice = await payments.GetReceivableCarryingAmountsAsync(postedIds);
        var creditedByInvoice = await creditNotes.GetPostedCompanyAmountsAsync(postedIds);

        var outstanding = allPostedInvoices.ToDictionary(x => x.Id, x => Math.Max(0,
            x.CompanyTotalAmount - paidByInvoice.GetValueOrDefault(x.Id) - creditedByInvoice.GetValueOrDefault(x.Id)));
        var revenue = Round(periodInvoices.Sum(x => x.CompanyNetAmount));
        var creditValue = Round(periodCredits.SelectMany(x => x.Lines).Sum(x => x.CompanyGrossAmount));
        var deliveryCost = Round(periodDeliveries.SelectMany(x => x.Lines).Sum(x => x.CostOfGoodsSold));
        var contribution = Round(revenue - periodCredits.SelectMany(x => x.Lines).Sum(x => x.CompanyNetAmount) - deliveryCost);
        var legacy = allLegacy.Where(x => InPeriod(DateOnly.FromDateTime(x.Date), query) &&
            (!query.CustomerId.HasValue || x.CustomerId == query.CustomerId)).ToList();
        var progressedQuotes = quoteCohort.Count(x => x.Status is SalesQuotationStatus.Sent or SalesQuotationStatus.Accepted or SalesQuotationStatus.Rejected or SalesQuotationStatus.Expired);
        var acceptedQuotes = quoteCohort.Count(x => x.Status == SalesQuotationStatus.Accepted);
        var periodText = $"{query.From:yyyy-MM-dd} – {query.To:yyyy-MM-dd}";

        return new SalesAnalyticsDto
        {
            Query = query,
            CustomerOptions = allCustomers.Where(x => x.IsActive).OrderBy(x => x.Name)
                .Select(x => new SalesAnalyticsOptionDto(x.Id, $"{x.CustomerCode} — {x.Name}")).ToList(),
            QuotationCount = quoteCohort.Count, QuotationValue = Round(quoteCohort.Sum(x => x.TotalAmount)),
            QuotationAcceptanceRate = Rate(acceptedQuotes, progressedQuotes),
            SalesOrderCount = periodOrders.Count, SalesOrderValue = Round(periodOrders.Sum(x => x.TotalAmount)),
            PostedInvoiceRevenue = revenue, CollectedAmount = Round(periodPayments.Sum(x => x.ReceivableCarryingAmount)),
            OutstandingReceivables = Round(outstanding.Values.Sum()),
            OverdueInvoiceCount = allPostedInvoices.Count(x => x.DueDate < today && outstanding[x.Id] > 0),
            CreditNoteValue = creditValue, DeliveryCost = deliveryCost, GrossContribution = contribution,
            GrossContributionPercent = Rate(contribution, revenue), LegacySaleCount = legacy.Count,
            LegacySaleValue = Round(legacy.Sum(x => x.TotalAmount)), ModernInvoiceCount = periodInvoices.Count,
            ModernInvoiceValue = Round(periodInvoices.Sum(x => x.CompanyTotalAmount)),
            Funnel = BuildFunnel(quoteCohort.Count, progressedQuotes, acceptedQuotes, cohortOrders.Count,
                cohortOrders.Count(x => x.Status == SalesOrderStatus.Confirmed), cohortDeliveries.Select(x => x.SalesOrderId).Distinct().Count(),
                cohortInvoices.Select(x => x.SalesOrderId).Distinct().Count(),
                cohortInvoices.Where(x => cohortPayments.SelectMany(p => p.Lines).Any(l => l.CustomerInvoiceId == x.Id)).Select(x => x.SalesOrderId).Distinct().Count()),
            Trends = BuildTrends(query.GroupBy, periodInvoices, periodCredits, periodPayments),
            Customers = BuildCustomers(allCustomers, quoteCohort, periodOrders, periodInvoices, periodPayments, outstanding, today),
            Products = BuildProducts(periodInvoices, periodCredits, periodDeliveries),
            Kpis =
            [
                new("Quotation acceptance rate", $"{Rate(acceptedQuotes, progressedQuotes).ToString("0.##", CultureInfo.CurrentCulture)}%", "Quotation cohort", periodText, "Accepted quotations divided by quotations that progressed beyond Draft; Cancelled quotations are excluded because cancellation may occur before or after sending."),
                new("Posted invoice net revenue", revenue.ToString("0.##", CultureInfo.CurrentCulture), "Posted customer invoices", periodText, "Company-currency net amount of invoices posted in the selected period before credit notes."),
                new("Collected amount", periodPayments.Sum(x => x.ReceivableCarryingAmount).ToString("0.##", CultureInfo.CurrentCulture), "Posted customer payments", periodText, "Receivable carrying amount allocated by posted payments dated in the selected period."),
                new("Gross contribution", contribution.ToString("0.##", CultureInfo.CurrentCulture), "Posted invoices, credit notes and deliveries", periodText, "Period posted invoice net revenue less posted credit-note net value and posted delivery COGS; timing differences may occur between delivery and invoicing.")
            ]
        };
    }

    private static List<SalesAnalyticsFunnelStageDto> BuildFunnel(params int[] counts)
    {
        var labels = new[] { "Quotations created", "Quotations progressed", "Quotations accepted", "Orders created", "Orders confirmed", "Orders delivered", "Orders invoiced", "Invoices collected" };
        var controllers = new[] { "SalesQuotations", "SalesQuotations", "SalesQuotations", "SalesOrders", "SalesOrders", "SalesDeliveries", "CustomerInvoices", "CustomerPayments" };
        var permissions = new[] { "SalesQuotations.View", "SalesQuotations.View", "SalesQuotations.View", "SalesOrders.View", "SalesOrders.View", "SalesDeliveries.View", "CustomerInvoices.View", "CustomerPayments.View" };
        var explanations = new[] { "Quotation-date cohort in the selected period.", "Cohort quotations that moved beyond Draft, excluding ambiguous cancellations.", "Accepted quotations in the cohort.", "Non-cancelled orders created from cohort quotations.", "Currently confirmed cohort orders.", "Cohort orders with at least one posted delivery.", "Cohort orders with at least one posted invoice.", "Distinct cohort invoices with a posted payment allocation." };
        return counts.Select((count, i) => new SalesAnalyticsFunnelStageDto(labels[i], count,
            i == 0 ? 100 : Rate(count, counts[i - 1]), controllers[i], permissions[i], explanations[i])).ToList();
    }

    private static List<SalesAnalyticsTrendDto> BuildTrends(SalesAnalyticsGroupBy groupBy,
        List<CustomerInvoice> invoices, List<CustomerCreditNote> credits, List<CustomerPayment> payments)
    {
        string InvoiceKey(CustomerInvoice x) => groupBy switch
        {
            SalesAnalyticsGroupBy.Customer => $"c:{x.CustomerId}|{x.CustomerName}",
            SalesAnalyticsGroupBy.Product => "all:products",
            _ => $"m:{x.InvoiceDate:yyyy-MM}"
        };
        string CreditKey(CustomerCreditNote x) => groupBy switch
        {
            SalesAnalyticsGroupBy.Customer => $"c:{x.CustomerId}|{invoices.FirstOrDefault(i => i.Id == x.CustomerInvoiceId)?.CustomerName ?? x.CustomerId.ToString(CultureInfo.InvariantCulture)}",
            SalesAnalyticsGroupBy.Product => "all:products",
            _ => $"m:{x.CreditDate:yyyy-MM}"
        };
        string PaymentKey(CustomerPayment x) => groupBy switch
        {
            SalesAnalyticsGroupBy.Customer => $"c:{x.CustomerId}|{invoices.FirstOrDefault(i => i.CustomerId == x.CustomerId)?.CustomerName ?? x.CustomerId.ToString(CultureInfo.InvariantCulture)}",
            SalesAnalyticsGroupBy.Product => "all:products",
            _ => $"m:{x.PaymentDate:yyyy-MM}"
        };
        if (groupBy == SalesAnalyticsGroupBy.Product)
            return invoices.SelectMany(x => x.Lines).GroupBy(x => $"p:{x.ProductId}|{x.ProductName}").Select(g =>
                new SalesAnalyticsTrendDto(g.Key, Label(g.Key), g.Select(x => x.CustomerInvoiceId).Distinct().Count(),
                    Round(g.Sum(x => x.CompanyNetAmount)), Round(g.Sum(x => x.CompanyTaxAmount)),
                    Round(g.Sum(x => x.CompanyTotalAmount)), Round(credits.SelectMany(x => x.Lines).Where(x => x.ProductId == g.First().ProductId).Sum(x => x.CompanyGrossAmount)), 0))
                .OrderByDescending(x => x.GrossRevenue).ToList();
        var invoiceGroups = invoices.GroupBy(InvoiceKey).ToDictionary(x => x.Key, x => x.ToList());
        var creditGroups = credits.GroupBy(CreditKey).ToDictionary(x => x.Key, x => x.ToList());
        var paymentGroups = payments.GroupBy(PaymentKey).ToDictionary(x => x.Key, x => x.ToList());
        return invoiceGroups.Keys.Union(creditGroups.Keys).Union(paymentGroups.Keys).Select(key =>
        {
            var inv = invoiceGroups.GetValueOrDefault(key) ?? [];
            var cr = creditGroups.GetValueOrDefault(key) ?? [];
            var pay = paymentGroups.GetValueOrDefault(key) ?? [];
            return new SalesAnalyticsTrendDto(key, Label(key), inv.Count, Round(inv.Sum(x => x.CompanyNetAmount)),
                Round(inv.Sum(x => x.CompanyTaxAmount)), Round(inv.Sum(x => x.CompanyTotalAmount)),
                Round(cr.SelectMany(x => x.Lines).Sum(x => x.CompanyGrossAmount)), Round(pay.Sum(x => x.ReceivableCarryingAmount)));
        }).OrderBy(x => x.GroupKey).ToList();
    }

    private static List<SalesCustomerPerformanceDto> BuildCustomers(List<Customer> customers,
        List<SalesQuotation> quotations, List<SalesOrder> orders, List<CustomerInvoice> invoices,
        List<CustomerPayment> payments, Dictionary<int, decimal> outstanding, DateOnly today) => customers.Select(customer =>
        {
            var customerInvoices = invoices.Where(x => x.CustomerId == customer.Id).ToList();
            return new SalesCustomerPerformanceDto(customer.Id, customer.CustomerCode, customer.Name,
                quotations.Count(x => x.CustomerId == customer.Id), orders.Count(x => x.CustomerId == customer.Id), customerInvoices.Count,
                Round(customerInvoices.Sum(x => x.CompanyNetAmount)), Round(payments.Where(x => x.CustomerId == customer.Id).Sum(x => x.ReceivableCarryingAmount)),
                Round(customerInvoices.Sum(x => outstanding.GetValueOrDefault(x.Id))), customerInvoices.Count(x => x.DueDate < today && outstanding.GetValueOrDefault(x.Id) > 0));
        }).Where(x => x.QuotationCount > 0 || x.OrderCount > 0 || x.InvoiceCount > 0 || x.Collected > 0)
        .OrderByDescending(x => x.Revenue).ThenBy(x => x.CustomerName).ToList();

    private static List<SalesProductPerformanceDto> BuildProducts(List<CustomerInvoice> invoices,
        List<CustomerCreditNote> credits, List<SalesDelivery> deliveries)
    {
        var creditLines = credits.SelectMany(x => x.Lines).ToList();
        var deliveryLines = deliveries.SelectMany(x => x.Lines).ToList();
        return invoices.SelectMany(x => x.Lines).GroupBy(x => new { x.ProductId, x.ProductCode, x.ProductName }).Select(group =>
        {
            var net = Round(group.Sum(x => x.CompanyNetAmount) - creditLines.Where(x => x.ProductId == group.Key.ProductId).Sum(x => x.CompanyNetAmount));
            var cost = Round(deliveryLines.Where(x => x.ProductId == group.Key.ProductId).Sum(x => x.CostOfGoodsSold));
            var margin = Round(net - cost);
            return new SalesProductPerformanceDto(group.Key.ProductId, group.Key.ProductCode, group.Key.ProductName,
                group.Sum(x => x.StockQuantity), net, cost, margin, Rate(margin, net));
        }).OrderByDescending(x => x.NetRevenue).ThenBy(x => x.ProductName).ToList();
    }

    private static string Label(string key) => key.Contains('|') ? key[(key.IndexOf('|') + 1)..] : key.StartsWith("m:") ? key[2..] : key;
    private static bool InPeriod(DateOnly date, SalesAnalyticsQueryDto query) => date >= query.From && date <= query.To;
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    private static decimal Rate(decimal value, decimal total) => total == 0 ? 0 : Round(value / total * 100);
    private static void Validate(SalesAnalyticsQueryDto query)
    {
        if (query.From > query.To) throw new ArgumentException("Sales analytics start date cannot be after the end date.");
        if (query.To.DayNumber - query.From.DayNumber > 731) throw new ArgumentException("Sales analytics period cannot exceed two years.");
        if (query.CustomerId is <= 0) throw new ArgumentException("Sales analytics customer is invalid.");
        if (!Enum.IsDefined(query.GroupBy)) throw new ArgumentException("Sales analytics grouping is invalid.");
    }
}
