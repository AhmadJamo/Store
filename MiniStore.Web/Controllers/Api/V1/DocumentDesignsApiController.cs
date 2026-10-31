using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/document-designs")]
[PermissionAuthorize("Administration.Access")]
public sealed class DocumentDesignsApiController(
    DocumentDesignService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DocumentDesignSummaryDto>>> GetAll(
        [FromQuery] DocumentDesignStatus? status,
        [FromQuery] DocumentDesignType? documentType,
        [FromQuery] string? search) =>
        Ok(await service.GetAllAsync(status, documentType, search));

    [HttpGet("form")]
    public async Task<ActionResult<CreateDocumentDesignDto>> GetForm(
        [FromQuery] int? sourceDesignId,
        [FromQuery] DocumentDesignType? documentType)
    {
        try { return Ok(await service.GetCreateAsync(sourceDesignId, documentType)); }
        catch (InvalidOperationException exception) { return Validation(exception); }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DocumentDesignDetailsDto>> Get(int id)
    {
        var design = await service.GetAsync(id);
        return design is null ? NotFound() : Ok(design);
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDesignDetailsDto>> Create(CreateDocumentDesignDto dto)
    {
        try
        {
            var id = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, await service.GetAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception); }
    }

    [HttpPost("{id:int}/activate")]
    public Task<ActionResult<DocumentDesignDetailsDto>> Activate(int id, DocumentDesignCommandDto dto) =>
        Change(id, dto, service.ActivateAsync);

    [HttpPost("{id:int}/archive")]
    public Task<ActionResult<DocumentDesignDetailsDto>> Archive(int id, DocumentDesignCommandDto dto) =>
        Change(id, dto, service.ArchiveAsync);

    [HttpPost("{id:int}/set-default")]
    public Task<ActionResult<DocumentDesignDetailsDto>> SetDefault(int id, DocumentDesignCommandDto dto) =>
        Change(id, dto, service.SetDefaultAsync);

    private async Task<ActionResult<DocumentDesignDetailsDto>> Change(
        int id, DocumentDesignCommandDto dto, Func<DocumentDesignCommandDto, Task> command)
    {
        if (id != dto.Id)
        {
            ModelState.AddModelError(nameof(dto.Id), localizer["The route and payload identifiers do not match."]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."]);
        }

        try
        {
            await command(dto);
            return Ok((await service.GetAsync(id))!);
        }
        catch (SettingsConcurrencyException exception)
        {
            return InternalApiProblemFactory.Create(HttpContext, StatusCodes.Status409Conflict,
                localizer["Settings changed since you opened this page."],
                InternalApiProblemCodes.Concurrency, localizer[exception.Message]);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception); }
    }

    private ActionResult Validation(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."]);
    }
}
