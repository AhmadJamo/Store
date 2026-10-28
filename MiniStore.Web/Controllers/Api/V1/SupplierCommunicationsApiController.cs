using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/supplier-communications")]
public sealed class SupplierCommunicationsApiController(
    SupplierCommunicationService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("{id:int}")]
    [PermissionAuthorize("SupplierCommunications.View")]
    public async Task<IActionResult> Get(int id) =>
        await service.GetAsync(id) is { } value ? Ok(value) : NotFound();

    [HttpGet("document/{documentType}/{documentId:int}")]
    [PermissionAuthorize("SupplierCommunications.View")]
    public async Task<IActionResult> GetByDocument(SupplierCommunicationDocumentType documentType, int documentId) =>
        Ok(await service.GetByDocumentAsync(documentType, documentId));

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("SupplierCommunications.Prepare")]
    public async Task<IActionResult> Prepare(PrepareSupplierCommunicationDto dto)
    {
        try
        {
            var id = await service.PrepareAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Problem(exception.Message);
        }
    }

    [HttpPost("{id:int}/activities")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("SupplierCommunications.RecordActivity")]
    public async Task<IActionResult> RecordActivity(int id, RecordSupplierCommunicationActivityDto dto)
    {
        if (id != dto.CommunicationId) return BadRequest();
        try
        {
            await service.RecordActivityAsync(dto);
            return Ok(await service.GetAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Problem(exception.Message);
        }
    }

    [HttpGet("{id:int}/attachments/{attachmentId:int}")]
    [PermissionAuthorize("SupplierCommunications.View")]
    public async Task<IActionResult> Attachment(int id, int attachmentId)
    {
        var value = await service.GetAttachmentAsync(id, attachmentId);
        return value is null ? NotFound() : File(value.Content, value.ContentType, value.FileName);
    }

    private ObjectResult Problem(string message) => InternalApiProblemFactory.Create(
        HttpContext,
        StatusCodes.Status409Conflict,
        localizer[message],
        InternalApiProblemCodes.Validation);
}
