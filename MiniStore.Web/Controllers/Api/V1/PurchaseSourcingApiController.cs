using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/purchase-sourcing")]
public sealed class PurchaseSourcingApiController(PurchaseSourcingService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<ActionResult<List<PurchaseSourcingDto>>> GetAll(PurchaseSourcingStatus? status, string? search) =>
        Ok(await service.GetAllAsync(status, search));

    [HttpGet("{id:int}"), PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<ActionResult<PurchaseSourcingDto>> Get(int id)
    { var item = await service.GetAsync(id); return item is null ? NotFound() : Ok(item); }

    [HttpGet("create-form"), PermissionAuthorize("PurchaseSourcing.Create")]
    public async Task<ActionResult<PurchaseSourcingCreatePageDto>> GetCreateForm(int purchaseRequestId)
    { var page = await service.GetCreatePageAsync(purchaseRequestId); return page.Request is null ? NotFound() : Ok(page); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseSourcing.Create")]
    [RequestFormLimits(MultipartBodyLengthLimit = 52_500_000)]
    public async Task<IActionResult> Create(
        [FromForm] CreatePurchaseSourcingDto input,
        [FromForm] List<IFormFile>? attachments)
    {
        try
        {
            input.Attachments = await ReadAttachmentsAsync(attachments);
            return Ok(new { id = await service.CreateAndSendAsync(input) });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Validation(exception); }
    }

    [HttpGet("{id:int}/attachments/{attachmentId:int}"), PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<IActionResult> DownloadAttachment(int id, int attachmentId)
    {
        var attachment = await service.GetAttachmentAsync(id, attachmentId);
        return attachment is null
            ? NotFound()
            : File(attachment.Content, SafeContentType(attachment.ContentType), attachment.FileName);
    }

    private static async Task<List<PurchaseSourcingAttachmentInputDto>> ReadAttachmentsAsync(
        List<IFormFile>? attachments)
    {
        var files = attachments ?? [];
        if (files.Count > 10) throw new ArgumentException("An RFQ can contain up to 10 attachments.");
        var result = new List<PurchaseSourcingAttachmentInputDto>();
        foreach (var file in files)
        {
            if (file.Length is <= 0 or > 5 * 1024 * 1024)
                throw new ArgumentException("Each RFQ attachment must be between 1 byte and 5 MB.");
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            result.Add(new PurchaseSourcingAttachmentInputDto
            {
                FileName = Path.GetFileName(file.FileName),
                ContentType = SafeContentType(file.ContentType),
                Content = stream.ToArray()
            });
        }
        return result;
    }

    private static string SafeContentType(string? contentType) =>
        !string.IsNullOrWhiteSpace(contentType) &&
        System.Net.Http.Headers.MediaTypeHeaderValue.TryParse(contentType, out var parsed)
            ? parsed.MediaType ?? "application/octet-stream"
            : "application/octet-stream";

    private IActionResult Validation(Exception exception)
    { ModelState.AddModelError(string.Empty, localizer[exception.Message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); }
}
