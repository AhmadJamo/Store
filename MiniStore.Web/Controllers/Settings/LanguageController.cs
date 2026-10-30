using Microsoft.AspNetCore.Mvc;
using MiniStore.Web.Localization;

namespace MiniStore.Web.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Set(string culture, string? returnUrl)
    {
        var normalized = SupportedUiCultures.Normalize(culture);
        if (normalized is null)
            return BadRequest();

        UiCultureCookie.Append(Response, normalized, Request.IsHttps);

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }
}
