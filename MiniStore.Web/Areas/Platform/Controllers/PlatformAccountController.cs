using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Web.Authorization;
using MiniStore.Web.Services;

namespace MiniStore.Web.Areas.Platform.Controllers;

[Area("Platform")]
[Route("platform/account")]
public class PlatformAccountController(
    PlatformAccountAuthenticationService authentication,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [AllowAnonymous, HttpGet("login")]
    public IActionResult Login(
        string? returnUrl = null,
        bool rateLimited = false,
        int? retryAfterSeconds = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.RateLimitMinutes = rateLimited
            ? (int?)Math.Max(1, (int)Math.Ceiling(Math.Clamp(retryAfterSeconds ?? 60, 1, 86400) / 60d))
            : null;
        return View();
    }

    [AllowAnonymous, HttpPost("login"), ValidateAntiForgeryToken, Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        var result = await authentication.AuthenticateAsync(username, password, HttpContext.RequestAborted);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", localizer["Invalid platform credentials."]);
            return View();
        }
        await HttpContext.SignInAsync(PlatformAuthentication.Scheme, result.Principal!);
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/platform");
    }

    [Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme), HttpPost("logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(PlatformAuthentication.Scheme); return RedirectToAction(nameof(Login)); }
}
