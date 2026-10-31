using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniStore.Application.Tenancy;
using MiniStore.Application.Saas;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Services;

namespace MiniStore.Web.Controllers;

public class AccountController : Controller
{
    private readonly PlatformSaasService _saas;
    private readonly TenantAccountAuthenticationService _authentication;
    private readonly TenantCompanyRegistrationService _registration;

    public AccountController(
        PlatformSaasService saas,
        TenantAccountAuthenticationService authentication,
        TenantCompanyRegistrationService registration)
    {
        _saas = saas;
        _authentication = authentication;
        _registration = registration;
    }

    [HttpGet]
    public IActionResult Login(
        string? companyCode = null,
        string? returnUrl = null,
        bool rateLimited = false,
        int? retryAfterSeconds = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

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

        var outcome = await _authentication.SignInAsync(username, password, companyCode);
        if (outcome.Status == TenantLoginStatus.Succeeded)
        {
            if (outcome.RequiresCompanyOnboarding)
                return RedirectToAction("Index", "CompanyOnboarding");

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl) &&
                !returnUrl.StartsWith("/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(
                "Index",
                "Dashboard");
        }

        ViewBag.Error = outcome.Status == TenantLoginStatus.CompanyAccessDenied
            ? string.IsNullOrWhiteSpace(companyCode)
                ? "Your account is not assigned to an active company."
                : "The company code is invalid or your account is not assigned to it."
            : "Invalid username or password.";

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.CompanyCode = companyCode;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authentication.SignOutAsync();

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
            await _registration.RegisterAndSignInAsync(command);
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
