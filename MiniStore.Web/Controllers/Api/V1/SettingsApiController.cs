using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Caching.Memory;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Localization;
using MiniStore.Application.Dtos.Settings;
using Microsoft.Extensions.Localization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/settings")]
public sealed class SettingsApiController(
    GeneralSettingsService settings,
    DiscountSettingsService discounts,
    PurchaseMatchingSettingsService purchaseMatching,
    InventorySettingsService inventory,
    PurchaseSourcingPolicyService purchaseSourcing,
    DocumentNumberService documentNumbers,
    FiscalPeriodService fiscalPeriods,
    CurrencyExchangeRateService currencyExchangeRates,
    MeasurementUnitService measurementUnits,
    AccountingSettingsService accountingSettings,
    AccountService accounts,
    BranchService branches,
    InventoryAccessService inventoryAccess,
    PosExperienceSettingsService posExperience,
    WorkflowCatalogService workflowCatalog,
    IAuthorizationService authorization,
    IMemoryCache memoryCache,
    ITenantContext tenantContext,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("organization-identity")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<OrganizationIdentityDto>> GetOrganizationIdentity() =>
        Ok(await settings.GetOrganizationIdentityAsync());

    [HttpPut("organization-identity")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<OrganizationIdentityDto>> UpdateOrganizationIdentity(
        OrganizationIdentityDto dto) => await ExecuteUpdateAsync(
            () => settings.UpdateOrganizationIdentityAsync(dto, null, removeLogo: false),
            settings.GetOrganizationIdentityAsync);

    [HttpGet("organization-logo")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<IActionResult> GetOrganizationLogo()
    {
        var logo = await settings.GetOrganizationLogoAsync();
        return logo is null ? NotFound() : File(logo.Content, logo.ContentType, enableRangeProcessing: false);
    }

    [HttpPost("organization-logo")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = 3 * 1024 * 1024)]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<OrganizationIdentityDto>> UploadOrganizationLogo(
        [FromForm] IFormFile logoFile,
        [FromForm] string rowVersion)
    {
        if (logoFile.Length == 0 || logoFile.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError(nameof(logoFile),
                localizer["Organization logo must be a non-empty image no larger than 2 MB."]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."]);
        }
        var identity = await IdentityForLogoChangeAsync(rowVersion);
        await using var stream = new MemoryStream((int)logoFile.Length);
        await logoFile.CopyToAsync(stream);
        var upload = new OrganizationLogoUploadDto(stream.ToArray(), logoFile.FileName);
        return await ExecuteUpdateAsync(
            () => settings.UpdateOrganizationIdentityAsync(identity, upload, removeLogo: false),
            settings.GetOrganizationIdentityAsync);
    }

    [HttpDelete("organization-logo")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<OrganizationIdentityDto>> DeleteOrganizationLogo(
        [FromBody] LogoVersionCommand command)
    {
        var identity = await IdentityForLogoChangeAsync(command.RowVersion);
        return await ExecuteUpdateAsync(
            () => settings.UpdateOrganizationIdentityAsync(identity, null, removeLogo: true),
            settings.GetOrganizationIdentityAsync);
    }

    [HttpGet("preferences")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<GeneralSettingsPreferencesDto>> GetPreferences() =>
        Ok(await settings.GetPreferencesAsync());

    [HttpPut("preferences")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<GeneralSettingsPreferencesDto>> UpdatePreferences(
        GeneralSettingsPreferencesDto dto) => await ExecuteUpdateAsync(
            async () =>
            {
                await settings.UpdatePreferencesAsync(dto);
                memoryCache.Remove(DatabaseRequestCultureProvider.GetCacheKey(tenantContext.TenantId));
            },
            settings.GetPreferencesAsync);

    [HttpGet("discounts")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<DiscountSettingsDto>> GetDiscounts() => Ok(await discounts.GetAsync());

    [HttpPut("discounts")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<DiscountSettingsDto>> UpdateDiscounts(DiscountSettingsDto dto) =>
        await ExecuteUpdateAsync(() => discounts.UpdateAsync(dto), discounts.GetAsync);

    [HttpGet("purchase-matching")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<PurchaseMatchingSettingsDto>> GetPurchaseMatching() =>
        Ok(await purchaseMatching.GetAsync());

    [HttpPut("purchase-matching")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<PurchaseMatchingSettingsDto>> UpdatePurchaseMatching(
        PurchaseMatchingSettingsDto dto) => await ExecuteUpdateAsync(
            () => purchaseMatching.UpdateAsync(dto), purchaseMatching.GetAsync);

    [HttpGet("inventory")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<InventorySettingsDto>> GetInventory() => Ok(await inventory.GetAsync());

    [HttpPut("inventory")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<InventorySettingsDto>> UpdateInventory(InventorySettingsDto dto) =>
        await ExecuteUpdateAsync(() => inventory.UpdateAsync(dto), inventory.GetAsync);

    [HttpGet("purchase-sourcing")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<PurchaseSourcingPolicyDto>> GetPurchaseSourcing() => Ok(await purchaseSourcing.GetAsync());

    [HttpPut("purchase-sourcing")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<PurchaseSourcingPolicyDto>> UpdatePurchaseSourcing(PurchaseSourcingPolicyDto dto) =>
        await ExecuteUpdateAsync(() => purchaseSourcing.UpdateAsync(dto), purchaseSourcing.GetAsync);

    [HttpGet("document-numbers")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<DocumentNumberSettingsPageDto>> GetDocumentNumbers() => Ok(await documentNumbers.GetPageAsync());

    [HttpPut("document-numbers")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<DocumentNumberSettingsPageDto>> UpdateDocumentNumbers(DocumentNumberSettingsPageDto dto) =>
        await ExecuteUpdateAsync(() => documentNumbers.UpdateAsync(dto), documentNumbers.GetPageAsync);

    [HttpGet("fiscal-periods")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<FiscalPeriodsPageDto>> GetFiscalPeriods() => Ok(await fiscalPeriods.GetPageAsync());

    [HttpPost("fiscal-periods")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<FiscalPeriodsPageDto>> CreateFiscalPeriod(CreateFiscalPeriodDto dto) =>
        await ExecuteUpdateAsync(() => fiscalPeriods.CreateAsync(dto), () => fiscalPeriods.GetPageAsync());

    [HttpPut("fiscal-periods/status")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<FiscalPeriodsPageDto>> ChangeFiscalPeriodStatus(ChangeFiscalPeriodStatusDto dto) =>
        await ExecuteUpdateAsync(() => fiscalPeriods.ChangeStatusAsync(dto), () => fiscalPeriods.GetPageAsync());

    [HttpGet("currency-exchange-rates")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<CurrencyExchangeRatePageDto>> GetCurrencyExchangeRates() =>
        Ok(await currencyExchangeRates.GetPageAsync());

    [HttpPost("currency-exchange-rates")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<CurrencyExchangeRatePageDto>> SaveCurrencyExchangeRate(
        SaveCurrencyExchangeRateDto dto) =>
        await ExecuteUpdateAsync(() => currencyExchangeRates.SaveAsync(dto), () => currencyExchangeRates.GetPageAsync());

    [HttpGet("measurement-units")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<MeasurementUnitsPageDto>> GetMeasurementUnits() =>
        Ok(await measurementUnits.GetPageAsync());

    [HttpPost("measurement-units")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<MeasurementUnitsPageDto>> CreateMeasurementUnit(CreateMeasurementUnitDto dto) =>
        await ExecuteUpdateAsync(() => measurementUnits.CreateAsync(dto), () => measurementUnits.GetPageAsync());

    [HttpPost("measurement-units/{id:int}/deactivate")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<MeasurementUnitsPageDto>> DeactivateMeasurementUnit(int id) =>
        await ExecuteUpdateAsync(() => measurementUnits.DeactivateAsync(id), () => measurementUnits.GetPageAsync());

    [HttpGet("accounting")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<AccountingSettingsPageDto>> GetAccountingSettings() =>
        Ok(await GetAccountingSettingsPageAsync());

    [HttpPut("accounting")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<AccountingSettingsPageDto>> UpdateAccountingSettings(AccountingSettingsDto dto) =>
        await ExecuteUpdateAsync(() => accountingSettings.UpdateAsync(dto), GetAccountingSettingsPageAsync);

    [HttpGet("branch-sales-accounts")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<BranchSalesAccountsPageDto>> GetBranchSalesAccounts() =>
        Ok(await branches.GetSalesAccountSettingsAsync());

    [HttpPut("branch-sales-accounts/{branchId:int}")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<BranchSalesAccountsPageDto>> UpdateBranchSalesAccount(
        int branchId, BranchSalesAccountCommand command) =>
        await ExecuteUpdateAsync(
            () => branches.ConfigureSalesAccountAsync(branchId, command.AccountId),
            branches.GetSalesAccountSettingsAsync);

    [HttpGet("inventory-access")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<InventoryAccessSettingsDto>> GetInventoryAccess() =>
        Ok(await inventoryAccess.GetPageAsync());

    [HttpPut("inventory-access/branch-warehouse")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<InventoryAccessSettingsDto>> ConfigureBranchWarehouse(
        ConfigureBranchWarehouseDto dto) =>
        await ExecuteUpdateAsync(
            () => inventoryAccess.ConfigureBranchWarehouseAsync(dto),
            inventoryAccess.GetPageAsync);

    [HttpPost("inventory-access/pos-terminals")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<InventoryAccessSettingsDto>> CreatePosTerminal(
        CreatePosTerminalDto dto) =>
        await ExecuteUpdateAsync(
            () => inventoryAccess.CreatePosTerminalAsync(dto),
            inventoryAccess.GetPageAsync);

    [HttpPut("inventory-access/pos-terminal-warehouse")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<InventoryAccessSettingsDto>> ConfigurePosTerminalWarehouse(
        ConfigurePosTerminalWarehouseDto dto) =>
        await ExecuteUpdateAsync(
            () => inventoryAccess.ConfigurePosTerminalWarehouseAsync(dto),
            inventoryAccess.GetPageAsync);

    [HttpGet("pos-experience")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<PosExperienceSettingsPageDto>> GetPosExperience([FromQuery] int? terminalId) =>
        Ok(await posExperience.GetPageAsync(terminalId));

    [HttpPut("pos-experience")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<PosExperienceSettingsPageDto>> UpdatePosExperience(
        PosExperienceSettingsDto dto) =>
        await ExecuteUpdateAsync(
            () => posExperience.UpdateAsync(dto),
            () => posExperience.GetPageAsync(dto.PosTerminalId));

    [HttpGet("workflow-catalog")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<WorkflowCatalogApiPage>> GetWorkflowCatalog()
    {
        var page = workflowCatalog.GetAll();
        var steps = new List<WorkflowCatalogApiStep>(page.Steps.Count);
        foreach (var step in page.Steps)
        {
            var canOpen = (await authorization.AuthorizeAsync(
                User, null, $"Permission:{step.RequiredPermission}")).Succeeded;
            steps.Add(new WorkflowCatalogApiStep(
                localizer[step.Module], step.Number, localizer[step.Phase], localizer[step.Title],
                localizer[step.ResponsibleRole], localizer[step.ReviewerRole], localizer[step.Instructions],
                localizer[step.ControlPrinciple], localizer[step.CompletionEvidence], step.Conditional,
                canOpen, canOpen ? Url.Action(step.Action, step.Controller) : null));
        }

        return Ok(new WorkflowCatalogApiPage(
            localizer[page.ModuleName], localizer[page.Purpose], steps));
    }

    private async Task<ActionResult<T>> ExecuteUpdateAsync<T>(Func<Task> update, Func<Task<T>> refresh)
    {
        try
        {
            await update();
            return Ok(await refresh());
        }
        catch (SettingsConcurrencyException exception)
        {
            return InternalApiProblemFactory.Create(HttpContext, StatusCodes.Status409Conflict,
                localizer["Settings changed since you opened this page."],
                InternalApiProblemCodes.Concurrency,
                localizer[exception.Message]);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."]);
        }
    }

    private async Task<OrganizationIdentityDto> IdentityForLogoChangeAsync(string rowVersion)
    {
        var identity = await settings.GetOrganizationIdentityAsync();
        try { identity.RowVersion = Convert.FromBase64String(rowVersion); }
        catch (FormatException) { identity.RowVersion = []; }
        return identity;
    }

    private async Task<AccountingSettingsPageDto> GetAccountingSettingsPageAsync() => new()
    {
        Settings = await accountingSettings.GetAsync(),
        Accounts = await accounts.GetAllAsync()
    };

    public sealed record LogoVersionCommand(string RowVersion);
    public sealed record BranchSalesAccountCommand(int AccountId);
    public sealed record WorkflowCatalogApiPage(
        string ModuleName, string Purpose, IReadOnlyList<WorkflowCatalogApiStep> Steps);
    public sealed record WorkflowCatalogApiStep(
        string Module, int Number, string Phase, string Title, string ResponsibleRole,
        string ReviewerRole, string Instructions, string ControlPrinciple,
        string CompletionEvidence, bool Conditional, bool CanOpen, string? WorkUrl);
}
