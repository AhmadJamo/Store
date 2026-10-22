using System.Globalization;
using System.Net;
using System.Text;
using MiniStore.Application.Dtos.Documents;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Documents;

public sealed class PlaywrightDocumentPdfRenderer(IHtmlToPdfEngine engine) : IDocumentPdfRenderer
{
    public async Task<byte[]> RenderSalesQuotationAsync(SalesQuotationPdfRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var language = ResolveLanguage(request.Snapshot.Design, request.LanguageOverride);
        var d = request.Snapshot.Design;
        var footer = d.ShowPageNumbers
            ? "<div style='width:100%;font-size:8px;color:#64748b;text-align:center;font-family:Arial,sans-serif'><span class='pageNumber'></span> / <span class='totalPages'></span></div>"
            : null;
        var page = new DocumentPdfPageOptions(
            d.PageSize == DocumentPageSize.Custom ? null : d.PageSize == DocumentPageSize.Letter ? "Letter" : "A4",
            d.Orientation == DocumentPageOrientation.Landscape, d.CustomPageWidthMm, d.CustomPageHeightMm,
            d.MarginTopMm, d.MarginEndMm, d.MarginBottomMm + (d.ShowPageNumbers ? 7 : 0), d.MarginStartMm);
        return await engine.RenderAsync(new DocumentPdfRenderRequest(
            BuildHtml(request, language), page, d.ShowPageNumbers, FooterHtml: footer), cancellationToken);
    }

    private static DocumentLanguage ResolveLanguage(SalesQuotationDocumentDesignDto design, DocumentLanguage? requested) =>
        requested.HasValue && design.AllowLanguageOverride ? requested.Value : design.DefaultLanguage;

    private static string BuildHtml(SalesQuotationPdfRequest request, DocumentLanguage language)
    {
        var s = request.Snapshot;
        var d = s.Design;
        var rtl = language != DocumentLanguage.English;
        var bilingual = language == DocumentLanguage.Bilingual;
        var body = new StringBuilder();
        foreach (var section in d.Sections.Where(x => x.IsVisible).OrderBy(x => x.Sequence))
        {
            var cls = section.PageBreakBefore ? "section page-break" : "section";
            if (section.SectionType == DocumentDesignSectionType.Cover)
                body.Append(Cover(section, s, request, language));
            else
                body.Append($"<section class='{cls}'>{Section(section, s, request, language)}</section>");
        }

        var font = d.FontFamily switch
        {
            DocumentFontFamily.Arial => "Arial, sans-serif",
            DocumentFontFamily.Tahoma => "Tahoma, Arial, sans-serif",
            DocumentFontFamily.Cairo => "Cairo, Tahoma, Arial, sans-serif",
            _ => "'Noto Sans Arabic', 'Noto Sans', Tahoma, Arial, sans-serif"
        };
        var watermark = d.ShowStatusWatermark ? Pick(d.WatermarkText, d.WatermarkTextArabic, language) ?? StatusLabel(s.Quotation.Status, language) : null;
        return $$"""
<!doctype html><html lang="{{(rtl ? "ar" : "en")}}" dir="{{(rtl ? "rtl" : "ltr")}}"><head><meta charset="utf-8" />
<style>
*{box-sizing:border-box} html,body{margin:0;padding:0;color:{{d.TextColor}};font-family:{{font}};font-size:11px;line-height:1.55}
body{position:relative} .watermark{position:fixed;inset:42% 0 auto;z-index:-1;text-align:center;transform:rotate(-30deg);font-size:72px;font-weight:800;color:{{d.PrimaryColor}};opacity:{{d.WatermarkOpacity.ToString(CultureInfo.InvariantCulture)}}}
.header{display:flex;align-items:center;justify-content:space-between;gap:24px;border-bottom:3px solid {{d.PrimaryColor}};padding-bottom:12px;margin-bottom:18px}.identity{flex:1}.identity.end{text-align:end}.identity.center{text-align:center}.logo{max-height:{{d.LogoMaximumHeightPx}}px;max-width:220px;object-fit:contain}.org-name{font-size:21px;font-weight:800;color:{{d.PrimaryColor}}}.muted{color:#64748b}.section{margin:0 0 16px}.page-break{break-before:page}.cover{height:90vh;display:flex;flex-direction:column;align-items:center;justify-content:center;text-align:center;break-after:page}.cover h1{font-size:34px;color:{{d.PrimaryColor}};margin:12px}.section-title{font-size:15px;font-weight:750;color:{{d.PrimaryColor}};border-bottom:1px solid {{d.AccentColor}};padding-bottom:5px;margin:0 0 9px}.grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:6px 22px}.label{font-weight:700}.custom-row{display:flex;gap:8px}.pre{white-space:pre-wrap}.bilingual{display:grid;grid-template-columns:1fr 1fr;gap:18px}.bilingual>[dir=rtl]{text-align:right}.bilingual>[dir=ltr]{text-align:left}table{width:100%;border-collapse:collapse}th{background:{{d.PrimaryColor}};color:{{d.TableHeaderTextColor}};padding:8px 6px;text-align:start}td{border-bottom:1px solid #dbe3ea;padding:7px 6px;vertical-align:top}.num{text-align:end;white-space:nowrap}.totals{margin-inline-start:auto;width:46%;min-width:260px}.totals>div{display:flex;justify-content:space-between;padding:5px 8px}.grand{background:{{d.AccentColor}};font-size:14px;font-weight:800;border-top:2px solid {{d.PrimaryColor}}}.acceptance{display:grid;grid-template-columns:1fr 1fr;gap:50px;margin-top:36px}.signature{border-top:1px solid #64748b;padding-top:5px;text-align:center}.footer{text-align:center;border-top:1px solid {{d.AccentColor}};padding-top:8px;color:#64748b}.ltr{direction:ltr;unicode-bidi:isolate}.no-break{break-inside:avoid}@media print{.section,table,.no-break{break-inside:avoid}thead{display:table-header-group} }
</style></head><body>{{(watermark is null ? "" : $"<div class='watermark'>{E(watermark)}</div>")}}{{body}}</body></html>
""";
    }

    private static string Section(SalesQuotationDocumentSectionDto section, SalesQuotationDocumentSnapshotDto s,
        SalesQuotationPdfRequest request, DocumentLanguage language)
    {
        var title = Pick(section.Title, section.TitleArabic, language) ?? section.Title;
        var inner = section.SectionType switch
        {
            DocumentDesignSectionType.OrganizationIdentity => Header(s, request, language),
            DocumentDesignSectionType.DocumentDetails => DocumentDetails(s, language),
            DocumentDesignSectionType.PartyDetails => Party(s, language),
            DocumentDesignSectionType.Introduction => TextBlock(s.Quotation.Introduction, section, language),
            DocumentDesignSectionType.LineItems => Lines(s, language),
            DocumentDesignSectionType.Totals => Totals(s, language),
            DocumentDesignSectionType.PaymentDetails => Payment(s, language),
            DocumentDesignSectionType.TermsAndConditions => TextBlock(s.Quotation.TermsAndConditions, section, language),
            DocumentDesignSectionType.Notes => TextBlock(s.Quotation.Notes, section, language),
            DocumentDesignSectionType.Acceptance => Acceptance(language),
            DocumentDesignSectionType.Footer => Footer(s, section, language),
            DocumentDesignSectionType.Custom => TextBlock(null, section, language),
            _ => string.Empty
        };
        return string.IsNullOrWhiteSpace(inner) ? string.Empty : $"<h2 class='section-title'>{E(title)}</h2>{inner}";
    }

    private static string Header(SalesQuotationDocumentSnapshotDto s, SalesQuotationPdfRequest request, DocumentLanguage language)
    {
        var d = s.Design; var o = s.Organization;
        var logo = d.UseOrganizationLogo && request.LogoContent is { Length: > 0 } && request.LogoContentType is not null
            ? $"<img class='logo' alt='' src='data:{E(request.LogoContentType)};base64,{Convert.ToBase64String(request.LogoContent)}' />" : string.Empty;
        var identity = new StringBuilder($"<div class='org-name'>{E(OrganizationName(o, language))}</div>");
        if (d.ShowLegalName && o.LegalName is not null) identity.Append($"<div>{E(Pick(o.LegalName, o.LegalNameArabic, language))}</div>");
        if (d.ShowRegistrationNumber && o.RegistrationNumber is not null) identity.Append(Row(T("Registration number", "رقم التسجيل", language), o.RegistrationNumber));
        if (d.ShowTaxNumber && o.TaxNumber is not null) identity.Append(Row(T("Tax number", "الرقم الضريبي", language), o.TaxNumber));
        if (d.ShowAddress) identity.Append($"<div>{E(Address(o, language))}</div>");
        if (d.ShowContactDetails) identity.Append($"<div class='ltr'>{E(string.Join(" · ", new[] { o.Phone, o.SecondaryPhone, o.Email, o.Website }.Where(x => !string.IsNullOrWhiteSpace(x))))}</div>");
        if (d.ShowCustomIdentityFields)
            foreach (var field in o.CustomFields.Where(x => x.Scope.HasFlag(OrganizationIdentityDocumentScope.Sales)).OrderBy(x => x.Sequence))
                identity.Append(Row(Pick(field.Label, field.LabelArabic, language)!, Pick(field.Value, field.ValueArabic, language)!));
        var placement = d.OrganizationNamePlacement == DocumentElementPlacement.Center ? "center" : d.OrganizationNamePlacement == DocumentElementPlacement.End ? "end" : string.Empty;
        var headerText = Pick(d.HeaderText, d.HeaderTextArabic, language);
        if (!string.IsNullOrWhiteSpace(headerText)) identity.Append($"<div class='pre muted'>{E(headerText)}</div>");
        var block = d.OrganizationNamePlacement == DocumentElementPlacement.Hidden
            ? string.Empty : $"<div class='identity {placement}'>{identity}</div>";
        if (d.LogoPlacement == DocumentElementPlacement.Hidden) logo = string.Empty;
        return d.LogoPlacement == DocumentElementPlacement.End ? $"<div class='header'>{block}{logo}</div>" : $"<div class='header'>{logo}{block}</div>";
    }

    private static string DocumentDetails(SalesQuotationDocumentSnapshotDto s, DocumentLanguage l) => $"<div class='grid'>" +
        Row(T("Quotation number", "رقم عرض السعر", l), s.Quotation.Number) +
        Row(T("Status", "الحالة", l), StatusLabel(s.Quotation.Status, l)) +
        Row(T("Quotation date", "تاريخ العرض", l), Date(s.Quotation.QuotationDate, l)) +
        Row(T("Valid until", "صالح حتى", l), Date(s.Quotation.ValidUntil, l)) + "</div>";

    private static string Party(SalesQuotationDocumentSnapshotDto s, DocumentLanguage l) => $"<div class='grid'>" +
        Row(T("Customer", "العميل", l), $"{s.Quotation.CustomerCode} — {s.Quotation.CustomerName}") +
        Row(T("Legal name", "الاسم القانوني", l), s.Quotation.CustomerLegalName) +
        Row(T("Tax number", "الرقم الضريبي", l), s.Quotation.CustomerTaxNumber) +
        Row(T("Address", "العنوان", l), s.Quotation.CustomerAddress) + "</div>";

    private static string Lines(SalesQuotationDocumentSnapshotDto s, DocumentLanguage l)
    {
        var d = s.Design; var b = new StringBuilder("<table><thead><tr>");
        if (d.ShowProductCode) b.Append($"<th>{E(T("Code", "الرمز", l))}</th>");
        b.Append($"<th>{E(T("Item", "الصنف", l))}</th><th class='num'>{E(T("Quantity", "الكمية", l))}</th>");
        if (d.ShowUnit) b.Append($"<th>{E(T("Unit", "الوحدة", l))}</th>");
        b.Append($"<th class='num'>{E(T("Unit price", "سعر الوحدة", l))}</th>");
        if (d.ShowTaxDetails) b.Append($"<th class='num'>{E(T("Tax", "الضريبة", l))}</th>");
        b.Append($"<th class='num'>{E(T("Total", "الإجمالي", l))}</th></tr></thead><tbody>");
        foreach (var x in s.Quotation.Lines)
        {
            b.Append("<tr>"); if (d.ShowProductCode) b.Append($"<td>{E(x.ProductCode)}</td>");
            b.Append($"<td><strong>{E(x.ProductName)}</strong>{(d.ShowLineNotes && x.Notes is not null ? $"<div class='muted'>{E(x.Notes)}</div>" : "")}</td><td class='num'>{N(x.Quantity)}</td>");
            if (d.ShowUnit) b.Append($"<td>{E(x.UnitName)}</td>");
            b.Append($"<td class='num'>{Money(x.UnitPrice, s.Quotation.CurrencyCode)}</td>");
            if (d.ShowTaxDetails) b.Append($"<td class='num'>{Money(x.TaxAmount, s.Quotation.CurrencyCode)}{(x.TaxRatePercent.HasValue ? $"<div class='muted'>{N(x.TaxRatePercent.Value)}%</div>" : "")}</td>");
            b.Append($"<td class='num'><strong>{Money(x.TotalAmount, s.Quotation.CurrencyCode)}</strong></td></tr>");
        }
        return b.Append("</tbody></table>").ToString();
    }

    private static string Totals(SalesQuotationDocumentSnapshotDto s, DocumentLanguage l)
    {
        var d = s.Design; var b = new StringBuilder("<div class='totals no-break'>");
        if (d.ShowNetTotal) b.Append(TotalRow(T("Net amount", "المبلغ الصافي", l), s.Quotation.NetAmount, s.Quotation.CurrencyCode));
        if (d.ShowTaxTotal) b.Append(TotalRow(T("Tax amount", "مبلغ الضريبة", l), s.Quotation.TaxAmount, s.Quotation.CurrencyCode));
        b.Append($"<div class='grand'><span>{E(T("Grand total", "الإجمالي النهائي", l))}</span><span>{Money(s.Quotation.TotalAmount, s.Quotation.CurrencyCode)}</span></div></div>");
        return b.ToString();
    }

    private static string Payment(SalesQuotationDocumentSnapshotDto s, DocumentLanguage l) => $"<div class='grid'>" +
        Row(T("Payment terms", "شروط الدفع", l), $"{s.Quotation.PaymentTermDays} {T("days", "يوم", l)}") +
        Row(T("Currency", "العملة", l), s.Quotation.CurrencyCode) +
        Row(T("Exchange rate", "سعر الصرف", l), N(s.Quotation.ExchangeRate)) + "</div>";

    private static string TextBlock(string? value, SalesQuotationDocumentSectionDto section, DocumentLanguage l)
    { var content = value ?? Pick(section.Content, section.ContentArabic, l); return string.IsNullOrWhiteSpace(content) ? string.Empty : $"<div class='pre'>{E(content)}</div>"; }

    private static string Acceptance(DocumentLanguage l) => $"<div class='acceptance'><div class='signature'>{E(T("Customer name and signature", "اسم وتوقيع العميل", l))}</div><div class='signature'>{E(T("Authorized signature", "التوقيع المعتمد", l))}</div></div>";

    private static string Footer(SalesQuotationDocumentSnapshotDto s, SalesQuotationDocumentSectionDto section, DocumentLanguage l)
    {
        var text = Pick(s.Design.FooterText, s.Design.FooterTextArabic, l) ?? Pick(section.Content, section.ContentArabic, l) ?? s.Quotation.FooterNotes;
        return string.IsNullOrWhiteSpace(text) ? string.Empty : $"<div class='footer pre'>{E(text)}</div>";
    }

    private static string Cover(SalesQuotationDocumentSectionDto section, SalesQuotationDocumentSnapshotDto s, SalesQuotationPdfRequest request, DocumentLanguage l)
    {
        var logo = s.Design.UseOrganizationLogo && request.LogoContent is { Length: > 0 } && request.LogoContentType is not null
            ? $"<img class='logo' alt='' src='data:{E(request.LogoContentType)};base64,{Convert.ToBase64String(request.LogoContent)}' />" : string.Empty;
        var title = Pick(s.Design.CoverTitle, s.Design.CoverTitleArabic, l) ?? Pick(section.Title, section.TitleArabic, l) ?? T("Sales quotation", "عرض سعر", l);
        var subtitle = Pick(s.Design.CoverSubtitle, s.Design.CoverSubtitleArabic, l) ?? Pick(section.Content, section.ContentArabic, l);
        return $"<section class='cover'>{logo}<div class='org-name'>{E(OrganizationName(s.Organization, l))}</div><h1>{E(title)}</h1><h2>{E(s.Quotation.Number)}</h2>{(subtitle is null ? "" : $"<div class='pre'>{E(subtitle)}</div>")}</section>";
    }

    private static string OrganizationName(SalesQuotationDocumentOrganizationDto o, DocumentLanguage l)
    {
        var trade = Pick(o.TradeName, o.TradeNameArabic, l) ?? o.TradeName;
        var legal = Pick(o.LegalName, o.LegalNameArabic, l);
        return o.DocumentNameStyle switch { OrganizationDocumentNameStyle.LegalName => legal ?? trade, OrganizationDocumentNameStyle.TradeAndLegalNames when legal is not null => $"{trade} · {legal}", _ => trade };
    }
    private static string Address(SalesQuotationDocumentOrganizationDto o, DocumentLanguage l) => string.Join(", ", new[] { Pick(o.Address, o.AddressArabic, l), Pick(o.City, o.CityArabic, l), o.StateOrProvince, o.PostalCode, o.CountryCode }.Where(x => !string.IsNullOrWhiteSpace(x)));
    private static string? Pick(string? en, string? ar, DocumentLanguage l) => l switch { DocumentLanguage.Arabic => ar ?? en, DocumentLanguage.Bilingual when !string.IsNullOrWhiteSpace(en) && !string.IsNullOrWhiteSpace(ar) => $"{ar} | {en}", _ => en ?? ar };
    private static string T(string en, string ar, DocumentLanguage l) => l switch { DocumentLanguage.Arabic => ar, DocumentLanguage.Bilingual => $"{ar} | {en}", _ => en };
    private static string StatusLabel(SalesQuotationStatus x, DocumentLanguage l) => T(x.ToString(), x switch { SalesQuotationStatus.Draft => "مسودة", SalesQuotationStatus.Sent => "مرسل", SalesQuotationStatus.Accepted => "مقبول", SalesQuotationStatus.Rejected => "مرفوض", SalesQuotationStatus.Expired => "منتهي", _ => "ملغي" }, l);
    private static string Date(DateOnly x, DocumentLanguage l) => x.ToString(l == DocumentLanguage.English ? "dd MMM yyyy" : "yyyy/MM/dd", CultureInfo.InvariantCulture);
    private static string N(decimal x) => x.ToString("0.######", CultureInfo.InvariantCulture);
    private static string Money(decimal x, string currency) => $"{x.ToString("N2", CultureInfo.InvariantCulture)} {E(currency)}";
    private static string Row(string label, string? value) => string.IsNullOrWhiteSpace(value) ? string.Empty : $"<div><span class='label'>{E(label)}:</span> {E(value)}</div>";
    private static string TotalRow(string label, decimal value, string currency) => $"<div><span>{E(label)}</span><span>{Money(value, currency)}</span></div>";
    private static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty).Replace("\r\n", "\n").Replace("\n", "<br />");
}
