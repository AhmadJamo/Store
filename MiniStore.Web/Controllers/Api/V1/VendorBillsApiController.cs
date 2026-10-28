using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/vendor-bills")]
public sealed class VendorBillsApiController(
    VendorBillService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("VendorBills.View")]
    public async Task<ActionResult<List<VendorBillDto>>> GetAll(string? search) => Ok(await service.GetAllAsync(search));

    [HttpGet("{id:int}"), PermissionAuthorize("VendorBills.View")]
    public async Task<ActionResult<VendorBillDto>> Get(int id)
    {
        var bill = await service.GetAsync(id);
        return bill is null ? NotFound() : Ok(bill);
    }

    [HttpGet("create-form"), PermissionAuthorize("VendorBills.Create")]
    public async Task<IActionResult> GetCreateForm(int purchaseOrderId)
    {
        var page = await service.GetCreatePageAsync(purchaseOrderId);
        return string.IsNullOrWhiteSpace(page.PurchaseOrderNumber) ? NotFound() : Ok(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.Create")]
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public Task<IActionResult> Create([FromForm] CreateVendorBillDto input, [FromForm] List<IFormFile>? attachments) =>
        Run(async () =>
        {
            var files = await ReadAttachments(attachments, 10);
            return Ok(new { id = await service.CreateDraftAsync(input, files) });
        });

    [HttpPost("{id:int}/approve"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.Approve")]
    public Task<IActionResult> Approve(int id, VendorBillCommandDto input) =>
        Command(id, input, () => service.ApproveAsync(id, input.RowVersion));

    [HttpPost("{id:int}/approve-override"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.OverrideMatch")]
    public Task<IActionResult> ApproveOverride(int id, VendorBillCommandDto input) =>
        Command(id, input, () => service.ApproveAsync(id, input.RowVersion, true, input.Reason));

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.Post")]
    public Task<IActionResult> Post(int id, VendorBillCommandDto input) =>
        Command(id, input, () => service.PostAsync(id, input.RowVersion));

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.Cancel")]
    public Task<IActionResult> Cancel(int id, VendorBillCommandDto input) =>
        Command(id, input, () => service.CancelAsync(id, input.RowVersion, input.Reason ?? string.Empty));

    [HttpGet("{id:int}/attachments/{attachmentId:int}"), PermissionAuthorize("VendorBills.View")]
    public async Task<IActionResult> DownloadAttachment(int id, int attachmentId)
    {
        var attachment = await service.GetAttachmentAsync(id, attachmentId);
        return attachment is null ? NotFound() : File(attachment.Content, attachment.ContentType, attachment.FileName);
    }

    [HttpPost("{id:int}/attachments"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.ManageAttachments")]
    [RequestFormLimits(MultipartBodyLengthLimit = 5_242_880)]
    public Task<IActionResult> AddAttachment(int id, [FromForm] string rowVersion, [FromForm] IFormFile attachment) =>
        Run(async () =>
        {
            var files = await ReadAttachments([attachment], 1);
            await service.AddAttachmentAsync(id, files[0], Convert.FromBase64String(rowVersion));
            return NoContent();
        });

    [HttpPost("{id:int}/extraction"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.ReviewExtraction")]
    public Task<IActionResult> GenerateExtraction(int id, GenerateVendorBillExtractionDto input) =>
        id == input.VendorBillId
            ? Run(async () =>
            {
                await service.GenerateExtractionSuggestionAsync(id, input.AttachmentId, input.RowVersion);
                return NoContent();
            })
            : Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));

    [HttpPost("{id:int}/extraction-review"), ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.ReviewExtraction")]
    public Task<IActionResult> ReviewExtraction(int id, ReviewVendorBillExtractionDto input) =>
        id == input.VendorBillId
            ? Run(async () =>
            {
                await service.ReviewExtractionAsync(input);
                return NoContent();
            })
            : Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));

    private Task<IActionResult> Command(int id, VendorBillCommandDto input, Func<Task> action) =>
        id == input.Id
            ? Run(async () => { await action(); return NoContent(); })
            : Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));

    private async Task<List<VendorBillAttachmentInputDto>> ReadAttachments(List<IFormFile>? attachments, int maximum)
    {
        if ((attachments?.Count ?? 0) > maximum)
            throw new ArgumentException(maximum == 1 ? "Only one attachment can be uploaded at a time." : "A vendor bill can contain at most ten attachments.");
        var result = new List<VendorBillAttachmentInputDto>();
        foreach (var file in attachments ?? [])
        {
            if (file.Length is <= 0 or > 5 * 1024 * 1024)
                throw new ArgumentException("Each vendor bill attachment must be between 1 byte and 5 MB.");
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            result.Add(new VendorBillAttachmentInputDto { FileName = file.FileName, ContentType = file.ContentType, Content = stream.ToArray() });
        }
        return result;
    }

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException)
        { return Validation(exception); }
    }

    private IActionResult Validation(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
    }
}
