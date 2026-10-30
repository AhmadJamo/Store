using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Web.Localization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

public sealed record SetUiCultureRequest(string Culture);

[ApiController]
[Route("api/v1/culture")]
public sealed class CultureApiController(IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Set(SetUiCultureRequest request)
    {
        var culture = SupportedUiCultures.Normalize(request.Culture);
        if (culture is null)
        {
            ModelState.AddModelError(nameof(request.Culture), localizer["One or more validation errors occurred."]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."]);
        }

        UiCultureCookie.Append(Response, culture, Request.IsHttps);
        return NoContent();
    }
}
