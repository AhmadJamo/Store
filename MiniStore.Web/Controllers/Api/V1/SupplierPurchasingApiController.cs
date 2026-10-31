using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/supplier-purchasing")]
public sealed class SupplierPurchasingApiController(SupplierPurchasingInfoService service, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> GetPage(int? supplierId = null) => await Run(() => service.GetPageAsync(supplierId));

    [HttpGet("exceptions"), PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> GetExceptions(SupplierPriceExceptionType? type = null, int page = 1) => await Run(() => service.GetPriceExceptionsPageAsync(type, page));

    [HttpGet("{id:int}/price-breaks"), PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> GetPriceBreaks(int id) => await Run(() => service.GetPriceBreaksPageAsync(id));

    [HttpGet("{id:int}/history"), PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> GetHistory(int id) => await Run(() => service.GetPriceHistoryPageAsync(id));

    [HttpPost("bulk"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SaveBulk(BulkSaveSupplierPurchasingInfoDto input) => await Run(async () => new { count = await service.SaveBulkAsync(input) });

    [HttpPost("{id:int}/price-breaks"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SavePriceBreaks(int id, SaveSupplierPriceBreaksDto input)
    { input.PurchasingInfoId = id; return await Run(async () => new { supplierId = await service.SavePriceBreaksAsync(input) }); }

    [HttpPost("{id:int}/active"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SetActive(int id, SupplierPurchasingActiveRequest input) => await Run(async () => { await service.SetActiveAsync(id, input.IsActive, input.RowVersion); return new { id }; });

    [HttpGet("export/csv"), PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> ExportCsv(int supplierId)
    { try { return File(new UTF8Encoding(true).GetBytes(await service.ExportCsvAsync(supplierId)), "text/csv; charset=utf-8", $"supplier-{supplierId}-purchasing.csv"); } catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Problem(e); } }

    [HttpGet("export/excel"), PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> ExportExcel(int supplierId)
    { try { return File(await service.ExportExcelAsync(supplierId), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"supplier-{supplierId}-purchasing.xlsx"); } catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Problem(e); } }

    [HttpPost("preview/csv"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage"), RequestFormLimits(MultipartBodyLengthLimit = 1_100_000)]
    public async Task<IActionResult> PreviewCsv([FromForm] int supplierId, [FromForm] IFormFile? file)
    { if (file is null || file.Length == 0 || file.Length > 1_048_576 || !string.Equals(Path.GetExtension(file.FileName), ".csv", StringComparison.OrdinalIgnoreCase)) return UploadProblem("Select a valid supplier purchasing CSV file up to 1 MB."); using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, true); return await Run(() => service.PreviewCsvAsync(supplierId, reader.ReadToEnd(), Path.GetFileName(file.FileName))); }

    [HttpPost("preview/excel"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage"), RequestFormLimits(MultipartBodyLengthLimit = 1_100_000)]
    public async Task<IActionResult> PreviewExcel([FromForm] int supplierId, [FromForm] IFormFile? file)
    { if (file is null || file.Length == 0 || file.Length > 1_048_576 || !string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase)) return UploadProblem("Select a valid supplier purchasing Excel file up to 1 MB."); using var stream = new MemoryStream(); await file.CopyToAsync(stream); return await Run(() => service.PreviewExcelAsync(supplierId, stream.ToArray(), Path.GetFileName(file.FileName))); }

    [HttpPost("import/csv"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage"), RequestFormLimits(ValueLengthLimit = 1_100_000)]
    public async Task<IActionResult> ImportCsv(ImportSupplierPurchasingCsvDto input) => await Run(async () => new { count = await service.ImportCsvAsync(input) });

    [HttpPost("import/excel"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.SupplierTerms.Manage"), RequestFormLimits(ValueLengthLimit = 1_500_000)]
    public async Task<IActionResult> ImportExcel(ImportSupplierPurchasingExcelDto input) => await Run(async () => new { count = await service.ImportExcelAsync(input) });

    private async Task<IActionResult> Run<T>(Func<Task<T>> action)
    { try { return Ok(await action()); } catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return ProblemResult(e); } }
    private IActionResult Problem(Exception e) => ProblemResult(e);
    private ObjectResult ProblemResult(Exception e) { ModelState.AddModelError(string.Empty, localizer[e.Message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); }
    private ObjectResult UploadProblem(string message) { ModelState.AddModelError("file", localizer[message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); }
}

public sealed class SupplierPurchasingActiveRequest
{
    public bool IsActive { get; set; }
    [System.ComponentModel.DataAnnotations.Required] public byte[] RowVersion { get; set; } = [];
}
