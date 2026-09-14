using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniStore.Application.Tenancy;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITenantMembershipRepository _memberships;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ITenantMembershipRepository memberships)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _memberships = memberships;
    }

    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error =
                "Username and password are required.";

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        var user = await _userManager.FindByNameAsync(username);
        var result = user is null
            ? Microsoft.AspNetCore.Identity.SignInResult.Failed
            : await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var membership = await _memberships.GetDefaultActiveAsync(user!.Id);
            if (membership is null)
            {
                ViewBag.Error = "Your account is not assigned to an active company.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            await _signInManager.SignInWithClaimsAsync(
                user,
                isPersistent: false,
                [new Claim(
                    TenantClaimTypes.TenantId,
                    membership.TenantId.ToString(System.Globalization.CultureInfo.InvariantCulture))]);

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

        ViewBag.Error =
            "Invalid username or password.";

        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
