using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Areas.Platform.Controllers;

[Area("Platform")]
[Route("platform/account")]
public class PlatformAccountController(UserManager<IdentityUser> users, SignInManager<IdentityUser> signInManager, ISaasRepository saas) : Controller
{
    [AllowAnonymous, HttpGet("login")]
    public IActionResult Login(string? returnUrl = null) { ViewBag.ReturnUrl = returnUrl; return View(); }

    [AllowAnonymous, HttpPost("login"), ValidateAntiForgeryToken, Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        var user = await users.FindByNameAsync(username);
        var op = user is null ? null : await saas.GetOperatorAsync(user.Id);
        if (user is null || op is null || !(await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true)).Succeeded)
        {
            ModelState.AddModelError("", "Invalid platform credentials.");
            return View();
        }
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName!), new Claim(ClaimTypes.Role, op.Role.ToString())], PlatformAuthentication.Scheme);
        await HttpContext.SignInAsync(PlatformAuthentication.Scheme, new ClaimsPrincipal(identity));
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/platform");
    }

    [Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme), HttpPost("logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(PlatformAuthentication.Scheme); return RedirectToAction(nameof(Login)); }
}
