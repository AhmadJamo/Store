using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniStore.Application.Tenancy;
using MiniStore.Domain.Interfaces;
using MiniStore.Application.Saas;

namespace MiniStore.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITenantMembershipRepository _memberships;
    private readonly ISaasOnboardingService _onboarding;
    private readonly ICompanyOnboardingService _companyOnboarding;
    private readonly PlatformSaasService _saas;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ITenantMembershipRepository memberships,
        ISaasOnboardingService onboarding,
        ICompanyOnboardingService companyOnboarding,
        PlatformSaasService saas)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _memberships = memberships;
        _onboarding = onboarding;
        _companyOnboarding = companyOnboarding;
        _saas = saas;
    }

    [HttpGet]
    public IActionResult Login(
        string? companyCode = null,
        string? returnUrl = null,
        bool rateLimited = false,
        int? retryAfterSeconds = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.CompanyCode = companyCode;
        ViewBag.RateLimitMinutes = GetRateLimitMinutes(rateLimited, retryAfterSeconds);

        return View();
    }

    [HttpPost]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string? companyCode = null,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error =
                "Username and password are required.";

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.CompanyCode = companyCode;

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
            var membership = string.IsNullOrWhiteSpace(companyCode)
                ? await _memberships.GetDefaultActiveAsync(user!.Id)
                : await _memberships.GetActiveByCompanyCodeAsync(user!.Id, companyCode);
            if (membership is null)
            {
                ViewBag.Error = string.IsNullOrWhiteSpace(companyCode)
                    ? "Your account is not assigned to an active company."
                    : "The company code is invalid or your account is not assigned to it.";
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.CompanyCode = companyCode;
                return View();
            }

            await _signInManager.SignInWithClaimsAsync(
                user,
                isPersistent: false,
                [new Claim(
                    TenantClaimTypes.TenantId,
                    membership.TenantId.ToString(System.Globalization.CultureInfo.InvariantCulture))]);

            if (!await _companyOnboarding.IsResolvedAsync(membership.TenantId))
                return RedirectToAction("Index", "CompanyOnboarding");

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
        ViewBag.CompanyCode = companyCode;

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
    public async Task<IActionResult> Register(
        int? planId = null,
        bool rateLimited = false,
        int? retryAfterSeconds = null)
    {
        ViewBag.Plans = await _saas.GetPublicPlansAsync();
        ViewBag.PlanId = planId;
        ViewBag.RateLimitMinutes = GetRateLimitMinutes(rateLimited, retryAfterSeconds);
        return View();
    }

    [HttpPost]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("registration")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterCompanyCommand command)
    {
        try
        {
            var result = await _onboarding.RegisterAsync(command);
            var user = await _userManager.FindByIdAsync(result.UserId)
                ?? throw new InvalidOperationException("The registered user was not found.");
            await _signInManager.SignInWithClaimsAsync(
                user,
                false,
                [new Claim(TenantClaimTypes.TenantId, result.TenantId.ToString(System.Globalization.CultureInfo.InvariantCulture))]);
            return RedirectToAction("Index", "CompanyOnboarding");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            ViewBag.Plans = await _saas.GetPublicPlansAsync();
            ViewBag.PlanId = command.PlanId;
            return View(command);
        }
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private static int? GetRateLimitMinutes(bool rateLimited, int? retryAfterSeconds)
    {
        if (!rateLimited)
            return null;

        return Math.Max(1, (int)Math.Ceiling(
            Math.Clamp(retryAfterSeconds ?? 60, 1, 86400) / 60d));
    }
}
