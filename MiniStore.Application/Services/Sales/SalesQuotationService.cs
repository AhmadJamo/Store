using MiniStore.Application.Dtos.Sales;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesQuotationService(ISalesQuotationRepository quotations, ICustomerRepository customers,
    IProductRepository products, IMeasurementUnitRepository units, SalesPriceListService pricing,
    ISalesOrderRepository orders, ISalesQuotationTemplateRepository templates, IDocumentDesignRepository designs, DocumentNumberService numbers,
    IGeneralSettingsRepository generalSettings, IDocumentPdfRenderer pdfRenderer,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    private static readonly JsonSerializerOptions SnapshotJsonOptions = new(JsonSerializerDefaults.Web);
    public async Task<List<SalesQuotationSummaryDto>> GetAllAsync(SalesQuotationStatus? status, string? search) =>
        (await quotations.GetAllAsync(status, search)).Select(x => new SalesQuotationSummaryDto(x.Id, x.QuotationNumber,
            x.CustomerCode, x.CustomerName, x.QuotationDate, x.ValidUntil, x.CurrencyCode, x.TotalAmount, x.Status)).ToList();

    public async Task<SalesQuotationDetailsDto?> GetAsync(int id)
    { var quotation = await quotations.GetByIdAsync(id); if(quotation is null)return null;var order=await orders.GetBySourceQuotationIdAsync(id);return Map(quotation,order?.Id); }

    public async Task<SalesQuotationCreatePageDto> GetCreatePageAsync(CreateSalesQuotationDto? form = null, int? templateId = null)
    {
        SalesQuotationTemplate? appliedTemplate = null;
        var requestedTemplateId = form?.SalesQuotationTemplateId ?? templateId;
        if (requestedTemplateId > 0)
            appliedTemplate = await templates.GetByIdAsync(requestedTemplateId.Value, false) ?? throw new InvalidOperationException("Selected quotation template was not found.");
        else if (form is null && templateId is null)
            appliedTemplate = await templates.GetDefaultAsync();
        if (appliedTemplate is not null && appliedTemplate.Status != SalesQuotationTemplateStatus.Active)
            throw new InvalidOperationException("Selected quotation template is not active.");
        DocumentDesign? selectedDesign = null;
        var requestedDesignId = form?.DocumentDesignId ?? appliedTemplate?.DocumentDesignId;
        if (requestedDesignId.HasValue)
            selectedDesign = await designs.GetByIdAsync(requestedDesignId.Value, false)
                ?? throw new InvalidOperationException("Selected document design was not found.");
        else if (form is null)
            selectedDesign = await designs.GetDefaultAsync(DocumentDesignType.SalesQuotation);
        if (selectedDesign is not null && (selectedDesign.DocumentType != DocumentDesignType.SalesQuotation ||
            selectedDesign.Status != DocumentDesignStatus.Active && appliedTemplate?.DocumentDesignId != selectedDesign.Id))
            throw new InvalidOperationException("Selected document design is not active for sales quotations.");
        if (form is null && appliedTemplate is not null)
        {
            form = new CreateSalesQuotationDto
            {
                SalesQuotationTemplateId = appliedTemplate.Id, SalesQuotationTemplateVersion = appliedTemplate.Version,
                DocumentDesignId = selectedDesign?.Id,
                PriceListId = appliedTemplate.SalesPriceListId, Channel = appliedTemplate.Channel,
                ValidUntil = DateOnly.FromDateTime(DateTime.Today.AddDays(appliedTemplate.ValidityDays)),
                Lines = appliedTemplate.Items.OrderBy(x => x.Sequence).Select(MapTemplateLine).ToList()
            };
        }
        else if (form is not null && appliedTemplate is not null)
        {
            form.SalesQuotationTemplateId = appliedTemplate.Id; form.SalesQuotationTemplateVersion = appliedTemplate.Version;
            if (selectedDesign is not null) form.DocumentDesignId = selectedDesign.Id;
            HydrateTemplateLines(form, appliedTemplate);
        }
        else if (form is null && selectedDesign is not null)
            form = new CreateSalesQuotationDto { DocumentDesignId = selectedDesign.Id };
        var pricingPage = await pricing.GetPageAsync();
        var customerRows = await customers.GetAllAsync();
        var addresses = new List<SalesQuotationAddressOptionDto>();
        foreach (var customer in customerRows)
            addresses.AddRange((await customers.GetAddressesAsync(customer.Id)).Select(x => new SalesQuotationAddressOptionDto(x.Id, customer.Id, FormatAddress(x))));
        var productRows = (await products.GetAllAsync(null)).Where(x => x.IsActive && x.IsSellableInSales).OrderBy(x => x.Name).ToList();
        return new SalesQuotationCreatePageDto
        {
            Form = form ?? new(),
            Customers = customerRows.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.CustomerCode} — {x.Name}")).ToList(),
            Addresses = addresses,
            Products = productRows.Select(x => new SalesQuotationProductOptionDto(x.Id, $"{x.ProductCode} — {x.Name}", $"{x.ProductCode} {x.Name} {x.Barcode}".Trim())).ToList(),
            Units = pricingPage.Units.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Name} ({x.Symbol})")).ToList(),
            PriceLists = pricingPage.PriceLists.Where(x => x.IsActive).Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Code} — {x.Name}")).ToList(),
            TaxRates = pricingPage.TaxRates.Select(x => new SalesQuotationOptionDto(x.Id, $"{x.Name} ({x.Rate:0.####}%)")).ToList(),
            Templates = (await templates.GetAllAsync(SalesQuotationTemplateStatus.Active, null)).Select(x =>
                new SalesQuotationOptionDto(x.Id, $"{x.Code} v{x.Version} — {x.Name}{(x.IsDefault ? " ★" : "")}")).ToList(),
            Designs = (await designs.GetAllAsync(DocumentDesignStatus.Active, DocumentDesignType.SalesQuotation, null)).Select(x =>
                new SalesQuotationOptionDto(x.Id, $"{x.Code} v{x.Version} — {x.Name}{(x.IsDefault ? " ★" : "")}")).ToList(),
            AppliedTemplate = appliedTemplate is null ? null : new SalesQuotationAppliedTemplateDto(appliedTemplate.Id,
                appliedTemplate.Code, appliedTemplate.Version, appliedTemplate.Name, appliedTemplate.Description,
                appliedTemplate.Category, appliedTemplate.Tags, appliedTemplate.AllowAdditionalLines,
                appliedTemplate.Introduction, appliedTemplate.TermsAndConditions, appliedTemplate.FooterNotes)
        };
    }

    public async Task<int> CreateAsync(CreateSalesQuotationDto dto)
    {
        if (!Guid.TryParse(dto.RequestKey, out _)) throw new ArgumentException("Sales quotation request key is invalid.");
        var existing = await quotations.GetByRequestKeyAsync(dto.RequestKey);
        if (existing is not null) return existing.Id;
        if (dto.Lines.Count == 0) throw new ArgumentException("A sales quotation must contain at least one line.");
        SalesQuotation? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            existing = await quotations.GetByRequestKeyAsync(dto.RequestKey);
            if (existing is not null) { created = existing; return; }
            var customer = await customers.GetByIdAsync(dto.CustomerId) ?? throw new InvalidOperationException("Selected quotation customer was not found.");
            if (!customer.IsActive) throw new InvalidOperationException("Selected quotation customer is archived.");
            CustomerAddress? address = null;
            if (dto.CustomerAddressId.HasValue)
            {
                address = await customers.GetAddressAsync(customer.Id, dto.CustomerAddressId.Value) ?? throw new InvalidOperationException("Selected quotation address was not found for this customer.");
                if (!address.IsActive) throw new InvalidOperationException("Selected quotation address is archived.");
            }
            if (dto.ValidUntil < dto.QuotationDate) throw new ArgumentException("Sales quotation expiry cannot be before its date.");
            SalesQuotationTemplate? template = null;
            var effectiveLines = dto.Lines;
            var priceListId = dto.PriceListId; var channel = dto.Channel;
            if (dto.SalesQuotationTemplateId.HasValue)
            {
                template = await templates.GetByIdAsync(dto.SalesQuotationTemplateId.Value, false) ?? throw new InvalidOperationException("Selected quotation template was not found.");
                if (template.Status != SalesQuotationTemplateStatus.Active) throw new InvalidOperationException("Selected quotation template is not active.");
                if (dto.SalesQuotationTemplateVersion != template.Version) throw new InvalidOperationException("Quotation template changed. Apply it again before creating the quotation.");
                effectiveLines = ValidateAndResolveTemplateLines(dto.Lines, template);
                priceListId = template.SalesPriceListId; channel = template.Channel;
            }
            else if (dto.Lines.Any(x => x.SalesQuotationTemplateItemId.HasValue))
                throw new InvalidOperationException("Template items cannot be submitted without an active quotation template.");
            var designId = dto.DocumentDesignId ?? template?.DocumentDesignId;
            DocumentDesign? design = null;
            if (designId.HasValue)
            {
                design = await designs.GetByIdAsync(designId.Value, false)
                    ?? throw new InvalidOperationException("Selected document design was not found.");
                if (design.DocumentType != DocumentDesignType.SalesQuotation ||
                    design.Status != DocumentDesignStatus.Active && template?.DocumentDesignId != design.Id)
                    throw new InvalidOperationException("Selected document design is not active for sales quotations.");
            }
            if (effectiveLines.Count == 0) throw new ArgumentException("Select at least one quotation product.");
            var evaluated = new List<(CreateSalesQuotationLineDto Input, Product Product, MeasurementUnit Unit, decimal Factor, SalesPriceEvaluationDto Price, SalesQuotationTemplateItem? TemplateItem)>();
            foreach (var line in effectiveLines)
            {
                var templateItem = template?.Items.SingleOrDefault(x => x.Id == line.SalesQuotationTemplateItemId);
                var productId = templateItem?.ProductId ?? line.ProductId; var unitId = templateItem?.MeasurementUnitId ?? line.MeasurementUnitId;
                var taxRateId = templateItem?.TaxRateId ?? line.TaxRateId;
                var product = await products.GetByIdAsync(productId) ?? throw new InvalidOperationException("Selected quotation product was not found.");
                var unit = await units.GetByIdAsync(unitId) ?? throw new InvalidOperationException("Selected quotation sales unit was not found.");
                if (!product.MeasurementUnitId.HasValue) throw new InvalidOperationException("Configure a managed measurement unit for the product before adding it to a quotation.");
                var stockUnit = await units.GetByIdAsync(product.MeasurementUnitId.Value) ?? throw new InvalidOperationException("Product stock unit was not found.");
                var result = await pricing.EvaluateAsync(new EvaluateSalesPriceDto { CustomerId = customer.Id, PriceListId = priceListId, ProductId = product.Id, MeasurementUnitId = unit.Id, Quantity = line.Quantity, PricingDate = dto.QuotationDate, Channel = channel, TaxRateId = taxRateId });
                evaluated.Add((line, product, unit, unit.FactorToBaseUnit / stockUnit.FactorToBaseUnit, result, templateItem));
            }
            var first = evaluated[0].Price;
            if (evaluated.Any(x => x.Price.TransactionCurrency != first.TransactionCurrency || x.Price.ExchangeRate != first.ExchangeRate || x.Price.ExchangeRateDate != first.ExchangeRateDate))
                throw new InvalidOperationException("Every quotation line must resolve to the same currency and exchange-rate snapshot.");
            created = new SalesQuotation(await numbers.GenerateAsync(DocumentNumberType.SalesQuotation, dto.QuotationDate.ToDateTime(TimeOnly.MinValue)), dto.RequestKey,
                customer, address, dto.QuotationDate, dto.ValidUntil, first.TransactionCurrency, first.ExchangeRate, first.ExchangeRateDate, first.ExchangeRateSource, dto.Notes, currentUser.UserId, template, design);
            foreach (var row in evaluated)
            {
                var p = row.Price;
                created.AddLine(new SalesQuotationLine(row.Product, row.Unit, row.Factor,
                    new SalesPriceEvaluationSnapshot(p.Quantity, p.NormalizedStockQuantity, p.TransactionCurrency, p.PriceListId, p.RuleId,
                        p.SourceCode, p.UnitPrice, p.CompanyUnitPrice, p.LineNetAmount, p.TaxRateId, p.TaxName, p.TaxRate,
                        p.TaxInclusive, p.TaxSource, p.TaxExemptionReason, p.TaxAmount, p.LineTotal), row.Input.Notes, row.TemplateItem));
            }
            await quotations.AddAsync(created);
        });
        return created!.Id;
    }

    public Task SendAsync(SalesQuotationStatusCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var quotation = await quotations.GetForDocumentAsync(dto.Id) ?? throw new InvalidOperationException("Sales quotation was not found.");
        EnsureVersion(quotation.RowVersion, dto.RowVersion);
        if (quotation.DocumentSnapshot is null)
        {
            var (snapshot, logo, logoType) = await BuildCurrentDocumentAsync(quotation, SalesQuotationStatus.Sent);
            var json = JsonSerializer.Serialize(snapshot, SnapshotJsonOptions);
            quotation.FreezeDocumentSnapshot(json, logo, logoType, Hash(json, logo, logoType));
        }
        quotation.Send(currentUser.UserId, DateOnly.FromDateTime(DateTime.Today));
    });

    public async Task<SalesQuotationPdfResult?> GeneratePdfAsync(int id, DocumentLanguage? language = null,
        CancellationToken cancellationToken = default)
    {
        var quotation = await quotations.GetForDocumentAsync(id);
        if (quotation is null) return null;
        SalesQuotationDocumentSnapshotDto snapshot; byte[]? logo; string? logoType;
        if (quotation.DocumentSnapshot is not null)
        {
            var stored = quotation.DocumentSnapshot;
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(stored.ContentHash), Encoding.ASCII.GetBytes(Hash(stored.SnapshotJson, stored.LogoContent, stored.LogoContentType))))
                throw new InvalidOperationException("The frozen sales quotation document failed its integrity check.");
            snapshot = JsonSerializer.Deserialize<SalesQuotationDocumentSnapshotDto>(stored.SnapshotJson, SnapshotJsonOptions)
                ?? throw new InvalidOperationException("The frozen sales quotation document could not be read.");
            logo = stored.LogoContent; logoType = stored.LogoContentType;
        }
        else (snapshot, logo, logoType) = await BuildCurrentDocumentAsync(quotation, quotation.Status);
        var pdf = await pdfRenderer.RenderSalesQuotationAsync(new SalesQuotationPdfRequest(snapshot, logo, logoType, language), cancellationToken);
        return new SalesQuotationPdfResult(pdf, SafeFileName($"{quotation.QuotationNumber}.pdf"));
    }
    public Task AcceptAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Accept(currentUser.UserId, DateOnly.FromDateTime(DateTime.Today)));
    public Task RejectAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Reject(currentUser.UserId, dto.Reason ?? ""));
    public Task ExpireAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Expire(currentUser.UserId, DateOnly.FromDateTime(DateTime.Today)));
    public Task CancelAsync(SalesQuotationStatusCommandDto dto) => ChangeAsync(dto, x => x.Cancel(currentUser.UserId, dto.Reason ?? ""));

    private Task ChangeAsync(SalesQuotationStatusCommandDto dto, Action<SalesQuotation> action) => unitOfWork.ExecuteInTransactionAsync(async () =>
    { var quotation = await quotations.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Sales quotation was not found."); EnsureVersion(quotation.RowVersion, dto.RowVersion); action(quotation); });
    private static void EnsureVersion(byte[] current, byte[] submitted) { if (submitted.Length == 0 || !current.SequenceEqual(submitted)) throw new InvalidOperationException("Sales quotation was modified by another user. Reload and try again."); }

    private async Task<(SalesQuotationDocumentSnapshotDto Snapshot, byte[]? Logo, string? LogoType)> BuildCurrentDocumentAsync(
        SalesQuotation quotation, SalesQuotationStatus renderedStatus)
    {
        var identity = await generalSettings.GetIdentityAsync() ?? throw new InvalidOperationException("Configure the organization identity before generating business documents.");
        DocumentDesign? design = quotation.DocumentDesignId.HasValue
            ? await designs.GetByIdAsync(quotation.DocumentDesignId.Value, false)
            : await designs.GetDefaultAsync(DocumentDesignType.SalesQuotation);
        var snapshot = new SalesQuotationDocumentSnapshotDto(1, MapOrganization(identity), MapDesign(design),
            new SalesQuotationDocumentContentDto(quotation.QuotationNumber, renderedStatus, quotation.CustomerCode,
                quotation.CustomerName, quotation.CustomerLegalName, quotation.CustomerTaxNumber,
                FormatQuotationAddress(quotation), quotation.QuotationDate, quotation.ValidUntil, quotation.CurrencyCode,
                quotation.ExchangeRate, quotation.ExchangeRateDate, quotation.ExchangeRateSource,
                quotation.PaymentTermDays, quotation.IntroductionSnapshot, quotation.TermsAndConditionsSnapshot,
                quotation.FooterNotesSnapshot, quotation.Notes, quotation.NetAmount, quotation.TaxAmount,
                quotation.TotalAmount, quotation.Lines.Select(x => new SalesQuotationDocumentLineDto(
                    x.ProductCode, x.ProductName, x.UnitCode, x.UnitName, x.Quantity, x.UnitPrice, x.NetAmount,
                    x.TaxName, x.TaxRatePercent, x.TaxAmount, x.TotalAmount, x.Notes)).ToList()));
        var logo = design?.UseOrganizationLogo == false ? null : identity.Logo?.Content.ToArray();
        return (snapshot, logo, logo is null ? null : identity.Logo?.ContentType);
    }

    private static SalesQuotationDocumentOrganizationDto MapOrganization(GeneralSettings x) => new(
        x.CompanyName, x.CompanyNameArabic, x.LegalName, x.LegalNameArabic, x.DocumentNameStyle,
        x.CommercialRegistrationNumber, x.TaxRegistrationNumber, x.Email, x.Website, x.Phone,
        x.SecondaryPhone, x.Address, x.AddressArabic, x.City, x.CityArabic, x.StateOrProvince,
        x.PostalCode, x.CountryCode, x.IdentityFields.OrderBy(f => f.Sequence)
            .Select(f => new SalesQuotationDocumentIdentityFieldDto(f.Sequence, f.Label, f.LabelArabic,
                f.Value, f.ValueArabic, f.Section, f.DocumentScope)).ToList());

    private static SalesQuotationDocumentDesignDto MapDesign(DocumentDesign? x)
    {
        if (x is null) return DefaultDesign();
        return new SalesQuotationDocumentDesignDto(x.Code, x.Version, x.Name, x.DefaultLanguage,
            x.AllowLanguageOverride, x.PageSize, x.Orientation, x.CustomPageWidthMm, x.CustomPageHeightMm,
            x.MarginTopMm, x.MarginEndMm, x.MarginBottomMm, x.MarginStartMm, x.PrimaryColor, x.AccentColor,
            x.TextColor, x.TableHeaderTextColor, x.FontFamily, x.HeaderLayout, x.UseOrganizationLogo,
            x.LogoPlacement, x.LogoMaximumHeightPx, x.OrganizationNamePlacement, x.ShowLegalName,
            x.ShowTradeName, x.ShowRegistrationNumber, x.ShowTaxNumber, x.ShowAddress, x.ShowContactDetails,
            x.ShowCustomIdentityFields, x.ShowPageNumbers, x.ShowStatusWatermark, x.ShowProductCode,
            x.ShowUnit, x.ShowLineNotes, x.ShowTaxDetails, x.ShowNetTotal, x.ShowTaxTotal, x.CoverTitle,
            x.CoverTitleArabic, x.CoverSubtitle, x.CoverSubtitleArabic, x.HeaderText, x.HeaderTextArabic,
            x.FooterText, x.FooterTextArabic, x.WatermarkText, x.WatermarkTextArabic, x.WatermarkOpacity,
            x.Sections.OrderBy(s => s.Sequence).Select(s => new SalesQuotationDocumentSectionDto(s.Sequence,
                s.SectionType, s.IsVisible, s.PageBreakBefore, s.Title, s.TitleArabic, s.Content, s.ContentArabic)).ToList());
    }

    private static SalesQuotationDocumentDesignDto DefaultDesign() => new(
        "SYSTEM", 1, "System standard", DocumentLanguage.Bilingual, true, DocumentPageSize.A4,
        DocumentPageOrientation.Portrait, null, null, 12, 12, 15, 12, "#1F4E78", "#D9EAF7",
        "#1F2937", "#FFFFFF", DocumentFontFamily.NotoSans,
        DocumentHeaderLayout.LogoStartIdentityEnd, true, DocumentElementPlacement.Start, 72,
        DocumentElementPlacement.End, true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, null, null, null, null, null, null,
        "Thank you for your business.", "شكرًا لتعاملكم معنا.", "QUOTATION", "عرض سعر", 0.12m,
        new[]
        {
            Section(10, DocumentDesignSectionType.OrganizationIdentity, "Organization", "المنشأة"),
            Section(20, DocumentDesignSectionType.DocumentDetails, "Document details", "بيانات المستند"),
            Section(30, DocumentDesignSectionType.PartyDetails, "Customer", "العميل"),
            Section(40, DocumentDesignSectionType.Introduction, "Introduction", "المقدمة"),
            Section(50, DocumentDesignSectionType.LineItems, "Items", "البنود"),
            Section(60, DocumentDesignSectionType.Totals, "Totals", "الإجماليات"),
            Section(70, DocumentDesignSectionType.TermsAndConditions, "Terms and conditions", "الشروط والأحكام"),
            Section(80, DocumentDesignSectionType.Notes, "Notes", "ملاحظات"),
            Section(90, DocumentDesignSectionType.Acceptance, "Acceptance", "الاعتماد"),
            Section(100, DocumentDesignSectionType.Footer, "Footer", "التذييل")
        });

    private static SalesQuotationDocumentSectionDto Section(int sequence, DocumentDesignSectionType type, string title, string titleArabic) =>
        new(sequence, type, true, false, title, titleArabic, null, null);
    private static string Hash(string json, byte[]? logo, string? logoContentType)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(Encoding.UTF8.GetBytes(json));
        if (logo is not null)
        {
            hash.AppendData(Encoding.UTF8.GetBytes(logoContentType ?? string.Empty));
            hash.AppendData(logo);
        }
        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }
    private static string SafeFileName(string value) => string.Concat(value.Select(x => Path.GetInvalidFileNameChars().Contains(x) ? '_' : x));
    private static string? FormatQuotationAddress(SalesQuotation x) => x.AddressLine1 is null ? null :
        $"{x.AddressLabel}: {x.AddressLine1}{(x.AddressLine2 is null ? "" : ", " + x.AddressLine2)}, {x.City}{(x.StateOrProvince is null ? "" : ", " + x.StateOrProvince)} {x.PostalCode}, {x.CountryCode}";
    private static string FormatAddress(CustomerAddress x) => $"{x.Label}: {x.AddressLine1}{(x.AddressLine2 is null ? "" : ", " + x.AddressLine2)}, {x.City}{(x.StateOrProvince is null ? "" : ", " + x.StateOrProvince)} {x.PostalCode}, {x.CountryCode}";
    private static SalesQuotationDetailsDto Map(SalesQuotation x,int? salesOrderId) => new()
    {
        Id = x.Id, QuotationNumber = x.QuotationNumber, Status = x.Status, CustomerCode = x.CustomerCode, CustomerName = x.CustomerName,
        CustomerLegalName = x.CustomerLegalName, CustomerTaxNumber = x.CustomerTaxNumber,
        Address = x.AddressLine1 is null ? null : $"{x.AddressLabel}: {x.AddressLine1}{(x.AddressLine2 is null ? "" : ", " + x.AddressLine2)}, {x.City}{(x.StateOrProvince is null ? "" : ", " + x.StateOrProvince)} {x.PostalCode}, {x.CountryCode}",
        QuotationDate = x.QuotationDate, ValidUntil = x.ValidUntil, CurrencyCode = x.CurrencyCode, ExchangeRate = x.ExchangeRate,
        ExchangeRateDate = x.ExchangeRateDate, ExchangeRateSource = x.ExchangeRateSource, PaymentTermDays = x.PaymentTermDays, Notes = x.Notes,
        SalesQuotationTemplateId = x.SalesQuotationTemplateId, TemplateCode = x.TemplateCodeSnapshot,
        TemplateVersion = x.TemplateVersionSnapshot, TemplateName = x.TemplateNameSnapshot,
        Introduction = x.IntroductionSnapshot, TermsAndConditions = x.TermsAndConditionsSnapshot, FooterNotes = x.FooterNotesSnapshot,
        DocumentDesignId = x.DocumentDesignId, DesignCode = x.DesignCodeSnapshot, DesignVersion = x.DesignVersionSnapshot,
        DesignName = x.DesignNameSnapshot, DesignLanguage = x.DesignLanguageSnapshot,
        NetAmount = x.NetAmount, TaxAmount = x.TaxAmount, TotalAmount = x.TotalAmount, CreatedByUserId = x.CreatedByUserId, CreatedAtUtc = x.CreatedAtUtc, RowVersion = x.RowVersion, SalesOrderId=salesOrderId,
        Lines = x.Lines.Select(l => new SalesQuotationLineDto(l.Id, l.ProductCode, l.ProductName, l.UnitCode, l.UnitName, l.Quantity, l.StockQuantity,
            l.UnitPrice, l.CompanyUnitPrice, l.NetAmount, l.PriceSourceCode, l.TaxName, l.TaxRatePercent, l.IsTaxInclusive, l.TaxSourceCode, l.TaxAmount, l.TotalAmount, l.Notes,
            l.TemplateItemType, l.TemplateSectionSnapshot, l.TemplateOptionGroupSnapshot)).ToList(),
        History = x.History.OrderBy(h => h.CreatedAtUtc).Select(h => new SalesQuotationHistoryDto(h.FromStatus, h.ToStatus, h.Action, h.UserId, h.Reason, h.CreatedAtUtc)).ToList()
    };

    private static CreateSalesQuotationLineDto MapTemplateLine(SalesQuotationTemplateItem x) => new()
    {
        ProductId = x.ProductId, MeasurementUnitId = x.MeasurementUnitId, Quantity = x.DefaultQuantity,
        TaxRateId = x.TaxRateId, Notes = x.Notes, SalesQuotationTemplateItemId = x.Id,
        IsSelected = x.ItemType == SalesQuotationTemplateItemType.Required || x.IsDefaultSelected,
        TemplateItemType = x.ItemType, TemplateSectionTitle = x.SectionTitle,
        TemplateOptionGroupCode = x.OptionGroupCode, TemplateOptionGroupName = x.OptionGroupName,
        AllowQuantityChange = x.AllowQuantityChange, MinimumQuantity = x.MinimumQuantity,
        MaximumQuantity = x.MaximumQuantity, ProductLabel = x.ProductNameSnapshot, UnitLabel = x.UnitNameSnapshot
    };

    private static void HydrateTemplateLines(CreateSalesQuotationDto form, SalesQuotationTemplate template)
    {
        var submitted = form.Lines.Where(x => x.SalesQuotationTemplateItemId.HasValue)
            .ToDictionary(x => x.SalesQuotationTemplateItemId!.Value);
        var custom = form.Lines.Where(x => !x.SalesQuotationTemplateItemId.HasValue).ToList();
        form.Lines = template.Items.OrderBy(x => x.Sequence).Select(item =>
        {
            var line = submitted.GetValueOrDefault(item.Id) ?? MapTemplateLine(item);
            line.ProductId = item.ProductId; line.MeasurementUnitId = item.MeasurementUnitId; line.TaxRateId = item.TaxRateId;
            line.TemplateItemType = item.ItemType; line.TemplateSectionTitle = item.SectionTitle;
            line.TemplateOptionGroupCode = item.OptionGroupCode; line.TemplateOptionGroupName = item.OptionGroupName;
            line.AllowQuantityChange = item.AllowQuantityChange; line.MinimumQuantity = item.MinimumQuantity;
            line.MaximumQuantity = item.MaximumQuantity; line.ProductLabel = item.ProductNameSnapshot; line.UnitLabel = item.UnitNameSnapshot;
            return line;
        }).Concat(custom).ToList();
    }

    private static List<CreateSalesQuotationLineDto> ValidateAndResolveTemplateLines(List<CreateSalesQuotationLineDto> submitted, SalesQuotationTemplate template)
    {
        var templateLines = submitted.Where(x => x.SalesQuotationTemplateItemId.HasValue).ToList();
        if (templateLines.GroupBy(x => x.SalesQuotationTemplateItemId).Any(x => x.Count() > 1))
            throw new InvalidOperationException("A quotation template item was submitted more than once.");
        if (!template.Items.Select(x => x.Id).OrderBy(x => x).SequenceEqual(templateLines.Select(x => x.SalesQuotationTemplateItemId!.Value).OrderBy(x => x)))
            throw new InvalidOperationException("Quotation template items changed. Apply the template again.");
        foreach (var item in template.Items)
        {
            var line = templateLines.Single(x => x.SalesQuotationTemplateItemId == item.Id);
            if (item.ItemType == SalesQuotationTemplateItemType.Required && !line.IsSelected)
                throw new InvalidOperationException("Required quotation template products cannot be removed.");
            if (line.IsSelected && (line.Quantity < item.MinimumQuantity || line.Quantity > item.MaximumQuantity))
                throw new InvalidOperationException("Selected quotation template quantity is outside its allowed range.");
            if (line.IsSelected && !item.AllowQuantityChange && line.Quantity != item.DefaultQuantity)
                throw new InvalidOperationException("This quotation template product uses a fixed quantity.");
            line.ProductId = item.ProductId; line.MeasurementUnitId = item.MeasurementUnitId; line.TaxRateId = item.TaxRateId;
        }
        foreach (var group in template.Items.Where(x => x.OptionGroupCode is not null).GroupBy(x => x.OptionGroupCode!, StringComparer.OrdinalIgnoreCase))
        {
            var first = group.First();
            var selected = group.Count(item => templateLines.Single(x => x.SalesQuotationTemplateItemId == item.Id).IsSelected);
            if (selected < first.GroupMinimumSelections || selected > first.GroupMaximumSelections)
                throw new InvalidOperationException("Quotation option-group selections are outside the configured limits.");
        }
        var custom = submitted.Where(x => !x.SalesQuotationTemplateItemId.HasValue).ToList();
        if (custom.Count > 0 && !template.AllowAdditionalLines) throw new InvalidOperationException("This quotation template does not allow additional products.");
        return templateLines.Where(x => x.IsSelected).Concat(custom.Where(x => x.IsSelected)).ToList();
    }
}
